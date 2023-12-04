using System.Collections.Generic;
using UnityEngine;

public class PartSelect : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private RenderOutline outlineScript;
    private OutlineSelectedPart outlineSelectedPart;
    private SortAndConvertList convertScript;
    public InputManager inputManager;
    public UiManager uiManager;

    public GameObject selectedObject;
    public List<GameObject> multiSelectedObjects = new List<GameObject>();
    private GameObject currentSelectedObject;

   
    private struct TouchData
    {
        public bool isSelectable;
        public List<KeyValuePair<RaycastHit, float>> hitList;
    }

    public struct SelectionState
    {
        public bool singleSelect;
        public bool multiSelect;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
        outlineScript = mainCamera.GetComponent<RenderOutline>();
        outlineSelectedPart = gameObject.GetComponent<OutlineSelectedPart>();
        convertScript = gameObject.GetComponent<SortAndConvertList>();
        uiManager = gameObject.GetComponent<UiManager>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += ToggleSelectPart;
        inputManager.OnStartTouch += outlineSelectedPart.OutlineSelected;
    }

    private void OnDisable()
    {
        inputManager.OnEndTouch -= ToggleSelectPart;
    }

    public void ToggleSelectPart(Vector2 screenPosition, float time)
    {
        //Assign currentSelectedObject for checking later
        if (selectedObject != null)
        {
            currentSelectedObject = selectedObject;
        }
        TouchData touchData = GetTouchData(screenPosition);
        if (touchData.isSelectable)
        {
            //Select Part
            for (int i = 0; i < touchData.hitList.Count; i++)
            {
                RaycastHit hitData = touchData.hitList[i].Key;
                //Detect if hit object is transparent
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 3)
                {
                    selectedObject = hitData.transform.gameObject;
                    continue;
                }
                //Detect if hit object is opaque
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 0)
                {
                    selectedObject = hitData.transform.gameObject;
                    break;
                }
            }
            //Unselect Part 
            if (selectedObject == currentSelectedObject)
            {
                selectedObject = null;
                currentSelectedObject = null;
            }
        }
        //Display select text

        uiManager.DisplaySelectName(uiManager.isMultiSelect);
    }

    public void ToggleMultiSelect(Vector2 screenPosition, float time)
    {
        GameObject chosenObject = null;
        TouchData touchData = GetTouchData(screenPosition);
        if (touchData.isSelectable)
        {
            //Select Part
            for (int i = 0; i < touchData.hitList.Count; i++)
            {
                RaycastHit hitData = touchData.hitList[i].Key;
                //Detect if hit object is transparent
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 3)
                {
                    chosenObject = hitData.transform.gameObject;
                    continue;
                }
                //Detect if hit object is opaque
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 0)
                {
                    chosenObject = hitData.transform.gameObject;
                    break;
                }
            }
            bool alrSelected = false;
            //Check if selected alr
            Debug.Log("crash");
            if (multiSelectedObjects.Count > 0)
            {
                //Unselect Part 
                foreach (GameObject alrSelect in multiSelectedObjects)
                {
                    if (alrSelect == chosenObject)
                    {
                        multiSelectedObjects.Remove(alrSelect);
                        alrSelected = true;
                        break;
                    }
                }
                Debug.Log("crash");
            }
            if(!alrSelected)
            {
                //Add selected object to List
                multiSelectedObjects.Add(chosenObject);
            }
        }

        uiManager.DisplaySelectName(uiManager.isMultiSelect);
    }

    private TouchData GetTouchData(Vector2 screenPosition)
    {
        TouchData data = new TouchData();
        //Get worldPos from screenPos
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane);
        //Raycast to worldPos
        var ray = mainCamera.ScreenPointToRay(screenCoordinates);
        //Check if raycast hit selectable
        bool hitSelectable = Physics.Raycast(ray, out var hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable");
        if (hitSelectable)
        {
            //Another raycast to get all hitobject data behind
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);
            //Convert array to sorted List based on length
            List<KeyValuePair<RaycastHit, float>> objectData = convertScript.GetShortLongHit(hits);
            data.isSelectable = hitSelectable;
            data.hitList = objectData;
            return data;
        }
        return data;
    }

    public SelectionState IsSelectionNull()
    {
        SelectionState state = new SelectionState();
        if(multiSelectedObjects.Count > 0)
        {
            state.multiSelect = false;
        }
        else
        {
            state.multiSelect = true;
        }
        if(selectedObject != null)
        {
            state.singleSelect = false;
        }
        else
        {
            state.singleSelect = true;
        }
        return state;
    }

    public void ClearSelection()
    {
        if (uiManager.isMultiSelect)
        {
            multiSelectedObjects.Clear();
        }
        else
        {
            selectedObject = null;
        }
    }
}
