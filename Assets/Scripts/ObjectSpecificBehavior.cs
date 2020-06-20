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

    private void K_FruitBasket_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly(false, false);

        if(objectProperties.LOSAUpdateResponse == 1)    // Option 'Eat The Edible Fruits' selected
        {
            // TODO: Replace the image of the fruit basket with one with lesser fruits
        }
    }

    private void K_GardenDoor_Behavior()
    {
        optionsManager.ShowTextOnDescriptionBox(objectProperties.description);
        // TODO: Check if the time of day is morning, then display a text if it is, otherwise proceed to the Garden level.
    }

    /// <summary>
    /// Behaviors for objects in the living room
    /// </summary>

    private void LR_Television_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly(false, false);

        // TODO: Turn off animation and sound of the television after interaction


    }

    private void LR_Gramophone_Behavior()
    {
        optionsManager.HandleOptionLOSAUpdateOnly(false, false);

        if(objectProperties.LOSAUpdateResponse == 0)    // Option 'Remove The Record' is selected
        {
            // TODO: Replace gramophone image with one without record in it
        }

        if(objectProperties.LOSAUpdateResponse == 1)    // Option 'Play The Record' is selected
        {
            // TODO: Play record playing animation
        }

    }
}
