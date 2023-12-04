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

    public delegate void PerformHoldEvent(Vector2 position, float time, Vector2 deltaPos);
    public event PerformHoldEvent OnPerformHold;

    private TouchControls touchControls;

    public bool isMoveSelected;

    public Camera mainCamera;
    public PartSelect partSelect;
    public CameraControls camControls;

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
                    //if move mode is true
                    //Get object position 
                    Debug.Log("Began");
                    Ray ray = mainCamera.ScreenPointToRay(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
                    RaycastHit hit;
                    isMoveSelected = false;
                    if(partSelect.selectedObject != null)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            Debug.Log(hit.transform.gameObject);
                            if (hit.transform.gameObject == partSelect.selectedObject)
                            {
                                isMoveSelected = true;
                            }
                            else
                            {
                                isMoveSelected = false;
                            }
                        }
                    }
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if(touch.time > touch.startTime + 0.5 && isMoveSelected == true)
                    {
                        camControls.enabled = false;
                        OnPerformHold(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime, touch.delta);
                    }
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    camControls.enabled = true;
                    if (touch.time > touch.startTime + 0.5)
                    {
                        //Perform hold 
                        
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
