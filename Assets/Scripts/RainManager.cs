using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
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
