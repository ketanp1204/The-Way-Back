using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        OptionBehaviorAfterChoice
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
    }

    void OnMouseEnter()
    {
        DisplayObjectName.ShowName_static(objectName);
    }

    void OnMouseExit()
    {
        DisplayObjectName.HideName_static();
    }

    public void HandleResponse(bool firstCall)
    {
        optionsManager = FindObjectOfType<OptionsManager>();
        ObjectType temp;
        if (firstCall)
        {
            temp = objectType[0];
        }
        else
        {
            temp = objectType[1];
        }
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
                optionsManager.HandleOptionLOSAUpdateOnly(false, false);
                break;

            case ObjectType.OptionDestroyOnPositive:
                optionsManager.HandleOptionLOSAUpdateOnly(true, false);
                break;

            case ObjectType.OptionDestroyOnNegative:
                optionsManager.HandleOptionLOSAUpdateOnly(false, true);
                break;

            case ObjectType.OptionBehaviorAfterChoice:
                optionsManager.HandleOptionBehaviorAfterChoice();
                break;

            default:
                break;
        }
    }
}
