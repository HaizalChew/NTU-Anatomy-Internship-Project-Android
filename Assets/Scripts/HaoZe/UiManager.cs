using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Collections.Generic;

public class UiManager : MonoBehaviour
{
    private Camera mainCamera;
    private IsolateSelectionScript isolateScript;
    private HideSelectionScript hideScript;
    private OutlineSelectedPart outlineSelectedPart;
    private RenderOutline renderOutline;
    public InputManager inputManager;
    private PartSelect partSelect;
    [SerializeField] GameObject scriptManager;

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

    public Button undoPanelBtn;
    public GameObject UndoPanelPortView;

    private void Awake()
    {
        mainCamera = Camera.main;
        isolateScript = scriptManager.GetComponent<IsolateSelectionScript>();
        hideScript = scriptManager.GetComponent<HideSelectionScript>();
        partSelect = scriptManager.GetComponent<PartSelect>();
        outlineSelectedPart = scriptManager.GetComponent<OutlineSelectedPart>();
        renderOutline = mainCamera.GetComponent<RenderOutline>();
        Button button = isolateBtn.GetComponent<Button>();
        button.onClick.AddListener(delegate { HideUiElement(isolateButtonsList,isolateScript.isIsolate); });

        undoPanelBtn.onClick.AddListener(delegate { OnButtonHideUI(historyContainerPanel); });
    }

    public void UpdateHistoryPanel()
    {
        Debug.Log("here???");
        if (undoHistoryPanel.transform.childCount > 0)
        {
            foreach(Transform child in undoHistoryPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
        Debug.Log("here????");
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
        undoPanelBtn.GetComponentInChildren<TextMeshProUGUI>().text = "UndoHistory [" + undoCounter + "]";
        if (undoCounter < 5)
        {
            UndoPanelPortView.GetComponent<RectTransform>().sizeDelta = new Vector2(248, (100 * undoCounter));
            Debug.Log("change");
        }
    }
    public void OnButtonHideUI(GameObject obj)
    {
        Debug.Log(obj.activeSelf);
       if(obj.activeSelf)
        {
            Debug.Log(obj);
            obj.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(obj);
            obj.gameObject.SetActive(true);
        }
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

    public void ToggleSetActive(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
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
