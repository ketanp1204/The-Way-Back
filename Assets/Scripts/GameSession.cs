using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public enum TimeOfDay
    {
        MORNING,
        NOON,
        EVENING
    }

    // Configuration parameters
    private float levelOfSelfAwareness = 0;
    private string[] instructions = {"During the last years, you have changed a lot. Above that, the outside world as well. Due to that, you have lost count on the days you’ve spent in a row in your house. Alone.",
                                     "Like clay, your days have been shaped by tiredness and you lost sight of the things that once fulfilled your life. Now the clay hardened. Seemingly unalterable.",
                                     "Today is one of these days. However, something is different. A nearly imperceptible sense of self-awareness is spreading through your body. " +
                                                "And it wants to unfurl, as it once used to.",
                                     "On this day, you have the chance to raise your self-awareness, by interacting with certain objects around the house. " + 
                                                "The interactions have a different impact on you and depending on your achieved self-awareness, you’ll get the chance to unlock new ones. " + 
                                                "But be wary, you can only act once."};

    // Cached References
    private UIReferences uiReferences;
    private GameObject backgroundImage;
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
    private GameObject rainSystem;

    // Static variables
    [HideInInspector]
    public static bool GameIsPaused = false;
    [HideInInspector]
    public static TimeOfDay currentTimeOfDay;
    [HideInInspector]
    public static bool closeUpObjects = false;
    [HideInInspector]
    public static float timeOfDayInterval = 300f;

    // Non-static variables
    public bool instructionsEnabled;
    [HideInInspector]
    public bool instructionsSeen = false;
    
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

        StartCoroutine(TimeOfDayChanger());
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
    }

    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        if(uiReferences != null)
        {
            backgroundImage = uiReferences.backgroundImage;
            descriptionBox = uiReferences.descriptionBox;
            if (descriptionBox != null)
            {
                descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
            }
            pauseMenuUI = uiReferences.pauseMenuUI;
            rainSystem = uiReferences.rainSystem;
        }
        LOSA = GameObject.Find("LOSA");
        objectManager = FindObjectOfType<ObjectManager>();
        optionsManager = FindObjectOfType<OptionsManager>();
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
                optionsManager.ShowTextOnDescriptionBox(instructions, 1f);
            }
        }
    }

    private IEnumerator TimeOfDayChanger()
    {
        int changes = 0;
        while(changes < 3)
        {
            if (Time.time < timeOfDayInterval)
            {
                currentTimeOfDay = TimeOfDay.MORNING;
                changes++;
            }
            else if (Time.time >= timeOfDayInterval && Time.time < timeOfDayInterval * 2)
            {
                Destroy(GameObject.Find("Rain Generator"));
                currentTimeOfDay = TimeOfDay.NOON;
                changes++;
            }
            else if (Time.time >= timeOfDayInterval * 2 && Time.time < timeOfDayInterval * 3)
            {
                currentTimeOfDay = TimeOfDay.EVENING;
                changes++;
            }
            yield return new WaitForSeconds(timeOfDayInterval);
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
    }

    public void disableBackgroundImage()
    {
        StartCoroutine(backgroundImageChange(false));
    }

    public void enableBackgroundImage()
    {
        StartCoroutine(backgroundImageChange(true));
    }

    private IEnumerator backgroundImageChange(bool enableOrDisable)
    {
        yield return new WaitForSeconds(1f);

        backgroundImage.SetActive(enableOrDisable);
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

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float delay, float lerpTime = 0.5f)
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
        yield return new WaitForSeconds(0.5f);

        gO.SetActive(false);
    }

    public static IEnumerator EnableGameObjectAfterDelay(GameObject gO)
    {
        yield return new WaitForSeconds(1f);

        gO.SetActive(true);
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
            sR.enabled = false;
        }
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
