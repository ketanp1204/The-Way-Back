using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayName : MonoBehaviour
{

    public GameObject objectNamePanel;
    
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        // Get object attributes
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        viewportPosition = new Vector3(viewportPosition.x, viewportPosition.y - 0.1f);

        OpenPanel(viewportPosition);
    }
    public void OpenPanel(Vector3 position)
    {
        if (objectNamePanel != null)
        {
            objectNamePanel.SetActive(true);
            objectNamePanel.GetComponentInChildren<TextMeshProUGUI>().text = gameObject.name;
            objectNamePanel.GetComponent<RectTransform>().anchorMin = position;
            objectNamePanel.GetComponent<RectTransform>().anchorMax = position;
            objectNamePanel.GetComponentInChildren<TextMeshProUGUI>().text = name;
        }
    }

    void OnMouseExit()
    {
        objectNamePanel.SetActive(false);
    }
}
