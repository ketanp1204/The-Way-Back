using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpecificBehavior : MonoBehaviour
{

    private OptionsManager optionsManager;          // Reference to the options manager gameobject
    private ObjectProperties objectProperties;      // Reference to the selected object's properties

    private string H_Chair_Description;

    private int behaviorIndex = 1;      // To allow for multiple behaviors

    // Specific Game Assets
    private Sprite B_MirrorShelfCabinetOpen;
    private Sprite B_MirrorShelfCabinetClosed;

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
    /// Behaviors for objects in the Living Room
    /// </summary>

    private void LR_Television_Behavior()
    {
        GameEventsTracker.LR_TV_On = false;

        AudioManager.Stop("LR_TV_Static");

        // TODO: Turn off animation of the television
    }

    private void LR_Gramophone_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 0)    // Option 'Remove The Record' is selected
        {
            Destroy(transform.Find("Record_Sprites").gameObject);
        }

        if(objectProperties.LOSAUpdateResponse == 1)    // Option 'Play The Record' is selected
        {
            AudioManager.Play("LR_Gramophone");
            
            Animator recordAnim = transform.Find("Record_Sprites").GetComponent<Animator>();
            Animator playerAnim = transform.Find("Player_Sprites").GetComponent<Animator>();

            Sound s = AudioManager.GetSound("LR_Gramophone");
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

    private void LR_Window_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 1)
        {
            AudioManager.Stop("LR_Morning_Window_Closed");
            AudioManager.PlaySoundAtCurrentGameTime("LR_Morning_Window_Open");
            // TODO: change LR image to window open and play curtain animation
        }
    }

    private void LR_HallwayDoor_Behavior()
    {
        LevelChanger.LoadLevel("Hallway");
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
            optionsManager.ShowTextOnDescriptionBox(objectProperties.description, 0f);      // Can't proceed to the garden level because of the rain
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

    private void K_HallwayDoor_Behavior()
    {
        LevelChanger.LoadLevel("Hallway");
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

    private void B_Light_Behavior()
    {
        if(behaviorIndex % 2 == 1)
        {
            behaviorIndex += 1;

            if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.EVENING)
            {
                objectProperties.objectName = "Turn Off";       // Change the text on the light to 'Turn Off'

                GameObject.Find("BackgroundImage").transform.Find("EveningImage").GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_Light_On;

                GameEventsTracker.B_Light_On = true;
            }
            else
            {
                objectProperties.HandleResponse(1);
            }
        }
        else
        {
            behaviorIndex += 1;

            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.EVENING)
            {
                objectProperties.objectName = "Turn On";       // Change the text on the light to 'Turn On'

                GameObject.Find("BackgroundImage").transform.Find("EveningImage").GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_Light_Off;

                GameEventsTracker.B_Light_On = false;
            }
            else
            {
                objectProperties.HandleResponse(1);
            }
        }
    }

    private void B_ShelfInteract_Behavior()
    {
        GameObject CU_MirrorShelf = transform.parent.parent.gameObject;     // Get the parent zoomed in object

        if (behaviorIndex % 2 == 1)
        {
            behaviorIndex += 1;

            objectProperties.objectName = "Close Shelf";      // Change the text to be displayed on hover to "Close Shelf"

            // Set the sprite of the close up object CU_MirrorShelf to the cabinet open sprite
            if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
            {
                CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Day_ShelfOpen;
            }
            else
            {
                if(GameEventsTracker.B_Light_On)
                {
                    CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_ShelfOpenLightOn;
                }
                else
                {
                    CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_ShelfOpenLightOff;
                }
            }

            transform.parent.transform.Find("B_Mirror").gameObject.SetActive(false);        // Disable the mirror since its no longer in view

            transform.parent.transform.Find("B_SleepingPills").gameObject.SetActive(true);  // Enable the sleeping pills object which is now in view

            transform.parent.transform.Find("B_MouthMask").gameObject.SetActive(true);      // Enable the mouth mask object which is now in view
        }
        else
        {
            behaviorIndex += 1;

            objectProperties.objectName = "Open Shelf";       // Change the text to be displayed on hover to "Open Shelf"

            // Set the sprite of the close up object CU_MirrorShelf to the cabinet closed sprite
            if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
            {
                CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Day_ShelfClosed;
            }
            else
            {
                if (GameEventsTracker.B_Light_On)
                {
                    CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_ShelfClosedLightOn;
                }
                else
                {
                    CU_MirrorShelf.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.B_Eve_ShelfClosedLightOff;
                }
            }

            transform.parent.transform.Find("B_Mirror").gameObject.SetActive(true);        // Enable the mirror object which is now in view

            transform.parent.transform.Find("B_SleepingPills").gameObject.SetActive(false);  // Disable the sleeping pills object since its no longer in view

            transform.parent.transform.Find("B_MouthMask").gameObject.SetActive(false);      // Disable the mouth mask object since its no longer in view
        }
    }

    private void B_HallwayDoor_Behavior()
    {
        LevelChanger.LoadLevel("Hallway");
    }

    /// <summary>
    /// Behaviors for objects in the Garden
    /// </summary>
    
    private void G_Shovel_Behavior()
    {
        if(behaviorIndex == 1)
        {
            // If player has interacted with the plant in the living room and chosen a positive response, then show the options and handle the behavior
            if (GameEventsTracker.LR_Plant_Interacted)
            {
                objectProperties.HandleResponse(2);
                behaviorIndex += 1;
            }
            else
            {
                objectProperties.HandleResponse(1);
            }
        }
        else
        {
            if(objectProperties.responseIndex == 0)
            {
                // Dig a deep hole
                GameAssets.instance.G_BigHole.SetActive(true);

                // Show the plant
                StartCoroutine(G_Show_Plant());
            }
            else
            {
                // Dig a small hole
                GameAssets.instance.G_SmallHole.SetActive(true);

                // Show the plant
                StartCoroutine(G_Show_Plant());
            }
        }
    }

    private IEnumerator G_Show_Plant()
    {
        yield return new WaitForSeconds(2.5f);

        GameAssets.instance.G_Plant.SetActive(true);
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

    private void G_LooseBrick_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 1)
        {
            // Put it out of the wall
            GameAssets.instance.G_LooseBrickOut.SetActive(true);
        }
        else
        {
            // Put it back in the hole
            // TODO: add image 
        }
    }

    private void G_PowerSocket_Behavior()
    {
        if(behaviorIndex == 1)
        {
            if (GameEventsTracker.G_Pond_Filled)
            {
                behaviorIndex += 1;
                // Show Options
                objectProperties.HandleResponse(2);
            }
            else
            {
                objectProperties.HandleResponse(1);
            }
        }
        else
        {
            if(objectProperties.LOSAUpdateResponse == 1)
            {
                // Plug in the socket
                GameAssets.instance.G_PluggedInSocket.SetActive(true);
            }
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
                    AudioManager.Play("G_Fire_Burning");
                }
            }
        }
    }

    private void G_KitchenDoor_Behavior()
    {
        LevelChanger.LoadLevel("Kitchen");
    }

    /// <summary>
    /// Behaviors for objects in the Bedroom
    /// </summary>

    private void Bed_Window_Behavior()
    {
        if (GameSession.currentTimeOfDay != GameSession.TimeOfDay.EVENING)
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            optionsManager.ShowTextOnDescriptionBox(new string[] { objectProperties.description }, 0f);
        }
    }

    private void Bed_Guitar_Behavior()
    {
        if(objectProperties.LOSAUpdateResponse == 1)
        {
            AudioManager.Play("Bed_Guitar_Strum");
        }
    }

    private void Bed_Bed_Behavior()
    {
        if (objectProperties.LOSAUpdateResponse == 1)
        {
            GameObject backgroundImage = GameObject.Find("BackgroundImage");

            backgroundImage.transform.Find("MorningImage").GetComponent<SpriteRenderer>().sprite = GameAssets.instance.Bed_Done_Day;
            backgroundImage.transform.Find("NoonImage").GetComponent<SpriteRenderer>().sprite = GameAssets.instance.Bed_Done_Noon;
            backgroundImage.transform.Find("EveningImage").GetComponent<SpriteRenderer>().sprite = GameAssets.instance.Bed_Done_Eve;

            GameAssets.instance.LR_Diary.SetActive(true);
        }
    }

    private void Bed_HallwayDoor_Behavior()
    {
        LevelChanger.LoadLevel("Hallway");
    }

   

    /// <summary>
    /// Behaviors for objects in the Hallway
    /// </summary>

    private void H_Chair_Behavior()
    {
        if (behaviorIndex == 1)
        {
            H_Chair_Description = objectProperties.description;

            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.EVENING)
            {
                optionsManager.ShowTextOnDescriptionBox(objectProperties.description, 0f);
            }
            else
            {
                behaviorIndex += 1;
                objectProperties.HandleResponse(1);
            }
        }
        else if (behaviorIndex == 2)
        {
            behaviorIndex += 1;

            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                if(objectProperties.LOSAUpdateResponse != -1)
                {
                    objectProperties.description = "";
                    if (objectProperties.LOSAUpdateResponse == 1)
                    {
                        
                        // TODO: advance game time
                    }
                    objectProperties.LOSAUpdateResponse = -1;       // Reset response
                }
                else
                {
                    objectProperties.HandleResponse(1);
                }
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                if(objectProperties.LOSAUpdateResponse != -1)
                {
                    objectProperties.description = "";
                    if(objectProperties.LOSAUpdateResponse == 1)
                    {
                        // TODO: advance game time
                    }
                    objectProperties.LOSAUpdateResponse = -1;
                }
                else
                {
                    objectProperties.HandleResponse(1);
                }
            }
            else
            {
                objectProperties.description = H_Chair_Description;
                optionsManager.ShowTextOnDescriptionBox(objectProperties.description, 0f);
            }
        }
        else
        {
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                if (objectProperties.LOSAUpdateResponse != -1)
                {
                    if (objectProperties.LOSAUpdateResponse == 1)
                    {
                        // TODO: advance game time
                    }
                    objectProperties.LOSAUpdateResponse = -1;       // Reset response
                }
                else
                {
                    objectProperties.HandleResponse(1);
                }
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                Debug.Log(objectProperties.LOSAUpdateResponse);
                if (objectProperties.LOSAUpdateResponse != -1)
                {
                    if (objectProperties.LOSAUpdateResponse == 1)
                    {
                        // TODO: advance game time
                    }
                    objectProperties.LOSAUpdateResponse = -1;       // Reset response
                }
                else
                {
                    objectProperties.HandleResponse(1);
                }
            }
            else
            {
                objectProperties.description = H_Chair_Description;
                optionsManager.ShowTextOnDescriptionBox(objectProperties.description, 0f);
            }
        }
    }

    private void H_AtticStairs_Behavior()
    {
        if (GameSession.GetLOSA() < 90)      // TODO: change LOSA calculation
        {
            objectProperties.HandleResponse(1);
        }
        else
        {
            optionsManager.ShowTextOnDescriptionBox(objectProperties.description, 0f);  
            StartCoroutine(GoToAttic());
        }
    }
    
    private IEnumerator GoToAttic()
    {
        yield return new WaitForSeconds(8f);

        GameSession.FadeOut(GameObject.Find("Description Box").GetComponent<CanvasGroup>(), 0f);

        LevelChanger.LoadLevel("Attic");
    }

    private void H_LivingRoomDoor_Behavior()
    { 
        LevelChanger.LoadLevel("LivingRoom");
    }

    private void H_BathroomDoor_Behavior()
    {
        LevelChanger.LoadLevel("Bathroom");
    }

    private void H_KitchenDoor_Behavior()
    {
        LevelChanger.LoadLevel("Kitchen");
    }

    private void H_BedroomDoor_Behavior()
    {
        LevelChanger.LoadLevel("Bedroom");
    }
}
