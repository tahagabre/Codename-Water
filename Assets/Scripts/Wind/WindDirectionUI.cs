using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class WindDirectionUI : MonoBehaviour
{
    
    public Transform wind;
    private Vector3 windDir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        windDir  = GameObject.Find("windArea").GetComponent<WindArea>().WindDirection; 
        Debug.Log(windDir);

        float angle = Mathf.Rad2Deg * Mathf.Atan(windDir.z / windDir.x);
        if (windDir.x < 0) angle += 180;
        wind.rotation = Quaternion.Euler(0, 0, angle);

    }
}
