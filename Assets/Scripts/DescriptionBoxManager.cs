using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DescriptionBoxManager : MonoBehaviour
{
    private static DescriptionBoxManager instance;

    // Local parameters
    private Button nextButton;                  // Local variable to instantiate next button prefab
    private WaitForUIButtons nextButtonWait;    // Custom yield instruction class to wait for button press inside a coroutine
    public static bool IsWriting;               // Boolean to store whether text is being typed on the description box
    private bool skipText;                      // Boolean to trigger skipping text while typing

    // Cached references
    private CanvasGroup canvasGroup;            // Reference to the description box CanvasGroup
    private TextMeshProUGUI descriptionText;    // Reference to the text child of the description box gameObject
    private GameObject dynamicUI;               // Reference to the Dynamic UI Canvas GameObject 
    public Button nextButtonPrefab;             // Reference to the next button prefab

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        descriptionText = GetComponentInChildren<TextMeshProUGUI>();
        dynamicUI = GameObject.Find("DynamicUI");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && !IsWriting)       // Close the description box if space is pressed and there is no text being typed
        {
            if (canvasGroup.alpha != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                CloseDescriptionBox();
            }
        }
    }

    public static void CloseDescriptionBox()
    {
        GameSession.FadeOut(instance.canvasGroup, 0f);
        GameSession.instance.StartCoroutine(GameSession.DisableGameObjectAfterDelay(instance.gameObject, 0.5f));
    }

    public static void ShowText(string[] texts, float delay)
    {
        instance.gameObject.SetActive(true);
        instance.StartCoroutine(instance.StartTyping(texts, delay));
    }

    private IEnumerator StartTyping(string[] texts, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        instance.StartCoroutine(instance.TypeText(texts));
    }

    private IEnumerator WaitForMouseClick()                         // Coroutine to detect mouse click on the description box while typing
    {
        while(IsWriting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                dynamicUI.GetComponent<GraphicRaycaster>().Raycast(pointer, raycastResults);

                if (!(raycastResults.Count == 0))
                {
                    if (raycastResults[0].gameObject.name == gameObject.name || raycastResults[0].gameObject.name == descriptionText.gameObject.name)
                    {
                        skipText = true;
                        break;
                    }
                }                
            }
            yield return null;
        }
    }

    public IEnumerator TypeText(string[] texts)
    {
        GameSession.FadeIn(canvasGroup, 0f);
        IsWriting = true;                               // Writing to the description box starts

        for (int i = 0; i < texts.Length; i++)
        {
            string text = texts[i];
            descriptionText.text = text;
            descriptionText.overflowMode = TextOverflowModes.Page;
            descriptionText.maxVisibleCharacters = 0;

            descriptionText.ForceMeshUpdate();
            int pageIndex = 1;

            while (pageIndex < descriptionText.textInfo.pageCount)
            {
                StartCoroutine(WaitForMouseClick());
                skipText = false;                               // Initialize skip text trigger

                descriptionText.pageToDisplay = pageIndex;

                int firstCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].firstCharacterIndex;
                int lastCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].lastCharacterIndex;

                for (int j = firstCharIndex; j <= lastCharIndex; j++)
                {
                    if (skipText)
                    {
                        // Stop typing and skip to end of page
                        descriptionText.maxVisibleCharacters = lastCharIndex;
                        break;
                    }
                    else
                    {
                        // Keep typing
                        descriptionText.maxVisibleCharacters = j + 1;
                        yield return new WaitForSeconds(0.015f);
                    }
                }

                pageIndex += 1;

                nextButton = Instantiate(nextButtonPrefab, dynamicUI.transform);
                nextButton.gameObject.SetActive(true);

                nextButtonWait = new WaitForUIButtons(nextButton);
                yield return nextButtonWait;

                Destroy(nextButton.gameObject);
            }
            if (pageIndex == descriptionText.textInfo.pageCount)            // Last page of the text
            {
                StartCoroutine(WaitForMouseClick());
                skipText = false;                               // Initialize skip text trigger

                descriptionText.pageToDisplay = pageIndex;

                int firstCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].firstCharacterIndex;
                int lastCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].lastCharacterIndex;

                for (int j = firstCharIndex; j <= lastCharIndex; j++)
                {
                    if (skipText)
                    {
                        // Stop typing and skip to end of page
                        descriptionText.maxVisibleCharacters = lastCharIndex;
                        break;
                    }
                    else
                    {
                        // Keep typing
                        descriptionText.maxVisibleCharacters = j + 1;
                        yield return new WaitForSeconds(0.015f);
                    }
                }
            }

            if (i < texts.Length - 1)                        // Creates a 'Next' button to move to the next page in the text
            {
                nextButton = Instantiate(nextButtonPrefab, GameObject.Find("DynamicUI").transform);
                nextButton.gameObject.SetActive(true);

                nextButtonWait = new WaitForUIButtons(nextButton);
                yield return nextButtonWait;                 // Wait for next button to be clicked before typing the next page of text

                Destroy(nextButton.gameObject);
            }
        }

        IsWriting = false;                                   // Writing to the description box is finished

        OptionsManager.ShowOptionsAfterTyping();
    }
}
