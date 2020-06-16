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
        GameSession.FadeIn(canvasGroup, 0f);
    }

    private void HideName()
    {
        GameSession.FadeOut(canvasGroup, 0f);
    }

    public static void ShowName_static(string name)
    {
        instance.ShowName(name);
    }

    public static void HideName_static()
    {
        instance.HideName();
    }
}
