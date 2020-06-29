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
    private string instructions = "Click on objects in the scene to get options to choose from, which modifies your LOSA score. " +
                                  "Certain objects will have only reaction texts when you click on them based on your current LOSA Score. " +
                                  "You can move between levels and see the effect of your choices on your previously interacted objects.";

    // Cached References
    private UIReferences uiReferences;
    private GameObject morningImage;
    private GameObject noonImage;
    private GameObject eveningImage;
    private GameObject k_shoppingList_day;
    private GameObject k_shoppingList_night;
    private GameObject k_sink_day;
    private GameObject k_sink_night;
    public GameObject LOSA;
    private GameObject descriptionBox;
    private GameObject pauseMenuUI;
    private CanvasGroup descriptionBoxCG;
    private ObjectManager objectManager;
    private OptionsManager optionsManager;
    private GameObject nextTextButton;

    // State variables
    [HideInInspector]
    public static bool GameIsPaused = false;
    public bool instructionsEnabled;
    [HideInInspector]
    public bool instructionsSeen = false;
    [HideInInspector]
    public bool timeOfDayNight = false;
    [HideInInspector]
    public bool closeUpObjects = false;
    private float intervalTime = 300f;
    private SpriteRenderer sR;
    
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
        descriptionBox.SetActive(false);
        pauseMenuUI.SetActive(false);
        ShowInstructionsAndLOSA();
        StartCoroutine(FadeOutImage(morningImage, intervalTime, false));
        StartCoroutine(FadeInImage(eveningImage, intervalTime));
        if(k_shoppingList_day != null)
        {
            StartCoroutine(FadeOutImage(k_shoppingList_day, intervalTime, false));
            StartCoroutine(FadeInImage(k_shoppingList_night, intervalTime));
        }
    }

    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        if(uiReferences != null)
        {
            morningImage = uiReferences.morningImage;
            noonImage = uiReferences.noonImage;
            eveningImage = uiReferences.eveningImage;
            descriptionBox = uiReferences.descriptionBox;
            if (descriptionBox != null)
            {
                descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
            }
            pauseMenuUI = uiReferences.pauseMenuUI;
            
        }
        LOSA = GameObject.Find("LOSA");
        objectManager = FindObjectOfType<ObjectManager>();
        optionsManager = FindObjectOfType<OptionsManager>();
        
        k_shoppingList_day = GameObject.Find("K_ShoppingList_Day");
        k_shoppingList_night = GameObject.Find("K_ShoppingList_Night");
        k_sink_day = GameObject.Find("CU_Sink_Day");
        k_sink_night = GameObject.Find("CU_Sink_Night");
    }

    void ShowInstructionsAndLOSA()
    {
        if(LOSA != null)
        {
            LOSA.GetComponent<TextMeshProUGUI>().text = "LOSA: " + levelOfSelfAwareness;
        }
        if(descriptionBox != null)
        {
            if (instructionsEnabled && !instructionsSeen)
            {
                instructionsSeen = true;
                optionsManager.ShowTextOnDescriptionBox(instructions);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (descriptionBox != null && descriptionBox.activeSelf && !optionsManager.IsWriting)
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
            eveningImage.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void ChangeLOSA(int LOSAUpdate)
    {
        if(LOSA == null)
        {
            LOSA = GameObject.Find("LOSA");
        }
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
            eveningImage.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (!timeOfDayNight)
        {
            timeOfDayNight = true;
            if (morningImage != null)
            {
                StartCoroutine(FadeOutImage(morningImage, 5f, false));
            }
            if (eveningImage != null)
            {
                StartCoroutine(FadeInImage(eveningImage, 5f));
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
            if (eveningImage != null)
            {
                StartCoroutine(FadeOutImage(eveningImage, 5f, false));
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
            if (g.name == "CU_Sink_Day")
            {
                objectManager.zoomedInObject = k_sink_night;
            }
            else if (g.name == "CU_Sink_Night")
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
