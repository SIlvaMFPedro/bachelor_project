using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

using UnityEngine.EventSystems;

public class swipeImgScript : MonoBehaviour
{
    Controller controller;
    private bool flagSwipe;
    public GameObject Panel2;
    public GameObject Panel2_1;
    public GameObject Panel3;
    public GameObject Panel3_1;
    public GameObject Panel3_2;
    public GameObject Panel3_3;
    public GameObject Panel3_4;
    public GameObject Panel3_5;
    public GameObject Panel3_6;
    public GameObject Panel4;
    public GameObject Panel4_1;
    public GameObject Quadro1;
    public GameObject Quadro2;

    // Use this for initialization
    void Start()
    {
        controller = new Controller();
        flagSwipe = false;
    }

    void FixedUpdate()
    {
        Frame frame = controller.Frame();
        Frame oldframe = controller.Frame(10); // it works in +/- 60fps; 10 tested for better performance

        if (!frame.Equals(oldframe)) // current frame != old frame
        {
            if (oldframe.Hands.Count > 0 && (oldframe.Hands[0].IsRight || oldframe.Hands[0].IsLeft))
            {
                Hand oldHand = oldframe.Hands[0];

                if (frame.Hands.Count > 0 && (frame.Hands[0].IsRight || frame.Hands[0].IsLeft))
                {
                    Hand firstHand = frame.Hands[0];
                    if (oldHand.Id == firstHand.Id)
                    {
                        if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) > 80 && (firstHand.PalmPosition.x - oldHand.PalmPosition.x) > 0 && !flagSwipe)
                        {
                            Debug.Log("isSwiping Right");
                            flagSwipe = true;
                            if (Panel3_1.activeInHierarchy)
                            {
                                Panel3_1.SetActive(false);
                                Panel3_6.SetActive(true);
                            }else if (Panel3_2.activeInHierarchy)
                            {
                                Panel3_2.SetActive(false);
                                Panel3_1.SetActive(true);
                            }else if (Panel3_3.activeInHierarchy)
                            {
                                Panel3_3.SetActive(false);
                                Panel3_2.SetActive(true);
                            }else if (Panel3_4.activeInHierarchy)
                            {
                                Panel3_4.SetActive(false);
                                Panel3_3.SetActive(true);
                            }else if (Panel3_5.activeInHierarchy)
                            {
                                Panel3_5.SetActive(false);
                                Panel3_4.SetActive(true);
                            }else if (Panel3_6.activeInHierarchy)
                            {
                                Panel3_6.SetActive(false);
                                Panel3_5.SetActive(true);
                            }else if (Panel2_1.activeInHierarchy)
                            {
                                Panel2_1.SetActive(true);
                                Panel3.SetActive(false);
                                Panel4.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel3.activeInHierarchy)
                            {
                                Panel3.SetActive(false);
                                Panel2.SetActive(true);
                                Panel4.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel4.activeInHierarchy)
                            {
                                Panel4.SetActive(false);
                                Panel3.SetActive(true);
                                Panel2.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel4_1.activeInHierarchy)
                            {
                                Panel4_1.SetActive(true);
                                Panel3.SetActive(false);
                                Panel2.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel2.activeInHierarchy)
                            {
                                Panel2.SetActive(false);
                                Panel3.SetActive(false);
                                Panel4.SetActive(false);
                                Quadro1.SetActive(true);
                            }
                            else if (Quadro1.activeInHierarchy)
                            {
                                Quadro1.SetActive(false);
                                Panel2.SetActive(false);
                                Panel3.SetActive(false);
                                Panel4.SetActive(true);
                            }
                        }
                        else if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) <= 5)
                        {
                            flagSwipe = false;
                        }
                        else if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) > 80 && (firstHand.PalmPosition.x - oldHand.PalmPosition.x) < 0 && !flagSwipe)
                        {
                            Debug.Log("isSwiping Left");
                            flagSwipe = true;
                            if (Panel3_1.activeInHierarchy)
                            {
                                Panel3_1.SetActive(false);
                                Panel3_2.SetActive(true);
                            }
                            else if (Panel3_2.activeInHierarchy)
                            {
                                Panel3_2.SetActive(false);
                                Panel3_3.SetActive(true);
                            }
                            else if (Panel3_3.activeInHierarchy)
                            {
                                Panel3_3.SetActive(false);
                                Panel3_4.SetActive(true);
                            }
                            else if (Panel3_4.activeInHierarchy)
                            {
                                Panel3_4.SetActive(false);
                                Panel3_5.SetActive(true);
                            }
                            else if (Panel3_5.activeInHierarchy)
                            {
                                Panel3_5.SetActive(false);
                                Panel3_6.SetActive(true);
                            }
                            else if (Panel3_6.activeInHierarchy)
                            {
                                Panel3_6.SetActive(false);
                                Panel3_1.SetActive(true);
                            }else if (Panel2_1.activeInHierarchy)
                            {
                                Panel2_1.SetActive(true);
                                Panel3.SetActive(false);
                                Panel4.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel2.activeInHierarchy)
                            {
                                Panel2.SetActive(false);
                                Panel3.SetActive(true);
                                Panel4.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel3.activeInHierarchy)
                            {
                                Panel2.SetActive(false);
                                Panel3.SetActive(false);
                                Panel4.SetActive(true);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel4_1.activeInHierarchy)
                            {
                                Panel4_1.SetActive(true);
                                Panel3.SetActive(false);
                                Panel2.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Panel4.activeInHierarchy)
                            {
                                Panel4.SetActive(false);
                                Panel3.SetActive(false);
                                Panel2.SetActive(false);
                                Quadro1.SetActive(true);
                            }
                            else if (Quadro1.activeInHierarchy)
                            {
                                Quadro1.SetActive(false);
                                Panel4.SetActive(false);
                                Panel3.SetActive(false);
                                Panel2.SetActive(true);
                            }
                        }
                        else if (firstHand.GrabStrength > 0.95)
                        {
                            Debug.Log("isGrabbing-Scroll");
                            if (Panel3_1.activeInHierarchy)
                            {
                                Panel3_1.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel3_2.activeInHierarchy)
                            {
                                Panel3_2.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel3_3.activeInHierarchy)
                            {
                                Panel3_3.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel3_4.activeInHierarchy)
                            {
                                Panel3_4.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel3_5.activeInHierarchy)
                            {
                                Panel3_5.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel3_6.activeInHierarchy)
                            {
                                Panel3_6.SetActive(false);
                                Panel3.SetActive(true);
                            }else if (Panel2_1.activeInHierarchy)
                            {
                                Panel2_1.SetActive(false);
                                Panel2.SetActive(true);
                            }else if (Panel4_1.activeInHierarchy)
                            {
                                Panel4_1.SetActive(false);
                                Panel4.SetActive(true);
                                Panel3.SetActive(false);
                                Panel2.SetActive(false);
                                Quadro1.SetActive(false);
                            }
                            else if (Quadro2.activeInHierarchy)
                            {
                                Quadro2.SetActive(false);
                                Quadro1.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetKey("escape")) Application.Quit();
    }
}
