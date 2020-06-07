using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectManager : MonoBehaviour
{
    // Configuration parameters
    [HideInInspector]
    public Vector3 mousePosition;
    [HideInInspector]
    public Vector3 mousePositionWorld;
    [HideInInspector]
    public Vector2 mousePositionWorld2D;
    private RaycastHit2D hit;

    // Cached References
    public Camera mainCamera;
    public OptionsManager optionsManager;
    public LevelChanger levelChanger;

    // Start is called before the first frame update
    void Start()
    {
        optionsManager = FindObjectOfType<OptionsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // Get 3D mouse position in Screen Space Coordinates
            mousePosition = Input.mousePosition;

            // Convert mouse position to 3D World Coordinates
            mousePositionWorld = mainCamera.ScreenToWorldPoint(mousePosition);

            // Convert mouse position to 2D world coordinates
            mousePositionWorld2D = new Vector2(mousePositionWorld.x, mousePositionWorld.y);

            // Raycast2D : public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance (length of ray), int layerMask (to exclude certain objects)
            hit = Physics2D.Raycast(mousePositionWorld2D, Vector2.zero);

            // Test whether ray hits any collider
            if(hit.collider != null)
            {


                if (hit.collider.gameObject.tag == "Object")
                {
                    if (!optionsManager.transform.Find("Description Box").gameObject.activeSelf)
                    {
                        if (hit.collider.gameObject.GetComponent<ObjectProperties>().interactedWith == false)
                        {
                            hit.collider.gameObject.GetComponent<ObjectProperties>().interactedWith = true;
                            Debug.Log("Object Hit: " + hit.collider.gameObject.name);
                            optionsManager.selectedObject = hit.collider.gameObject;
                            optionsManager.InitializeResponse();
                        }
                    }
                }

                if (hit.collider.gameObject.tag == "LOSAResponseObject")
                {
                    if (!optionsManager.transform.Find("Description Box").gameObject.activeSelf)
                    {
                        optionsManager.selectedObject = hit.collider.gameObject;
                        optionsManager.InitializeResponse();
                    }
                }

                if (hit.collider.gameObject.tag == "PrevLevel")
                {
                    levelChanger.LoadPreviousLevel();
                }
                
                if(hit.collider.gameObject.tag == "NextLevel")
                {
                    levelChanger.LoadNextLevel();
                }
            }
        }
    }
}
