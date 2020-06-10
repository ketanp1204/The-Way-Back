using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;
    // Configuration parameters
    public float levelOfSelfAwareness = 0;
    private string instructions = "Click on objects to get options to choose from which modifies your LOSA score. " +
                                  "The LOSA score shown is just for visualization and will not be included in the final game. " +
                                  "Certain objects like the window will have only reactions when you click on them based on your current LOSA Score";

    // Cached References
    public GameObject LOSA;
    public GameObject descriptionBox;
    public GameObject pauseMenuUI;
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


    // Start is called before the first frame update
    void Start()
    {
        LOSA = GameObject.Find("LOSA");

        LOSA.GetComponent<TextMeshProUGUI>().text = "LOSA: " + levelOfSelfAwareness;
        if(instructionsEnabled)
        {
            StartCoroutine(showInstructions());
        }
        descriptionBoxCG = descriptionBox.GetComponent<CanvasGroup>();
        descriptionBoxCG.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(LOSA == null)
        {
            LOSA = GameObject.Find("LOSA");
        }
        if(descriptionBox == null)
        {
            descriptionBox = GameObject.Find("Description Box");
        }
        if(pauseMenuUI == null)
        {
            pauseMenuUI = GameObject.Find("PauseMenu");
        }

        LOSA.GetComponent<TextMeshProUGUI>().text = "LOSA: " + levelOfSelfAwareness;

        if (descriptionBox != null && descriptionBoxCG != null && descriptionBox.activeSelf)
        {
            if (descriptionBoxCG.alpha != 0 && Input.GetKeyDown(KeyCode.Space))
            {
                FadeOut(descriptionBoxCG);
                // descriptionBox.SetActive(false);
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

    private IEnumerator showInstructions()
    {
        yield return new WaitForSeconds(1.0f);
        descriptionBox.SetActive(true);
        descriptionBox.GetComponentInChildren<TextMeshProUGUI>().text = instructions;
        FadeIn(descriptionBoxCG);
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
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
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
