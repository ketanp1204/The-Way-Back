using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{

    // Configuration parameters
    [HideInInspector]
    public int numberOfButtons;
    Button option;
    UnityAction handleClick;
    Dictionary<int, string[]> responses = new Dictionary<int, string[]>();

    // Cached references
    [HideInInspector]
    public GameObject selectedObject;
    public Button optionPrefab;
    public GameObject optionsBox;
    public GameObject descriptionBox;
    private CanvasGroup optionsBoxCG;
    private CanvasGroup descriptionBoxCG;
    private TextMeshProUGUI descriptionText;
    private GameSession gameSession;
    public ObjectProperties objectProperties;

    // Start is called before the first frame update
    void Start()
    {
        optionsBoxCG = optionsBox.GetComponent<CanvasGroup>();
        optionsBoxCG.alpha = 0f;
        descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        descriptionText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
        optionsBox.SetActive(false);
        gameSession = FindObjectOfType<GameSession>();
    }

    void Update()
    {
        if(optionsBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            CloseAndClearOptionsBox();
        }
    }

    public void ShowTextOnDescriptionBox(string text)
    {
        descriptionBox.SetActive(true);
        descriptionText.text = text;
        descriptionText.ForceMeshUpdate();
        StartCoroutine(TypeText());
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }

    IEnumerator TypeText()
    {
        int pageCount = descriptionText.textInfo.pageCount;
        string text = descriptionText.text;
        for (int i = 0; i < pageCount; i++)
        {
            int firstCharIndex = descriptionText.textInfo.pageInfo[i].firstCharacterIndex;
            int lastCharIndex = descriptionText.textInfo.pageInfo[i].lastCharacterIndex;
            string pageText = text.Substring(firstCharIndex, lastCharIndex - firstCharIndex + 1);
            descriptionText.text = "";
            foreach (char letter in pageText.ToCharArray())
            {
                descriptionText.text += letter;
                yield return new WaitForSeconds(0.035f);
            }
        }
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
        descriptionBox.SetActive(true);
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
        ShowTextOnDescriptionBox(responseText);
    }

    public void HandleLOSAMediumThenOptions()
    {
        descriptionBox.SetActive(true);
        float LOSA = gameSession.GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            ShowTextOnDescriptionBox(responseText);
        }
        else
        {
            objectProperties.HandleResponse(false);
        }
    }

    public void HandleLOSAHighThenOptions()
    {
        descriptionBox.SetActive(true);
        float LOSA = gameSession.GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            ShowTextOnDescriptionBox(responseText);
        }
        else if(LOSA >= 30 && LOSA < 70)
        {
            responseText = objectProperties.losaResponseTexts.MedLOSA;
            ShowTextOnDescriptionBox(responseText);
        }
        else
        {
            objectProperties.HandleResponse(false);
        }
    }

    private void HandleOptionResponse(int buttonIndex, int reaction, bool destroyOnPositive, bool destroyOnNegative)
    {
        selectedObject.GetComponent<ObjectProperties>().LOSAUpdateResponse = reaction;
        // Check whether response is positive or negative
        gameSession.ChangeLOSA(reaction);

        // Show a response text if any present
        if (objectProperties.responses[buttonIndex] != null)
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

        // Clear the options and descriptions
        CloseAndClearOptionsBox();
    }

    private void ShowNextResponse(int buttonIndex)
    {
        if (descriptionBoxCG.alpha != 0)
        {
            descriptionBox.SetActive(true);
        }
        ShowTextOnDescriptionBox(objectProperties.responses[buttonIndex][0]);
    }

    public void HandleOptionLOSAUpdateOnly(bool destroyOnPositive, bool destroyOnNegative)
    {
        if (objectProperties.description != "")
        {
            descriptionBox.SetActive(true);
            ShowTextOnDescriptionBox(objectProperties.description);
        }
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
                handleClick = () => HandleOptionResponse(buttonIndex, reaction, destroyOnPositive, destroyOnNegative);
                option.onClick.AddListener(handleClick);
            }

            GameSession.FadeIn(optionsBoxCG, 1.5f);
        }
    }

    public void HandleOptionBehaviorAfterChoice()
    {
        selectedObject.GetComponent<ObjectSpecificBehavior>().HandleBehavior(selectedObject);
    }
}
