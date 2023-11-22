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
    List<Renderer> renderObjectList = new List<Renderer>();

    private void Awake()
    {
        mainCamera = Camera.main;
        renderOutlineScript = mainCamera.GetComponent<SelectRenderOutline>();
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
            }
            renderObjectList.Clear();
        }
    }

    public void UnhideSelection()
    {
        if(historyContainer.transform.childCount != 0)
        {
            Transform container = historyContainer.transform.GetChild(0);
            foreach (Transform child in container)
            {
                child.gameObject.SetActive(true);
                Instantiate(child.gameObject, mainModel.transform);
            }
            Destroy(container.gameObject);
        }
    }

}
