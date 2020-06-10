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
    private GameObject staticUI;
    private GameObject dynamicUI;
    private GameObject LOSA;
    private GameObject descriptionBox;
    private GameObject pauseMenuUI;
    private CanvasGroup descriptionBoxCG;

    // State variables
    [HideInInspector]
    public static bool GameIsPaused = false;
    public bool instructionsEnabled;
    
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
        FadeIn(descriptionBoxCG);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (descriptionBox != null && descriptionBox.activeSelf)
        {
            if (descriptionBoxCG.alpha != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                FadeOut(descriptionBoxCG);
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

    public void ChangeLOSA(int positiveOrNegative)
    {
        if(positiveOrNegative == 1)
        {
            levelOfSelfAwareness += 10f;
            
        }
        else
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

    public static void FadeIn(CanvasGroup canvasGroup)
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f));
    }

    public static void FadeOut(CanvasGroup canvasGroup)
    {
        instance.StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f));
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float lerpTime = 0.3f)
    {
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
}
