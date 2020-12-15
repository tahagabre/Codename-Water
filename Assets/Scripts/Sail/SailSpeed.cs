using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailSpeed : MonoBehaviour
{
    public float sailSpeed;

    [SerializeField] private float fast = 10.0f;
    [SerializeField] private float regular = 1.00f;
    [SerializeField] private float slow = 0.5f;
    private enum sailPos {Up, Down};
    private sailPos sailState;
    public Vector3 windDir;
    [SerializeField] private Vector3 scaleChange;
    WaitForSeconds angleUpdateDelay = new WaitForSeconds(2);

    private GameObject SailOBJ;
    void Start()
    {
        sailState = sailPos.Down;
        SailOBJ = this.gameObject;
        scaleChange = new Vector3(0.0f, 250.0f, 0.0f);
        sailUp();
    }

    // Update is called once per frame
    void Update()
    {
        windDir = GameObject.Find("windArea").GetComponent<WindArea>().WindDirection;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown("1")) {
            if(sailState == sailPos.Down)
                sailUp();
            }
        if(Input.GetKeyDown("2")) {
            if(sailState == sailPos.Up)
                sailDown();
            }
    }

    private void updateSailSpeed()
    {
        float angle = Vector3.Angle(windDir, SailOBJ.transform.parent.eulerAngles );
            //If you're sailing 'with the wind'
            if(angle < 90) {
                Debug.Log("You're sailing with the wind! Speed is fast!");
                sailSpeed = fast;
            }
            //If you're sailing 'against the wind'
            else if(angle > 90) {
                Debug.Log("You're sailing against the wind! Speed is slow!");
                sailSpeed = slow;
            }
            else {
                Debug.Log("You're sailing normally with the wind");
            }
            Debug.Log(angle);
    }

    private void sailUp()
    {
        StopAllCoroutines();
        //StopCoroutine(shipAngleCoroutine());
        sailState = sailPos.Up;
        Debug.Log("Sail is up! Speed is regular");
        gameObject.transform.localScale -= scaleChange;
        sailSpeed = regular;
        Debug.Log(sailState);
    }
    private void sailDown()
    {
        StartCoroutine(shipAngleCoroutine());
        sailState = sailPos.Down;
        Debug.Log("Sail is down.");
        gameObject.transform.localScale += scaleChange;
        Debug.Log(sailState);
    }

    IEnumerator shipAngleCoroutine()
    {
        Debug.Log("POOP");
        int num = 0;
        while(num>=0)
        {
            updateSailSpeed();
            yield return angleUpdateDelay; //Wait for 2 seconds
        }
    }

}
