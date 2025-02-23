﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayObjectName : MonoBehaviour
{
    // Cached References
    private static DisplayObjectName instance;
    private UIReferences uiReferences;                  // Stores the references to the UI elements in the current scene
    private TextMeshProUGUI objectNameText;             // Reference to the Text field which displays the name
    private RectTransform backgroundRectTransform;      // To create the background for the name text
    private CanvasGroup canvasGroup;                    // Reference to the canvas group of the object name gameobject
    private LevelChanger levelChanger;                  // Reference to the level changer gameobject

    private void OnEnable()
    {
        instance = this;
        uiReferences = FindObjectOfType<UIReferences>();
        levelChanger = FindObjectOfType<LevelChanger>();
        backgroundRectTransform = uiReferences.objectNameBackground.GetComponent<RectTransform>();
        objectNameText = uiReferences.objectNameText.GetComponent<TextMeshProUGUI>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (levelChanger.fadeAnimationRunning)      // Prevent displaying object names when screen fade animation is running
        {
            HideName();
        }
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        transform.localPosition = localPoint;
    }

    private void ShowName(string name)              // Display the name on hovering over the gameObject
    {
        objectNameText.text = name;
        float textPaddingSize = 8f;
        Vector2 backgroundSize = new Vector2(objectNameText.preferredWidth + textPaddingSize * 2f, objectNameText.preferredHeight + textPaddingSize * 2f);
        backgroundRectTransform.sizeDelta = backgroundSize;
        GameSession.instance.StartCoroutine(GameSession.FadeCanvasGroup(canvasGroup, 0f, 1f, 0f, 0.1f));
    }

    private void HideName()                         // Hide the name after clicking or moving away from the gameObject
    {
        if(canvasGroup.alpha != 0)
        {
            GameSession.instance.StartCoroutine(GameSession.FadeCanvasGroup(canvasGroup, 1f, 0f, 0f, 0.1f));
        }
    }

    public static void ShowName_static(string name)         // Static method to call ShowName()
    {
        instance.ShowName(name);
    }

    public static void HideName_static()                    // Static method to call HideName()
    {
        instance.HideName();
    }
}
