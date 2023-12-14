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
    // GONNA CLEAN UP & REFINE SOME STUFF - Haizal

    // Singleton references
    [Header("Singleton")]
    [SerializeField] GameObject scriptManager; // Contains all other scripts
    [SerializeField] InputManager inputManager; // Input controls
    // ====================================== //


    // Script references to ScriptManager
    private IsolateSelectionScript isolateScript;
    private HideSelectionScript hideScript;
    private OutlineSelectedPart outlineSelectedPart;
    private PartSelect partSelect;
    private MovePart movePart;
    // ====================================== //


    // Script references to Camera
    private Camera mainCamera;
    private RenderOutline renderOutline;
    private CameraControls cameraControls;
    // ====================================== //


    // Script references to Self
    private Animator animator;
    // ====================================== //


    // Main Panel references
    [Header("Main Panel")]
    [SerializeField] Button unHideBtn;
    [SerializeField] Button resetAllBtn;
    [SerializeField] Button hideBtn;
    [SerializeField] Button isolateBtn;
    [SerializeField] Button multiSelectBtn;
    [SerializeField] Button centerBtn;
    // ====================================== //


    //Advanced Panel references
    [Header("Advanced Panel")]
    [SerializeField] Button clearSelectionBtn;
    [SerializeField] Button undoMoveBtn;
    [SerializeField] Button undoAllMoveBtn;
    // ====================================== //


    // Other references
    [Header("Others")]
    public GameObject[] isolateButtonsList;
    
    public TextMeshProUGUI unHideText;
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI isolateText;

    public GameObject undoHistoryPanel;
    public GameObject moveHistoryPanel;
    public GameObject historyContainerPanel;
    public GameObject testChild;

    public bool isMultiSelect;
    public GameObject testBtn;

    public TMP_Text unHidePanel;
    public GameObject multiSelectActiveIndicator;
    // ====================================== //

    private void Awake()
    {
        // Initialize References
        mainCamera = Camera.main;
        isolateScript = scriptManager.GetComponent<IsolateSelectionScript>();
        hideScript = scriptManager.GetComponent<HideSelectionScript>();
        partSelect = scriptManager.GetComponent<PartSelect>();
        outlineSelectedPart = scriptManager.GetComponent<OutlineSelectedPart>();
        movePart = scriptManager.GetComponent<MovePart>();
        renderOutline = mainCamera.GetComponent<RenderOutline>();
        cameraControls = mainCamera.GetComponent<CameraControls>();
        animator = GetComponent<Animator>();


        // Initialize Main Panel buttons
        //unHideBtn.onClick.AddListener(() => hideScript.UnhideSelection());
        //resetAllBtn.onClick.AddListener(() => hideScript.ResetUnHide());
        hideBtn.onClick.AddListener(() => SendHideCommand());
        isolateBtn.onClick.AddListener(() => isolateScript.IsolateSelection());
        isolateBtn.onClick.AddListener(() => HideUiElement(isolateButtonsList,isolateScript.isIsolate));
        multiSelectBtn.onClick.AddListener(() => ToggleMultiSelectMode());
        centerBtn.onClick.AddListener(() => cameraControls.ActivateRecenteringOnButton());

        // Initialize Advanced Panel buttons
        clearSelectionBtn.onClick.AddListener(() => partSelect.ClearSelection());
        undoMoveBtn.onClick.AddListener(() => CommandInvoker.ExecuteUndo());
        undoAllMoveBtn.onClick.AddListener(() => movePart.UndoAllMove());
    }

    public void SendHideCommand()
    {
        if (isMultiSelect)
        {
            if(partSelect.multiSelectedObjects.Count > 0)
            {
                GameObject[] array;
                array = inputManager.ConvertSelectedObjectsToArray(null, partSelect.multiSelectedObjects);
                hideScript.HideSelection(array);
                ICommand command = new HideCommand(array, hideScript);
                CommandInvoker.ExecuteSave(command);
            }
        }
        else
        {
            if (partSelect.selectedObject != null)
            {
                GameObject[] array;
                array = inputManager.ConvertSelectedObjectsToArray(partSelect.selectedObject);
                hideScript.HideSelection(array);
                ICommand command = new HideCommand(array, hideScript);
                CommandInvoker.ExecuteSave(command);
            }
        }

    }

    public void UpdateHistoryPanel()
    {
        if (undoHistoryPanel.transform.childCount > 0)
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
   

    public IEnumerator UpdateHistoryCount()
    {
        yield return new WaitForEndOfFrame();
        int undoCounter = hideScript.GetUndoCounter();
        unHideText.text = "UnHide [" + undoCounter + "]";
        unHidePanel.text = "UnHide History [" + undoCounter + "]";
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

        multiSelectActiveIndicator.SetActive(isMultiSelect);

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

    public void ToggleAnimBool(string parameterName)
    {
        try
        {
            animator.SetBool(parameterName, !animator.GetBool(parameterName));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
