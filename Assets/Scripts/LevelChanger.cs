using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public float transitionTime = 1f;               // Time for which the transition fade animation runs
    public Animator transition;                     // Reference to the Animator
    public static LevelChanger instance;
    [HideInInspector]
    public bool fadeAnimationRunning = false;       // Status of the transition animation

    void Start()
    {
        instance = this;
    }

    public static void LoadLevel(string sceneName)
    {
        // Start fade animation co-routine
        instance.StartCoroutine(CrossFadeStart(false));

        instance.StartCoroutine(LoadScene(sceneName));
    }

    public void LoadNext()                          // Loads next level
    {
        LoadNextLevel();
    }

    public void LoadPrevious()                      // Loads previous level
    {
        LoadPreviousLevel();
    }

    public static void LoadNextLevel()              // Static method to load next level
    {
        // Start fade animation co-routine
        instance.StartCoroutine(CrossFadeStart(false));

        // Load Next Scene
        int sceneIndexToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        if(sceneIndexToLoad != SceneManager.sceneCountInBuildSettings)
        {
            instance.StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
        else
        {
            Debug.LogWarning("No next scene exists");
        }
    }

    public static void LoadPreviousLevel()          // Static method to load next level
    {
        // Start fade animation co-routine
        instance.StartCoroutine(CrossFadeStart(false));

        // Load Previous Scene
        instance.StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex - 1));
    }

    private static IEnumerator LoadScene(int buildIndex)        // Loads the scene with the given index number
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(buildIndex);
    }

    private static IEnumerator LoadScene(string sceneName)      // Loads the scene with the given name
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }

    public static IEnumerator CrossFadeStart(bool endAfter)     // Coroutine to start fade animation
    {
        instance.fadeAnimationRunning = true;
        // Play fade animation
        instance.transition.SetTrigger("CrossfadeStart");

        // Wait for fade animation to end
        yield return new WaitForSeconds(instance.transitionTime);

        instance.fadeAnimationRunning = false;
        if(endAfter)
        {
            instance.transition.SetTrigger("CrossfadeEnd");
        }
    }
    /*
    public static IEnumerator CrossFadeEnd()
    {
        instance.fadeAnimationRunning = true;
        instance.transition.SetTrigger("CrossfadeEnd");
        yield return new WaitForSeconds(instance.transitionTime);
    }
    */
    public void QuitGame()                                      // Quits the game
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}
