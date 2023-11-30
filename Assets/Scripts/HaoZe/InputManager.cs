using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    public delegate void PerformHoldEvent(Vector2 position, float time);
    public event PerformHoldEvent OnPerformHold;

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

        //touchControls.Touch.TouchHold.performed += ctx => PerformSelect(ctx);
        //touchControls.Touch.TouchHold.started += ctx => PerformSelect(ctx);
        //touchControls.Touch.TouchHold.canceled+= ctx => PerformSelect(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {


    }

    private void EndTouch(InputAction.CallbackContext context)
    {

    }

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

                    }
                    else
                    {
                        OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime);
                    }
                    break;
            }
        }
    }
}
