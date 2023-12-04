using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public bool isMultiSelect;

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

    public void DisplaySelectName(bool isMultiSelect)
    {
        if(isMultiSelect)
        {
            int count = partSelect.multiSelectedObjects.Count;
            if(count > 0)
            {
                Debug.Log("yes");
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
