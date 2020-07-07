using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayManager : MonoBehaviour
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
            StartCoroutine(FadeOutImage(morningSR, GameSession.timeOfDayInterval));
            // StartCoroutine(FadeInImage(noonSR, GameSession.timeOfDayInterval)); // TODO uncomment when noon image is available
            StartCoroutine(FadeInImage(eveningSR, GameSession.timeOfDayInterval));

        }
        else if(GameSession.currentTimeOfDay == GameSession.TimeOfDay.NOON)
        {
            //noonSR.enabled = true;
            // StartCoroutine(FadeOutImage(noonSR, GameSession.timeOfDayInterval));
            StartCoroutine(FadeInImage(eveningSR, GameSession.timeOfDayInterval));
        }
        else
        {
            eveningSR.enabled = true;
        }
    }

    private IEnumerator FadeOutImage(SpriteRenderer sR, float duration)
    {
        if(sR.enabled)
        {
            float start = Time.time;
            while (Time.time <= start + duration)
            {
                Color color = sR.color;
                color.a = 1f - Mathf.Clamp01((Time.time - start) / duration);
                sR.color = color;
                yield return new WaitForEndOfFrame();
            }
            sR.enabled = false;
        }
    }

    private IEnumerator FadeInImage(SpriteRenderer sR, float duration)
    {
        if(!sR.enabled)
        {
            sR.enabled = true;
        }
        float start = Time.time;
        while (Time.time <= start + duration)
        {
            Color color = sR.color;
            color.a = 0f + Mathf.Clamp01((Time.time - start) / duration);
            sR.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
