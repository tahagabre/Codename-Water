using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionPoint : MonoBehaviour
{
    public GameObject PointOfAttraction;
    [SerializeField] private float magnitude = 5;
    void OnTriggerStay(Collider col)
    {
        Rigidbody colRigidbody = col.GetComponent<Rigidbody>();

        if (colRigidbody != null)
        {
            Vector3 directionDraw = PointOfAttraction.transform.position - colRigidbody.position;
            Vector3 drawForce = directionDraw.normalized * magnitude;
            colRigidbody.AddForce(drawForce);
        }
    }
}
