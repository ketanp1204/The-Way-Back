using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayObjectName : MonoBehaviour
{

    // Cached References
    private static DisplayObjectName instance;
    private TextMeshProUGUI objectNameText;
    private RectTransform backgroundRectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        instance = this;
        backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        objectNameText = transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        transform.localPosition = localPoint;
    }

    private void ShowName(string name)
    {
        objectNameText.text = name;
        float textPaddingSize = 8f;
        Vector2 backgroundSize = new Vector2(objectNameText.preferredWidth + textPaddingSize * 2f, objectNameText.preferredHeight + textPaddingSize * 2f);
        backgroundRectTransform.sizeDelta = backgroundSize;
        FadeIn();
    }

    private void HideName()
    {
        FadeOut();
    }

    public static void ShowName_static(string name)
    {
        instance.ShowName(name);
    }

    public static void HideName_static()
    {
        instance.HideName();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(1f, 0f));
    }

    public IEnumerator FadeCanvasGroup(float start, float end, float lerpTime = 0.3f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while(true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            canvasGroup.alpha = currentValue;

            if(percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
