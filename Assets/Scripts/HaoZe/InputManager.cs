using System;
using System.Collections.Generic;
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

    public delegate void PerformHoldEvent(Vector3 position, Transform hit);
    public event PerformHoldEvent OnPerformHold;

    public delegate void CheckPositionChanged();
    public event CheckPositionChanged checkPositionChanged;

    private TouchControls touchControls;

    public bool isMoveSelected;

    public Camera mainCamera;
    public PartSelect partSelect;
    public CameraControls camControls;
    public UiManager uiManager;
    public MovePart movePart;

    //Move Data to send
    [Header("MoveCommandData")]
    public GameObject[] movedObjects;
    public Transform transformHit;
    public Vector3 currentPos;
    public Vector3 originPos;

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
                                        //Get hit transform
                                        transformHit = hit.transform;
                                        isMoveSelected = true;
                                        //Get MovedObjs
                                        movedObjects = ConvertSelectedObjectsToArray(null,partSelect.multiSelectedObjects);
                                        //Get originPos
                                        originPos = hit.transform.position;
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
                                    transformHit = hit.transform;
                                    isMoveSelected = true;
                                    movedObjects = ConvertSelectedObjectsToArray(partSelect.selectedObject);
                                    originPos = hit.transform.position;
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
                        //Get ScreenPos and transform
                        Vector2 screenPosition = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
                        float worldZ = mainCamera.WorldToScreenPoint(transformHit.transform.position).z;
                        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
                        OnPerformHold(worldPos, transformHit);
                        currentPos = worldPos;
                    }
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    camControls.enabled = true;
                    //Check if posiiton different from last time if yes save history
                    checkPositionChanged();
                    if (touch.time > touch.startTime + 0.5 && isMoveSelected == true)
                    {
                        //Finish Move 
                        //Save Move Command
                        SaveMoveCommand();

                    }
                    else
                    {
                        OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)touch.startTime);
                    }
                    break;
            }
        }
    }

    public void SaveMoveCommand()
    {
        ICommand command = new MoveCommand(originPos, currentPos, movedObjects, transformHit, movePart);
        CommandInvoker.ExecuteSave(command);
    }

    public GameObject[] ConvertSelectedObjectsToArray(GameObject obj = null, List<GameObject> objList = null)
    {
        if (obj != null)
        {
            GameObject[] array = new GameObject[1];
            array[0] = obj;
            return array;
        }
        if (objList != null)
        {
            GameObject[] array = new GameObject[objList.Count];
            for(int i = 0; i < objList.Count; i++)
            {
                array[i] = objList[i];
            }
            return array;
        }
        return null;
    }
}
