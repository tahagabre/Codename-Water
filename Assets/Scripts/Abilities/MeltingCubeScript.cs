using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltingCubeScript : MonoBehaviour
{

    protected bool melting;

    // Start is called before the first frame update
    void Start()
    {
        melting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (melting) transform.localScale -= new Vector3(0, 0.05f, 0);
        if (transform.localScale.y < 0.5f) Destroy(gameObject);
    }

    public void melt( Vector3 hitCoordinates )
    {
        melting = true;
        transform.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
    }

    public void freeze( Vector3 hitCoordinates)
    {
        melting = false;
        transform.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
    }
}
