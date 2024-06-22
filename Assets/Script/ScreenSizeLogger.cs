using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeLogger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Screen Width: " + Screen.width);
        Debug.Log("Screen Height: " + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
