using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName;

    [TextArea(3, 10)]
    public string description;

    [HideInInspector]
    public int numberOfResponses;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
