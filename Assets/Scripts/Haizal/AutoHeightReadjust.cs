using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHeightReadjust : MonoBehaviour
{
    VerticalLayoutGroup verticalLayoutGroup;
    RectTransform thisRectTransform;

    private void Start()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        thisRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        UpdateContentHeight();
    }

    public void UpdateContentHeight()
    {
        //activeChild = parentSearch.GetComponentsInChildren<RectTransform>();
        float height = 0;

        //for (int i = 0; i < activeChild.Length; i++)
        //{
        //    height += activeChild[i].sizeDelta.y + verticalLayoutGroup.spacing;
        //}

        foreach (RectTransform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                height += child.sizeDelta.y + verticalLayoutGroup.spacing;
            }
        }

        height += verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom;

        thisRectTransform.sizeDelta = new Vector2(thisRectTransform.sizeDelta.x, height);
    }
}
