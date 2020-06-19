using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectSpecificBehavior : MonoBehaviour
{
    private OptionsManager optionsManager;
    private ObjectProperties objectProperties;

    // Start is called before the first frame update
    void Start()
    {
        optionsManager = FindObjectOfType<OptionsManager>();
        objectProperties = gameObject.GetComponent<ObjectProperties>();
    }

    public void HandleBehavior(GameObject gO)
    {
        if(gO == gameObject)
        {
            Invoke(gameObject.name + "_Behavior", 0f);
        }
    }

    /// <summary>
    /// Behaviors for objects in the kitchen
    /// </summary>

    private void K_Shopping_List_Day_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly();

        if(objectProperties.LOSAUpdateResponse == 2 && (objectProperties.objectType[0] == ObjectProperties.ObjectType.OptionDestroyOnNegative ||
                                                        objectProperties.objectType[1] == ObjectProperties.ObjectType.OptionDestroyOnNegative))
        {
            Destroy(gameObject);
        }
    }

    private void K_Shopping_List_Night_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly();

        if (objectProperties.LOSAUpdateResponse == 2 && (objectProperties.objectType[0] == ObjectProperties.ObjectType.OptionDestroyOnNegative ||
                                                         objectProperties.objectType[1] == ObjectProperties.ObjectType.OptionDestroyOnNegative))
        {
            Destroy(gameObject);
        }
    }

    private void K_FruitBasket_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly();

        Debug.Log("Implementation of fruit image change pending");
    }

    /// <summary>
    /// Behaviors for objects in the living room
    /// </summary>

    private void LR_Television_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly();

        // Turn off animation and sound of the television after interaction


    }
}
