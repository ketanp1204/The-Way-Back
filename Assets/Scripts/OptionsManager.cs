using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    // Configuration parameters
    [HideInInspector]
    public int numberOfButtons;
    Button option;
    Button nextButton;
    UnityAction handleClick;
    Dictionary<int, string[]> responses = new Dictionary<int, string[]>();
    private WaitForUIButtons nextButtonWait;

    // Cached references
    [HideInInspector]
    public GameObject selectedObject;
    private UIReferences uiReferences;
    public Button optionPrefab;
    private GameObject optionsBox;
    private GameObject descriptionBox;
    private CanvasGroup optionsBoxCG;
    private CanvasGroup descriptionBoxCG;
    private TextMeshProUGUI descriptionText;
    private GameSession gameSession;
    [HideInInspector]
    public ObjectProperties objectProperties;
    public Button nextButtonPrefab;
    [HideInInspector]
    public bool IsWriting = false;
    private GameObject dynamicUI;

    // Start is called before the first frame update
    void OnEnable()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        optionsBox = uiReferences.optionsBox;
        optionsBoxCG = optionsBox.GetComponent<CanvasGroup>();
        optionsBoxCG.alpha = 0f;
        descriptionBox = uiReferences.descriptionBox;
        descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        descriptionText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
        optionsBox.SetActive(false);
        gameSession = FindObjectOfType<GameSession>();
        dynamicUI = GameObject.Find("DynamicUI");
    }

    void Update()
    {
        if(optionsBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            CloseAndClearOptionsBox();
        }
    }

    public void ShowTextOnDescriptionBox(string text, float delay)
    {
        StartCoroutine(TypeText(new string[] {text}, delay));
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }

    public void ShowTextOnDescriptionBox(string[] texts, float delay)
    {
        StartCoroutine(TypeText(texts, delay));
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }

    IEnumerator TypeText(string[] texts, float delay)
    {
        yield return new WaitForSeconds(delay);

        descriptionBox.SetActive(true);
        IsWriting = true;
        for(int i = 0; i < texts.Length; i++)
        {
            string text = texts[i];
            descriptionText.text = text;
            descriptionText.overflowMode = TextOverflowModes.Page;
            descriptionText.maxVisibleCharacters = 0;

            descriptionText.ForceMeshUpdate();
            int pageIndex = 1;

            while (pageIndex < descriptionText.textInfo.pageCount)
            {
                descriptionText.pageToDisplay = pageIndex;

                int firstCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].firstCharacterIndex;
                int lastCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].lastCharacterIndex;

                for (int j = firstCharIndex; j <= lastCharIndex; j++)
                {
                    if (j > 5 && Input.GetMouseButtonDown(0))
                    {
                        bool flag = false;
                        PointerEventData pointer = new PointerEventData(EventSystem.current);
                        pointer.position = Input.mousePosition;

                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        dynamicUI.GetComponent<GraphicRaycaster>().Raycast(pointer, raycastResults);
                        // EventSystem.current.RaycastAll(pointer, raycastResults);

                        if (!(raycastResults.Count == 0))
                        {
                            if (raycastResults[0].gameObject.name == descriptionBox.name || raycastResults[0].gameObject.name == descriptionText.name)
                            {
                                flag = true;
                            }
                            if (flag == true)
                            {
                                descriptionText.maxVisibleCharacters = lastCharIndex;
                                break;
                            }
                        }
                    }

                    descriptionText.maxVisibleCharacters = j + 1;
                    yield return new WaitForSeconds(0.015f);

                }

                pageIndex += 1;

                nextButton = Instantiate(nextButtonPrefab, GameObject.Find("DynamicUI").transform);
                nextButton.gameObject.SetActive(true);

                nextButtonWait = new WaitForUIButtons(nextButton);
                yield return nextButtonWait;

                Destroy(nextButton.gameObject);
            }
            if (pageIndex == descriptionText.textInfo.pageCount)
            {
                descriptionText.pageToDisplay = pageIndex;

                int firstCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].firstCharacterIndex;
                int lastCharIndex = descriptionText.textInfo.pageInfo[pageIndex - 1].lastCharacterIndex;

                for (int j = firstCharIndex; j <= lastCharIndex; j++)
                {
                    if (j > 5 && Input.GetMouseButtonDown(0))
                    {
                        bool flag = false;
                        PointerEventData pointer = new PointerEventData(EventSystem.current);
                        pointer.position = Input.mousePosition;

                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        dynamicUI.GetComponent<GraphicRaycaster>().Raycast(pointer, raycastResults);
                        //EventSystem.current.RaycastAll(pointer, raycastResults);

                        if (!(raycastResults.Count == 0))
                        {
                            if (raycastResults[0].gameObject.name == descriptionBox.name || raycastResults[0].gameObject.name == descriptionText.name)
                            {
                                flag = true;
                            }
                            if (flag == true)
                            {
                                descriptionText.maxVisibleCharacters = lastCharIndex;
                                break;
                            }
                        }
                    }

                    descriptionText.maxVisibleCharacters = j + 1;
                    yield return new WaitForSeconds(0.015f);
                }
            }

            if (objectProperties != null)
            {
                if (objectProperties.numberOfResponses > 0
                && !objectProperties.responseSelected
                && objectProperties.showOptions)
                {
                    ShowOptions();
                }
            }

            if(i < texts.Length - 1)
            {
                nextButton = Instantiate(nextButtonPrefab, GameObject.Find("DynamicUI").transform);
                nextButton.gameObject.SetActive(true);

                nextButtonWait = new WaitForUIButtons(nextButton);
                yield return nextButtonWait;

                Destroy(nextButton.gameObject);
            }
        }

        IsWriting = false;
    }

    public void SetSelectedObjectReference(GameObject gameObject)
    {
        selectedObject = gameObject;
        objectProperties = selectedObject.GetComponent<ObjectProperties>();
    }

    public void CloseAndClearOptionsBox()
    {
        responses.Clear();
        foreach (Transform child in optionsBox.transform)
        {
            Destroy(child.gameObject);
        }
        GameSession.FadeOut(optionsBoxCG, 0f);
        StartCoroutine(GameSession.DisableGameObjectAfterDelay(optionsBox));
    }

    /// <summary>
    /// Section for handling the responses to various types of object behavior
    /// </summary>

    public void HandleLOSAResponseOnly()
    {
        float LOSA = gameSession.GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
        }
        else if (LOSA >= 30 && LOSA < 70)
        {
            responseText = objectProperties.losaResponseTexts.MedLOSA;
        }
        else
        {
            responseText = objectProperties.losaResponseTexts.HighLOSA;
        }
        ShowTextOnDescriptionBox(responseText, 0f);
    }

    public void HandleLOSAMediumThenOptions()
    {
        float LOSA = gameSession.GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            ShowTextOnDescriptionBox(responseText, 0f);
        }
        else
        {
            objectProperties.showOptions = true;
            objectProperties.HandleResponse(1);
        }
    }

    public void HandleLOSAHighThenOptions()
    {
        float LOSA = gameSession.GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            ShowTextOnDescriptionBox(responseText, 0f);
        }
        else if(LOSA >= 30 && LOSA < 70)
        {
            responseText = objectProperties.losaResponseTexts.MedLOSA;
            ShowTextOnDescriptionBox(responseText, 0f);
        }
        else
        {
            objectProperties.showOptions = true;
            objectProperties.HandleResponse(1);
        }
    }

    private void HandleOptionResponse(int buttonIndex, int reaction, bool destroyOnPositive, bool destroyOnNegative, bool behaviorAfterChoice)
    {
        objectProperties.LOSAUpdateResponse = reaction;
        objectProperties.responseSelected = true;
        // Check whether response is positive or negative
        gameSession.ChangeLOSA(reaction);

        // Show a response text if any present
        if (objectProperties.responses[buttonIndex].Length != 0)
        {
            if(objectProperties.responses[buttonIndex].Length > 0)
            {
                GameSession.FadeOut(descriptionBoxCG, 0f);
                ShowNextResponse(buttonIndex);
            }
        }
        else
        {
            GameSession.FadeOut(descriptionBoxCG, 0f);
            StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox));
        }

        // Destroy an object after positive response
        if(reaction == 1 && destroyOnPositive == true)
        {
            Destroy(selectedObject);
        }

        // Destroy an object after negative response
        if (reaction == 2 && destroyOnNegative == true)
        {
            Destroy(selectedObject);
        }

        // Run specific object behavior if present
        if (behaviorAfterChoice)
        {
            selectedObject.GetComponent<ObjectSpecificBehavior>().HandleBehavior(selectedObject);
        }

        // Clear the options and descriptions
        CloseAndClearOptionsBox();
    }

    private void ShowNextResponse(int buttonIndex)
    {
        if (descriptionBoxCG.alpha != 0)
        {
            descriptionBox.SetActive(true);
        }
        ShowTextOnDescriptionBox(objectProperties.responses[buttonIndex][0], 0f);
    }

    public void HandleOptionLOSAUpdateOnly()
    {
        objectProperties.showOptions = true;
        if (objectProperties.description != "")
        {
            ShowTextOnDescriptionBox(objectProperties.description, 0f);
        }
    }

    private void ShowOptions()
    {
        numberOfButtons = objectProperties.numberOfResponses;
        if (numberOfButtons != 0)
        {
            optionsBox.SetActive(true);

            for (int i = 0; i < numberOfButtons; i++)
            {
                // Instantiate a new option button
                option = Instantiate(optionPrefab, optionsBox.transform);
                option.gameObject.SetActive(true);

                // Fill the text field with the option text
                option.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.optionTexts[i];

                // Handle click behaviour of the button
                int buttonIndex = i;
                int reaction = objectProperties.reactions[i];
                handleClick = () => HandleOptionResponse(buttonIndex, 
                                                         reaction, 
                                                         objectProperties.destroyOnPositive, 
                                                         objectProperties.destroyOnNegative, 
                                                         objectProperties.hasBehavior);
                option.onClick.AddListener(handleClick);
            }

            GameSession.FadeIn(optionsBoxCG, 0.5f);
        }
    }

    public void HandleBehaviorOnly()
    {
        selectedObject.GetComponent<ObjectSpecificBehavior>().HandleBehavior(selectedObject);
    }
}
