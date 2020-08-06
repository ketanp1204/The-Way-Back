using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoemManager : MonoBehaviour
{
    // Singleton
    public static PoemManager instance;

    // Local parameters
    private Button poemNextButton;
    private Button poemCloseButton;
    private WaitForUIButtons buttonWait;

    // Cached references
    public Button poemNextButtonPrefab;         // Reference to the poem next button prefab
    public Button poemCloseButtonPrefab;        // Reference to the poem close button prefab
    private CanvasGroup canvasGroup;            // Reference to the CanvasGroup component of the gameObject
    private Animator animator;                  // Reference to the Animator component of the gameObject
    private TextMeshProUGUI poemText;           // Reference to the TextMeshPro child of the poem gameObject


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        poemText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public static void ShowPoem(string text, float delay)
    {
        instance.StartCoroutine(instance.StartTyping(text, delay));
    }

    private IEnumerator StartTyping(string text, float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.enabled = true;
        // animator.Play("Base Layer.PoemPage");
        StartCoroutine(PoemDisplay(text));
    }

    private IEnumerator PoemDisplay(string text)
    {
        int pageIndex = 1;

        poemText.overflowMode = TextOverflowModes.Page;
        poemText.text = text;
        poemText.ForceMeshUpdate();

        while (pageIndex < poemText.textInfo.pageCount)
        {
            poemText.pageToDisplay = pageIndex;

            pageIndex += 1;

            poemNextButton = Instantiate(poemNextButtonPrefab, transform);
            poemNextButton.gameObject.SetActive(true);

            buttonWait = new WaitForUIButtons(poemNextButton);
            yield return buttonWait;

            Destroy(poemNextButton.gameObject);
        }
        if (pageIndex == poemText.textInfo.pageCount)
        {
            poemText.pageToDisplay = pageIndex;

            poemCloseButton = Instantiate(poemCloseButtonPrefab, transform);
            poemCloseButton.gameObject.SetActive(true);

            buttonWait = new WaitForUIButtons(poemCloseButton);
            yield return buttonWait;

            OptionsManager.ShowResponseAfterPoem();

            Destroy(poemCloseButton);
            animator.Play("Base Layer.PoemPageClose");
            yield return new WaitForSeconds(0.5f);
            animator.enabled = false;
        }
    }
}
