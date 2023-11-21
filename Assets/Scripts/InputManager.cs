using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Interactions;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


public class InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    public delegate void PerformHoldEvent(Vector2 position, float time);
    public event PerformHoldEvent OnPeformHold;

    private TouchControls touchControls;

    private void Awake()
    {
       touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        touchControls.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        //touchControls.Touch.TouchPress.performed += ctx => StartTouch(ctx);
        //touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

        //touchControls.Touch.TouchHold.performed += ctx => PeformSelect(ctx);
        //touchControls.Touch.TouchHold.started += ctx => PeformSelect(ctx);
        //touchControls.Touch.TouchHold.canceled+= ctx => PeformSelect(ctx);



    }
    
    private void PeformSelect(InputAction.CallbackContext context)
    {
        //Debug.Log(context.interaction);
        //if (OnPeformHold != null)
        //{
        //    OnPeformHold(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        //}
        Debug.Log(context.duration);
        if (context.duration > 0.5)
        {
            Debug.Log("hold");
        }
        else
        {
            Debug.Log("tap");
        }

    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        //if (OnStartTouch != null)
        //{
        //    OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        //}
        Debug.Log(context.duration);                         
        if(context.duration > 0.5)
        {
            Debug.Log("hold");
        }
        else
        {
            Debug.Log("tap");
        }
  
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        //if (OnEndTouch != null)
        //{
        //    OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        //}
        double time = context.time;
    }

    IEnumerator CheckFingerOff(EnhancedTouch.Touch touch)
    {
        while (true)
        {
            Debug.Log(touch.phase);
            yield return new WaitWhile(() => touch.ended == true);
        }
    }

<<<<<<< Updated upstream
=======
<<<<<<< HEAD
=======
    // Update is called once per frame
    void Update()
    {
        foreach (var touch in EnhancedTouch.Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    break;
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    if (touch.time > touch.startTime + 0.5)
                    {
                        OnPeformHold(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime);
                        Debug.Log("hold");
                    }
                    else
                    {
                        OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime);
                        Debug.Log("tap");
                    }
                    break;
            }
        }
    }
>>>>>>> fa4041157eea150cbc489a2dc63667e7be1886e9
>>>>>>> Stashed changes
}
