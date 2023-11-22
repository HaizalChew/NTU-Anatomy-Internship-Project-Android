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
    SelectRenderOutline renderOutlineScript;
    UiManager uiManagerScript;
    List<Renderer> renderObjectList = new List<Renderer>();

    private void Awake()
    {
        mainCamera = Camera.main;
        renderOutlineScript = mainCamera.GetComponent<SelectRenderOutline>();
        uiManagerScript = gameObject.GetComponent<UiManager>();
    }
    
    public void HideSelection()
    {
        renderObjectList = renderOutlineScript.SelectRenderObject;
        if (renderObjectList.Count != 0)
        {
            historyCell = new GameObject("Container");
            historyCell.transform.parent = historyContainer.transform;
            for(int i = 0; i < renderObjectList.Count; i++)
            {
                renderObjectList[i].gameObject.SetActive(false);
                Instantiate(renderObjectList[i].gameObject, historyCell.transform);
                Destroy(renderObjectList[i].gameObject);
                uiManagerScript.UpdateHistoryCount();
            }
            renderObjectList.Clear();
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
