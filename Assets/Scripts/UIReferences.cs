using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIReferences : MonoBehaviour
{
    public GameObject clockText;
    public GameObject poemPage;
    public TextMeshProUGUI poemText;
    public GameObject pauseMenuUI;
    public GameObject optionsBox;
    public GameObject descriptionBox;
    public GameObject objectNameBackground;
    public GameObject objectNameText;
    public GameObject backgroundImage;
    public GameObject rain;
    public GameObject interactableObjects;
    public GameObject closeUpObjects;
    [HideInInspector]
    public GameObject gameSession;

    void Start()
    {
        gameSession = GameObject.Find("GameSession");
    }
}
