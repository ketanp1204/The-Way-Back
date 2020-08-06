using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    // Configuration parameters
    [HideInInspector]
    public Vector3 mousePositionWorld;              // Stores the current mouse position in 3D
    [HideInInspector]
    public Vector2 mousePositionWorld2D;            // Stores the current mouse position in 2D
    private RaycastHit2D hit;                       // Stores the raycast hit result
    Button backButton;

    // Cached References
    private Camera mainCamera;
    private UIReferences uiReferences;
    private ObjectProperties objectProperties;      // Reference to the selected object properties
    private OptionsManager optionsManager;          // Reference to the options manager
    private GameObject interactableObjects;         // Reference to the interactable objects
    private GameObject descriptionBox;              // Reference to the description box
    private GameObject optionsBox;                  // Reference to the option box
    private GameObject rain;                        // Reference to the rain particle system
    public Button backButtonPrefab;                 // Reference to the back button prefab
    public GameObject zoomedInObject;               // Reference to the zoomed in object 
    private PersistentObjectData objectData;        // Reference to the singleton object data instance

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetReferences();
    }

    private void SetReferences()
    {
        mainCamera = Camera.main;
        uiReferences = FindObjectOfType<UIReferences>();
        optionsManager = FindObjectOfType<OptionsManager>();
        descriptionBox = uiReferences.descriptionBox;
        optionsBox = uiReferences.optionsBox;
        rain = uiReferences.rain;
        interactableObjects = uiReferences.interactableObjects;
        objectData = FindObjectOfType<PersistentObjectData>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))                                                                     // Handle mouse click in game
        {
            if (EventSystem.current.IsPointerOverGameObject())                                              // Prevent player from clicking through UI elements
            {
                return;
            }

            mousePositionWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);                        // Convert mouse position to 3D World Coordinates
            mousePositionWorld2D = new Vector2(mousePositionWorld.x, mousePositionWorld.y);                 // Convert mouse position to 2D world coordinates
            hit = Physics2D.Raycast(mousePositionWorld2D, Vector2.zero);                                    // Origin, Direction, Length of Ray, int layerMask (to exclude certain objects)

           
            if (hit.collider != null)                                                                       // Test whether ray hits any collider
            {
                if (!descriptionBox.activeSelf)
                {
                    if (hit.collider.gameObject.tag == "Object")                                            // Player clicks on an object which can be only clicked once
                    {
                        AudioManager.Play("Object_Click");
                        if(!objectData.interactedObjects.Contains(hit.collider.gameObject.name))
                        {
                            objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                            if (objectProperties.interactedWith == false)                                       // Prevent player from clicking an object twice
                            {
                                optionsManager.SetSelectedObjectReference(hit.collider.gameObject);
                                objectProperties.HandleResponse(0);
                            }
                        }
                    }

                    if (hit.collider.gameObject.tag == "ObjectMultipleClick")                               // Player clicks on an object which can be clicked on multiple times
                    {
                        AudioManager.Play("Object_Click");
                        objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                        optionsManager.SetSelectedObjectReference(hit.collider.gameObject);
                        objectProperties.HandleResponse(0);
                    }

                    if(hit.collider.gameObject.tag == "MultipleObjectChild")                                // Player clicks on an object with multiple collision boxes
                    {
                        AudioManager.Play("Object_Click");
                        GameObject selectedObject = hit.collider.gameObject.transform.parent.gameObject;
                        objectProperties = selectedObject.GetComponent<ObjectProperties>();
                        optionsManager.SetSelectedObjectReference(selectedObject);
                        objectProperties.HandleResponse(0);
                    }

                    if (hit.collider.gameObject.tag == "CloseUp")                                           // Player clicks on an object which has a close up view
                    {
                        AudioManager.Play("Object_Click");
                        if (rain != null)
                        {
                            foreach(Transform c in rain.transform)
                            {
                                foreach(Transform child in c)
                                {
                                    child.gameObject.GetComponent<ParticleSystem>().Stop();
                                }
                            }
                        }
                        objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                        GameSession.closeUpObjects = true;
                        GameSession.instance.disableBackgroundImage();
                        StartCoroutine(LevelChanger.CrossFadeStart(true));                                  // Fade In and Out Animation
                        StartCoroutine(LoadCloseUp());                                                      // Load close up view
                    }

                    if (hit.collider.gameObject.tag == "PrevLevel")                                         // Player clicks on the next level object/button
                    {
                        AudioManager.Play("Object_Click");
                        LevelChanger.LoadPreviousLevel();
                    }
                        
                    if (hit.collider.gameObject.tag == "NextLevel")                                         // Player clicks on the previous level object/button
                    {
                        AudioManager.Play("Object_Click");
                        LevelChanger.LoadNextLevel();
                    }
                }
                else
                {
                    if(!DescriptionBoxManager.IsWriting && !GameSession.instance.atticEnding)                      // Close the description and option boxes when clicking outside of them if no text is being typed currently
                    {
                        AudioManager.Play("Object_Click");
                        CloseBoxes();
                    }
                }
            }
            else
            {
                if(!DescriptionBoxManager.IsWriting && !GameSession.instance.atticEnding)                          // Close the description and option boxes when clicking outside of them if no text is being typed currently
                {
                    AudioManager.Play("Object_Click");
                    CloseBoxes();
                }
            }
        }
    }

    private void CloseBoxes()                                                                           // Method that closes the description and option boxes
    {
        if (optionsBox.activeSelf)
        {
            GameSession.FadeOut(optionsBox.GetComponent<CanvasGroup>(), 0f);
        }
        DescriptionBoxManager.CloseDescriptionBox();
        optionsManager.CloseAndClearOptionsBox();
    }

    public void ExitCloseUpView()                                                                           // Return to the main scene from a close up view
    {
        GameSession.closeUpObjects = false;
        GameSession.instance.enableBackgroundImage();
        if (rain != null)
        {
            foreach (Transform c in rain.transform)
            {
                foreach (Transform child in c)
                {
                    child.gameObject.GetComponent<ParticleSystem>().Play();
                }
            }
        }
        StartCoroutine(LevelChanger.CrossFadeStart(true));                                                  // Fade In and Out Animation
        StartCoroutine(ExitCloseUp());                                                                      // Go back from Close Up View
    }
    public IEnumerator LoadCloseUp()                                                                        // Load the close up view of an object
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(false);                                                               // Hide and disable main scene objects

        // Spawn a back button to go back to the main scene
        backButton = Instantiate(backButtonPrefab, GameObject.Find("StaticUI").transform);
        backButton.gameObject.SetActive(true);
        backButton.onClick.AddListener(() => HandleBackButton());

        // Load the zoomed in object and its interactable objects
        zoomedInObject = objectProperties.closeUpObject;
        zoomedInObject.SetActive(true);
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(true);
    }

    private void HandleBackButton()                                                                         // Return to the main scene on clicking the back button
    {
        AudioManager.Play("Object_Click");
        ExitCloseUpView();
    }

    public IEnumerator ExitCloseUp()                                                                        // Coroutine that closes the close up view and loads the main scene objects
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(true);
        Destroy(backButton.gameObject);

        zoomedInObject.SetActive(false);
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(false);
        zoomedInObject = null;
    }
}
