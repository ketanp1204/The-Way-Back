using System.Collections;
using System.Threading;
using UnityEngine;

public class TimeOfDaySprites : MonoBehaviour
{
    // To be set in the inspector
    public GameObject morningImage;
    public GameObject noonImage;
    public GameObject eveningImage;

    // To be updated in script
    private SpriteRenderer morningSR;
    //private SpriteRenderer noonSR;
    private SpriteRenderer eveningSR;

    // Start is called before the first frame update
    void Start()
    {
        
        morningSR = morningImage.GetComponent<SpriteRenderer>();
        // noonSR = noonImage.GetComponent<SpriteRenderer>();       // TODO: uncomment when noon image is available
        eveningSR = eveningImage.GetComponent<SpriteRenderer>();
        
        if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.MORNING)
        {
            morningSR.enabled = true;
            float timeElapsed = Time.time;
            StartCoroutine(FadeOutImage(morningSR, timeElapsed, 0));
            // StartCoroutine(FadeInImage(noonSR, timeElapsed, 0));     // TODO: uncomment when noon image is available
            StartCoroutine(FadeInImage(eveningSR, timeElapsed, 0));
        }
        else if (GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
        {
            //noonSR.enabled = true;
            // StartCoroutine(FadeOutImage(noonSR, GameSession.timeOfDayInterval));
            float timeElapsed = Time.time;
            StartCoroutine(FadeInImage(eveningSR, timeElapsed, 1));
        }
        else
        {
            eveningSR.enabled = true;
        }
    }
    
    private IEnumerator FadeOutImage(SpriteRenderer sR, float timeElapsed, int timeOfDayIndex)     // Coroutine to fade out a sprite over a specified duration
    {
        if (sR.enabled)
        {
            while (Time.time <= timeElapsed + GameSession.timeOfDayInterval)
            {
                Color color = sR.color;
                color.a = 1f - Mathf.Clamp01((Time.time - GameSession.timeOfDayInterval * timeOfDayIndex) / GameSession.timeOfDayInterval);
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
        float start = Time.time;
        while (Time.time <= timeElapsed + GameSession.timeOfDayInterval)
        {
            Color color = sR.color;
            color.a = 0f + Mathf.Clamp01((Time.time - GameSession.timeOfDayInterval * timeOfDayIndex) / GameSession.timeOfDayInterval);
            Debug.Log(color.a);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
