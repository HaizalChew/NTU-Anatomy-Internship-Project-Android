using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class HideSelectionScript : MonoBehaviour
{
    // Singleton references
    [Header("Singleton")]
    [SerializeField] UiManager uiManagerScript;
    // ====================================== //

    // Script References to Self
    private Camera mainCamera;
    private GameObject historyCell;
    private PartSelect partSelect;
    private RenderOutline renderOutline;
    // ====================================== //


    [Header("Others")]
    public GameObject mainModel;
    public GameObject historyContainer;
    // ====================================== //


    private GameObject clone;
    //List<Renderer> renderObjectList = new List<Renderer>();
    List<GameObject> selectedObjectList = new List<GameObject>();
    private void Awake()
    {
        mainCamera = Camera.main;
        renderOutline = mainCamera.GetComponent<RenderOutline>();
        partSelect = gameObject.GetComponent<PartSelect>();
    }
    
    public void HideSelection()
    {
        //Check if got selection
        bool isMultiSelect = uiManagerScript.isMultiSelect;
        PartSelect.SelectionState state = partSelect.IsSelectionNull();
        if (isMultiSelect)
        {
            if (state.multiSelect == false)
            {
                if (partSelect.multiSelectedObjects.Count > 0)
                {
                    selectedObjectList = partSelect.multiSelectedObjects;
                    renderOutline.RenderObject.Clear();
                }
            }
        }
        else
        {
            if (state.singleSelect == false)
            {
                selectedObjectList.Add(partSelect.selectedObject);
                partSelect.selectedObject = null;
                renderOutline.RenderObject.Clear();
            }
        }

        //Create history cell
        if (selectedObjectList.Count > 0)
        {
            Debug.Log("creating");
            int count = historyContainer.transform.childCount + 1;
            historyCell = new GameObject("Undo " + count);
            historyCell.transform.parent = historyContainer.transform;
            for(int i = 0; i < selectedObjectList.Count; i++)
            {
                selectedObjectList[i].gameObject.SetActive(false);
                selectedObjectList[i].transform.parent = historyCell.transform;
                uiManagerScript.UpdateHistoryCount();
            }
            Debug.Log("here?");
            selectedObjectList.Clear();
        }
        //Update Undo Counter
        StartCoroutine(uiManagerScript.UpdateHistoryCount());
        //Update History Panel
        Debug.Log("here??");
        uiManagerScript.UpdateHistoryPanel();
    }

    public void UnhideSelection(bool reset = false)
    {
        int historyChildCount = historyContainer.transform.childCount;
        if(historyChildCount != 0)
        {
            Transform container = historyContainer.transform.GetChild(historyChildCount - 1);
            UnparentToMainModel(container, mainModel.transform);
            DestroyImmediate(container.gameObject);
        }
        if (!reset)
        {
            //Update Undo Counter
            StartCoroutine(uiManagerScript.UpdateHistoryCount());
            uiManagerScript.UpdateHistoryPanel();
        }
    }

    public void UnparentToMainModel(Transform parent, Transform mainModel)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(true);
            list.Add(parent.GetChild(i).gameObject);
        }
        foreach (GameObject child in list)
        {
            child.transform.parent = mainModel.transform;
        }
    }

    public void HistoryShowHide(Transform undoContainer)
    {
        foreach (Transform childObject in undoContainer)
        {
            if (childObject.gameObject.activeSelf == true)
            {
                childObject.gameObject.SetActive(false);
            }
            else
            {
                childObject.gameObject.SetActive(true);
            }
        }
    }
    public int GetUndoCounter()
    {
        int undoCounter = historyContainer.transform.childCount;
        return undoCounter;
    }

    public void ResetUnHide()
    {
        //Check for all history 
        int childCount = historyContainer.transform.childCount;
        if(childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                Debug.Log(i);
                UnhideSelection(true);
                StartCoroutine(WaitForDestroy());
                continue;
            }
        }
        StartCoroutine(uiManagerScript.UpdateHistoryCount());
        uiManagerScript.UpdateHistoryPanel();
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForEndOfFrame();
    }

}
