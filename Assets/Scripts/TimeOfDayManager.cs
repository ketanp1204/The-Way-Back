using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayManager : MonoBehaviour
{
    // State variables
    private float fadeTime = 10f;

    // Cached references
    public Sprite morningImage;
    public Sprite nightImage;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionIntoNight()
    {

    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        Color c = spriteRenderer.color;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForSeconds(10f);
            elapsedTime += Time.deltaTime;
            c.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            spriteRenderer.color = c;
        }
    }
}
