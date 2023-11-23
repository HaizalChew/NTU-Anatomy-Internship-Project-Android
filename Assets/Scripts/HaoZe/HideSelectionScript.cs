using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class HideSelectionScript : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject mainModel;
    public GameObject historyContainer;
    private GameObject historyCell;
    private UiManager uiManagerScript;
    private PartSelect partSelect;
    private RenderOutline renderOutline;
    //List<Renderer> renderObjectList = new List<Renderer>();
    List<GameObject> selectedObjectList = new List<GameObject>();
    private void Awake()
    {
        mainCamera = Camera.main;
        uiManagerScript = gameObject.GetComponent<UiManager>();
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
                Debug.Log("yes");
                Debug.Log(state.multiSelect);
                if (partSelect.multiSelectedObjects.Count > 0)
                {
                    Debug.Log(selectedObjectList);
                    selectedObjectList = partSelect.multiSelectedObjects;
                    Debug.Log(selectedObjectList);
                    renderOutline.RenderObject.Clear();
                    Debug.Log(selectedObjectList.Count);
                }
            }
        }
        else
        {
            if (state.singleSelect == false)
            {
                Debug.Log("yes");
                Debug.Log(state.singleSelect);
                selectedObjectList.Add(partSelect.selectedObject);
                partSelect.selectedObject = null;
                renderOutline.RenderObject.Clear();
            }
        }
        Debug.Log("here");
        Debug.Log(selectedObjectList.Count);

        //Create history cell
        if (selectedObjectList.Count > 0)
        {
            Debug.Log("creating");
            historyCell = new GameObject("Container");
            historyCell.transform.parent = historyContainer.transform;
            for(int i = 0; i < selectedObjectList.Count; i++)
            {
                selectedObjectList[i].gameObject.SetActive(false);
                Instantiate(selectedObjectList[i].gameObject, historyCell.transform);
                Destroy(selectedObjectList[i].gameObject);
                uiManagerScript.UpdateHistoryCount();
            }
            selectedObjectList.Clear();
        }
        //Update Undo Counter
        StartCoroutine(uiManagerScript.UpdateHistoryCount());
    }

    public void UnhideSelection(bool reset = false)
    {
        int childCount = historyContainer.transform.childCount;
        if(childCount != 0)
        {
            Transform container = historyContainer.transform.GetChild(childCount - 1);
            foreach (Transform child in container)
            {
                child.gameObject.SetActive(true);
                Instantiate(child.gameObject, mainModel.transform);
            }
            DestroyImmediate(container.gameObject);
        }
        if (!reset)
        {
            //Update Undo Counter
            StartCoroutine(uiManagerScript.UpdateHistoryCount());
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
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForEndOfFrame();
    }

}
