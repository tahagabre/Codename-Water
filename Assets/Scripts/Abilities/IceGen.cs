using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGen : MonoBehaviour
{

    public GameObject icePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void freeze( Vector3 coordinates )
    {
        Instantiate( icePrefab, coordinates, Quaternion.identity, transform );
    }

}
