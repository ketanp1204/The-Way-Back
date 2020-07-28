using System.Collections;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEngine;

public class TimeOfDaySprites : MonoBehaviour
{
    private GameObject morningImage;                 // Reference to the morning image gameObject
    private GameObject noonImage;                    // Reference to the noon image gameObject
    private GameObject eveningImage;                 // Reference to the evening image gameObject

    private SpriteRenderer morningSR;               // Reference to the morning image sprite renderer
    private SpriteRenderer noonSR;                  // Reference to the noon image sprite renderer
    private SpriteRenderer eveningSR;               // Reference to the evening image sprite renderer

    // Start is called before the first frame update
    void OnEnable()
    {
        morningImage = transform.Find("MorningImage").gameObject;
        noonImage = transform.Find("NoonImage").gameObject;
        eveningImage = transform.Find("EveningImage").gameObject;

        // Get the references to the sprite renderers
        morningSR = morningImage.GetComponent<SpriteRenderer>();
        noonSR = noonImage.GetComponent<SpriteRenderer>();
        eveningSR = eveningImage.GetComponent<SpriteRenderer>();

        StartCoroutine(LoadSprites());
    }

    private IEnumerator LoadSprites()
    {
        int changes = 0;
        while(changes < 3)
        {
            changes += 1;
            while (!GameSession.instructionsSeen)
            {
                yield return new WaitForEndOfFrame();
            }
            // Load sprites based on the current time of day
            if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
            {
                morningSR.enabled = true;
                noonSR.enabled = true;
                float timeElapsed = GameSession.gameTime;
                StartCoroutine(FadeOutImage(morningSR, timeElapsed, GameSession.GetTimeOfDayIndex()));
                StartCoroutine(FadeInImage(noonSR, timeElapsed, GameSession.GetTimeOfDayIndex()));
            }
            else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
            {
                morningSR.enabled = false;
                noonSR.enabled = true;
                eveningSR.enabled = true;
                float timeElapsed = GameSession.gameTime;
                StartCoroutine(FadeOutImage(noonSR, timeElapsed, GameSession.GetTimeOfDayIndex()));
                StartCoroutine(FadeInImage(eveningSR, timeElapsed, GameSession.GetTimeOfDayIndex()));
            }
            else
            {
                morningSR.enabled = false;
                if (!eveningSR.enabled)
                    eveningSR.enabled = true;
            }
            yield return new WaitForSeconds(GameSession.timeOfDayInterval);
        }
    }
    
    private IEnumerator FadeOutImage(SpriteRenderer sR, float timeElapsed, int timeOfDayIndex)     // Coroutine to fade out a sprite over a specified duration
    {
        if (sR.enabled)
        {
            while (GameSession.gameTime <= timeElapsed + GameSession.timeOfDayInterval)
            {
                Color color = sR.color;
                color.a = 1f - Mathf.Clamp01((GameSession.gameTime - GameSession.timeOfDayInterval * timeOfDayIndex) / GameSession.timeOfDayInterval);
                sR.color = color;
                yield return new WaitForEndOfFrame();
            }
            sR.enabled = false;
        }
    }

    private IEnumerator FadeInImage(SpriteRenderer sR, float timeElapsed, int timeOfDayIndex)      // Coroutine to fade in a sprite over a specified duration
    {
        if (!sR.enabled)
        {
            sR.enabled = true;
        }
        while (GameSession.gameTime <= timeElapsed + GameSession.timeOfDayInterval)
        {
            Color color = sR.color;
            color.a = 0f + Mathf.Clamp01((GameSession.gameTime - GameSession.timeOfDayInterval * timeOfDayIndex) / GameSession.timeOfDayInterval);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
