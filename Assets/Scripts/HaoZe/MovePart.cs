using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class MovePart : MonoBehaviour
{
    public InputManager inputManager;
    private PartSelect partSelect;
    public Camera mainCamera;
    public UiManager uiManager;
    public HideSelectionScript hideSelectionScript;
    //public List<movedObjectData> movedObjectList = new List<movedObjectData>();

    [SerializeField]
    public List<List<movedObjectData>> movedObjectList = new List<List<movedObjectData>>();

    //public delegate void SaveMoveHistory(List<movedObjectData> movedObjectContainer);
    //public static SaveMoveHistory saveMoveHistory;


    [System.Serializable]
    public struct movedObjectData
    {
        public GameObject movedObject;
        public Vector3 movedObjectOriginalPos;

    }

    private void Awake()
    {
        partSelect = gameObject.GetComponent<PartSelect>();
    }

    private void OnEnable()
    {
        inputManager.checkPositionChanged += SaveMove;
        inputManager.OnPerformHold += MovePartPosition;
    }

    private void OnDisable()
    {

    }

    // x = selected
    // y = history
    private bool CheckSelectedInList(int x)
    {
        //Loop list
        for (int y = 0; y < movedObjectList.Count; y++)
        {
            //Loop container
            foreach(movedObjectData container in movedObjectList[y])
            {
                if (partSelect.multiSelectedObjects[x] == container.movedObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void MovePartPosition(Vector3 screenPosition, Transform hit)
    {
        //Move Part 
        //Save Object and position 
        if (uiManager.isMultiSelect)
        {
            //Multiselect
            //Save
            if(movedObjectList.Count > 0)
            {
                List<movedObjectData> movedObjectContainer = new List<movedObjectData>();
                for (int x = 0; x < partSelect.multiSelectedObjects.Count; x++)
                {
                    bool selectedInList = CheckSelectedInList(x);
                    //Save select data if not in list
                    if (selectedInList == false)
                    {
                        movedObjectData data = new movedObjectData();
                        data.movedObject = partSelect.multiSelectedObjects[x];
                        data.movedObjectOriginalPos = partSelect.multiSelectedObjects[x].transform.position;

                        movedObjectContainer.Add(data);
                    }
                }
                if (movedObjectContainer.Count > 0)
                {
                    movedObjectList.Add(movedObjectContainer);
                    //saveMoveHistory(movedObjectContainer);
                }
            }
            else
            {
                List<movedObjectData> movedObjectContainer = new List<movedObjectData>();
                foreach(GameObject selectObject in partSelect.multiSelectedObjects)
                {
                    movedObjectData data = new movedObjectData();
                    data.movedObject = selectObject;
                    data.movedObjectOriginalPos = selectObject.transform.position;
                    movedObjectContainer.Add(data);
                }
                movedObjectList.Add(movedObjectContainer);
                //saveMoveHistory(movedObjectContainer);
            }
            

            //Move
            foreach (GameObject selectedObj in partSelect.multiSelectedObjects)
            {
                if(selectedObj.transform != hit)
                {
                    selectedObj.transform.parent = hit.transform;
                }
            }

            Vector3 offset = screenPosition - hit.transform.position;
            hit.transform.position += offset;
            //Unparent child of hit back to model
            hideSelectionScript.UnparentToCorrectStructure(hit.transform, hideSelectionScript.mainModel.transform);
        }
        else
        {
            //Save
            if (movedObjectList.Count > 0)
            {
                bool CheckIfInList()
                {
                    for (int i = 0; i < movedObjectList.Count; i++)
                    {
                        //LOOP CONTAINERS IN LIST
                        for (int x = 0; x < movedObjectList[i].Count; x++)
                        {
                            if (movedObjectList[i][x].movedObject == partSelect.selectedObject)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                bool inList = CheckIfInList();
                if (!inList)
                {
                    List<movedObjectData> movedObjectContainer = new List<movedObjectData>();
                    movedObjectData data = new movedObjectData();
                    data.movedObject = partSelect.selectedObject;
                    data.movedObjectOriginalPos = partSelect.selectedObject.transform.position;
                    movedObjectContainer.Add(data);
                    //saveMoveHistory(movedObjectContainer);
                    movedObjectList.Add(movedObjectContainer);
                }
            }
            else
            {
                List<movedObjectData> movedObjectContainer = new List<movedObjectData>();
                movedObjectData data = new movedObjectData();
                data.movedObject = partSelect.selectedObject;
                data.movedObjectOriginalPos = partSelect.selectedObject.transform.position;
                movedObjectContainer.Add(data);
                movedObjectList.Add(movedObjectContainer);
                //saveMoveHistory(movedObjectContainer);
            }

            //Move
            Vector3 offset = screenPosition - hit.transform.position;
            hit.transform.position += offset;
        }
    }

    public void ToggleMove(Transform hit, GameObject[] movedObjects, Vector3 currentPos, Vector3 originPos) 
    {
        Debug.Log("Toggle");
        foreach (var obj in movedObjects)
        {
            if (obj.transform != hit)
            {
                obj.transform.parent = hit.transform;
            }
        }

        if(hit.transform.position == currentPos)
        {
            hit.transform.position = originPos;
            hideSelectionScript.UnparentToCorrectStructure(hit, hideSelectionScript.mainModel.transform);
            return;
        }
        if(hit.transform.position == originPos)
        {
            hit.transform.position = currentPos;
            hideSelectionScript.UnparentToCorrectStructure(hit, hideSelectionScript.mainModel.transform);
            return;
        }
        hideSelectionScript.UnparentToCorrectStructure(hit, hideSelectionScript.mainModel.transform);
    }

    public void UndoMove(GameObject[] movedObjects, Transform hit, Vector3 originPos)
    {
        foreach (var obj in movedObjects)
        {
            if (obj.transform != hit)
            {
                obj.transform.parent = hit.transform;
            }
        }

        hit.transform.position = originPos;
        hideSelectionScript.UnparentToCorrectStructure(hit, hideSelectionScript.mainModel.transform);
    }

    public void UndoAllMove()
    {
        if (movedObjectList.Count > 0)
        {
            for(int i = 0; i < movedObjectList.Count; i++)
            {
               for(int x = 0; x < movedObjectList[i].Count; x++)
                {
                    movedObjectList[i][x].movedObject.transform.position = movedObjectList[i][x].movedObjectOriginalPos;
                }
            }
            movedObjectList.Clear();
        }
    }

    public void SaveMove()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
