using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torchscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void heat( Vector3 pos )
    {
        transform.GetComponent<ParticleSystem>().Play();
    }

}
