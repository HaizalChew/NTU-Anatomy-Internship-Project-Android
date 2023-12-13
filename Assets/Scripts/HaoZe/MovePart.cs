using System.Collections.Generic;
using UnityEngine;

public class MovePart : MonoBehaviour
{
    public InputManager inputManager;
    private PartSelect partSelect;
    public Camera mainCamera;
    public UiManager uiManager;
    public HideSelectionScript hideSelectionScript;
    public SaveHistory saveHistory;

    [SerializeField]
    public List<List<movedObjectData>> movedObjectList = new List<List<movedObjectData>>();

    public delegate void SaveMoveHistory(List<movedObjectData> movedObjectContainer);
    public static SaveMoveHistory saveMoveHistory;

    private Transform hitTransform;
    public List<GameObject> movedObjects = new List<GameObject>();
    public Vector3 hitCurrentPosition;

    public List<originalData> originalPositions = new List<originalData>();

    [System.Serializable]
    public struct originalData
    {
        public GameObject movedObject;
        public Vector3 originalPosition;
    }

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
        inputManager.OnPerformHold += MovePartPosition;
        inputManager.checkPositionChanged += SaveMove;
    }

    private void OnDisable()
    {
        inputManager.OnPerformHold -= MovePartPosition;
    }

    // x = selected
    // y = history
    private bool CheckSelectedInList(int x)
    {
        //Loop list
        for (int y = 0; y < movedObjectList.Count; y++)
        {
            //Loop container
            foreach (movedObjectData container in movedObjectList[y])
            {
                if (partSelect.multiSelectedObjects[x] == container.movedObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool isPositionSavedBefore(int i)
    {
        foreach(originalData data in originalPositions)
        {
            if (partSelect.multiSelectedObjects[i] == data.movedObject)
            {
                return true;
            }
        }
        return false;
    }

    private void MovePartPosition(Vector2 screenPosition, float time, Transform hit)
    {
        //Move Part 
        //Save Object and position 
        if (uiManager.isMultiSelect)
        {
            //Multiselect

            //Saved Orginal Posiiton if havent
            if(originalPositions.Count > 0)
            {
                for (int i = 0; i < partSelect.multiSelectedObjects.Count; i++)
                {
                    bool isSavedBefore = isPositionSavedBefore(i);
                    if (!isSavedBefore)
                    {
                        //Save originalPos
                        originalData originalData = new originalData();
                        originalData.movedObject = partSelect.multiSelectedObjects[i];
                        originalData.originalPosition = partSelect.multiSelectedObjects[i].transform.position;
                        originalPositions.Add(originalData);
                    }
                }
            }
            else
            {
                foreach (GameObject selected in partSelect.multiSelectedObjects)
                {
                    //Save originalPos
                    originalData originalData = new originalData();
                    originalData.movedObject = selected;
                    originalData.originalPosition = selected.transform.position;
                    originalPositions.Add(originalData);
                }
            }


            //Move
            //Get hitTransform to move selectedobject together
            foreach (GameObject selectedObj in partSelect.multiSelectedObjects)
            {
                if (selectedObj.transform != hit)
                {
                    selectedObj.transform.parent = hit.transform;
                }
            }

            float worldZ = mainCamera.WorldToScreenPoint(hit.transform.position).z;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
            Vector3 offset = worldPos - hit.transform.position;
            hit.transform.position += offset;
            //Store moveData
            hitTransform = hit.transform;
            hitCurrentPosition = worldPos;
            movedObjects = partSelect.multiSelectedObjects;

            //Unparent child of hit back to model
            hideSelectionScript.UnparentToMainModel(hit.transform, hideSelectionScript.mainModel.transform);
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
            float worldZ = mainCamera.WorldToScreenPoint(partSelect.selectedObject.transform.position).z;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
            Vector3 offset = partSelect.selectedObject.transform.position - worldPos;
            partSelect.selectedObject.transform.position = worldPos;
        }
    }

    public void UndoMove()
    {
        if (movedObjectList.Count > 0)
        {
            int count = movedObjectList.Count - 1;
            //Get Data from Last Container of list
            foreach (movedObjectData data in movedObjectList[count])
            {
                data.movedObject.transform.position = data.movedObjectOriginalPos;
            }
            //Remove last Container when done
            movedObjectList.Remove(movedObjectList[count]);
        }
    }

    public void UndoAllMove()
    {
        if (movedObjectList.Count > 0)
        {
            for (int i = 0; i < movedObjectList.Count; i++)
            {
                for (int x = 0; x < movedObjectList[i].Count; x++)
                {
                    movedObjectList[i][x].movedObject.transform.position = movedObjectList[i][x].movedObjectOriginalPos;
                }
            }
            movedObjectList.Clear();
        }
    }

    public void SaveMove()
    {
        //MovedObjects
        //hit Transform
        //CurrentPositon
        GameObject[] box = new GameObject[movedObjects.Count];
        for (int i = 0; i < box.Length; i++)
        {
            box[i] = movedObjects[i];
        }

        Debug.Log("Send");
        saveHistory.TransferMoveData(box, hitTransform, hitCurrentPosition, inputManager.startPos);
        saveHistory.StoreNewData();
        movedObjectList.Clear();
        hitTransform = null;
        hitCurrentPosition = Vector3.zero;
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
