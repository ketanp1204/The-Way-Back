using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpecificBehavior : MonoBehaviour
{

    private OptionsManager optionsManager;
    private ObjectProperties objectProperties;
    private AudioManager audioManager;

    private int behaviorIndex = 1;      // To allow for multiple behaviors

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
    /// Behaviors for objects in the Living Room
    /// </summary>

    private void LR_Television_Behavior()
    {
        // TODO: Turn off animation and sound of the television after interaction


    }

    private void LR_Gramophone_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 0)    // Option 'Remove The Record' is selected
        {
            Destroy(transform.Find("Record_Sprites").gameObject);
        }

        if(objectProperties.LOSAUpdateResponse == 1)    // Option 'Play The Record' is selected
        {
            Sound s = AudioManager.GetSound("LR_Gramophone_Record");
            
            audioManager.Play("LR_Gramophone_Record");
            
            Animator recordAnim = transform.Find("Record_Sprites").GetComponent<Animator>();
            Animator playerAnim = transform.Find("Player_Sprites").GetComponent<Animator>();

            StartCoroutine(LR_G_PlayerAnimation(s, playerAnim));
            StartCoroutine(LR_G_RecordAnimation(s, recordAnim));
        }
    }

    IEnumerator LR_G_PlayerAnimation(Sound s, Animator anim)
    {
        anim.enabled = true;
        if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
        {
            anim.Play("Base Layer.LR_G_Player_Day");
        }
        else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.EVENING)
        {
            anim.Play("Base Layer.LR_G_Player_Night");
        }
        
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
        // Storing a bool whether 'Check the plant's ground' or 'Answer' is selected for future reference for the object 'Shovel' in the Garden
        if (objectProperties.LOSAUpdateResponse == 1)
        {
            GameEventsTracker.LR_Plant_Interacted = true; 
        }
    }


    /// <summary>
    /// Behaviors for objects in the Kitchen
    /// </summary>

    private void K_ShoppingList_Behavior()
    {
        if (objectProperties.LOSAUpdateResponse == 2)
        {
            StartCoroutine(DestroyShoppingListAfterDelay(gameObject));
        }
    }

    private IEnumerator DestroyShoppingListAfterDelay(GameObject shoppingList)
    {
        SpriteRenderer[] sRs = shoppingList.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer sR = Array.Find(sRs, s => s.enabled == true);
        float start = Time.time;

        while (Time.time <= start + 0.3f)
        {
            Color color = sR.color;
            color.a = 1f - Mathf.Clamp01((Time.time - start) / 0.3f);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
        Destroy(shoppingList);
    }

    private void K_FruitBasket_Behavior()
    {
        if (objectProperties.LOSAUpdateResponse == 1)    // Option 'Eat The Edible Fruits' selected
        {
            // TODO: Replace the image of the fruit basket with one with lesser fruits
        }
    }

    private void K_GardenDoor_Behavior()
    {
        if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
        {
            // TODO: Uncomment for actual game
            // optionsManager.ShowTextOnDescriptionBox(objectProperties.description);      // Can't proceed to the garden level because of the rain

            // Only for preview 
            LevelChanger.LoadNextLevel();       // Proceed to the Garden level
        }
        else
        {
            LevelChanger.LoadNextLevel();       // Proceed to the Garden level
        }
    }

    private void K_WineBottle_Behavior()
    {
        if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            objectProperties.HandleResponse(2);
        }
    }

    private void K_Window_Behavior()
    {
        if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            optionsManager.ShowTextOnDescriptionBox(new string[] { objectProperties.description }, 0f);
        }
    }


    /// <summary>
    /// Behaviors for objects in the Bathroom
    /// </summary>

    private void B_Buckets_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 1)
        {
            GameEventsTracker.B_Buckets_Kept = true;
        }
    }
    
    private void B_BathtubTap_Behavior()
    {
        // Stop water dripping sound
        Sound s = AudioManager.GetSound("B_Water_Dripping");
        if(s.source.isPlaying)
        {
            s.source.Stop();
        }
        GameEventsTracker.B_Tap_Water_Dripping = false;
        // TODO Stop water animation 
    }

    private void B_SleepingPills_Behavior()
    {
        if (GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            objectProperties.HandleResponse(2);

            // Pills must disappear if 'Wash Them Down' option is selected
            if(objectProperties.LOSAUpdateResponse == 0)
            {
                // TODO: remove pills from scene
            }
        }
    }

    /// <summary>
    /// Behaviors for objects in the Garden
    /// </summary>
    
    private void G_Shovel_Behavior()
    {
        // If player has interacted with the plant in the living room and chosen a positive response, then show the options and handle the behavior
        if(GameEventsTracker.LR_Plant_Interacted)
        {
            // TODO: show options
        }

        // TODO: Add hole digging animation/photo

        // TODO: Add plant image to the scene after digging
    }

    private void G_Pond_Behavior()
    {
        if(GameEventsTracker.B_Buckets_Kept == true)
        {
            if(behaviorIndex == 1)          // First interaction behavior
            {
                behaviorIndex += 1;
                objectProperties.HandleResponse(2);     // Show response choices
            }
            else if(behaviorIndex == 2)     // Second interaction behavior
            {
                if(objectProperties.LOSAUpdateResponse == 1)        // Option 'Fill the Pond' is selected
                {
                    // TODO: Replace the image of the pond
                }
            }
        }
        else
        {
            // Show LOSA Responses
            objectProperties.HandleResponse(1);
        }
    }

    private void G_Brazier_Behavior()
    {
        if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            if(behaviorIndex == 1)          // First Interaction Behavior
            {
                behaviorIndex += 1;
                objectProperties.HandleResponse(2);     // Show response choices
            }
            else if(behaviorIndex == 2)     // Second interaction behavior
            {
                if(objectProperties.LOSAUpdateResponse == 1)       // Option 'Light the Fire' is selected
                {
                    // TODO: Start fire animation + sound
                }
            }
        }
    }
}
