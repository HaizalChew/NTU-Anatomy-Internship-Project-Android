using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortAndConvertList : MonoBehaviour
{
    //Covert hits array to list for sorting and return it
    public List<KeyValuePair<RaycastHit, float>> GetShortLongHit(RaycastHit[] hits)
    {
        List<KeyValuePair<RaycastHit, float>> objectData = new List<KeyValuePair<RaycastHit, float>>();

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            objectData.Add(new KeyValuePair<RaycastHit, float>(hit, hit.distance));
        }

        objectData.Sort(CompareLength);
        return objectData;
    }
    // Compare hit distance in the list 
    private int CompareLength(KeyValuePair<RaycastHit, float> a, KeyValuePair<RaycastHit, float> b)
    {
        return a.Value.CompareTo(b.Value);
    }
}
