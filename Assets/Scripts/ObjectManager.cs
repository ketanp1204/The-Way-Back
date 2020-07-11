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
    public Vector3 mousePositionWorld;
    [HideInInspector]
    public Vector2 mousePositionWorld2D;
    private RaycastHit2D hit;
    Button backButton;

    // Cached References
    private Camera mainCamera;
    private UIReferences uiReferences;
    private ObjectProperties objectProperties;
    private OptionsManager optionsManager;
    private LevelChanger levelChanger;
    private GameSession gameSession;
    private GameObject interactableObjects;
    private GameObject closeUpObjects;
    private GameObject descriptionBox;
    private GameObject optionsBox;
    private GameObject staticUI;
    private GameObject rainSystem;
    public Button backButtonPrefab;
    public GameObject zoomedInObject;


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
        rainSystem = uiReferences.rainSystem;
        levelChanger = FindObjectOfType<LevelChanger>();
        gameSession = FindObjectOfType<GameSession>();
        interactableObjects = uiReferences.interactableObjects;
        closeUpObjects = uiReferences.closeUpObjects;
        staticUI = GameObject.Find("StaticUI");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())                                              // Prevent player from clicking through UI elements
                return;

            mousePositionWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);                        // Convert mouse position to 3D World Coordinates
            mousePositionWorld2D = new Vector2(mousePositionWorld.x, mousePositionWorld.y);                 // Convert mouse position to 2D world coordinates
            hit = Physics2D.Raycast(mousePositionWorld2D, Vector2.zero);                                    // Origin, Direction, Length of Ray, int layerMask (to exclude certain objects)

           
            if (hit.collider != null)                                                                       // Test whether ray hits any collider
            {
                if (!descriptionBox.activeSelf)
                {
                    /*
                    if (hit.collider.gameObject.tag == "Object")
                    {
                        objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                        if (objectProperties.interactedWith == false)                                       // Prevent player from clicking an object twice
                        {
                            optionsManager.SetSelectedObjectReference(hit.collider.gameObject);
                            objectProperties.interactedWith = true;
                            objectProperties.HandleResponse(true);
                        }
                    }
                    */

                    /* To enable the ability to click on an object twice*/
                    if (hit.collider.gameObject.tag == "Object")
                    {
                        objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                        optionsManager.SetSelectedObjectReference(hit.collider.gameObject);
                        objectProperties.HandleResponse(0);
                    }
                    /* */

                    if(hit.collider.gameObject.tag == "MultipleObjectChild")
                    {
                        GameObject selectedObject = hit.collider.gameObject.transform.parent.gameObject;
                        objectProperties = selectedObject.GetComponent<ObjectProperties>();
                        optionsManager.SetSelectedObjectReference(selectedObject);
                        objectProperties.HandleResponse(0);
                    }

                    if (hit.collider.gameObject.tag == "CloseUp")
                    {
                        if(rainSystem != null)
                        {
                            StartCoroutine(GameSession.DisableGameObjectAfterDelay(rainSystem, 1f));     // TODO: Refactor into a rain manager script
                        }
                        objectProperties = hit.collider.gameObject.GetComponent<ObjectProperties>();
                        GameSession.closeUpObjects = true;
                        gameSession.disableBackgroundImage();
                        StartCoroutine(LevelChanger.CrossFadeStart(true));               // Fade In and Out Animation
                        StartCoroutine(LoadCloseUp());                                   // Load close up view
                    }

                    if (hit.collider.gameObject.tag == "PrevLevel")
                    {
                        LevelChanger.LoadPreviousLevel();
                    }

                    if (hit.collider.gameObject.tag == "NextLevel")
                    {
                        LevelChanger.LoadNextLevel();
                    }
                }
                else
                {
                    if(!optionsManager.IsWriting)
                        CloseTextBoxes();
                }
            }
            else
            {
                if(!optionsManager.IsWriting)
                    CloseTextBoxes();
            }
        }
    }

    private void CloseTextBoxes()
    {
        GameSession.FadeOut(descriptionBox.GetComponent<CanvasGroup>(), 0f);
        if (optionsBox.activeSelf)
        {
            GameSession.FadeOut(optionsBox.GetComponent<CanvasGroup>(), 0f);
        }
        StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox, 0.5f));
        optionsManager.CloseAndClearOptionsBox();
    }

    public void ExitCloseUpView(GameSession gameSession)
    {
        GameSession.closeUpObjects = false;
        gameSession.enableBackgroundImage();
        if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
        {
            StartCoroutine(GameSession.EnableGameObjectAfterDelay(rainSystem, 1f));      // TODO: Refactor into a rain manager script
        }
        StartCoroutine(LevelChanger.CrossFadeStart(true));          // Fade In and Out Animation
        StartCoroutine(ExitCloseUp());                              // Go back from Close Up View
    }
    public IEnumerator LoadCloseUp()
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(false);                       // Hide and disable main scene objects

        // Spawn a back button to go back to the main scene
        backButton = Instantiate(backButtonPrefab, GameObject.Find("StaticUI").transform);
        backButton.gameObject.SetActive(true);
        backButton.onClick.AddListener(() => HandleBackButton(gameSession));

        // Load the zoomed in object and its interactable objects
        zoomedInObject = objectProperties.closeUpObject;
        zoomedInObject.SetActive(true);
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(true);
    }

    private void HandleBackButton(GameSession gameSession)
    {
        ExitCloseUpView(gameSession);
    }

    public IEnumerator ExitCloseUp()
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(true);
        Destroy(backButton.gameObject);

        zoomedInObject.SetActive(false);
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(false);
        zoomedInObject = null;
    }
}
