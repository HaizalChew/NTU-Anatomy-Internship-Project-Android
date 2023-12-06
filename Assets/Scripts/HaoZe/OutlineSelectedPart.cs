using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelectedPart : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private PartSelect partSelect;
    private RenderOutline renderOutline;
    public InputManager inputManager;
    private void Awake()
    {
        mainCamera = Camera.main;
        partSelect = gameObject.GetComponent<PartSelect>();
        renderOutline = mainCamera.GetComponent<RenderOutline>();
    }

    public void OutlineSelected(Vector2 screenPosition, float time)
    {
        //Check if there is selected part
        if(partSelect.selectedObject != null)
        {
            renderOutline.enabled = true;
            //Check if rendOutline first or second
            if (renderOutline.RenderObject.Count > 0)
            {
                renderOutline.RenderObject[0] = partSelect.selectedObject.transform.GetComponent<Renderer>();
            }
            else
            {
                renderOutline.RenderObject.Add(partSelect.selectedObject.transform.GetComponent<Renderer>());
            }
        }
        else
        {
            renderOutline.enabled = false;
            renderOutline.RenderObject.Clear();
        }
    }

    public void OutlineMultiSelected(Vector2 screenPosition, float time)
    {
        //Check if there is selected part
        if (partSelect.multiSelectedObjects.Count > 0)
        {
            renderOutline.enabled = true;
            renderOutline.RenderObject.Clear();
            foreach(GameObject selectedObject in partSelect.multiSelectedObjects)
            {
                renderOutline.RenderObject.Add(selectedObject.transform.GetComponent<Renderer>());
            }
        }
        if(partSelect.multiSelectedObjects.Count == 0)
        {
            renderOutline.enabled = false;
            renderOutline.RenderObject.Clear();
        }
    }
}
