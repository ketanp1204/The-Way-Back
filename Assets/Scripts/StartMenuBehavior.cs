using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuBehavior : MonoBehaviour
{
    // To be set in the inspector
    public CanvasGroup topTextCanvasCG;             // Reference to the top text box canvas group
    public CanvasGroup bottomTextCanvasCG;          // Reference to the bottom text box canvas group
    public CanvasGroup startMenuUICanvasCG;         // Reference to the startMenuUI canvas group
    public TextMeshProUGUI topText;                 // Reference to the top text box text
    public TextMeshProUGUI bottomText;              // Reference to the bottom text box text
    public GameObject background;                      // Reference to the background image animation

    // To be set in the inspector
    [TextArea(3, 10)]
    public string[] topTexts;                       // Stores the texts for the top text box
    [TextArea(3, 10)]
    public string[] bottomTexts;                    // Stores the texts for the bottom text box

    void Start()
    {
        Cursor.visible = true;
        AudioManager.Play("G_Noon");
    }

    public void StartStory()                        // Method to start the texts coroutine
    {
        background.GetComponent<Animator>().enabled = false;

        StartCoroutine(FadeInImage(background.GetComponent<SpriteRenderer>()));

        AudioManager.Stop("G_Noon");

        StartCoroutine(ShowIntroTexts());
        
        Cursor.visible = false;
        startMenuUICanvasCG.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
    }

    private IEnumerator FadeInImage(SpriteRenderer sR)
    {
        Color c = sR.color;
        while(c.a != 0)
        {
            c.a -= 0.01f;
            sR.color = c;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ShowIntroTexts()            // Coroutine that displays the texts with delays and loads the next level at the end
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

        yield return new WaitForSeconds(29f);

        topText.text = "The Way Back";
        topText.fontSize = 125;
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(4f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));

        yield return new WaitForSeconds(0.5f);

        Cursor.visible = true;

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
