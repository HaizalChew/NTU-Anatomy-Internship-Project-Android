using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovePart : MonoBehaviour
{
    public InputManager inputManager;
    private PartSelect partSelect;
    public Camera mainCamera;
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

    private void MovePartPosition(Vector2 screenPosition, float time, Vector2 deltaPos)
    {
        //Get MoveData to save
        Debug.Log("Start");
        bool inList = false;
        if (movedObjectList.Count > 0)
        {
            foreach (movedObjectData movedObjectData in movedObjectList)
            {
                Debug.Log(movedObjectData.movedObject);
                Debug.Log(partSelect.selectedObject);
                if (movedObjectData.movedObject == partSelect.selectedObject)
                {
                    inList = true;
                    break;
                }
            }
            if(inList == false)
            {
                SaveMoveHistory();
                Debug.Log("saved");
            }
        }
        else
        {
            SaveMoveHistory();
            Debug.Log("yte");
        }

        //Move Part
        float worldZ = mainCamera.WorldToScreenPoint(partSelect.selectedObject.transform.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
        Vector3 offset = partSelect.selectedObject.transform.position - worldPos;
        partSelect.selectedObject.transform.position = worldPos;
    }

    private void SaveMoveHistory()
    {
        movedObjectData objectData = new movedObjectData();
        objectData.movedObject = partSelect.selectedObject;
        objectData.movedObjectOriginalPos = partSelect.selectedObject.transform.position;
        movedObjectList.Add(objectData);
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
