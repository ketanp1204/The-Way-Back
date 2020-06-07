using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    // Configuration parameters
    private float levelOfSelfAwareness;
    private string instructions = "Click on objects to get options to choose from which modifies your LOSA score. " +
                                  "The LOSA score shown is just for visualization and will not be included in the final game. " +
                                  "Certain objects like the window will have only reactions when you click on them based on your current LOSA Score";

    // Cached References
    public TextMeshProUGUI LOSA;
    public GameObject descriptionBox;
    public GameObject pauseMenuUI;

    // State variables
    [HideInInspector]
    public static bool GameIsPaused = false;
    public bool instructionsEnabled;


    // Start is called before the first frame update
    void Start()
    {
        levelOfSelfAwareness = 0f;
        LOSA.text = "LOSA: " + levelOfSelfAwareness;
        if(instructionsEnabled)
        {
            StartCoroutine(showInstructions());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (descriptionBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            descriptionBox.SetActive(false);
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
        LOSA.text = "LOSA: " + levelOfSelfAwareness;
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
}
