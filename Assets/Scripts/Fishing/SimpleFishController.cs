using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleFishController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private bool reverseMove = false;
    [SerializeField] private Transform objectToUse;
    [SerializeField] private BobberController bobber;
    [SerializeField] private float timeToWait;
    private float startTime;
    private float journeyLength;
    private float distCovered;
    private float fracJourney;
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(pointA.transform.TransformPoint(pointA.transform.localPosition), pointB.transform.localPosition);
        bobber = GameObject.FindObjectOfType<BobberController>();
    }

    // Update is called once per frame
    void Update()
    {
        distCovered = (Time.time - startTime) * moveSpeed;
        fracJourney = distCovered / journeyLength;
        if (!bobber.isBobberCast)
        {
            
            if (reverseMove)
            {
                objectToUse.position = Vector3.Lerp(pointB.transform.localPosition, pointA.transform.localPosition, fracJourney);
            }
            else
            {
                objectToUse.position = Vector3.Lerp(pointA.transform.localPosition, pointB.transform.localPosition, fracJourney);
            }
            if (Mathf.Approximately(Vector3.Distance(objectToUse.localPosition, pointB.transform.localPosition), 0f) || Mathf.Approximately(Vector3.Distance(objectToUse.localPosition, pointA.transform.localPosition), 0f)) //Checks if the object has travelled to one of the points
            {
                if (reverseMove)
                {
                    reverseMove = false;
                }
                else
                {
                    reverseMove = true;
                }
                startTime = Time.time;
            }
        }
        else
        {
            objectToUse.position = Vector3.MoveTowards(transform.position, bobber.transform.position, moveSpeed * Time.deltaTime);
            if (Mathf.Approximately(Vector3.Distance(objectToUse.position, bobber.transform.position), 0f))
            {
                bobber.setIsFishHooked(true);
                //objectToUse.transform.parent = bobber.transform;
            }
        }
    }
}
