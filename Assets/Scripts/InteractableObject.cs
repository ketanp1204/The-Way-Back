using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
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

    public ObjectType objectType;

    public string objectName;

    [TextArea(3, 10)]
    public string description;

    [HideInInspector]
    public int numberOfResponses;

    [TextArea(3, 10)]
    public string[] option1responses;
    [TextArea(3, 10)]
    public string[] option2responses;
    [TextArea(3, 10)]
    public string[] option3responses;

    public int[] reactions;

    [TextArea(3, 10)]
    public string losaResponseLow;

    [TextArea(3, 10)]
    public string losaResponseMedium;

    [TextArea(3, 10)]
    public string losaResponseHigh;

    [HideInInspector]
    public int numberOfLOSAResponses;

    public Dictionary<int, string[]> responses = new Dictionary<int, string[]>();

    [HideInInspector]
    public bool interactedWith = false;

    // Start is called before the first frame update
    void Start()
    {
        if(objectType == ObjectType.LOSAResponseOnly)
        {
            numberOfLOSAResponses = 3;
        }
        // Store the responses of the individual option buttons into a dictionary
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
        else
        {
            responses = new Dictionary<int, string[]>();
        }
    }
}
