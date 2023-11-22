using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    // Control camera focus
    public Transform target;

    [SerializeField] Transform focus;
    [SerializeField] Transform orientation;
    [SerializeField, Range(0.5f, 10f)] float distance = 5f;
    [SerializeField, Min(0f)] float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] float focusCentering = 0.5f;

    public bool stopRecentering;
    [SerializeField] float recenteringZoom = 3f;

    // Control orbit rotation expressed in degrees
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField] float cameraPanningSensitivity = 1.0f;
    [SerializeField] InputActionReference orbitDeltaInput, zoomInput, secondZoomInput, zoomUnlockInput;

    // Control camera zoom
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] float zoomSpeed = 1f;

    // Set orbit angles
    Vector2 orbitAngles = new(45f, 0f);

    float zoomDistance, pinchDelta, previousDistance;


    private void Start()
    {
        orbitAngles = transform.localRotation.eulerAngles;
        //focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Model").transform;
        }
    }

    void OnValidate()
    {
        // Validate the min does not go beyond the max, this will run in the inspector
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    private void LateUpdate()
    {
        // Updates camera position and rotation
        if (!stopRecentering)
        {
            UpdateFocusPoint();
        }
        

        Quaternion lookRotation;
        if (OrbitCamera())
        {
            ConstrainAnglesInOrbit();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }

        // Zoom the camera
        ZoomCamera();


        // Enable camera panning
        Pan();

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focus.transform.position - lookDirection * distance;
        transform.SetPositionAndRotation(lookPosition, lookRotation);


    }


    // This will update the focus pont everytime the model moves/changes
    // Applies easing in whenever it moves to prevent sharp snapping
    void UpdateFocusPoint()
    {
        if (target != null)
        {
            target.TryGetComponent(out Renderer renderer);
            Vector3 targetPoint = renderer?.bounds.center ?? target.position;

            float _distance = Vector3.Distance(targetPoint, focus.transform.position);
            float t = 1f;
            if (_distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (_distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / _distance);
            }
            focus.transform.position = Vector3.Lerp(targetPoint, focus.transform.position, t);
        }
    }

    public void ActivateRecentering(Transform targetPos)
    {
        target = targetPos;
        stopRecentering = false;

        StartCoroutine(InterpolateZooming());
    }

    public void ActivateRecenteringOnButton()
    {
        stopRecentering = false;
        StartCoroutine(InterpolateZooming());   
    }

    IEnumerator InterpolateZooming()
    {
        float currentDistance = distance;
        float zoomRate = 0.02f;
        float t = 0;

        while (t < 1f)
        {
            
            distance = Mathf.Lerp(currentDistance, recenteringZoom, t);
            t += zoomRate;

            yield return new WaitForSeconds(0.002f);
        }
    }
    
    // This will orbit the camera around the focus
    bool OrbitCamera()
    {
        Vector2 input = new(-orbitDeltaInput.action.ReadValue<Vector2>().y, orbitDeltaInput.action.ReadValue<Vector2>().x);
        
        if (float.IsInfinity(input.sqrMagnitude))
        {
            input = Vector2.zero;
        }

        const float e = 0.001f;
        if ((input.x < -e || input.x > e || input.y < -e || input.y > e) && !zoomUnlockInput.action.IsPressed())
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            
            return true;
        }

        return false;
    }

    // This will constrain the camera in the assigned boundaries
    void ConstrainAnglesInOrbit()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    // This will control the zoom function of the camera
    void ZoomCamera()
    {
        // Check if second touch is being held down
        if (zoomUnlockInput.action.IsPressed())
        {
            zoomDistance = Vector2.Distance(zoomInput.action.ReadValue<Vector2>(),
                    secondZoomInput.action.ReadValue<Vector2>());

            if (zoomUnlockInput.action.WasPressedThisFrame())
            {
                previousDistance = zoomDistance;
            }

            // Return delta value
            pinchDelta = zoomDistance - previousDistance;

            // keep track of distance for next loop
            previousDistance = zoomDistance;
        }
        else
        {
            pinchDelta = 0f;
        }

        float input = pinchDelta * 0.001f;

        const float e = 0.001f;

        if (input > -e)
        {
            distance -= zoomSpeed * Mathf.Abs(input);
        }
        else if (input < e)
        {
            distance += zoomSpeed * Mathf.Abs(input);
        }

        distance = Mathf.Clamp(distance, .5f, 10f);
    }

    // This will control camera panning
    void Pan()
    {
        if (zoomUnlockInput.action.IsPressed())
        {
            stopRecentering = true;

            Vector3 direction = orbitDeltaInput.action.ReadValue<Vector2>() * 0.01f;

            Vector3 viewDir = focus.position - transform.position;
            orientation.forward = viewDir.normalized;

            Vector3 movement = orientation.up * direction.y + orientation.right * direction.x;

            focus.transform.Translate(movement * cameraPanningSensitivity);

        }


    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(focus.transform.position, .5f);
    //}

    ////Returns 'true' if we touched or hovering on Unity UI element.
    //public bool IsPointerOverUIElement()
    //{
    //    return IsPointerOverUIElement(GetEventSystemRaycastResults());
    //}


    ////Returns 'true' if we touched or hovering on Unity UI element.
    //private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    //{
    //    for (int index = 0; index < eventSystemRaysastResults.Count; index++)
    //    {
    //        RaycastResult curRaysastResult = eventSystemRaysastResults[index];
    //        if (curRaysastResult.gameObject.layer == UILayer)
    //            return true;
    //    }
    //    return false;
    //}


    ////Gets all event system raycast results of current mouse or touch position.
    //static List<RaycastResult> GetEventSystemRaycastResults()
    //{
    //    PointerEventData eventData = new PointerEventData(EventSystem.current);
    //    eventData.position = Input.mousePosition;
    //    List<RaycastResult> raysastResults = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(eventData, raysastResults);
    //    return raysastResults;
    //}

}
