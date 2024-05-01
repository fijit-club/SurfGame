using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    public static SwipeInput Instance;
    private bool tap, swipeLeft, swipeRight, swipeDown, swipeUp;
    private Vector2 swipeDelta, startTouch;

    private bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool SwipeUp { get { return swipeUp; } }

    private void Awake()
    {
        Instance = this;

    }

    private void Update()
    {
        swipeLeft = swipeRight = swipeDown = swipeUp = false;

        if (Input.touches.Length != 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
            else if(Input.touches[0].phase==TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }

        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            swipeDelta = Input.touches[0].position - startTouch;
        }

        if (swipeDelta.magnitude > 100)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0)
                {
                    swipeRight = true;
                }
                else
                {
                    swipeLeft = true;
                }
            }
            else
            {
                if (y > 0)
                {
                    swipeUp = true;
                }
                else
                {
                    swipeDown = true;
                }
            }
            startTouch = swipeDelta= Vector2.zero;
        }
    }
}
