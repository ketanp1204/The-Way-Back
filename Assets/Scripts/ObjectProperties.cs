using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
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

    public bool destroyOnNegativeResponse;

    [HideInInspector]
    public bool interactedWith = false;

    // Start is called before the first frame update
    void Start()
    {
        if (losaResponseLow != "")
        {
            numberOfLOSAResponses = 3;
        }
        else
        {
            numberOfLOSAResponses = 0;

            if (option3responses.Length != 0 & option2responses.Length > 0 && option1responses.Length > 0)
            {
                numberOfResponses = 3;
            }
            else if (option3responses.Length == 0 && option2responses.Length > 0 && option1responses.Length > 0)
            {
                numberOfResponses = 2;
            }
            else if (option3responses.Length == 0 && option2responses.Length == 0 && option1responses.Length > 0)
            {
                numberOfResponses = 1;
            }
            else
            {
                numberOfResponses = 0;
            }
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
}
