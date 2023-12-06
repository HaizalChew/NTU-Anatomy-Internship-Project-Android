using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovePart : MonoBehaviour
{
    public InputManager inputManager;
    private PartSelect partSelect;
    public Camera mainCamera;
    public UiManager uiManager;
    public List<movedObjectData> movedObjectList = new List<movedObjectData>();

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
    }

    private void OnDisable()
    {
        inputManager.OnPerformHold -= MovePartPosition;
    }

    // x = selected
    // y = history
    private bool CheckSelectedInList(int x)
    {
        for (int y = 0; y < movedObjectList.Count; y++)
        {
            if (partSelect.multiSelectedObjects[x] == movedObjectList[y].movedObject)
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
            //Save
            if(movedObjectList.Count > 0)
            {
                for (int x = 0; x < partSelect.multiSelectedObjects.Count; x++)
                {
                    bool selectedInList = CheckSelectedInList(x);
                    //Save select data if not in list
                    if (selectedInList == false)
                    {
                        movedObjectData data = new movedObjectData();
                        data.movedObject = partSelect.multiSelectedObjects[x];
                        data.movedObjectOriginalPos = partSelect.multiSelectedObjects[x].transform.position;
                        movedObjectList.Add(data);
                    }
                }
            }
            else
            {
                foreach(GameObject selectObject in partSelect.multiSelectedObjects)
                {
                    movedObjectData data = new movedObjectData();
                    data.movedObject = selectObject;
                    data.movedObjectOriginalPos = selectObject.transform.position;
                    movedObjectList.Add(data);
                }
            }
            

            //Move
            foreach (GameObject selectedObj in partSelect.multiSelectedObjects)
            {
                if(selectedObj.transform != hit)
                {
                    selectedObj.transform.parent = hit.transform;
                }
            }

            float worldZ = mainCamera.WorldToScreenPoint(hit.transform.position).z;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
            Vector3 offset = worldPos - hit.transform.position;
            hit.transform.position += offset;
            hit.transform.DetachChildren();
        }
        else
        {
            //Save
            if(movedObjectList.Count > 0)
            {
                //Check if select is in list
                bool inList = true;
                for(int i = 0; i < movedObjectList.Count; i++)
                {
                    if(movedObjectList[i].movedObject == partSelect.selectedObject)
                    {
                        inList = true;
                        break;
                    }
                    inList = false;
                }
                if(inList == false)
                {
                    movedObjectData data = new movedObjectData();
                    data.movedObject = partSelect.selectedObject;
                    data.movedObjectOriginalPos = partSelect.selectedObject.transform.position;
                    movedObjectList.Add(data);
                }
            }
            else
            {
                movedObjectData data = new movedObjectData();
                data.movedObject = partSelect.selectedObject;
                data.movedObjectOriginalPos = partSelect.selectedObject.transform.position;
                movedObjectList.Add(data);
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
        if(movedObjectList.Count > 0)
        {
            int count = movedObjectList.Count - 1;
            movedObjectList[count].movedObject.transform.position = movedObjectList[count].movedObjectOriginalPos;
            movedObjectList.Remove(movedObjectList[count]);
        }
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
