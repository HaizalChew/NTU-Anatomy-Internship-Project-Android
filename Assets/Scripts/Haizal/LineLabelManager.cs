using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LineLabelManager : MonoBehaviour
{
    // References
    [Header("References")]
    [SerializeField] GameObject lineLabelPrefab;
    [SerializeField] Transform labelParent;
    [SerializeField] PartSelect partSelectScript;

    Dictionary<string, GameObject> labels = new Dictionary<string, GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (partSelectScript.selectedObject != null)
        {
            if (!labels.ContainsKey(partSelectScript.selectedObject.name))
            {
                DestroyAllLabels();
                CreateLabel(partSelectScript.selectedObject);
            }

        }
        else if (partSelectScript.selectedObject == null && partSelectScript.multiSelectedObjects.Count > 0)
        {
            List<string> multiSelectedList = new List<string>();

            foreach (GameObject selectedObject in partSelectScript.multiSelectedObjects)
            {
                if (!labels.ContainsKey(selectedObject.name))
                {
                    CreateLabel(selectedObject);
                }

                multiSelectedList.Add(selectedObject.name);
            }

            if (labels.Count > partSelectScript.multiSelectedObjects.Count)
            {
                var missingObjects = labels.Keys.Where(x => !multiSelectedList.Contains(x)).ToList();

                foreach (string obj in missingObjects)
                {
                    DestroyLabel(obj);
                }
            }
            

        }
        else if (partSelectScript.selectedObject == null && partSelectScript.multiSelectedObjects.Count == 0)
        {
            if (labels.Count > 0)
            {
                DestroyAllLabels();
            }

        }
    }

    private void CreateLabel(GameObject highlightObject)
    {
        Debug.Log("Created " + highlightObject.name);
        GameObject label = Instantiate(lineLabelPrefab, labelParent);
        label.name = highlightObject.name;

        label.GetComponent<LineLabels>().highlightedObj = highlightObject;
        labels.Add(highlightObject.name, label);
    }

    private void DestroyLabel(string key)
    {
        labels.TryGetValue(key, out GameObject label);

        if (label != null)
        {
            Destroy(label);
            labels.Remove(key);
        }
        
    }

    private void DestroyAllLabels()
    {
        foreach (GameObject label in labels.Values)
        {
            Destroy(label);
        }
        labels.Clear();
    }
}
