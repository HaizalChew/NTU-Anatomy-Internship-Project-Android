using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeMenuBehaviour : MonoBehaviour, IEndDragHandler
{
    [SerializeField] RectTransform menuPanelRect;
    [SerializeField] RectTransform canvasUIRect;
    [SerializeField] float dragTreshold = 1f;


    [SerializeField] MenuState currentState;
    [SerializeField] bool updateState;

    MenuStateVector2Values menuStateValues;

    private void Start()
    {
        if (!canvasUIRect)
        {
            canvasUIRect = transform.root.GetComponent<RectTransform>();
        }

        menuStateValues.SetDimensionValues(canvasUIRect);
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(menuPanelRect.anchoredPosition.y);
        if (menuPanelRect.anchoredPosition.y > 0)
        {
            menuPanelRect.anchoredPosition = new Vector2(menuPanelRect.anchoredPosition.x, 0);
        }
        else if (menuPanelRect.anchoredPosition.y < -canvasUIRect.position.y)
        {
            menuPanelRect.anchoredPosition = new Vector2(menuPanelRect.anchoredPosition.x, -canvasUIRect.position.y);
        }

        if (updateState)
        {
            updateState = false;

            switch (currentState)
            {
                case MenuState.Closed:
                    StartCoroutine(LerpStates(menuStateValues.closeVector2, 1));
                    return;

                case MenuState.Partial:
                    StartCoroutine(LerpStates(menuStateValues.partialVector2, 1));
                    return;

                case MenuState.Open:
                    StartCoroutine(LerpStates(menuStateValues.openVector2, 1));
                    return;
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.y - eventData.pressPosition.y) > dragTreshold)
        {
            updateState = true;

            if (eventData.position.y > eventData.pressPosition.y)
            {
                // Value is positive (Swipe Up)
                switch (currentState)
                {
                    case MenuState.Closed:
                        currentState = MenuState.Partial;
                        return;

                    case MenuState.Partial:
                        currentState = MenuState.Open;
                        return;

                    case MenuState.Open:
                        return;
                }

            }
            else
            {
                // else Swipe down
                switch(currentState)
                {
                    case MenuState.Open:
                        currentState = MenuState.Partial;
                        return;

                    case MenuState.Partial:
                        currentState = MenuState.Closed;
                        return;

                    case MenuState.Closed:
                        return;
                }
            }
        }
    }

    IEnumerator LerpStates(Vector2 endValue, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            menuPanelRect.anchoredPosition = Vector2.Lerp(menuPanelRect.anchoredPosition, endValue, timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        menuPanelRect.anchoredPosition = endValue;
    }

    enum MenuState
    {
        Open,
        Partial,
        Closed
    }

    struct MenuStateVector2Values
    {
        public Vector2 openVector2 { get; set; }
        public Vector2 partialVector2 { get; set; }
        public Vector2 closeVector2 { get; set; }

        public void SetDimensionValues(RectTransform canvasUIRect)
        {
            openVector2 = new Vector2(0, 0);
            partialVector2 = new Vector2(0, -(canvasUIRect.position.y / 3));
            closeVector2 = new Vector2(0, -canvasUIRect.position.y);
        }
    }
}
