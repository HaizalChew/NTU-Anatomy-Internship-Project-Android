using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineLabels : MonoBehaviour
{
    [SerializeField] RectTransform start;
    [SerializeField] RectTransform end;
    [SerializeField] Transform lineGroup;
    [SerializeField] TMP_Text labelText;
    public GameObject highlightedObj;

    [SerializeField] float endTargetDistance = 2f;
    [SerializeField] float endLerpSpeed = 5f;


    RectTransform[] points = new RectTransform[2];
    GameObject[] lines;

    private void Awake()
    {
        points[0] = start;
        points[1] = end;

        RandomizedEndPosition();
    }

    private void Update()
    {
        if (highlightedObj != null)
        {
            if (lines == null)
            {  
                DrawLine(points);
            }

            labelText.text = highlightedObj.name;
            SetStartPosition();
            SetEndPosition();

            UpdateLineAnchoredPosition(lines);
        }
        else
        {
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    Destroy(line.gameObject);
                }

                labelText.text = "";
                lines = null;
            }
        }

    }

    private void DrawLine(RectTransform[] points)
    {
        lines = new GameObject[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            GameObject drawLine = new GameObject("dotConnection", typeof(Image));
            RectTransform drawLineRectTransform = drawLine.GetComponent<RectTransform>();
            drawLineRectTransform.SetParent(lineGroup, false);

            drawLineRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            drawLineRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            lines[i] = drawLine;
        }

        UpdateLineAnchoredPosition(lines);
    }

    private void UpdateLineAnchoredPosition(GameObject[] drawLines)
    {
        for (int i = 0; i < drawLines.Length; i++)
        {
            RectTransform drawLineRectTransform = drawLines[i].GetComponent<RectTransform>();

            Vector2 lastPoint = i - 1 < 0 ? points[i].anchoredPosition : points[i - 1].anchoredPosition;

            // calculate distance
            Vector2 midPoint = Vector2.Lerp(points[i].anchoredPosition, lastPoint, 0.5f);
            float distance = Vector2.Distance(points[i].anchoredPosition, lastPoint);

            // calculate angle
            Vector2 dir = points[i].anchoredPosition - lastPoint;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // set values
            drawLineRectTransform.sizeDelta = new Vector3(distance, 3f);
            drawLineRectTransform.anchoredPosition = midPoint;
            drawLineRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    private void SetStartPosition()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(highlightedObj.GetComponent<MeshRenderer>().bounds.center);
        start.position = screenPosition;
    }

    private void RandomizedEndPosition()
    {
        Vector2 canvasRect = end.root.GetComponent<RectTransform>().rect.size;
        float xPos = Random.Range(-canvasRect.x / 2 + end.rect.size.x / 2, canvasRect.x / 2 - end.rect.size.x / 2);
        float yPos = Random.Range(-canvasRect.y / 2 + end.rect.size.y / 2, canvasRect.x / 2 - end.rect.size.y / 2);
        end.anchoredPosition = new Vector2(xPos, yPos);
    }

    private void SetEndPosition()
    {
        if (Vector2.Distance(start.position, end.position) > endTargetDistance)
        {
            // Control smoothing
            Vector2 dir = start.position - end.position;
            Vector3 movement = dir * endLerpSpeed * Time.deltaTime;
            end.position += movement;

            // Clamp position to screen space
            Vector2 aPos = end.anchoredPosition;
            Vector2 canvasRect = end.root.GetComponent<RectTransform>().rect.size;
            float xPos = Mathf.Clamp(aPos.x, -canvasRect.x / 2 + end.rect.size.x / 2, canvasRect.x / 2 - end.rect.size.x / 2);
            float yPos = Mathf.Clamp(aPos.y, -canvasRect.y / 2 + end.rect.size.y / 2, canvasRect.y / 2 - end.rect.size.y / 2);
            end.anchoredPosition = new Vector2(xPos, yPos);
        }



    }
}
