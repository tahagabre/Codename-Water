using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailRotate : MonoBehaviour
{
    public Transform customPivot;

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            transform.RotateAround(customPivot.position, Vector3.up, -20 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.U))
        {
            transform.RotateAround(customPivot.position, Vector3.up, 20 * Time.deltaTime);
        }
    }
}
