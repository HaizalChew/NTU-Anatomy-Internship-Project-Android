using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject isolateBtn;

    private IsolateSelectionScript isolateScript;
    public GameObject[] isolateButtonsList;

    private void Awake()
    {
        isolateScript = gameObject.GetComponent<IsolateSelectionScript>();
        Button button = isolateBtn.GetComponent<Button>();
        button.onClick.AddListener(delegate { HideUiElement(isolateButtonsList,isolateScript.isIsolate); });
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
