using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectProperties : MonoBehaviour
{
    public enum ObjectType              // The types of behaviors possible when clicking on an object
    {
        LOSAResponseOnly,
        LOSAMediumThenOptions,
        LOSAHighThenOptions,
        OptionLOSAUpdateOnly,
        OptionDestroyOnNegative,
        OptionDestroyOnPositive,
        OptionBehaviorAfterChoice,
        BehaviorOnly
    }

    [System.Serializable]
    public class LOSAResponseTexts      
    {
        public string[] LowLOSA;
        public string[] MedLOSA;
        public string[] HighLOSA;
    }

    // Cached References
    private OptionsManager optionsManager;      // Reference to the options manager gameobject

    // Variables that need to be set in inspector
    public List<string> additionalTags;         // Additional tags for an object if required
    public ObjectType[] objectType;             // To store the response type for the current object
    public string objectName;                   // Name of the object
    [TextArea(3, 10)]
    public string[] description;                // Description of the object
    public string option1Text;                  // Text for the first option 
    [TextArea(3, 10)]
    public string[] option1responses;           // Responses for the first option
    public string option2Text;                  // Text for the second option
    [TextArea(3, 10)]
    public string[] option2responses;           // Responses for the second option
    public string option3Text;                  // Text for the thrid option
    [TextArea(3, 10)]
    public string[] option3responses;           // Responses for the third option
    public int[] reactions;                     // Positive, negative or neutral response
    public LOSAResponseTexts losaResponseTexts; // LOSA reactions
    [TextArea(6, 10)]
    public string poem;                         // Poem if any present
    public GameObject closeUpObject;            // To store the close up object reference, if any
  
    // Variables that will be calculated in the code
    [HideInInspector]
    public int numberOfResponses;               // Number of options an object has to choose from
    [HideInInspector]
    public int numberOfLOSAResponses;
    [HideInInspector]
    public Dictionary<int, string[]> responses = new Dictionary<int, string[]>();
    [HideInInspector]
    public string[] optionTexts;                // Texts for the options
    [HideInInspector]
    public bool interactedWith = false;         // Stores whether an object has been already clicked on
    [HideInInspector]
    public int LOSAUpdateResponse = -1;         // The response that the player chooses for the object
    [HideInInspector]
    public bool destroyOnPositive = false;      // Stores whether an object has to be destroyed on a positive response
    [HideInInspector]
    public bool destroyOnNegative = false;      // Stores whether an object has to be destroyed on a negative response
    [HideInInspector]
    public bool hasBehavior = false;            // Stores whether an object has custom behavior
    [HideInInspector]
    public bool hasPoem = false;        // Stores whether an object has a poem in its positive response
    [HideInInspector]
    public bool responseSelected = false;       // Stores whether an option has been chosen 
    [HideInInspector]
    public int callIndex;                       // Indicates which behavior of an object to call (for objects that have multiple behaviors specified)
    [HideInInspector]
    public bool showOptions = false;            // Bool to store whether to display the options
    [HideInInspector]
    public int responseIndex;                   // Index of the option that was selected

    // Cached References
    private GameObject descriptionBox;          // Reference to the description box gameobject

    // Start is called before the first frame update
    void Start()
    {
        descriptionBox = FindObjectOfType<UIReferences>().descriptionBox;

    
        if (option3Text != "" & option2Text != "" && option1Text != "")           // Storing the number and the texts of the options 
        {
            numberOfResponses = 3;
            optionTexts = new string[3];
            optionTexts[0] = option1Text;
            optionTexts[1] = option2Text;
            optionTexts[2] = option3Text;
        }
        else if (option3Text == "" && option2Text != "" && option1Text != "")
        {
            numberOfResponses = 2;
            optionTexts = new string[2];
            optionTexts[0] = option1Text;
            optionTexts[1] = option2Text;
        }
        else if (option3Text == "" && option2Text == "" && option1Text != "")
        {
            numberOfResponses = 1;
            optionTexts = new string[1];
            optionTexts[0] = option1Text;
        }
        else
        {
            numberOfResponses = 0;
            optionTexts = null;
        }


        
        if (numberOfResponses == 3)                                              // Storing the responses to each option, if present
        {
            responses.Add(0, option1responses);
            responses.Add(1, option2responses);
            responses.Add(2, option3responses);
        }
        else if (numberOfResponses == 2)
        {
            responses.Add(0, option1responses);
            responses.Add(1, option2responses);
        }
        else if (numberOfResponses == 1)
        {
            responses.Add(0, option1responses);
        }

        if (objectType.Contains(ObjectType.OptionDestroyOnNegative))
        {
            destroyOnNegative = true;
        }
        else if(objectType.Contains(ObjectType.OptionDestroyOnPositive))
        {
            destroyOnPositive = true;
        }

        if (additionalTags.Contains("Poem"))
        {
            hasPoem = true;
        }
        
        if(objectType.Contains(ObjectType.OptionBehaviorAfterChoice))
        {
            hasBehavior = true;
        }
    }



    public bool HasTag(string tag)                                               // Additional tags
    {
        return additionalTags.Contains(tag);
    }

    public IEnumerable<string> GetTags()
    {
        return additionalTags;
    }

    public void Rename(int index, string tagName)
    {
        additionalTags[index] = tagName;
    }

    public string GetAtIndex(int index)
    {
        return additionalTags[index];
    }

    public int Count
    {
        get { return additionalTags.Count; }
    }

    
    void OnMouseEnter()                                                         // Display object name on hover
    {
        if (EventSystem.current.IsPointerOverGameObject())                      // Prevent player from clicking through UI elements
            return;
        
        if(!GameSession.GameIsPaused && !descriptionBox.activeSelf)             // Prevent displaying object name when the game is paused         
        {
            PersistentObjectData objectData = FindObjectOfType<PersistentObjectData>();

            if(!interactedWith && !objectData.interactedObjects.Contains(gameObject.name) && GameSession.instructionsSeen)
                DisplayObjectName.ShowName_static(objectName);
        }
    }

    void OnMouseDown()                                                          // Hide object name on clicking on the object
    {
        DisplayObjectName.HideName_static();
    }

    void OnMouseExit()                                                          // Hide object name on moving the mouse away from the object
    {
        DisplayObjectName.HideName_static();
    }


    public void HandleResponse(int callIndex)                                   // Handle option click response
    {
        optionsManager = FindObjectOfType<OptionsManager>();
        ObjectType temp;
        temp = objectType[callIndex];
        switch(temp)
        {
            case ObjectType.LOSAResponseOnly:
                optionsManager.HandleLOSAResponseOnly();
                break;

            case ObjectType.LOSAMediumThenOptions:
                optionsManager.HandleLOSAMediumThenOptions();
                break;

            case ObjectType.LOSAHighThenOptions:
                optionsManager.HandleLOSAHighThenOptions();
                break;

            case ObjectType.OptionLOSAUpdateOnly:
                optionsManager.HandleOptionLOSAUpdateOnly();
                break;

            case ObjectType.OptionDestroyOnPositive:
                optionsManager.HandleOptionLOSAUpdateOnly();
                break;

            case ObjectType.OptionDestroyOnNegative:
                optionsManager.HandleOptionLOSAUpdateOnly();
                break;

            case ObjectType.OptionBehaviorAfterChoice:
                optionsManager.HandleOptionLOSAUpdateOnly();
                break;
            case ObjectType.BehaviorOnly:
                optionsManager.HandleBehaviorOnly();
                break;

            default:
                break;
        }
    }
}
