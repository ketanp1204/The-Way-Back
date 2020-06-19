using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    // Configuration parameters
    private float levelOfSelfAwareness = 0;
    private string instructions = "Click on objects to get options to choose from which modifies your LOSA score. " +
                                  "The LOSA score shown is just for visualization and will not be included in the final game. " +
                                  "Certain objects like the window will have only reactions when you click on them based on your current LOSA Score";

    // Cached References
    public GameObject morningImage;
    public GameObject nightImage;
    public GameObject k_shoppingList_day;
    public GameObject k_shoppingList_night;
    public GameObject k_sink_day;
    public GameObject k_sink_night;
    private GameObject staticUI;
    private GameObject dynamicUI;
    private GameObject LOSA;
    private GameObject descriptionBox;
    private GameObject pauseMenuUI;
    private CanvasGroup descriptionBoxCG;
    private ObjectManager objectManager;

    // State variables
    [HideInInspector]
    public static bool GameIsPaused = false;
    public bool instructionsEnabled;
    public bool timeOfDayNight = false;
    public bool closeUpObjects = false;
    
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
        ShowInstructionsAndLOSA();
    }

    void SetReferences()
    {
        morningImage = GameObject.Find("MorningImage");
        nightImage = GameObject.Find("NightImage");
        k_shoppingList_day = GameObject.Find("K_ShoppingList_Day");
        k_shoppingList_night = GameObject.Find("K_ShoppingList_Night");
        k_sink_day = GameObject.Find("CU_Sink_Day");
        k_sink_night = GameObject.Find("CU_Sink_Night");
        if(!timeOfDayNight)
        {
            if(k_shoppingList_day != null)
            {
                k_shoppingList_night.GetComponent<Transform>().Translate(Vector3.forward);
                k_sink_night.GetComponent<Transform>().Translate(Vector3.forward);
            }
            
        }
        else
        {
            if(k_shoppingList_day != null)
            {
                k_shoppingList_day.GetComponent<Transform>().Translate(Vector3.forward);
                k_sink_day.GetComponent<Transform>().Translate(Vector3.forward);
            }
        }
        staticUI = GameObject.Find("StaticUI");
        dynamicUI = GameObject.Find("DynamicUI");
        LOSA = staticUI.transform.Find("LOSAPanel/LOSA").gameObject;
        descriptionBox = dynamicUI.transform.Find("OptionsManager/Description Box").gameObject;
        if(descriptionBox != null)
        {
            descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        }
        descriptionBox.SetActive(false);
        pauseMenuUI = staticUI.transform.Find("PauseMenu").gameObject;
        pauseMenuUI.SetActive(false);
        objectManager = FindObjectOfType<ObjectManager>();
    }

    void ShowInstructionsAndLOSA()
    {
        if(LOSA != null)
        {
            LOSA.GetComponent<TextMeshProUGUI>().text = "LOSA: " + levelOfSelfAwareness;
        }
        if(descriptionBox != null)
        {
            if (instructionsEnabled)
            {
                StartCoroutine(showInstructions());
            }
        }
    }

    private IEnumerator showInstructions()
    {
        yield return new WaitForSeconds(1.0f);
        descriptionBox.SetActive(true);
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = instructions;
        FadeIn(descriptionBoxCG, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (descriptionBox != null && descriptionBox.activeSelf)
        {
            if (descriptionBoxCG.alpha != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                FadeOut(descriptionBoxCG, 0f);
                StartCoroutine(DisableGameObjectAfterDelay(descriptionBox));
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (!closeUpObjects)
        {
            morningImage.GetComponent<SpriteRenderer>().enabled = true;
            nightImage.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void ChangeLOSA(int LOSAUpdate)
    {
        if(LOSAUpdate == 1)
        {
            levelOfSelfAwareness += 10f;
            
        }
        else if(LOSAUpdate == 2)
        {
            if(levelOfSelfAwareness != 0f)
            {
                levelOfSelfAwareness -= 10f;
            }
        }
        LOSA.GetComponent<TextMeshProUGUI>().text = "LOSA: " + levelOfSelfAwareness;
    }

    public float GetLOSA()
    {
        return levelOfSelfAwareness;
    }

    public void Resume()
    {
        if(pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    }

    void Pause()
    {
        if(pauseMenuUI)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static void FadeIn(CanvasGroup canvasGroup, float delay)
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, delay));
    }

    public static void FadeOut(CanvasGroup canvasGroup, float delay)
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, delay));
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float delay, float lerpTime = 0.3f)
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            if(canvasGroup != null)
            {
                canvasGroup.alpha = currentValue;
            }

            if (percentageComplete >= 1)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator DisableGameObjectAfterDelay(GameObject gO)
    {
        yield return new WaitForSeconds(0.3f);
        
        gO.SetActive(false);
    }

    public void transitionIntoNight()
    {
        if (closeUpObjects)
        {
            morningImage.GetComponent<SpriteRenderer>().enabled = false;
            nightImage.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (!timeOfDayNight)
        {
            timeOfDayNight = true;
            if (morningImage != null)
            {
                StartCoroutine(FadeOutImage(morningImage, 5f, false));
            }
            if (nightImage != null)
            {
                StartCoroutine(FadeInImage(nightImage, 5f));
            }
            if (k_shoppingList_day != null)
            {
                StartCoroutine(FadeOutImage(k_shoppingList_day, 5f, false));
            }
            if (k_shoppingList_night != null)
            {
                StartCoroutine(FadeInImage(k_shoppingList_night, 5f));
            }
            if (k_sink_day != null && closeUpObjects == true)
            {
                StartCoroutine(FadeOutImage(k_sink_day, 5f, true));
            }
            if (k_sink_night != null && closeUpObjects == true)
            {
                if (k_sink_night.GetComponent<SpriteRenderer>().enabled == false)
                {
                    k_sink_night.GetComponent<SpriteRenderer>().enabled = true;
                }
                StartCoroutine(FadeInImage(k_sink_night, 5f));
            }
        }
        else
        {
            timeOfDayNight = false;
            if (morningImage != null)
            {
                StartCoroutine(FadeInImage(morningImage, 5f));
            }
            if (nightImage != null)
            {
                StartCoroutine(FadeOutImage(nightImage, 5f, false));
            }
            if (k_shoppingList_day != null)
            {
                StartCoroutine(FadeInImage(k_shoppingList_day, 5f));
            }
            if (k_shoppingList_night != null)
            {
                StartCoroutine(FadeOutImage(k_shoppingList_night, 5f, false));
            }
            if (k_sink_day != null && closeUpObjects == true)
            {
                if (k_sink_day.GetComponent<SpriteRenderer>().enabled == false)
                {
                    k_sink_day.GetComponent<SpriteRenderer>().enabled = true;
                }
                StartCoroutine(FadeInImage(k_sink_day, 5f));
            }
            if (k_sink_night != null && closeUpObjects == true)
            {
                StartCoroutine(FadeOutImage(k_sink_night, 5f, true));
            }
        }
    }

    private IEnumerator FadeOutImage(GameObject g, float duration, bool dSR)
    {
        float start = Time.time;
        SpriteRenderer sR = g.GetComponent<SpriteRenderer>();
        while(Time.time <= start + duration)
        {
            Color color = sR.color;
            color.a = 1f - Mathf.Clamp01((Time.time - start) / duration);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
        if (dSR)
        {
            disableSpriteRenderer(sR);
            if(g.name == "CU_Sink_Day")
            {
                objectManager.zoomedInObject = k_sink_night;
            }
            else if(g.name == "CU_Sink_Night")
            {
                objectManager.zoomedInObject = k_sink_day;
            }
        }
    }

    private void disableSpriteRenderer(SpriteRenderer sR)
    {
        sR.enabled = false;
    }

    private IEnumerator FadeInImage(GameObject g, float duration)
    {
        float start = Time.time;
        SpriteRenderer sR = g.GetComponent<SpriteRenderer>();
        while (Time.time <= start + duration)
        {
            Color color = sR.color;
            color.a = 0f + Mathf.Clamp01((Time.time - start) / duration);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
