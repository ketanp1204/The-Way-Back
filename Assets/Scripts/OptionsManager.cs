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
    private GameObject gameSession;
    public ObjectProperties objectProperties;

    // Start is called before the first frame update
    void Start()
    {
        optionsBoxCG = optionsBox.GetComponent<CanvasGroup>();
        optionsBoxCG.alpha = 0f;
        descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        optionsBox.SetActive(false);
        gameSession = GameObject.Find("GameSession");
    }

    void Update()
    {
        if(optionsBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            CloseAndClearOptionsBox();
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
        // optionsBox.SetActive(false);
        StartCoroutine(GameSession.DisableGameObjectAfterDelay(optionsBox));
    }

    

    /// <summary>
    /// Section for handling the responses to various types of object behavior
    /// </summary>

    public void HandleLOSAResponseOnly()
    {
        descriptionBox.SetActive(true);
        float LOSA = gameSession.GetComponent<GameSession>().GetLOSA();
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
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responseText;
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }

    public void HandleLOSAMediumThenOptions()
    {
        descriptionBox.SetActive(true);
        float LOSA = gameSession.GetComponent<GameSession>().GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responseText;
            GameSession.FadeIn(descriptionBoxCG, 0f);
        }
        else
        {
            objectProperties.HandleResponse(false);
        }
    }

    public void HandleLOSAHighThenOptions()
    {
        descriptionBox.SetActive(true);
        float LOSA = gameSession.GetComponent<GameSession>().GetLOSA();
        string responseText = "";
        if (LOSA < 30)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responseText;
            GameSession.FadeIn(descriptionBoxCG, 0f);
        }
        else if(LOSA >= 30 && LOSA < 70)
        {
            responseText = objectProperties.losaResponseTexts.MedLOSA;
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responseText;
            GameSession.FadeIn(descriptionBoxCG, 0f);
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
        gameSession.GetComponent<GameSession>().ChangeLOSA(reaction);

        // Show a response text if any present
        if (objectProperties.responses[buttonIndex].Length > 0)
        {
            GameSession.FadeOut(descriptionBoxCG, 0f);
            ShowNextDescription(buttonIndex);
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

    private void ShowNextDescription(int buttonIndex)
    {
        if (descriptionBoxCG.alpha != 0)
        {
            descriptionBox.SetActive(true);
        }
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.responses[buttonIndex][0];
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }

    public void HandleOptionLOSAUpdateOnly()
    {
        if (objectProperties.description != "")
        {
            descriptionBox.SetActive(true);
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.description;
            GameSession.FadeIn(descriptionBoxCG, 0f);
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
                handleClick = () => HandleOptionResponse(buttonIndex, reaction, false, false);
                option.onClick.AddListener(handleClick);
            }

            GameSession.FadeIn(optionsBoxCG, 1.5f);
        }
    }

    public void HandleOptionDestroyOnPositive()
    {
        if (objectProperties.description != "")
        {
            descriptionBox.SetActive(true);
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.description;
            GameSession.FadeIn(descriptionBoxCG, 0f);
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
                handleClick = () => HandleOptionResponse(buttonIndex, reaction, true, false);
                option.onClick.AddListener(handleClick);
            }

            GameSession.FadeIn(optionsBoxCG, 1.5f);
        }
    }

    public void HandleOptionDestroyOnNegative()
    {
        if (objectProperties.description != "")
        {
            descriptionBox.SetActive(true);
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.description;
            GameSession.FadeIn(descriptionBoxCG, 0f);
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
                handleClick = () => HandleOptionResponse(buttonIndex, reaction, false, true);
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
