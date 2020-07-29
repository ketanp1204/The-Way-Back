using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AtticEnding : MonoBehaviour
{
    // To be set in the inspector
    public CanvasGroup topTextCanvasCG;             // Reference to the top text box canvas group
    public CanvasGroup bottomTextCanvasCG;          // Reference to the bottom text box canvas group
    public TextMeshProUGUI topText;                 // Reference to the top text box text
    public TextMeshProUGUI bottomText;              // Reference to the bottom text box text

    private readonly string[] endingText = { "It’s frightening, to not know who you are today, even though you know who you’ve been your entire life.",
                                        "To forget – or even worse: to suppress – your true self, is the worst a person can do to oneself.",
                                        "I am glad I’ve experienced all these things today. I feel bound and free at the same time.",
                                        "I feel disconnected and connected at the same time. I feel like the secret and the truth at the same time.",
                                        "I feel like I’ve finally met a long lost friend in myself. Nonetheless, after all this time, I recognized this person immediately."};

    void Start()
    {
        StartCoroutine(ShowEnding());
    }

    private IEnumerator ShowEnding()
    {
        yield return new WaitForSeconds(2f);

        AudioManager.Play("Ending_Music");

        yield return new WaitForSeconds(2f);

        topText.text = endingText[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3.5f);

        bottomText.text = endingText[1];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        yield return new WaitForSeconds(2.5f);

        topText.text = endingText[2];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3.5f);

        bottomText.text = endingText[3];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        yield return new WaitForSeconds(2.5f);

        topText.text = endingText[4];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(3.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));

        yield return new WaitForSeconds(2.5f);

        LevelChanger.LoadLevel("GameEnding");
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
