using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
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
    private ObjectProperties objectProperties;

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

    public void InitializeResponse()
    {
        objectProperties = selectedObject.GetComponent<ObjectProperties>();

        // Object has options to choose from instead of only LOSA reactions
        if (objectProperties.numberOfLOSAResponses == 0)
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

                // Store the responses of the individual option buttons into a dictionary
                if (numberOfButtons == 3)
                {
                    responses.Add(0, objectProperties.option1responses);
                    responses.Add(1, objectProperties.option2responses);
                    responses.Add(2, objectProperties.option3responses);
                }
                else if (numberOfButtons == 2)
                {
                    responses.Add(0, objectProperties.option1responses);
                    responses.Add(1, objectProperties.option2responses);
                }
                else if (numberOfButtons == 1)
                {
                    responses.Add(0, objectProperties.option1responses);
                }

                for(int i = 0; i < numberOfButtons; i++)
                {
                    // Instantiate a new option button
                    option = Instantiate(optionPrefab, optionsBox.transform);
                    option.gameObject.SetActive(true);

                    // Fill the text field with the option text
                    option.GetComponentInChildren<TextMeshProUGUI>().text = responses[i][0];

                    // Handle click behaviour of the button
                    int buttonIndex = i;
                    int reaction = objectProperties.reactions[i];
                    handleClick = () => HandleResponse(buttonIndex, reaction);
                    option.onClick.AddListener(handleClick);
                }

                GameSession.FadeIn(optionsBoxCG, 1f);
            }
        }
        // Object has only LOSA responses
        else
        {
            descriptionBox.SetActive(true);
            float LOSA = gameSession.GetComponent<GameSession>().GetLOSA();
            string responseText = "";
            if(LOSA < 30)
            {
                responseText = objectProperties.losaResponseLow;
            }
            else if(LOSA >= 30 && LOSA < 70)
            {
                responseText = objectProperties.losaResponseMedium;
            }
            else
            {
                responseText = objectProperties.losaResponseHigh;
            }
            descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responseText;
            GameSession.FadeIn(descriptionBoxCG, 0f);
        }
        
    }

    private void HandleResponse(int buttonIndex, int reaction)
    {
        // Check whether response is positive or negative
        gameSession.GetComponent<GameSession>().ChangeLOSA(reaction);

        // Destroy object if specified in object properties
        if (reaction == 0 && objectProperties.destroyOnNegativeResponse)
        {
            selectedObject.SetActive(false);
        }

        // Show a response text if any present
        if (responses.Count > 0)
        {
            if (responses[buttonIndex].Length > 1)
            {
                GameSession.FadeOut(descriptionBoxCG, 0f);
                ShowNextDescription(buttonIndex);
            }
            else
            {
                GameSession.FadeOut(descriptionBoxCG, 0f);
                // descriptionBox.SetActive(false);
                StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox));
            }
        }

        // Clear the options and descriptions
        CloseAndClearOptionsBox();
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

    private void ShowNextDescription(int buttonIndex)
    {
        if(descriptionBoxCG.alpha != 0)
        {
            descriptionBox.SetActive(true);
        }
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responses[buttonIndex][1];
        GameSession.FadeIn(descriptionBoxCG, 0f);
    }
}
