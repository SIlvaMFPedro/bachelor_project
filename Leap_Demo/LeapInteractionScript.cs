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
    
    // Use this for initialization
    void Start()
    {
        controller = new Controller();
        flagSwipe = false;
    }

    void FixedUpdate()
    {
        Frame frame = controller.Frame();
        /* Works in +/- 60fps; 10 tested for better performance */
        Frame oldframe = controller.Frame(10); 

        /* current frame != old frame */
        if (!frame.Equals(oldframe))
        {
        	/* Checks for hand over the sensor */
            if (oldframe.Hands.Count > 0 && (oldframe.Hands[0].IsRight || oldframe.Hands[0].IsLeft))
            {
                /* Hand is detected */
                Hand oldHand = oldframe.Hands[0];
            	/* Checks for hand over the sensor in the newest frame for comparison */
                if (frame.Hands.Count > 0 && (frame.Hands[0].IsRight || frame.Hands[0].IsLeft))
                {
                	/* Hand is detected in newest frame for comparison*/
                    Hand firstHand = frame.Hands[0];
                    /* Checks if it's the same hand making the movement */
                    if (oldHand.Id == firstHand.Id)
                    {
                    	/* To detect a Swipe movement */
                        if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) > 80 && (firstHand.PalmPosition.x - oldHand.PalmPosition.x) > 0 && !flagSwipe)
                        {
                            Debug.Log("isSwiping Right");
                            flagSwipe = true;
                            /* Actions/Changes on the Canvas goes here */
                        }
                        /* To reset the flag of the swipe movement, if the hand doesn't check the swipe movements (end of swipe /no swipe) */
                        else if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) <= 5)
                        {
                            flagSwipe = false;
                        }
                        /* To detect a Swipe movement */
                        else if (oldHand.PalmPosition.DistanceTo(firstHand.PalmPosition) > 80 && (firstHand.PalmPosition.x - oldHand.PalmPosition.x) < 0 && !flagSwipe)
                        {
                            Debug.Log("isSwiping Left");
                            flagSwipe = true;
                        	/* Actions/Changes on the Canvas goes here */
                        }
                        /* To detect if Hand is making a grabbing movement */
                        else if (firstHand.GrabStrength > 0.95)
                        {
                            Debug.Log("isGrabbing-Scroll");
                            /* Actions/Changes on the Canvas goes here */
                        }
                    }
                }
            }
        }
        /* To close the application press key "Esc" */
        if (Input.GetKey("escape")) Application.Quit();
    }
}
