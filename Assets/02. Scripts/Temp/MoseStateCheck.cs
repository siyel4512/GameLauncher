using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoseStateCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("ÁÂ Å¬¸¯");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("ÈÙ Å¬¸¯");
        }
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("¿ì Å¬¸¯");
        }
    }
}
