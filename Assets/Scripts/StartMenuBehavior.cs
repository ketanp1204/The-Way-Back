using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMenuBehavior : MonoBehaviour
{
    public CanvasGroup topTextCanvasCG;
    public CanvasGroup bottomTextCanvasCG;
    public CanvasGroup startMenuUICanvasCG;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;

    [TextArea(3, 10)]
    public string[] topTexts;
    [TextArea(3, 10)]
    public string[] bottomTexts;

    public void StartStory()
    {
        StartCoroutine(ShowIntroTexts());
    }

    private IEnumerator ShowIntroTexts()
    {
        StartCoroutine(FadeCanvasGroup(startMenuUICanvasCG, 1f, 0f, 0f, 0.6f));

        yield return new WaitForSeconds(3f);

        AudioManager.Play("Intro_Sounds");

        yield return new WaitForSeconds(4f);

        topText.text = topTexts[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3f);

        bottomText.text = bottomTexts[0];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        yield return new WaitForSeconds(3f);

        topText.text = topTexts[1];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3f);

        bottomText.text = bottomTexts[1];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1.5f, 0.7f));

        yield return new WaitForSeconds(37f);

        topText.text = "Extra Time";
        topText.fontSize = 150;
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(4f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));

        yield return new WaitForSeconds(1f);

        LevelChanger.LoadNextLevel();
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float delay, float lerpTime)  // Coroutine to fade canvas group
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

            if (canvasGroup != null)
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
}
