using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private int levelOfSelfAwareness;
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
    private OptionsManager optionsManager;                          // Stores the reference to the options manager

    // Static variables
    [HideInInspector]
    public static bool GameIsPaused;                                // Is true when the game is paused
    [HideInInspector]
    public static TimeOfDay currentTimeOfDay;                       // Static reference to the current time of day (Morning, Noon or Evening)
    [HideInInspector]
    public static bool closeUpObjects;                              // Is true when an object is in close up view
    [HideInInspector]
    public static float timeOfDayInterval;                          // The interval between consecutive time of day changes (currently set to 5 minutes)
    [HideInInspector]
    public static float gameTime;                                   // The custom game timer
    [HideInInspector]
    public static TimeSpan clockTime;                               // Sets the starting clock time to 06:00
    [HideInInspector]
    public static bool instructionsSeen;                            // Stores whether the user has seen the instructions
    [SerializeField]
    public bool instructionsEnabled;                                // Stores whether to display the instructions of the game
    public bool atticEnding = false;
    private bool gameEndingScene = false;

    // LOSA calculations
    public static int maxScore;
    public static int minScore;


    public enum LOSAStatus
    {
        LOW,
        MEDIUM,
        HIGH,
        MAX
    }
    
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

        Initialize();
        GameEventsTracker.Initialize();

        if (SceneManager.GetActiveScene().name != "GameEnding")
        {
            if(SceneManager.GetActiveScene().name != "Attic")
            {
                if (clockText == null)
                {
                    clockText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
                }
                StartCoroutine(WaitForInstructionsSeen());
            }
            else
            {
                atticEnding = true;
            }
        }
        else
        {
            gameEndingScene = true;
        }
    }

    void Initialize()
    {
        levelOfSelfAwareness = 0;
        GameIsPaused = false;
        currentTimeOfDay = TimeOfDay.MORNING;
        closeUpObjects = false;
        timeOfDayInterval = 480f;
        gameTime = 0f;
        clockTime = new TimeSpan(6, 00, 00);
        instructionsSeen = false;
        atticEnding = false;
        gameEndingScene = false;
        maxScore = 30;
        minScore = 3;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!gameEndingScene)
        {
            SetReferences();
            descriptionBox.SetActive(false);
            pauseMenuUI.SetActive(false);
            ShowInstructions();
        }
        HandleSceneImageChanges(scene);
    }

    void SetReferences()
    {
        uiReferences = FindObjectOfType<UIReferences>();
        if(uiReferences != null)
        {
            clockText = uiReferences.clockText.GetComponent<TextMeshProUGUI>();
            backgroundImage = uiReferences.backgroundImage;
            descriptionBox = uiReferences.descriptionBox;
            pauseMenuUI = uiReferences.pauseMenuUI;
            pauseMenuUI.transform.Find("ResumeButton").gameObject.GetComponent<Button>().onClick.AddListener(() => Resume());
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
        StartCoroutine(TimeOfDaySounds());
    }

    private IEnumerator GameTimer()                     // Coroutine that increments the custom game time counter every frame
    {
        while(gameTime < timeOfDayInterval * 3 + 10f)
        {
            gameTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ClockTimer()                    // Coroutine that increments the clock time
    {
        while(true)
        {
            yield return new WaitForSeconds(13.33f);
            clockTime = clockTime.Add(TimeSpan.FromMinutes(10f));
        }
    }

    private IEnumerator TimeOfDayChanger()              // Coroutine that manages the time of day changes
    {
        int changes = 0;
        while(changes < 4)
        {
            if (GameSession.gameTime < timeOfDayInterval)
            {
                currentTimeOfDay = TimeOfDay.MORNING;
                changes++;
            }
            else if (GameSession.gameTime >= timeOfDayInterval && GameSession.gameTime < timeOfDayInterval * 2)
            {
                string scene = SceneManager.GetActiveScene().name;

                if (scene != "GameEnding")
                {
                    AudioManager.Play("H_Clock_Strike_Noon");
                    if (scene != "Hallway")
                    {
                        AudioManager.ChangeVolume("H_Clock_Strike_Noon", 0.25f);
                    }
                    currentTimeOfDay = TimeOfDay.NOON;
                    changes++;
                }
            }
            else if (GameSession.gameTime >= timeOfDayInterval * 2 && GameSession.gameTime < timeOfDayInterval * 3)
            {
                string scene = SceneManager.GetActiveScene().name;

                if (scene != "GameEnding")
                {
                    AudioManager.Play("H_Clock_Strike_Evening_6");
                    if (scene != "Hallway")
                    {
                        AudioManager.ChangeVolume("H_Clock_Strike_Evening_6", 0.25f);
                    }
                    currentTimeOfDay = TimeOfDay.EVENING;
                    changes++;
                }
            }
            else if (GameSession.gameTime >= timeOfDayInterval * 3)
            {
                AudioManager.Play("H_Clock_Strike_Midnight");
                LevelChanger.LoadLevel("GameEnding");
            }
            yield return new WaitForSeconds(timeOfDayInterval);
        }
    }

    private IEnumerator TimeOfDaySounds()
    {
        int changes = 0;
        while(changes < 3)
        {
            if(currentTimeOfDay == TimeOfDay.MORNING)
            {
                changes++;
            }
            else if(currentTimeOfDay == TimeOfDay.NOON)
            {
                changes++;

                AudioManager.Stop("Morning_Rain_Inside");
                AudioManager.Stop("Morning_Rain_Outside");

                string scene = SceneManager.GetActiveScene().name;

                if(scene != "Garden")
                {
                    AudioManager.Play("Noon_Inside");
                }
            }
            else if(currentTimeOfDay == TimeOfDay.EVENING)
            {
                changes++;

                AudioManager.Stop("Noon_Inside");
                AudioManager.Stop("G_Noon");

                string scene = SceneManager.GetActiveScene().name;

                if(scene == "Garden")
                {
                    AudioManager.Play("G_Evening");
                }
                else
                {
                    AudioManager.Play("Evening_Inside");
                }
            }
            yield return new WaitForSeconds(timeOfDayInterval);
        }
    }

    private void HandleSceneImageChanges(Scene scene)
    {
        if(scene.name == "LivingRoom")
        {
            if (GameEventsTracker.G_Plant_Planted)
            {
                GameAssets.instance.LR_Plant.SetActive(false);
            }
            else
            {
                GameAssets.instance.LR_Plant.SetActive(true);
            }

            if(GameEventsTracker.LR_TV_On)
            {
                GameAssets.instance.LR_TV_Static.GetComponent<Animator>().enabled = true;
                GameAssets.instance.LR_TV_Static.GetComponent<Animator>().Play("Base Layer.LR_TV_Static");
            }
            else
            {
                GameAssets.instance.LR_TV_Static.GetComponent<Animator>().enabled = false;
                GameAssets.instance.LR_TV_Static.GetComponent<SpriteRenderer>().sprite = null;
            }

            if(GameEventsTracker.LR_Gramophone_Animation_Playing)
            {
                StartCoroutine(ObjectSpecificBehavior.LR_G_PlayerAnimation());
                StartCoroutine(ObjectSpecificBehavior.LR_G_RecordAnimation());
            }
        }
        
        if(scene.name == "Bedroom")
        {
            if(GameEventsTracker.Bed_BedDone)
            {
                GameAssets.instance.Bed_Day_Image.sprite = GameAssets.instance.Bed_Done_Day;
                GameAssets.instance.Bed_Noon_Image.sprite = GameAssets.instance.Bed_Done_Noon;
                GameAssets.instance.Bed_Eve_Image.sprite = GameAssets.instance.Bed_Done_Eve;
                GameAssets.instance.Bed_Diary.SetActive(true);
            }

            if(GameEventsTracker.Bed_Diary_Kept)
            {
                Destroy(GameAssets.instance.Bed_Diary);
            }
        }

        if(scene.name == "Kitchen")
        {
            if(GameEventsTracker.K_Fruits_HalfBasket)
            {
                GameAssets.instance.K_Fruits_Day.sprite = GameAssets.instance.K_Fruits_HalfBasket_Day;
                GameAssets.instance.K_Fruits_Noon.sprite = GameAssets.instance.K_Fruits_HalfBasket_Noon;
                GameAssets.instance.K_Fruits_Eve.sprite = GameAssets.instance.K_Fruits_HalfBasket_Eve;
            }

            if(GameEventsTracker.K_ShoppingList_Removed)
            {
                Destroy(GameAssets.instance.K_ShoppingList);
            }
        }

        if(scene.name == "Bathroom")
        {
            if(GameEventsTracker.B_SleepingPillsWashedDown)
            {
                Destroy(GameAssets.instance.B_SleepingPills);
            }
        }

        if(scene.name == "Garden")
        {
            if(GameEventsTracker.G_Fire_Animation_Playing)
            {
                GameAssets.instance.G_Brazier.enabled = true;
                GameAssets.instance.G_Brazier.Play("Base Layer.G_Brazier_Fire");
            }

            if(GameEventsTracker.G_Pond_Filled)
            {
                GameAssets.instance.G_Noon_Image.sprite = GameAssets.instance.G_Pond_MoreWater_Noon;
                GameAssets.instance.G_Eve_Image.sprite = GameAssets.instance.G_Pond_MoreWater_Eve;
            }

            if(GameEventsTracker.G_Pond_Animation_Playing)
            {
                GameAssets.instance.G_Fountain.enabled = true;
                GameAssets.instance.G_Fountain.Play("Base Layer.G_Fountain");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameEndingScene)
        {
            clockText.text = clockTime.ToString(@"hh\:mm");             // Update the clock display
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
            instance.levelOfSelfAwareness += 1;
            
        }
        else if(LOSAUpdate == 2)     // Negative response
        {
            if(levelOfSelfAwareness != 0)
            {
                instance.levelOfSelfAwareness -= 1;
            }
        }
    }

    public static int GetLOSA()                      // Retreive the current LOSA score
    {
        return instance.levelOfSelfAwareness;
    }

    public static LOSAStatus GetLOSAStatus()         // Retreive the current LOSA Status
    {
        float currentScore = (float)(instance.levelOfSelfAwareness - minScore) / (float)(maxScore - minScore) * 100;

        float currentScoreAsPercentage = Mathf.Floor(currentScore);

        if(currentScoreAsPercentage < 30f)
        {
            return LOSAStatus.LOW;
        }
        else if(currentScoreAsPercentage >= 30f && currentScoreAsPercentage < 60f)
        {
            return LOSAStatus.MEDIUM;
        }
        else if(currentScoreAsPercentage >= 60f && currentScoreAsPercentage < 80f)
        {
            return LOSAStatus.HIGH;
        }
        else
        {
            return LOSAStatus.MAX;
        }
    }

    public void ResumeGame()
    {
        Resume();
    }

    public static void Resume()                // Resume game from the pause menu
    {
        if(instance.pauseMenuUI != null)
        {
            instance.pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }
    }

    public static void Pause()                        // Pauses the game when 'Escape' is pressed
    {
        if(instance.pauseMenuUI)
        {
            instance.pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }

    public void QuitGame()              // Quits the game from the Pause Menu
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public static void FadeIn(CanvasGroup canvasGroup, float delay)     // Fade in canvas group after delay
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, delay));
    }

    public static void FadeOut(CanvasGroup canvasGroup, float delay)    // Fade out canvas group after delay
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, delay));
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

    public IEnumerator GoToAttic()
    {
        AudioManager.Play("Ending_Music");

        yield return new WaitForSeconds(12f);

        GameSession.FadeOut(GameObject.Find("Description Box").GetComponent<CanvasGroup>(), 0f);

        LevelChanger.LoadLevel("Attic");
    }
}