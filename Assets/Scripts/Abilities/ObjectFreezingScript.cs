using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FreezingEvent : UnityEvent<Vector3> {}

public class ObjectFreezingScript : MonoBehaviour
{

    public FreezingEvent onObjectFreeze;

    // Start is called before the first frame update
    void Start()
    {
        if (onObjectFreeze == null) onObjectFreeze = new FreezingEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
