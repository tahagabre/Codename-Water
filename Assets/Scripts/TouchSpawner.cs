using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSpawner : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] private GameObject summonPoint;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Vector3 touchPos = summonPoint.transform.position;
            Instantiate(prefab, touchPos, Quaternion.identity);
        }
    }
}
