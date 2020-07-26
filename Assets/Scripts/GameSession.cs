using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // Singleton
    public static GameSession instance;

    public enum TimeOfDay                                   // The different times of day 
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
    private UIReferences uiReferences;                              // Stores the references to the UI objects in the scene
    private TextMeshProUGUI clockText;                              // Stores the reference to the clock text display text field
    private GameObject backgroundImage;                             // Stores the reference to the background image gameObject
    private GameObject descriptionBox;                              // Stores the reference to the description box
    private GameObject pauseMenuUI;                                 // Stores the reference to the Pause Menu UI gameObject
    private CanvasGroup descriptionBoxCG;                           // Stores the reference to the description box canvas group
    private OptionsManager optionsManager;                          // Stores the reference to the options manager
    private GameObject rainSystem;                                  // Stores the reference to the rain particle system

    // Static variables
    [HideInInspector]
    public static bool GameIsPaused = false;                        // Is true when the game is paused
    [HideInInspector]
    public static TimeOfDay currentTimeOfDay;                       // Static reference to the current time of day (Morning, Noon or Evening)
    [HideInInspector]
    public static bool closeUpObjects = false;                      // Is true when an object is in close up view
    [HideInInspector]
    public static float timeOfDayInterval = 300f;                   // The interval between consecutive time of day changes (currently set to 5 minutes)
    [HideInInspector]   
    public static float gameTime = 0f;                              // The custom game timer
    //[HideInInspector]
    //public static float clockTime = 0f;                           // The clock time to display which shows the current time of the day
    [HideInInspector]
    public static TimeSpan clockTime = new TimeSpan(6, 00, 00);     // Sets the starting clock time to 06:00
    [HideInInspector]
    public static bool instructionsSeen = false;                    // Stores whether the user has seen the instructions
    [SerializeField]
    public bool instructionsEnabled;                         // Stores whether to display the instructions of the game
    
    
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

        if(clockText == null)
        {
            clockText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        }
        StartCoroutine(WaitForInstructionsSeen());
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
        ShowInstructions();
    }

    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        if(uiReferences != null)
        {
            clockText = uiReferences.clockText.GetComponent<TextMeshProUGUI>();
            backgroundImage = uiReferences.backgroundImage;
            descriptionBox = uiReferences.descriptionBox;
            if (descriptionBox != null)
            {
                descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
            }
            pauseMenuUI = uiReferences.pauseMenuUI;
            rainSystem = uiReferences.rainSystem;
        }
        optionsManager = FindObjectOfType<OptionsManager>();
    }

    void ShowInstructions()                      // Displays the instructions
    {
        if(descriptionBox != null)
        {
            if (instructionsEnabled && !instructionsSeen)
            {
                optionsManager.ShowTextOnDescriptionBox(instructions, 1f);
            }
        }
    }

    private IEnumerator WaitForInstructionsSeen()
    {
        while(!instructionsSeen)
        {
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(GameTimer());
        StartCoroutine(ClockTimer());
        StartCoroutine(TimeOfDayChanger());
    }

    private IEnumerator GameTimer()                     // Coroutine that increments the custom game time counter every frame
    {
        while(gameTime < timeOfDayInterval * 3)
        {
            gameTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ClockTimer()                    // Coroutine that increments the clock time every 1 second
    {
        while(true)
        {
            yield return new WaitForSeconds(8.33f);
            clockTime = clockTime.Add(TimeSpan.FromMinutes(10f));
        }
    }

    private IEnumerator TimeOfDayChanger()              // Coroutine that manages the time of day changes
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
                if(rainSystem != null)          // Stop rain when noon starts
                    Destroy(rainSystem);

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
        clockText.text = clockTime.ToString(@"hh\:mm");             // Update the clock display

        if (descriptionBox != null && descriptionBox.activeSelf && !optionsManager.IsWriting)       // Close the description box if space is pressed and there is no text being typed
        {
            if (descriptionBoxCG.alpha != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                FadeOut(descriptionBoxCG, 0f);
                StartCoroutine(DisableGameObjectAfterDelay(descriptionBox, 0.5f));
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))                   // Pauses the game on pressing 'Escape'
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

    public void disableBackgroundImage()            // To disable the background image GameObject after a certain delay
    {
        StartCoroutine(DisableGameObjectAfterDelay(backgroundImage, 1f));
    }

    public void enableBackgroundImage()             // To enable the background image GameObject after a certain delay
    {
        StartCoroutine(EnableGameObjectAfterDelay(backgroundImage, 1f));
    }

    public void ChangeLOSA(int LOSAUpdate)          // Change the level of self awareness score
    {
        if(LOSAUpdate == 1)     // Positive response
        {
            levelOfSelfAwareness += 10f;
            
        }
        else if(LOSAUpdate == 2)     // Negative response
        {
            if(levelOfSelfAwareness != 0f)
            {
                levelOfSelfAwareness -= 10f;
            }
        }
    }

    public static float GetLOSA()       // Retreive the current LOSA score
    {
        return instance.levelOfSelfAwareness;
    }

    public void Resume()                // Resume game from the pause menu
    {
        if(pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    }

    void Pause()                        // Pauses the game when 'Escape' is pressed
    {
        if(pauseMenuUI)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }

    public void QuitGame()              // Quits the game from the Pause Menu
    {
        Application.Quit();
    }

    public static void FadeIn(CanvasGroup canvasGroup, float delay)     // Fade in canvas group after delay
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, delay));
    }

    public static void FadeOut(CanvasGroup canvasGroup, float delay)    // Fade out canvas group after delay
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, delay));
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float delay, float lerpTime = 0.5f)  // Coroutine to fade canvas group
    {
        yield return new WaitForSeconds(delay);

        float _timeStartedLerping = Time.time;
        float timeSinceStarted;
        float percentageComplete;

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

    public static IEnumerator DisableGameObjectAfterDelay(GameObject gO, float delay)    // Coroutine to disable a GameObject after delay
    {
        yield return new WaitForSeconds(delay);

        gO.SetActive(false);
    }

    public static IEnumerator EnableGameObjectAfterDelay(GameObject gO, float delay)     // Coroutine to enable a GameObject after delay
    {
        yield return new WaitForSeconds(delay);

        gO.SetActive(true);
    }

    public static string GetTimeOfDayString()                                           // Helper method to retreive a capitalized string for the current time of day
    {
        if(currentTimeOfDay == TimeOfDay.MORNING)
        {
            return "Morning";
        }
        else if(currentTimeOfDay == TimeOfDay.NOON)
        {
            return "Noon";
        }
        else
        {
            return "Evening";
        }
    }

    public static int GetTimeOfDayIndex()
    {
        if(currentTimeOfDay == TimeOfDay.MORNING)
        {
            return 0;
        }
        else if (currentTimeOfDay == TimeOfDay.NOON)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
}