using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TubeController firstTube;
    [SerializeField] private TubeController secondTube;


    void Start()
    {

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<TubeController>() != null)
                {
                    if (firstTube == null)
                    {
                        firstTube = hit.collider.GetComponent<TubeController>();
                    }
                    else
                    {
                        if (firstTube == hit.collider.GetComponent<TubeController>())
                        {
                            firstTube = null;
                        }
                        else
                        {
                            secondTube = hit.collider.GetComponent<TubeController>();
                            firstTube.tubeController = secondTube;

                            firstTube.UpdateTopColorValues();
                            secondTube.UpdateTopColorValues();

                            if (secondTube.FillTubeCheck(firstTube.topColor) == true)
                            {
                                firstTube.StartColorTransfer();
                                firstTube = null;
                                secondTube = null;
                            }
                            else
                            {
                                firstTube = null;
                                secondTube = null;
                            }
                        }
                    }


                }
            }
        }
    }

}