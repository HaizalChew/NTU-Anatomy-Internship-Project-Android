using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterBodyPart : MonoBehaviour
{
    public Transform parentModel;
    public Transform partListParent;
    [SerializeField] GameObject filterListPrefab;
    [SerializeField] GameObject[] uiElements;
    [SerializeField] TMP_Text warningMessage;

    struct FilterStruct
    {
        public Transform refrencedObj;
        public StrictButtonExtension buttonToggle;
    }

    List<FilterStruct> filters = new List<FilterStruct>();

    private void Awake()
    {
        parentModel = parentModel != null ? parentModel : GameObject.FindGameObjectWithTag("Model").transform;

        try
        {
            InitializeDictionary(parentModel);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
    }

    public void InitializeDictionary(Transform parentModel)
    {
        // Hierachy:
        // Parent
        // --> AnatomyGroup
        foreach (Transform child in parentModel)
        {
            GameObject _anatomyGroup = Instantiate(filterListPrefab, partListParent);

            _anatomyGroup.transform.GetComponentInChildren<TMP_Text>().text = child.name;
            _anatomyGroup.name = child.name;

            FilterStruct _filterStruct = new FilterStruct();
            _filterStruct.refrencedObj = child;
            _filterStruct.buttonToggle = _anatomyGroup.GetComponent<StrictButtonExtension>();

            filters.Add(_filterStruct);
        }
    }

    public void HideOrShowFilteredBodyParts()
    {
        if (!CheckIfAllIsSelected())
        {         
            foreach (FilterStruct filterStruct in filters)
            {
                filterStruct.refrencedObj.gameObject.SetActive(!filterStruct.buttonToggle.isOn);
            }

            foreach (GameObject ui in uiElements)
            {
                ui.SetActive(false);
            }

            warningMessage.text = "";
        }
        else
        {
            warningMessage.text = "You cannot hide all body parts.";
        }
        
    }

    bool CheckIfAllIsSelected()
    {
        bool currentBoolIsSetToTrue = true;

        foreach (FilterStruct filterStruct in filters)
        {
            if (!filterStruct.buttonToggle.isOn)
            {
                currentBoolIsSetToTrue = false;
                break;
            }
        }

        return currentBoolIsSetToTrue;
    }
}
