using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ObjectSpecificBehavior : MonoBehaviour
{

    private OptionsManager optionsManager;
    private ObjectProperties objectProperties;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        optionsManager = FindObjectOfType<OptionsManager>();
        objectProperties = gameObject.GetComponent<ObjectProperties>();
        audioManager = FindObjectOfType<AudioManager>();
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

    private void K_ShoppingList_Day_Behavior()
    {
        Debug.Log(objectProperties.LOSAUpdateResponse);
        if(objectProperties.LOSAUpdateResponse == 2)
        {
            Destroy(gameObject);
            GameObject shoppingListNight = GameObject.Find("K_ShoppingList_Night");
            Destroy(shoppingListNight);
        }
    }

    private void K_ShoppingList_Night_Behavior()
    {
        if (objectProperties.LOSAUpdateResponse == 2)
        {
            Destroy(gameObject);
            GameObject shoppingListNight = GameObject.Find("K_ShoppingList_Day");
            Destroy(shoppingListNight);
        }
    }

    private void K_FruitBasket_Behavior()
    {
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
        // TODO: Turn off animation and sound of the television after interaction


    }

    private void LR_Gramophone_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 0)    // Option 'Remove The Record' is selected
        {
            // TODO: Replace gramophone image with one without record in it
        }

        if(objectProperties.LOSAUpdateResponse == 1)    // Option 'Play The Record' is selected
        {
            audioManager.Play("Gramophone Record");
            Sound s = audioManager.GetSound("Gramophone Record");

            Animator recordAnim = transform.Find("Record_Sprites").GetComponent<Animator>();
            Animator playerAnim = transform.Find("Player_Sprites").GetComponent<Animator>();

            StartCoroutine(LR_G_PlayerAnimation(s, playerAnim));
            StartCoroutine(LR_G_RecordAnimation(s, recordAnim));
        }
    }

    IEnumerator LR_G_PlayerAnimation(Sound s, Animator anim)
    {
        anim.enabled = true;
        anim.Play("Base Layer.LR_G_Player");
        while (s.source.isPlaying)
        {
            yield return new WaitForSeconds(0.01f);
        }
        anim.enabled = false;
    }

    IEnumerator LR_G_RecordAnimation(Sound s, Animator anim)
    {
        anim.enabled = true;
        anim.Play("Base Layer.LR_G_Record");
        while(s.source.isPlaying)
        {
            yield return new WaitForSeconds(0.01f);
        }
        anim.enabled = false;
    }

    private void LR_Plants_Behavior()
    {
        // TODO: Store a bool whether 'Check the plant's ground' or 'Answer' is selected for future reference for the object 'Shovel' in the Garden
    }
}
