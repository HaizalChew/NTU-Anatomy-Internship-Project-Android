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
    
    public void HideSelection(GameObject[] hideObjects)
    {
        foreach(GameObject objects in hideObjects)
        {
            objects.SetActive(false);
        }
    }

    public void UnhideSelection(GameObject[] hideObjects)
    {
        foreach (GameObject objects in hideObjects)
        {
            objects.SetActive(true);
        }
    }

    public void UnparentToCorrectStructure(Transform parent, Transform mainModel)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(true);
            list.Add(parent.GetChild(i).gameObject);
        }
        foreach (GameObject child in list)
        {
            //if child layer == Check main model structure layer
            foreach(Transform structure in mainModel.transform)
            {
                Debug.Log(structure);
                if(child.layer == structure.gameObject.layer)
                {
                    child.transform.parent = structure.transform;
                    break;
                }
            }
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

    //public void ResetUnHide()
    //{
    //    //Check for all history 
    //    int childCount = historyContainer.transform.childCount;
    //    if(childCount > 0)
    //    {
    //        for (int i = 0; i < childCount; i++)
    //        {
    //            Debug.Log(i);
    //            UnhideSelection(true);
    //            StartCoroutine(WaitForDestroy());
    //            continue;
    //        }
    //    }
    //    StartCoroutine(uiManagerScript.UpdateHistoryCount());
    //    uiManagerScript.UpdateHistoryPanel();
    //}

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForEndOfFrame();
    }

}
