using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectProperties : MonoBehaviour
{
    public enum ObjectType
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
    private OptionsManager optionsManager;

    // Variables that need to be set in inspector
    public List<string> additionalTags;
    public ObjectType[] objectType;
    public string objectName;
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
    public int[] reactions;
    public LOSAResponseTexts losaResponseTexts;
  
    // Variables that will be calculated in the code
    [HideInInspector]
    public int numberOfResponses;
    [HideInInspector]
    public int numberOfLOSAResponses;
    [HideInInspector]
    public Dictionary<int, string[]> responses = new Dictionary<int, string[]>();
    [HideInInspector]
    public string[] optionTexts;
    [HideInInspector]
    public bool interactedWith = false;
    [HideInInspector]
    public int LOSAUpdateResponse;
    [HideInInspector]
    public bool destroyOnPositive = false;
    [HideInInspector]
    public bool destroyOnNegative = false;
    [HideInInspector]
    public bool hasBehavior = false;
    [HideInInspector]
    public bool responseSelected = false;
    [HideInInspector]
    public int callIndex;
    [HideInInspector]
    public bool showOptions = false;


    // Start is called before the first frame update
    void Start()
    {
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
        if (EventSystem.current.IsPointerOverGameObject())                                              // Prevent player from clicking through UI elements
            return;

        if (gameObject.tag == "CloseUp")
        {
            DisplayObjectName.ShowName_static(objectName + " (Zoom In)");
        }
        else
        {
            DisplayObjectName.ShowName_static(objectName);
        }
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
