using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectData : MonoBehaviour
{
    public static PersistentObjectData instance;

    public List<string> interactedObjects;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
