using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEV : MonoBehaviour
{
    public static DEV instance;
    public bool isTEST;

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
        }
    }
}
