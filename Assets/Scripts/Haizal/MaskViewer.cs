using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskViewer : MonoBehaviour
{
    [SerializeField] RenderTexture filterRenderTexture;
    [SerializeField] RectTransform filterMaskTransform;
    [SerializeField] RectTransform filterMaskTransformParent;
    [SerializeField] Slider resizeSlider;

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
}
