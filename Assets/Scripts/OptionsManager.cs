using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    // State variables
    public bool optionsActive = false;
    public bool descriptionActive = false;

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
        descriptionBox.SetActive(false);
    }

    public void InitializeResponse()
    {
        objectProperties = selectedObject.GetComponent<ObjectProperties>();

        // Object has options to choose from
        if (objectProperties.numberOfLOSAResponses == 0)
        {
            if (objectProperties.description != "")
            {
                descriptionActive = true;
                descriptionBox.SetActive(true);
                descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = objectProperties.description;
            }
            numberOfButtons = objectProperties.numberOfResponses;
            if (numberOfButtons != 0)
            {
                optionsActive = true;
                optionsBox.SetActive(true);

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
                    option = Instantiate(optionPrefab, optionsBox.transform);
                    option.gameObject.SetActive(true);
                    option.GetComponentInChildren<TextMeshProUGUI>().text = responses[i][0];
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
            descriptionActive = true;
            descriptionBox.SetActive(true);
            float LOSA = gameSession.GetLOSA();
            string responseText = "";
            if(LOSA <= 30)
            {
                responseText = objectProperties.losaResponseLow;
            }
            else if(LOSA > 30 && LOSA <= 70)
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
        if(reaction == 0 && objectProperties.destroyOnNegativeResponse)
        {
            selectedObject.SetActive(false);
        } 
        if(responses.Count > 0)
        {
            Debug.Log("button index: " + buttonIndex);
            Debug.Log("response length: " + responses[buttonIndex].Length);
            if (responses[buttonIndex].Length > 1)
            {
                ShowNextDescription(buttonIndex);
            }
        }
        

        responses.Clear();
        foreach(Transform child in optionsBox.transform)
        {
            Destroy(child.gameObject);
        }
        optionsActive = false;
        optionsBox.SetActive(false);
    }

    private void ShowNextDescription(int buttonIndex)
    {
        if(!descriptionActive)
        {
            descriptionActive = true;
            descriptionBox.SetActive(true);
        }
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = responses[buttonIndex][1];
    }

    // Update is called once per frame
    void Update()
    {
        if(descriptionActive && Input.GetKeyDown(KeyCode.Space))
        {
            descriptionBox.SetActive(false);
            descriptionActive = false;
        }
    }
}
