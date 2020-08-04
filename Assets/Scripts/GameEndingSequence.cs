using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndingSequence : MonoBehaviour
{
    // To be set in the inspector
    public CanvasGroup topTextCanvasCG;             // Reference to the top text box canvas group
    public CanvasGroup bottomTextCanvasCG;          // Reference to the bottom text box canvas group
    public TextMeshProUGUI topText;                 // Reference to the top text box text
    public TextMeshProUGUI bottomText;              // Reference to the bottom text box text

    private readonly string[] lowLOSAText = { "One day feels like the other. However, particularly this day has been so trivial, it kind of feels wasted. ",
                                        "I feel tired and it’s late. I should go to bed." };

    private readonly string[] mediumLOSAText = { "Before I had opened my eyes, I already lost my belief that this day might once turn out well. However, I proved myself wrong.",
                                            "I kind of enjoyed the time and I think it’s smart to make a plan of things I’d like to do in the next days. But nothing too complex. Babysteps!"};

    private readonly string[] highLOSAText = { "I should make a big red cross in today’s agenda! I feel surprisingly good and even though I’ve been living here for already quite a while now, " + 
                                                "it was super nice to explore my surroundings.",
                                        "It’s strange to live in a place which feels foreign and therefore I am glad, that I got to know it better, " + 
                                            "even though there are areas that I’d like to avoid. Disconnection is reasoned."};

    // Start is called before the first frame update
    void Start()
    {
        foreach (Sound s in AudioManager.instance.sounds)
        {
            if (s.source.isPlaying)
            {
                if (s.name != "H_Clock_Strike_Midnight")
                {
                    if(s.name != "Ending_Music")
                    {
                        s.source.Stop();
                    }
                }
            }
        }

        Cursor.visible = false;
        GameSession.LOSAStatus LOSAStatus = GameSession.GetLOSAStatus();

        if (LOSAStatus == GameSession.LOSAStatus.LOW)
        {
            // Show LOW LOSA ending
            StartCoroutine(LowLOSAEnding());
        }
        else if (LOSAStatus == GameSession.LOSAStatus.MEDIUM)
        {
            // Show MEDIUM LOSA ending
            StartCoroutine(MediumLOSAEnding());
        }
        else if (LOSAStatus == GameSession.LOSAStatus.HIGH)
        {
            // Show HIGH LOSA ending
            StartCoroutine(HighLOSAEnding());
        }
        else if (LOSAStatus == GameSession.LOSAStatus.MAX) 
        {
            // Show Credits
            StartCoroutine(ShowCredits());
        }
    }

    private IEnumerator LowLOSAEnding()
    {
        yield return new WaitForSeconds(3f);

        topText.text = lowLOSAText[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(4.5f);

        bottomText.text = lowLOSAText[1];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        StartCoroutine(ShowCredits());
    }

    private IEnumerator MediumLOSAEnding()
    {
        yield return new WaitForSeconds(3f);

        topText.text = mediumLOSAText[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(6f);

        bottomText.text = mediumLOSAText[1];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(6f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        StartCoroutine(ShowCredits());
    }

    private IEnumerator HighLOSAEnding()
    {
        yield return new WaitForSeconds(3f);

        topText.text = highLOSAText[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(10f);

        bottomText.text = highLOSAText[1];
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 0.7f));

        yield return new WaitForSeconds(10f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 0.7f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 1f, 0.7f));

        StartCoroutine(ShowCredits());
    }

    private IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(4f);

        topText.fontSize = 125;
        topText.text = "The Way Back";
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(2f);

        topText.fontSize = 40;
        topText.text = "Project Coordinator";
        bottomText.text = "Gianluca Pandolfo";
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(2);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Game Art";
        bottomText.text = "Parva Zahed";
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(2);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Narrative";
        bottomText.text = "Justin Ziemba";
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Programming";
        bottomText.text = "Ketan Patel";
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(2f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Gramophone Music";
        bottomText.text = "Accell: \"Broken\" composed by Patricia George";

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Ending Music";
        bottomText.text = "Accell: \"Italian Birdie\" - Eline Duerinck (cello) and Patricia George (accordion)";

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(4.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        topText.text = "Artwork for the \"Attic\" ending";
        bottomText.text = "Ashkan Goodarzi, Alireza Omarani & Soheila Aghajafari";

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 0f, 1f, 0f, 1f));

        yield return new WaitForSeconds(2.5f);

        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 1f, 0f, 0f, 1f));
        StartCoroutine(FadeCanvasGroup(bottomTextCanvasCG, 1f, 0f, 0f, 1f));

        yield return new WaitForSeconds(1.5f);

        Destroy(GameObject.Find("GameSession"));

        LevelChanger.LoadLevel("Start Scene");
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
