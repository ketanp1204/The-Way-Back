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

    // Cached References
    private Camera mainCamera;
    private OptionsManager optionsManager;
    private LevelChanger levelChanger;
    private GameObject interactableObjects;
    private GameObject closeUpObjects;
    private GameObject descriptionBox;
    private GameObject optionsBox;
    private GameObject zoomedInObject;
    private GameObject staticUI;
    private GameObject backButton;

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
        optionsManager = FindObjectOfType<OptionsManager>();
        if (optionsManager != null)
        {
            descriptionBox = optionsManager.transform.Find("Description Box").gameObject;
            optionsBox = optionsManager.transform.Find("Options Box").gameObject;
        }
        levelChanger = FindObjectOfType<LevelChanger>();
        interactableObjects = GameObject.Find("Interactable Objects");
        closeUpObjects = GameObject.Find("CloseUpObjects");
        staticUI = GameObject.Find("StaticUI");
        backButton = staticUI.transform.Find("BackButton").gameObject;
        backButton.SetActive(false);
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
                    if (hit.collider.gameObject.tag == "Object")
                    {
                        if (hit.collider.gameObject.GetComponent<ObjectProperties>().interactedWith == false)   // Prevent player from clicking an object twice
                        {
                            hit.collider.gameObject.GetComponent<ObjectProperties>().interactedWith = true;
                            optionsManager.selectedObject = hit.collider.gameObject;
                            optionsManager.InitializeResponse();
                        }
                    }

                    if (hit.collider.gameObject.tag == "LOSAResponseObject")
                    {
                        optionsManager.selectedObject = hit.collider.gameObject;
                        optionsManager.InitializeResponse();
                    }

                    if (hit.collider.gameObject.tag == "CloseUp")
                    {
                        
                        StartCoroutine(LevelChanger.CrossFadeStart(true));               // Fade In and Out Animation
                        StartCoroutine(LoadCloseUp());                                   // Load close up view
                    }

                    if (hit.collider.gameObject.tag == "PrevLevel")
                    {
                        levelChanger.LoadPreviousLevel();
                    }

                    if (hit.collider.gameObject.tag == "NextLevel")
                    {
                        levelChanger.LoadNextLevel();
                    }
                }
                else
                {
                    CloseTextBoxes();
                }
            }
            else
            {
                CloseTextBoxes();
            }
        }
    }

    private void CloseTextBoxes()
    {
        GameSession.FadeOut(descriptionBox.GetComponent<CanvasGroup>());
        if (optionsBox.activeSelf)
        {
            GameSession.FadeOut(optionsBox.GetComponent<CanvasGroup>());
        }
        StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox));
        optionsManager.CloseAndClearOptionsBox();
    }

    public void ExitCloseUpView()
    {
        StartCoroutine(LevelChanger.CrossFadeStart(true));          // Fade In and Out Animation
        StartCoroutine(ExitCloseUp());                              // Go back from Close Up View
    }
    public IEnumerator LoadCloseUp()
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(false);
        backButton.SetActive(true);
        zoomedInObject = closeUpObjects.transform.Find(hit.collider.gameObject.GetComponent<ObjectProperties>().objectName).gameObject;
        zoomedInObject.GetComponent<SpriteRenderer>().enabled = true;
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(true);
    }

    public IEnumerator ExitCloseUp()
    {
        yield return new WaitForSeconds(1f);

        interactableObjects.SetActive(true);
        backButton.SetActive(false);
        zoomedInObject.GetComponent<SpriteRenderer>().enabled = false;
        zoomedInObject.transform.Find("InteractableObjects").gameObject.SetActive(false);
        zoomedInObject = null;
    }
}
