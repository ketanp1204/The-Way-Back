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

    private string[] lowLOSAText = { "One day feels like the other. However, particularly this day has been so trivial, it kind of feels wasted. ",
                                        "I feel tired and it’s late. I should go to bed." };

    private string[] mediumLOSAText = { "Before I had opened my eyes, I already lost my believe that this day might once turn out well. However, I proved myself wrong.",
                                            "I kind of enjoyed the time and I think it’s smart to make a plan of things I’d like to do in the next days. " + 
                                                "But nothing too complex. Babysteps!"};

    private string[] highLOSAText = { "I should make a big red cross in today’s agenda! " + 
                                            "I feel surprisingly good and even though I’ve been living here for already quite a while now, " + 
                                                "it was super nice to explore my surrounding.",
                                        "It’s strange to live in a place which feels foreign and therefore I am glad, that I got to know it better, " + 
                                            "even though there are areas that I’d like to avoid. Disconnection is reasoned."};

    private string[] maxLOSAText = { "It’s frightening, to not know who you are today, even though you know who you’ve been your entire life.",
                                         "To forget – or even worse: to suppress – your true self, is the worst a person can do to itself.",
                                         "I am glad I’ve experienced all these things today. I feel bound and free at the same time.",
                                         "I feel disconnected and connected at the same time. I feel like a secret and the truth at the same time.",
                                         "I feel like I’ve finally met a long lost friend in myself. Nonetheless, after all this time, I recognized this person immediately."};

    // Start is called before the first frame update
    void Start()
    {
        GameSession.LOSAStatus LOSAStatus = GameSession.GetLOSAStatus();

        if(LOSAStatus == GameSession.LOSAStatus.LOW)
        {
            // Show LOW LOSA ending

        }
        else if (LOSAStatus == GameSession.LOSAStatus.MEDIUM)
        {
            // Show MEDIUM LOSA ending

        }
        else if (LOSAStatus == GameSession.LOSAStatus.HIGH)
        {
            // Show HIGH LOSA ending
        }
    }

    private IEnumerator lowLOSAEnding()
    {
        yield return new WaitForSeconds(3f);

        AudioManager.Play("Ending_Music");

        yield return new WaitForSeconds(2f);

        topText.text = lowLOSAText[0];
        StartCoroutine(FadeCanvasGroup(topTextCanvasCG, 0f, 1f, 0f, 0.7f));
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
