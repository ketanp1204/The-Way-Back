using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    // Bathroom
    public Sprite B_Day_ShelfOpen;
    public Sprite B_Day_ShelfClosed;
    public Sprite B_Eve_ShelfOpenLightOn;
    public Sprite B_Eve_ShelfOpenLightOff;
    public Sprite B_Eve_ShelfClosedLightOn;
    public Sprite B_Eve_ShelfClosedLightOff;
    public Sprite B_Eve_Light_On;
    public Sprite B_Eve_Light_Off;

    void Awake()
    {
        instance = this;
    }
}
