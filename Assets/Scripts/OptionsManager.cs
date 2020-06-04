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
    public int numberOfButtons;
    Button option;
    UnityAction handleClick;
    Dictionary<int, string[]> responses = new Dictionary<int, string[]>();

    // Cached references
    public GameObject optionsBox;
    public Button optionPrefab;
    public Button[] optionButtons;
    public GameObject descriptionBox;
    public GameObject selectedObject;
    public GameSession gameSession;
    private ObjectProperties objectProperties;

    // Start is called before the first frame update
    void Start()
    {
        optionsBox.SetActive(false);
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
            }
        }
        // Object has only LOSA responses
        else
        {
            descriptionBox.SetActive(true);
            float LOSA = gameSession.GetLOSA();
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
        }
        
    }

    private void HandleResponse(int buttonIndex, int reaction)
    {
        // Check whether response is positive or negative
        gameSession.ChangeLOSA(reaction);

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
                ShowNextDescription(buttonIndex);
            }
            else
            {
                descriptionBox.SetActive(false);
            }
        }

        // Clear the options and descriptions
        CloseAndClearOptionsBox();
    }

    private void CloseAndClearOptionsBox()
    {
        responses.Clear();
        foreach (Transform child in optionsBox.transform)
        {
            Destroy(child.gameObject);
        }
        optionsBox.SetActive(false);
    }

    private void ShowNextDescription(int buttonIndex)
    {
        if(!descriptionBox.activeSelf)
        {
            descriptionBox.SetActive(true);
        }
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responses[buttonIndex][1];
    }
}
