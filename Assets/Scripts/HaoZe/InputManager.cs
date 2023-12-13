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

    public delegate void PerformHoldEvent(Vector2 position, float time, Transform hit);
    public event PerformHoldEvent OnPerformHold;

    public delegate void CheckPositionChanged();
    public event CheckPositionChanged checkPositionChanged;

    private TouchControls touchControls;

    public bool isMoveSelected;

    public Camera mainCamera;
    public PartSelect partSelect;
    public CameraControls camControls;
    public UiManager uiManager;

    private Transform transformHit;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
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
                    Ray ray = mainCamera.ScreenPointToRay(touchControls.Touch.TouchPosition.ReadValue<Vector2>());
                    RaycastHit hit;
                    isMoveSelected = false;
                    if (uiManager.isMultiSelect)
                    {
                        if(partSelect.multiSelectedObjects.Count > 0)
                        {
                            if(Physics.Raycast(ray,out hit))
                            {
                                foreach (GameObject selectedObj in partSelect.multiSelectedObjects)
                                {
                                    if(selectedObj == hit.transform.gameObject)
                                    {
                                        transformHit = hit.transform;
                                        isMoveSelected = true;
                                        break;
                                    }
                                    else
                                    {
                                        isMoveSelected = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (partSelect.selectedObject != null)
                        {
                            if (Physics.Raycast(ray, out hit))
                            {
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
                    }
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if(touch.time > touch.startTime + 0.5 && isMoveSelected == true)
                    {
                        camControls.enabled = false;
                        OnPerformHold(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime, transformHit);
                    }
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    camControls.enabled = true;
                    Debug.Log("end");
                    //Check if posiiton different from last time if yes save history
                    checkPositionChanged();
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
