using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    private int nextLevelIndex;
    public float transitionTime = 1f;
    public Animator transition;

    public void LoadNextLevel()
    {
        // Start fade animation co-routine
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));

    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play fade animation
        transition.SetTrigger("CrossfadeStart");

        // Wait for fade animation to end
        yield return new WaitForSeconds(transitionTime);

        // Load next Scene
        SceneManager.LoadScene(levelIndex);
    }
}
