using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public float transitionTime = 1f;
    public Animator transition;
    private static LevelChanger instance;

    void Start()
    {
        instance = this;
    }

    public void LoadNextLevel()
    {
        // Start fade animation co-routine
        instance.StartCoroutine(CrossFadeStart(false));

        // Load Next Scene
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadPreviousLevel()
    {
        // Start fade animation co-routine
        instance.StartCoroutine(CrossFadeStart(false));

        // Load Previous Scene
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex - 1));
    }

    IEnumerator LoadScene(int buildIndex)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(buildIndex);
    }

    public static IEnumerator CrossFadeStart(bool endAfter)
    {
        // Play fade animation
        instance.transition.SetTrigger("CrossfadeStart");

        // Wait for fade animation to end
        yield return new WaitForSeconds(instance.transitionTime);

        if(endAfter)
        {
            instance.transition.SetTrigger("CrossfadeEnd");
            endAfter = false;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    /*
    IEnumerator NextLevel(int levelIndex)
    {
        
        // Play fade animation
        transition.SetTrigger("CrossfadeStart");

        // Wait for fade animation to end
        yield return new WaitForSeconds(transitionTime);

        // Load next Scene
        SceneManager.LoadScene(levelIndex);
        
    }
    */



    /*
    IEnumerator PreviousLevel(int levelIndex)
    {
        // Play fade animation
        transition.SetTrigger("CrossfadeStart");

        // Wait for fade animation to end
        yield return new WaitForSeconds(transitionTime);

        // Load next Scene
        SceneManager.LoadScene(levelIndex);
    }
    */


}
