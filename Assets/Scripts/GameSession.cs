using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSession : MonoBehaviour
{

    private float levelOfSelfAwareness;
    public TextMeshProUGUI LOSA;

    // Start is called before the first frame update
    void Start()
    {
        levelOfSelfAwareness = 0f;
        LOSA.text = "LOSA: " + levelOfSelfAwareness;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLOSA(int positiveOrNegative)
    {
        if(positiveOrNegative == 1)
        {
            levelOfSelfAwareness += 10f;
            
        }
        else
        {
            if(levelOfSelfAwareness != 0f)
            {
                levelOfSelfAwareness -= 10f;
            }
        }
        LOSA.text = "LOSA: " + levelOfSelfAwareness;
    }

    public float GetLOSA()
    {
        return levelOfSelfAwareness;
    }

}
