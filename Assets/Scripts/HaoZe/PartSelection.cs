using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class PartSelection : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private SelectRenderOutline outlineScript;
    private RenderOutline highlightOutlineScript;
    private SortAndConvertList convertScript;
    public InputManager inputManager;

    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;
        outlineScript = mainCamera.GetComponent<SelectRenderOutline>();
        highlightOutlineScript = mainCamera.GetComponent<RenderOutline>();
        convertScript = gameObject.GetComponent<SortAndConvertList>();
    }

    private void OnEnable()
    {
        inputManager.OnPerformHold += DetectHoldPart;
    }

    private void OnDisable()
    {
        inputManager.OnPerformHold -= DetectHoldPart;
    }

    //Detect Hold and display Outline
    public void DetectHoldPart(Vector2 screenPosition, float time)
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
                    OutlineSelectionPart(hitData);
                    continue;
                }
                //Detect if hit object is opaque
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 0)
                {
                    OutlineSelectionPart(hitData);
                    break;
                }
            }
        }
        else
        {
            if(outlineScript.SelectRenderObject.Count == 0)
            {
                outlineScript.enabled = false;
                outlineScript.SelectRenderObject.Clear();
            }
        }
    }

    //Outline selection Part
    private void OutlineSelectionPart(RaycastHit hitData)
    {
        outlineScript.enabled = true;
        Debug.Log("Hold");
        //Check if holded object has already been selected
        isHoldedObjectSelected(hitData);
    }

    //Check if object holded is selected already
    private void isHoldedObjectSelected(RaycastHit hitData)
    {
        bool isSelected = false;
        if (outlineScript.SelectRenderObject.Count > 0)
        {
            foreach (Renderer renderObject in outlineScript.SelectRenderObject)
            {
                if (renderObject.name == hitData.transform.GetComponent<Renderer>().name)
                {
                    isSelected = true;
                    outlineScript.SelectRenderObject.Remove(renderObject);
                    if (outlineScript.SelectRenderObject.Count == 0)
                    {
                        outlineScript.enabled = false;
                        outlineScript.SelectRenderObject.Clear();
                    }
                    break;
                }
            }
        }

        if (isSelected == false)
        {
            //Check if already highlight
            for (int i = 0; i < highlightOutlineScript.RenderObject.Count; i++)
            {
                if (highlightOutlineScript.RenderObject[i].name == hitData.transform.name)
                {
                    highlightOutlineScript.RenderObject.Remove(highlightOutlineScript.RenderObject[i]);
                    highlightOutlineScript.RenderObject.Clear();
                }
            }
            outlineScript.SelectRenderObject.Add(hitData.transform.GetComponent<Renderer>());
        }
    }

}
