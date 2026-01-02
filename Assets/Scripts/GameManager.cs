using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TubeController firstTube;
    [SerializeField] private TubeController secondTube;

    public List<TubeController> tubes = new List<TubeController>();
    
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }


    void Update()
    {
        if (CheckWin())
        {
            UIManager.Instance.winPanel.SetActive(true);
            return;
        }
        else if (!HasAnyValidStep() && !CheckWin())
        {

            UIManager.Instance.LosePanel.SetActive(true);
            return;
        }
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

    public bool CheckWin()
    {
        foreach (var t in tubes)
        {
            if (t.numberOfColors == 0)
            {
                continue;
            }

            if (t.numberOfColors != 4)
            {
                return false;
            }

            Color c = t.tubeColors[0];
            for (int i = 1; i < 4; i++)
            {
                if (!t.tubeColors[i].Equals(c))
                {
                    return false;
                }
            }
        }
        return true;

    }

    public bool HasAnyValidStep()
    {
        for (int i = 0; i < tubes.Count; i++)
        {
            for (int j = 0; j < tubes.Count; j++)
            {
                if (i == j) continue;

                TubeController from = tubes[i];
                TubeController to = tubes[j];

                from.UpdateTopColorValues();
                to.UpdateTopColorValues();

                if (from.numberOfColors == 0) continue;
                if (to.numberOfColors == 4) continue;

                if (to.FillTubeCheck(from.topColor))
                    return true;
            }
        }
        return false;
    }
 }





