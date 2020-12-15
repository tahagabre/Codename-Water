using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MeltingEvent : UnityEvent<Vector3> {}

public class ObjectMeltingScript : MonoBehaviour
{


    public MeltingEvent onObjectMelt;

    // Start is called before the first frame update
    void Start()
    {
        if (onObjectMelt == null) onObjectMelt = new MeltingEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
