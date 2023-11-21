using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

public class SelectPart : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    private RenderOutline outlineScript;
    public InputManager inputManager;


    // Start is called before the first frame update
    private void Awake()
    {
        camera = Camera.main;
        outlineScript = camera.GetComponent<RenderOutline>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += DetectTouchPart;
        inputManager.OnPeformHold += DetectHoldPart;
    }

    private void OnDisable()
    {
        inputManager.OnEndTouch -= DetectTouchPart;
        inputManager.OnPeformHold -= DetectHoldPart;
    }

    void Start()
    {
        
    }
        

    public void DetectTouchPart(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, camera.nearClipPlane);
        var ray = camera.ScreenPointToRay(screenCoordinates);
        bool hitSelectable = Physics.Raycast(ray, out var hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable");
        if (hitSelectable)
        {
            OutlineTouchedPart(hitSelectable, ray);
        }
        else
        {
            OutlineTouchedPart(hitSelectable, ray);
        }
    }

    public void DetectHoldPart(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, camera.nearClipPlane);
        var ray = camera.ScreenPointToRay(screenCoordinates);
        bool hitSelectable = Physics.Raycast(ray, out var hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Selectable");
        if (hitSelectable)
        {
            OutlineTouchedPart(hitSelectable, ray, true);
        }
        else
        {
            OutlineTouchedPart(hitSelectable, ray, true);
        }
        Debug.Log("Hold");
    }

    public void OutlineTouchedPart(bool hitSelectable, Ray ray, bool isMultiSelect = false)
    {
        if (hitSelectable)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity);
            List<KeyValuePair<RaycastHit, float>> objectData = GetShortLongHit(hits);
            for (int i = 0; i < objectData.Count; i++)
            {
                var hitData = objectData[i].Key;
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 3)
                {
                    outlineScript.enabled = true;
                    if(isMultiSelect == true)
                    {
                        outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                    }
                    else
                    {
                        if (outlineScript.RenderObject != null)
                        {
                            if (outlineScript.RenderObject.Count != 0)
                            {
                                Debug.Log("yes");
                                outlineScript.RenderObject[0] = hitData.transform.GetComponent<Renderer>();

                            }
                            else
                            {
                                outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                            }

                        }
                        else
                        {
                            if (outlineScript.RenderObject == null)
                            {
                                if (outlineScript.RenderObject.Count != 0)
                                {
                                    outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                                }
                            }
                        }
                    }
                    continue;
                }
                if (hitData.transform.GetComponent<MeshRenderer>().material.GetFloat("_Mode") == 0)
                {
                    outlineScript.enabled = true;
                    if (isMultiSelect == true)
                    {
                        outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                    }
                    else
                    {
                        if (outlineScript.RenderObject != null)
                        {
                            if(outlineScript.RenderObject.Count != 0)
                            {
                                Debug.Log("yes");
                                outlineScript.RenderObject[0] = hitData.transform.GetComponent<Renderer>();

                            }
                            else
                            {
                                outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                            }
                            
                        }
                        else
                        {
                            if (outlineScript.RenderObject?[0])
                            {
                                outlineScript.RenderObject.Add(hitData.transform.GetComponent<Renderer>());
                            }
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            outlineScript.enabled = false;
            //outlineScript.RenderObject = null;
            outlineScript.RenderObject.Clear();
        }
      

    }

    private int CompareLength(KeyValuePair<RaycastHit, float> a, KeyValuePair<RaycastHit, float> b)
    {
        return a.Value.CompareTo(b.Value);
    }

    private List<KeyValuePair<RaycastHit, float>> GetShortLongHit(RaycastHit[] hits)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
