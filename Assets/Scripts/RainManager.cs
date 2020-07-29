using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    public static RainManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(GameSession.currentTimeOfDay != GameSession.TimeOfDay.MORNING)
        {
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowRainAnimationTillNoon());
        }
    }

    public static void StopRain()
    {
        instance.gameObject.GetComponent<ParticleSystem>().Stop();
    }

    public static void StartRain()
    {
        instance.gameObject.GetComponent<ParticleSystem>().Play();
    }

    private IEnumerator ShowRainAnimationTillNoon()
    {
        gameObject.SetActive(true);

        while (!GameSession.instructionsSeen)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(GameSession.timeOfDayInterval - GameSession.gameTime);
        
        gameObject.SetActive(false);
    }
}
