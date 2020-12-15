using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public float strength;
    private GameObject curvedWind; //The particle effect
    private GameObject straightWind; //The particle effect
    private int angleInt = 0;
    public Vector3 ForceDirection;
    public Vector3 WindDirection;

    public void Start()
    {
        Debug.Log(this);
        straightWind = this.transform.GetChild(0).gameObject;
        curvedWind = this.transform.GetChild(1).gameObject;
        Debug.Log(straightWind);
        Debug.Log(curvedWind);
    }
    public void Update()
    {
        if(Input.GetKeyDown("right"))
        {
            angleInt +=1;
            straightWind.transform.Rotate(0.0f, 45.0f, 0.0f, Space.Self);
            curvedWind.transform.Rotate(0.0f, 45.0f, 0.0f, Space.Self);
            windAngle();
        }
        if(Input.GetKeyDown("left"))
        {
            angleInt -=1;
            straightWind.transform.Rotate(0.0f, -45.0f, 0.0f, Space.Self);
            curvedWind.transform.Rotate(0.0f, -45.0f, 0.0f, Space.Self);
            windAngle();
        }
    }

    void OnTriggerStay(Collider col)
    {
        Rigidbody colRigidbody = col.GetComponent<Rigidbody>();
        if (colRigidbody != null)
        {
            colRigidbody.AddForce(ForceDirection * strength);
        }
    }

    public void windAngle()
    {
        if(angleInt == 8) {
            // num = 0;
            angleInt = 0;
        }
        if(angleInt == -1) {
            // num = 7;
            angleInt = 7;
        }
        switch(angleInt)
        {
            case 0:
                ForceDirection = new Vector3(0,0,90);
                WindDirection = new Vector3(0,0,90);
                break;
            case 1:
                ForceDirection = new Vector3(90,0,90);
                WindDirection = new Vector3(90,0,90);
                break;
            case 2:
                ForceDirection = new Vector3(90,0,0);
                WindDirection = new Vector3(90,0,0);
                break;
            case 3:
                ForceDirection = new Vector3(90,0,-90);
                WindDirection = new Vector3(90,0,-90);
                break;
            case 4:
                ForceDirection = new Vector3(0,0,-90);
                WindDirection = new Vector3(0,0,-90);
                break;
            case 5:
                ForceDirection = new Vector3(-90,0,-90);
                WindDirection = new Vector3(-90,0,-90);
                break;
            case 6:
                ForceDirection = new Vector3(-90,0,0);
                WindDirection = new Vector3(-90,0,0);
                break;
            case 7:
                ForceDirection = new Vector3(-90,0,90);
                WindDirection = new Vector3(-90,0,90);
                break;
        }
    }
}
