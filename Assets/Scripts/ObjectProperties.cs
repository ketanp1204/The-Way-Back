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
        public string LowLOSA;
        public string MedLOSA;
        public string HighLOSA;
    }

    // Cached References
    private OptionsManager optionsManager;      // Reference to the options manager gameobject

    // Variables that need to be set in inspector
    public List<string> additionalTags;         // Additional tags for an object if required
    public ObjectType[] objectType;             // To store the response type for the current object
    public string objectName;                   // Name of the object
    [TextArea(3, 10)]
    public string description;                  
    [TextArea(3, 10)]
    public string option1Text;
    [TextArea(3, 10)]
    public string[] option1responses;
    [TextArea(3, 10)]
    public string option2Text;
    [TextArea(3, 10)]
    public string[] option2responses;
    [TextArea(3, 10)]
    public string option3Text;
    [TextArea(3, 10)]
    public string[] option3responses;
    public int[] reactions;                     // Positive, negative or neutral response
    public LOSAResponseTexts losaResponseTexts;
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
    public int LOSAUpdateResponse;              // The response that the player chooses for the object
    [HideInInspector]
    public bool destroyOnPositive = false;      // Stores whether an object has to be destroyed on a positive response
    [HideInInspector]
    public bool destroyOnNegative = false;      // Stores whether an object has to be destroyed on a negative response
    [HideInInspector]
    public bool hasBehavior = false;            // Stores whether an object has custom behavior
    [HideInInspector]
    public bool responseSelected = false;       // Stores whether an option has been chosen 
    [HideInInspector]
    public int callIndex;                       // Indicates which behavior of an object to call (for objects that have multiple behaviors specified)
    [HideInInspector]
    public bool showOptions = false;            // Bool to store whether to display the options

    // Cached References
    private GameObject descriptionBox;          // Reference to the description box gameobject

    // Start is called before the first frame update
    void Start()
    {
        descriptionBox = FindObjectOfType<UIReferences>().descriptionBox;

        // Storing the number and the texts of the options 
        if (option3Text != "" & option2Text != "" && option1Text != "")
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

        // Storing the responses to each option, if present
        if (numberOfResponses == 3)
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
        
        if(objectType.Contains(ObjectType.OptionBehaviorAfterChoice))
        {
            hasBehavior = true;
        }
    }


    // Additional tags

    public bool HasTag(string tag)
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

    // Display object name on hover
    
    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())                      // Prevent player from clicking through UI elements
            return;
        
        if(!GameSession.GameIsPaused && !descriptionBox.activeSelf)             // Prevent displaying object name when the game is paused         
        {
            DisplayObjectName.ShowName_static(objectName);
        }
    }

    void OnMouseDown()
    {
        DisplayObjectName.HideName_static();
    }

    void OnMouseExit()
    {
        DisplayObjectName.HideName_static();
    }

    // Handle option click response
    public void HandleResponse(int callIndex)
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
