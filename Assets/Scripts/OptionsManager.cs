﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    // Local parameters
    [HideInInspector]
    public int numberOfButtons;     // Number of response options for an object calculated at runtime
    private Button option;          // Local variable to instantiate option prefab
    private Button nextButton;      // Local variable to instantiate next button prefab
    private Button poemNextButton;  // Local variable to instantiate poem next button prefab;
    UnityAction handleClick;        // Custom action for option response
    Dictionary<int, string[]> responses = new Dictionary<int, string[]>();      // Stores response texts for option choices
    private WaitForUIButtons nextButtonWait;        // Custom yield instruction class to wait for button press inside a coroutine

    // Cached references
    [HideInInspector]
    public GameObject selectedObject;               // The object which is selected in the scene
    private UIReferences uiReferences;              // Stores references to UI objects in the current scene
    public Button optionPrefab;                     // Stores a reference to the Option prefab
    public Button nextButtonPrefab;                 // Stores a reference to the nextButton prefab
    private GameObject optionsBox;                  // Stores a reference to the options box
    private GameObject descriptionBox;              // Stores a reference to the description box
    private CanvasGroup optionsBoxCG;               // Stores a reference to the options box canvas group
    private CanvasGroup descriptionBoxCG;           // Stores a reference to the description box canvas group
    private TextMeshProUGUI descriptionText;        // Stores a reference to the description text
    private GameSession gameSession;                // Stores a reference to the current GameSession instance
    private PersistentObjectData objectData;        // Stores a reference to the singleton object data instance
    private GameObject poemPage;                    // Stores a reference to the poem page object
    private TextMeshProUGUI poemText;               // Stores a reference to the poem text object
    public Button poemNextButtonPrefab;            // Stores a reference to the poem next button prefab

    [HideInInspector]
    public ObjectProperties objectProperties;       // Stores a reference to the selected object's properties
    [HideInInspector]
    public bool IsWriting = false;                  // Boolean to store whether text is being automatically typed on the description box
    private GameObject dynamicUI;                   // Stores a reference to the DynamicUI canvas
    [SerializeField]
    public Sprite redSelectedOptionSprite;          // Stores the selected red sprite for the option button
    [SerializeField]
    public Sprite greenSelectedOptionSprite;        // Stores the selected green sprite for the option button

    // Start is called before the first frame update
    void OnEnable()
    {
        instance = this;
        uiReferences = FindObjectOfType<UIReferences>();
        optionsBox = uiReferences.optionsBox;
        optionsBoxCG = optionsBox.GetComponent<CanvasGroup>();
        optionsBoxCG.alpha = 0f;
        descriptionBox = uiReferences.descriptionBox;
        descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        descriptionText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
        poemPage = uiReferences.poemPage;
        poemText = uiReferences.poemText;
        optionsBox.SetActive(false);
        gameSession = FindObjectOfType<GameSession>();
        dynamicUI = GameObject.Find("DynamicUI");
        objectData = FindObjectOfType<PersistentObjectData>();
    }

    void Update()
    {
        if(optionsBox.activeSelf && Input.GetKeyDown(KeyCode.Space))        // Close options box if space is pressed
        {
            CloseAndClearOptionsBox();
        }
    }

    public void ShowTextOnDescriptionBox(string[] texts, float delay)       // Method to type multiple texts automatically on the description box
    {
        DescriptionBoxManager.ShowText(texts, delay);
    }

    public static void ShowOptionsAfterTyping()
    {
        instance.OptionsAfterTyping();
    }

    private void OptionsAfterTyping()
    {
        if (objectProperties != null)
        {
            if (objectProperties.numberOfResponses > 0
            && !objectProperties.responseSelected
            && objectProperties.showOptions)            // If object has options to choose from, then display them
            {
                ShowOptions();
                objectProperties.showOptions = false;
            }
        }

        if (GameSession.instance.instructionsEnabled)
        {
            if (!GameSession.instructionsSeen)
            {
                GameSession.instructionsSeen = true;
            }
        }
    }

    public void SetSelectedObjectReference(GameObject gameObject)       // Sets the references to an object when it is selected in the scene
    {
        selectedObject = gameObject;
        objectProperties = selectedObject.GetComponent<ObjectProperties>();
    }

    public void CloseAndClearOptionsBox()           // Clear option texts and hide the options box
    {
        StartCoroutine(OptionsFadeOut(optionsBoxCG, 0f));
        StartCoroutine(GameSession.DisableGameObjectAfterDelay(optionsBox, 1f));
    }

    private IEnumerator OptionsFadeOut(CanvasGroup canvasGroup, float delay)    // Fade out options box after delay
    {
        GameSession.instance.StartCoroutine(GameSession.FadeCanvasGroup(canvasGroup, 1f, 0f, delay, 0.3f));

        yield return new WaitForSeconds(0.3f);

        responses.Clear();
        foreach (Transform child in optionsBox.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Section for handling the responses to various types of object behavior
    /// Abbreviations:
    /// LOSA: Level of self-awareness score
    /// </summary>

    public void HandleLOSAResponseOnly()    // Object will only have a text response based on the current LOSA score
    {
        string[] responseText;
        if (GameSession.GetLOSAStatus() == GameSession.LOSAStatus.LOW)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
        }
        else if (GameSession.GetLOSAStatus() == GameSession.LOSAStatus.MEDIUM)
        {
            responseText = objectProperties.losaResponseTexts.MedLOSA;
        }
        else
        {
            responseText = objectProperties.losaResponseTexts.HighLOSA;
        }
        ShowTextOnDescriptionBox(responseText, 0f);
    }

    public void HandleLOSAMediumThenOptions()       // If LOSA is medium, then object has options to choose from, otherwise has a low LOSA response
    {
        string[] responseText;
        if (GameSession.GetLOSAStatus() == GameSession.LOSAStatus.LOW)
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

    public void HandleLOSAHighThenOptions()         // If LOSA is high, then object has options to choose from, otherwise has a low and a medium LOSA response
    {
        string[] responseText;
        if (GameSession.GetLOSAStatus() == GameSession.LOSAStatus.LOW)
        {
            responseText = objectProperties.losaResponseTexts.LowLOSA;
            ShowTextOnDescriptionBox(responseText, 0f);
        }
        else if (GameSession.GetLOSAStatus() == GameSession.LOSAStatus.MEDIUM)
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

    // Handles the events that take place when an option is selected
    private void HandleOptionResponse(int buttonIndex, int reaction, bool destroyOnPositive, bool destroyOnNegative, bool behaviorAfterChoice, bool hasPoem)
    {
        objectProperties.LOSAUpdateResponse = reaction;     // Stores the type of response selected in the object's properties
        objectProperties.responseIndex = buttonIndex;       // Stores the index of the response selected in the object's properties
        
        // Check whether response is positive or negative
        gameSession.ChangeLOSA(reaction);                   // Update the LOSA score

        // Show a poem on positive response if present
        if (hasPoem && reaction == 1)
        {
            uiReferences.interactableObjects.SetActive(false);
            ShowPoem();

            GameSession.FadeOut(descriptionBoxCG, 0f);
            StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox, 0.5f));
        }
        else
        {
            // Show a response text if any present
            if (objectProperties.responses[buttonIndex].Length != 0)
            {
                if (objectProperties.responses[buttonIndex].Length > 0)
                {
                    GameSession.FadeOut(descriptionBoxCG, 0f);
                    ShowNextResponse(buttonIndex);
                }
            }
            else
            {
                // Close the description box
                GameSession.FadeOut(descriptionBoxCG, 0f);
                StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox, 0.5f));
            }
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

        

        // Set the interactedWith boolean of the object so that it can't be clicked on again
        if(!objectProperties.additionalTags.Contains("OptionsMultipleTimes"))
        {
            objectProperties.interactedWith = true;
            objectProperties.responseSelected = true;
        }

        // Store the interacted object in the singleton data instance
        objectData.interactedObjects.Add(selectedObject.name);

        // Clear and hide the options box
        CloseAndClearOptionsBox();
    }

    private void ShowNextResponse(int buttonIndex)      // Shows the response if present when an option is selected
    {
        if (descriptionBoxCG.alpha != 0)
        {
            descriptionBox.SetActive(true);
        }
        ShowTextOnDescriptionBox(objectProperties.responses[buttonIndex], 0f);
    }

    private void ShowPoem()
    {
        PoemManager.ShowPoem(objectProperties.poem, 0f);
    }

    public static void ShowResponseAfterPoem()
    {
        instance.uiReferences.interactableObjects.SetActive(true);
        instance.StartCoroutine(instance.ResponseAfterPoem());
    }

    private IEnumerator ResponseAfterPoem()
    {
        yield return new WaitForSeconds(0.5f);

        // Show a response text if any present
        if (objectProperties.responses[objectProperties.responseIndex].Length != 0)
        {
            if (objectProperties.responses[objectProperties.responseIndex].Length > 0)
            {
                ShowNextResponse(objectProperties.responseIndex);
            }
        }
    }

    public void HandleOptionLOSAUpdateOnly()       // Option has only options to choose from
    {
        objectProperties.showOptions = true;
        if (objectProperties.description[0] != "")
        {
            ShowTextOnDescriptionBox(objectProperties.description, 0f);
        }
        else
        {
            if(objectProperties.additionalTags.Contains("OptionsWithoutDescription"))
            {
                ShowOptions();
            }
        }
    }

    public void ShowOptions()          // Display the option texts when an object is clicked on
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
                if(reaction == 1)
                {
                    SpriteState selectedSprite = option.spriteState;
                    selectedSprite.selectedSprite = greenSelectedOptionSprite;
                    option.spriteState = selectedSprite;
                }
                else if(reaction == 2)
                {
                    SpriteState selectedSprite = option.spriteState;
                    selectedSprite.selectedSprite = redSelectedOptionSprite;
                    option.spriteState = selectedSprite;
                }
                handleClick = () => HandleOptionResponse(buttonIndex, 
                                                         reaction, 
                                                         objectProperties.destroyOnPositive, 
                                                         objectProperties.destroyOnNegative, 
                                                         objectProperties.hasBehavior,
                                                         objectProperties.hasPoem);
                option.onClick.AddListener(handleClick);
            }

            GameSession.FadeIn(optionsBoxCG, 0.5f);
        }
    }

    public void HandleBehaviorOnly()        // For objects that only have specific behavior on option response
    {
        selectedObject.GetComponent<ObjectSpecificBehavior>().HandleBehavior(selectedObject);
    }
}
