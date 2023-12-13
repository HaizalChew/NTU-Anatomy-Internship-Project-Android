using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskViewer : MonoBehaviour
{
    [SerializeField] RenderTexture filterRenderTexture;
    [SerializeField] RectTransform filterMaskTransform;
    [SerializeField] RectTransform filterMaskTransformParent;
    [SerializeField] Slider resizeSlider;
    [SerializeField] Camera filterCamera;

    [SerializeField] Transform filterMenu;
    [SerializeField] FilterBodyPart filterBodyPart;
    [SerializeField] TMP_Text warningMessage;

    private void Awake()
    {
        if (filterBodyPart != null)
        {
            filterBodyPart = GetComponent<FilterBodyPart>();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        Resize(filterRenderTexture, Screen.width, Screen.height);
    }

    void Resize(RenderTexture renderTexture, int width, int height)
    {
        if (renderTexture)
        {
            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
        }
    }

    public void ChangeFilterMaskSize()
    {
        filterMaskTransform.offsetMin = new Vector2(filterMaskTransform.offsetMin.x, filterMaskTransformParent.rect.height * (1 - resizeSlider.value));
    }

    public void ActivateCompareMode()
    {
        if (filterBodyPart.CheckIfAllIsSelected())
        {
            warningMessage.text = "You cannot compare all body parts.";
        }
        else if (filterBodyPart.CheckIfNoneIsSelected())
        {
            warningMessage.text = "Please select at least one to continue";
        }
        else
        {
            warningMessage.text = "";
            ChangeFilterMaskSize();

            foreach (FilterBodyPart.FilterStruct filter in filterBodyPart.filters)
            {
                filter.refrencedObj.gameObject.SetActive(true);

                if (filter.buttonToggle.isOn)
                {
                    filterCamera.cullingMask |= (1 << filter.refrencedObj.gameObject.layer);
                }
                else
                {
                    filterCamera.cullingMask &= ~(1 << filter.refrencedObj.gameObject.layer);
                }
            }

            foreach (Transform uiElement in transform)
            {
                uiElement.gameObject.SetActive(false);
            }

            filterMenu.gameObject.SetActive(true);
        }
    }

    public void DeactivateCompareMode()
    {
        foreach (Transform uiElement in transform)
        {
            uiElement.gameObject.SetActive(true);
        }

        filterMenu.gameObject.SetActive(false);
    }
}
