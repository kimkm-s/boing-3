using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeSnapSimple : MonoBehaviour
{
    [Header("Settings")]
    public float snapSpeed = 10f;

    public int currentStage = 0;   //  현재 선택된 스테이지 번호 (Start 버튼에서 사용)

    private int stageCount;
    private float[] pos;
    private float scrollPos;
    private bool isDragging = false;

    private RectTransform content;

    public GameObject LeftArrow;
    public GameObject RightArrow;

    void Start()
    {
        stageCount = transform.childCount;
        content = GetComponent<RectTransform>();
        pos = new float[stageCount];

        for (int i = 0; i < stageCount; i++)
        {
            pos[i] = (float)i / (stageCount - 1);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            scrollPos = -content.anchoredPosition.x / (content.rect.width - ((RectTransform)transform.parent).rect.width);
            scrollPos = Mathf.Clamp01(scrollPos);
        }
        else
        {
            int nearestIndex = 0;
            float minDistance = Mathf.Abs(scrollPos - pos[0]);

            for (int i = 1; i < stageCount; i++)
            {
                float distance = Mathf.Abs(scrollPos - pos[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            currentStage = nearestIndex;   //  현재 선택된 스테이지 번호 저장

            float targetX = -pos[nearestIndex] * (content.rect.width - ((RectTransform)transform.parent).rect.width);
            Vector2 targetPos = new Vector2(targetX, content.anchoredPosition.y);
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, snapSpeed * Time.deltaTime);

            HighlightStage(nearestIndex);
        }
    }

    public void OnBeginDrag()
    {
        isDragging = true;
    }

    public void OnEndDrag()
    {
        isDragging = false;
    }

    private void HighlightStage(int index)
    {
        for (int i = 0; i < stageCount; i++)
        {
            if (i == index)
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, Vector2.one, 0.1f);
            else
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(0.8f, 0.8f), 0.1f);

            if (LeftArrow != null)
                LeftArrow.SetActive(index > 0);
            if (RightArrow != null)
                RightArrow.SetActive(index < stageCount - 1);
        }
    }
}