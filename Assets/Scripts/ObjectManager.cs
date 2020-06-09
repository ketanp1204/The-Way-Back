using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    // Configuration parameters
    [HideInInspector]
    public Vector3 mousePosition;
    [HideInInspector]
    public Vector3 mousePositionWorld;
    [HideInInspector]
    public Vector2 mousePositionWorld2D;
    private RaycastHit2D hit;

    // Cached References
    public Camera mainCamera;
    public OptionsManager optionsManager;
    private GameObject descriptionBox;
    private GameObject optionsBox;
    public LevelChanger levelChanger;
    public GameObject interactableObjects;
    public GameObject closeUpObjects;
    public GameObject backButton;
    private GameObject zoomedInObject;

    // Start is called before the first frame update
    void Start()
    {
        optionsManager = FindObjectOfType<OptionsManager>();
        descriptionBox = optionsManager.transform.Find("Description Box").gameObject;
        optionsBox = optionsManager.transform.Find("Options Box").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // Get 3D mouse position in Screen Space Coordinates
            mousePosition = Input.mousePosition;

            // Convert mouse position to 3D World Coordinates
            mousePositionWorld = mainCamera.ScreenToWorldPoint(mousePosition);

            // Convert mouse position to 2D world coordinates
            mousePositionWorld2D = new Vector2(mousePositionWorld.x, mousePositionWorld.y);

            // Raycast2D : public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance (length of ray), int layerMask (to exclude certain objects)
            hit = Physics2D.Raycast(mousePositionWorld2D, Vector2.zero);

            // Test whether ray hits any collider
            if (hit.collider != null)
            {
                if (!descriptionBox.activeSelf)
                {
                    if (hit.collider.gameObject.tag == "Object")
                    {
                        if (hit.collider.gameObject.GetComponent<ObjectProperties>().interactedWith == false)
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
                        // Fade In and Out Animation
                        StartCoroutine(LevelChanger.CrossFadeStart(true));

                        // Load close up view
                        StartCoroutine(LoadCloseUp());
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
                    GameSession.FadeOut(descriptionBox.GetComponent<CanvasGroup>());
                    if(optionsBox.activeSelf)
                    {
                        GameSession.FadeOut(optionsBox.GetComponent<CanvasGroup>());
                    }
                    StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox));
                    optionsManager.CloseAndClearOptionsBox();
                }
            }
            else
            {
                GameSession.FadeOut(descriptionBox.GetComponent<CanvasGroup>());
                if (optionsBox.gameObject.activeSelf)
                {
                    GameSession.FadeOut(optionsBox.GetComponent<CanvasGroup>());
                }
                StartCoroutine(GameSession.DisableGameObjectAfterDelay(descriptionBox));
                optionsManager.CloseAndClearOptionsBox();
            }
        }
    }

    public void ExitCloseUpView()
    {
        // Fade In and Out Animation
        StartCoroutine(LevelChanger.CrossFadeStart(true));
        
        // Go back from Close Up View
        StartCoroutine(ExitCloseUp());
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
