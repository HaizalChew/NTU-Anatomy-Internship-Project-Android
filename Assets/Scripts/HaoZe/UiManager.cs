using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using Unity.VisualScripting;

public class UiManager : MonoBehaviour
{
    private Camera mainCamera;
    private IsolateSelectionScript isolateScript;
    private HideSelectionScript hideScript;
    private OutlineSelectedPart outlineSelectedPart;
    private RenderOutline renderOutline;
    public InputManager inputManager;
    private PartSelect partSelect;

    public GameObject[] isolateButtonsList;
    public GameObject isolateBtn;
    public TextMeshProUGUI undoText;
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI isolateText;

    public GameObject undoHistoryPanel;
    public GameObject moveHistoryPanel;
    public GameObject historyContainerPanel;
    public GameObject testChild;

    public bool isMultiSelect;
    public GameObject testBtn;

    private void Awake()
    {
        mainCamera = Camera.main;
        isolateScript = gameObject.GetComponent<IsolateSelectionScript>();
        hideScript = gameObject.GetComponent<HideSelectionScript>();
        partSelect = gameObject.GetComponent<PartSelect>();
        outlineSelectedPart = gameObject.GetComponent<OutlineSelectedPart>();
        renderOutline = mainCamera.GetComponent<RenderOutline>();
        Button button = isolateBtn.GetComponent<Button>();
        button.onClick.AddListener(delegate { HideUiElement(isolateButtonsList,isolateScript.isIsolate); });
    }

    public void UpdateHistoryPanel()
    {
        if(undoHistoryPanel.transform.childCount > 0)
        {
            foreach(Transform child in undoHistoryPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach(Transform child in hideScript.historyContainer.transform)
        {
            GameObject clone = Instantiate(testBtn, undoHistoryPanel.transform);
            clone.GetComponent<Button>().onClick.AddListener(delegate { hideScript.HistoryShowHide(child.transform); });
            clone.GetComponentInChildren<TextMeshProUGUI>().text = child.transform.name;
        }

    }

    public void SayHi()
    {
        Debug.Log("yo");
    }
   

    public void DisplaySelectName(bool isMultiSelect)
    {
        if(isMultiSelect)
        {
            int count = partSelect.multiSelectedObjects.Count;
            if(count > 0)
            {
                selectText.text = partSelect.multiSelectedObjects[count-1].name;
            }
            else
            {
                selectText.text = null;
            }
        }
        else
        {
            if (partSelect.selectedObject != null)
            {
                selectText.text = partSelect.selectedObject.name;
            }
            else
            {
                selectText.text = null;
            }
        }
    }

    public IEnumerator UpdateHistoryCount()
    {
        yield return new WaitForEndOfFrame();
        int undoCounter = hideScript.GetUndoCounter();
        undoText.text = "Undo [" + undoCounter + "]";
    }
    public bool HideUiElement(GameObject[] uiElements, bool isToggle)
    {
        if (isToggle == true)
        {
            foreach(GameObject element in uiElements)
            {
                element.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject element in uiElements)
            {
                element.SetActive(true);
            }
        }
        return isToggle;
    }

    public void ToggleMultiSelectMode()
    {
        isMultiSelect = !isMultiSelect;
        renderOutline.RenderObject.Clear();
        if (isMultiSelect)
        {
            inputManager.OnStartTouch -= partSelect.ToggleSelectPart;
            inputManager.OnStartTouch -= outlineSelectedPart.OutlineSelected;
            inputManager.OnStartTouch += partSelect.ToggleMultiSelect;
            inputManager.OnStartTouch += outlineSelectedPart.OutlineMultiSelected;

            //Clear selections
            partSelect.selectedObject = null;
            

        }
        else
        {
            inputManager.OnStartTouch += partSelect.ToggleSelectPart;
            inputManager.OnStartTouch += outlineSelectedPart.OutlineSelected;
            inputManager.OnStartTouch -= partSelect.ToggleMultiSelect;
            inputManager.OnStartTouch -= outlineSelectedPart.OutlineMultiSelected;

            //Clear selections
            partSelect.multiSelectedObjects.Clear();
        }
    }
}
