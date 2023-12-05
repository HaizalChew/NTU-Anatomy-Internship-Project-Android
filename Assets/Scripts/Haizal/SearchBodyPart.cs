using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SearchBodyPart : MonoBehaviour
{
    public Transform parentModel;
    public Transform partListParent;
    [SerializeField] GameObject anatomyGroupPrefab;
    [SerializeField] GameObject partListPrefab;

    Dictionary<string, GameObject> partDict = new Dictionary<string, GameObject>();

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
        // --> --> BodyPart
        foreach (Transform child in parentModel)
        {
            GameObject _anatomyGroup = Instantiate(anatomyGroupPrefab, partListParent);

            _anatomyGroup.transform.GetComponentInChildren<TMP_Text>().text = child.name;
            _anatomyGroup.name = child.name;

            if (child.childCount > 0)
            {
                for (int j = 0; j < child.childCount; j++)
                {
                    GameObject _spawnName = Instantiate(partListPrefab, _anatomyGroup.transform);
                    // _spawnName.GetComponent<StoredScript>().referencedGameObject = child.GetChild(j).gameObject;
                    string _name = child.GetChild(j).name;

                    _spawnName.GetComponentInChildren<TMP_Text>().text = _name;
                    _spawnName.name = _name;

                    partDict.Add(_name.ToLower(), _spawnName);
                }
            }
        }
    }

    public void SearchForPart(TMP_InputField input)
    {
        if (input.text != "")
        {
            var parts = partDict.Where(kvp => kvp.Key.Contains(input.text.ToLower()));

            foreach (Transform child in partListParent)
            {
                if (child.childCount > 0)
                {
                    for (int j = 0; j < child.childCount; j++)
                    {
                        Transform _childChild = child.GetChild(j);
                        
                        if (_childChild.tag == "IgnoreUI")
                        {
                            _childChild.gameObject.SetActive(true);
                        }
                        else
                        {
                            _childChild.gameObject.SetActive(false);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, GameObject> part in parts)
            {
                part.Value.gameObject.SetActive(true);
            }


        }
        else
        {
            foreach (Transform child in partListParent)
            {
                if (child.childCount > 0)
                {
                    for (int j = 0; j < child.childCount; j++)
                    {
                        child.GetChild(j).gameObject.SetActive(true);
                    }
                }
            }
        }

    }

}
