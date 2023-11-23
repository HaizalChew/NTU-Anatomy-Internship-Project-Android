using UnityEditor;
using UnityEngine;

public class IsolateSelectionScript : MonoBehaviour
{
    public GameObject mainModel;

    private Camera mainCamera;
    private PartSelect partSelect;
    private UiManager uiManager;

    public bool isIsolate;
    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
        partSelect = gameObject.GetComponent<PartSelect>();
        uiManager = gameObject.GetComponent<UiManager>();
    }

    public void IsolateSelection()
    {
        //Get MultiSelect Mode 
        bool isMultiSelect = uiManager.isMultiSelect;
        //Check if already isolated 
        if (isIsolate)
        {
            //Deactivate
            isIsolate = !isIsolate;
            foreach (Transform child in mainModel.transform)
            {
                child.gameObject.SetActive(!isIsolate);
            }
        }
        else
        {
            //Check if there is selection to isolate
            bool isSelectedNull = CheckSelectedNull(isMultiSelect);
            if (!isSelectedNull)
            {
                //Activate
                //Hide all child objects
                isIsolate = !isIsolate;
                foreach (Transform child in mainModel.transform)
                {
                    child.gameObject.SetActive(!isIsolate);
                }
                //Get selected objects and set active
                Renderer[] selectedArray = CreateSelectionArray(isMultiSelect);
                foreach (Renderer selected in selectedArray)
                {
                    selected.gameObject.SetActive(true);
                }
            }
        }
    }

    private bool CheckSelectedNull(bool IsMultiSelect)
    {
        if (IsMultiSelect)
        {
            if(partSelect.multiSelectedObjects.Count == 0)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (partSelect.selectedObject is null)
            {
                return true;
            }
            return false;
        }
    }

    private Renderer[] CreateSelectionArray(bool isMultiSelect)
    {
        if (isMultiSelect)
        {
            Renderer[] array = new Renderer[partSelect.multiSelectedObjects.Count];
            for (int i = 0; i < partSelect.multiSelectedObjects.Count; i++)
            {
                array[i] = partSelect.multiSelectedObjects[i].GetComponent<Renderer>();
            }
            return array;
        }
        else
        {
            Renderer[] array = new Renderer[1];
            array[0] = partSelect.selectedObject.GetComponent<Renderer>();
            return array;
        }
    }

}
