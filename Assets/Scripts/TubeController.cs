using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour
{
    public Color[] tubeColors;
    [SerializeField] private SpriteRenderer tubeMaskSR;
    [SerializeField] AnimationCurve scaleAndRotationCurve;
    [SerializeField] AnimationCurve fillAmountCurve;
    [SerializeField] AnimationCurve rotationSpeed;

    [SerializeField]private float[] fillAmounts;
    [SerializeField]private float[] rotationValues;
    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColors = 4;
    public Color topColor;
    [SerializeField]private int numberOfTopColorCount = 1;

    public TubeController tubeController;
    [SerializeField] private bool justThisTube = false;
    private int amountOfColorTransfer = 0;

    [SerializeField] Transform leftPoint;
    [SerializeField] Transform rightPoint;
    private Transform ChosePoint;

    private float dir = 1.0f;

    Vector3 originPos;
    Vector3 startPos;
    Vector3 endPos;

    public LineRenderer lineRenderer;


    // Start is called before the first frame update
    void Start()
    {
        tubeMaskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColors]);
        originPos = transform.position;

        UpdateColor();

        UpdateTopColorValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P) && justThisTube)
        {
            UpdateTopColorValues();
            if (tubeController.FillTubeCheck(topColor))
            {
                ChoseRotationPointDir();

                amountOfColorTransfer = Mathf.Min(numberOfTopColorCount, 4 - tubeController.numberOfColors);
                for (int i = 0; i < amountOfColorTransfer; i++)
                {
                    tubeController.tubeColors[tubeController.numberOfColors + i] = topColor;
                }
                tubeController.UpdateColor();
            }
            CalculRotationIndex(4-tubeController.numberOfColors);
            StartCoroutine(RotateTube());
        } 
    }

    IEnumerator MoveTube()
    {
        startPos = transform.position;
        if (ChosePoint == leftPoint)
        {
            endPos = tubeController.rightPoint.position;
        }
        else
        {
            endPos= tubeController.leftPoint.position;
        }

        float t = 0;
        while (t <= 1)
        {
            transform.position=Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;

        StartCoroutine(RotateTube());
    }

    IEnumerator MoveTubeBack()
    {
        startPos = transform.position;
        endPos = originPos;
        float t = 0;
        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;


        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        tubeMaskSR.sortingOrder -= 2;
    }

    public void StartColorTransfer()
    {
        ChoseRotationPointDir();

        amountOfColorTransfer = Mathf.Min(numberOfTopColorCount, 4 - tubeController.numberOfColors);
        for (int i = 0; i < amountOfColorTransfer; i++)
        {
            tubeController.tubeColors[tubeController.numberOfColors + i] = topColor;
        }
        tubeController.UpdateColor();

        CalculRotationIndex(4 - tubeController.numberOfColors);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        tubeMaskSR.sortingOrder+= 2;
        StartCoroutine(MoveTube());
    }
    public void UpdateColor()
    {
        tubeMaskSR.material.SetColor("_C1", tubeColors[0]);
        tubeMaskSR.material.SetColor("_C2", tubeColors[1]);
        tubeMaskSR.material.SetColor("_C3", tubeColors[2]);
        tubeMaskSR.material.SetColor("_C4", tubeColors[3]);
    }

    public float timeToRotate = 1.0f;

    IEnumerator RotateTube()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = 0;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue=Mathf.Lerp(0.0f,dir* rotationValues[rotationIndex], lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(ChosePoint.position, Vector3.forward, lastAngleValue - angleValue);

            tubeMaskSR.material.SetFloat("_SARM", scaleAndRotationCurve.Evaluate(angleValue));

            if (numberOfColors > 0 && fillAmounts[numberOfColors] > fillAmountCurve.Evaluate(angleValue)+0.005f)
            {
                if (lineRenderer.enabled == false)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;

                    lineRenderer.SetPosition(0, ChosePoint.position);
                    lineRenderer.SetPosition(1, ChosePoint.position-Vector3.up*1.45f);
                    lineRenderer.enabled = true;
                    tubeController.FillUp(fillAmountCurve.Evaluate(lastAngleValue) - fillAmountCurve.Evaluate(angleValue));
                }

                tubeMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

                tubeController.FillUp(fillAmountCurve.Evaluate(lastAngleValue)-fillAmountCurve.Evaluate(angleValue));

            }


            t += Time.deltaTime * rotationSpeed.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }
        angleValue = dir * rotationValues[rotationIndex];
        //transform.eulerAngles = new Vector3(0, 0, angleValue);
        tubeMaskSR.material.SetFloat("_SARM", scaleAndRotationCurve.Evaluate(angleValue));
        tubeMaskSR.material.SetFloat("_FillAmount", fillAmountCurve.Evaluate(angleValue));

        numberOfColors -= amountOfColorTransfer;
        tubeController.numberOfColors += amountOfColorTransfer;
        if (numberOfColors - amountOfColorTransfer <= 0)
        {
            tubeMaskSR.material.SetFloat("_FillAmount", fillAmounts[0]);
        }
        GameManager.instance.CheckWin();

        lineRenderer.enabled = false;
        StartCoroutine(RotationTubeBack());
    }

    IEnumerator RotationTubeBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = dir * rotationValues[rotationIndex];

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp( dir * rotationValues[rotationIndex], 0.0f , lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(ChosePoint.position, Vector3.forward, lastAngleValue - angleValue);

            tubeMaskSR.material.SetFloat("_SARM", scaleAndRotationCurve.Evaluate(angleValue));

            lastAngleValue = angleValue;

            t += Time.deltaTime ;

            yield return new WaitForEndOfFrame();
        }
        UpdateTopColorValues();
        angleValue = 0;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        tubeMaskSR.material.SetFloat("_SARM", scaleAndRotationCurve.Evaluate(angleValue));

        StartCoroutine(MoveTubeBack());
    }

     public void UpdateTopColorValues()
    {
        if (numberOfColors != 0)
        {
            numberOfTopColorCount = 1;
            topColor = tubeColors[numberOfColors - 1];
            if (numberOfColors == 4)
            {
                if (tubeColors[3].Equals(tubeColors[2]))
                {
                    numberOfTopColorCount = 2;
                    if (tubeColors[2].Equals(tubeColors[1]))
                    {
                        numberOfTopColorCount = 3;
                        if (tubeColors[1].Equals(tubeColors[0]))
                        {
                            numberOfTopColorCount = 4;

                        }
                    }
                }
            }
        else if (numberOfColors == 3)
            {
                if (tubeColors[2].Equals(tubeColors[1]))
                {
                    numberOfTopColorCount = 2;
                    if (tubeColors[1].Equals(tubeColors[0]))
                    {
                        numberOfTopColorCount = 3;

                    }
                }
            }
        else if(numberOfColors == 2)
            {
                if (tubeColors[1].Equals(tubeColors[0]))
                {
                    numberOfTopColorCount = 2;

                }
            }
            rotationIndex = 3 - (numberOfColors - numberOfTopColorCount);
        }
    }

    public bool FillTubeCheck(Color colorCheck)
    {
        if (numberOfColors == 0) return true;
        else
        {
            if (numberOfColors == 4)
            {
                return false;

            }
            else
            {
                if (topColor.Equals(colorCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public void CalculRotationIndex(int numberOfEmpty)
    {
        rotationIndex = 3 - (numberOfColors - Mathf.Min(numberOfEmpty, numberOfTopColorCount));
    }

    public void FillUp(float fillAmountToAdd)
    {
        float current = tubeMaskSR.material.GetFloat("_FillAmount");
        float next = Mathf.Clamp01(current + fillAmountToAdd);
        tubeMaskSR.material.SetFloat("_FillAmount", next);
    }


    public void ChoseRotationPointDir()
    {
        if (transform.position.x > tubeController.transform.position.x)
        {
            ChosePoint = leftPoint;
            dir = -1.0f;
        }
        else
        {
            ChosePoint = rightPoint;
            dir = 1.0f;
        }
    }


}
