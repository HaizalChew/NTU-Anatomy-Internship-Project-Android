using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartHighlight : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private RenderOutline outlineScript;
    private SelectRenderOutline selectOutlineScript;
    private SortAndConvertList convertScript;
    public InputManager inputManager;


    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
        outlineScript = mainCamera.GetComponent<RenderOutline>();
        selectOutlineScript = mainCamera.GetComponent<SelectRenderOutline>();
        convertScript = gameObject.GetComponent<SortAndConvertList>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += DetectTouchPart;
    }

    private void OnDisable()
    {
        inputManager.OnEndTouch -= DetectTouchPart;
    }

    public void DetectTouchPart(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane);
        var ray = mainCamera.ScreenPointToRay(screenCoordinates);
        bool hitSelectable = Physics.Raycast(ray, out var hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable");
        if (hitSelectable)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);
            //Get sorted List of ray cast hit data
            List<KeyValuePair<RaycastHit, float>> objectData = convertScript.GetShortLongHit(hits);
            for (int i = 0; i < objectData.Count; i++)
            {
                RaycastHit hitData = objectData[i].Key;
                //Detect if hit object is transparent
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 3)
                {
                    OutlineHighlightPart(hitData);
                    continue;
                }
                //Detect if hit object is opaque
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 0)
                {
                    OutlineHighlightPart(hitData);
                    break;
                }
            }
        }
        else
        {
            outlineScript.enabled = false;
            outlineScript.RenderObject.Clear();
        }
    }

    private void OutlineHighlightPart(RaycastHit hitData)
    {
        //Check if tap Object has been selected
        bool isSelected = isTappedObjectSelected(hitData);
        //then check is first tap 
        if(!isSelected)
        {
            outlineScript.enabled = true;
            if (outlineScript.RenderObject.Count > 0)
            {
                outlineScript.RenderObject[0] = hitData.transform.GetComponent<Renderer>();
                isTappedObjectSelected(hitData);
            }
            else
            {
                outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
            }
        } 
    }

    //Check if object is selected already
    private bool isTappedObjectSelected(RaycastHit hitData)
    {
        bool isSelected = false;
        foreach (Renderer renderObject in selectOutlineScript.SelectRenderObject)
        {
            if (renderObject.name == hitData.transform.GetComponent<Renderer>().name)
            {
                isSelected = true;
                return isSelected;
            }
        }
        return isSelected;
    }
}
