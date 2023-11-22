using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    private IsolateSelectionScript isolateScript;
    private HideSelectionScript hideScript;

    public GameObject[] isolateButtonsList;
    public GameObject isolateBtn;
    public TextMeshProUGUI undoText;

    private void Awake()
    {
        isolateScript = gameObject.GetComponent<IsolateSelectionScript>();
        hideScript = gameObject.GetComponent<HideSelectionScript>();
        Button button = isolateBtn.GetComponent<Button>();
        button.onClick.AddListener(delegate { HideUiElement(isolateButtonsList,isolateScript.isIsolate); });
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
}
