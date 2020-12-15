using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScript : MonoBehaviour
{

    protected bool generating;
    protected bool melting;


    // Start is called before the first frame update
    void Start()
    {
        generating = true;
        melting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x > 5) generating = false;
        if (transform.localScale.x < .5f && !generating) Destroy(gameObject);
        if (generating) transform.localScale += new Vector3(0.01f, 0, 0.01f);
        if (melting) transform.localScale -= new Vector3(0.01f, 0, 0.01f);
    }

    public void melt( Vector3 hitCoordinates )
    {
        melting = true;
        generating = false;
    }

    public void freeze( Vector3 hitCoordinates)
    {
        melting = false;
    }
}
