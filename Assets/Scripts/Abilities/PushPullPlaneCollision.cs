using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullPlaneCollision : MonoBehaviour
{
    [SerializeField] private float collisionDeathTimer = 2.0f;
    [SerializeField] private float existenceTime = 3000f;
    private float counter;

    private void Update()
    {
        counter += Time.fixedDeltaTime;

        if (counter > existenceTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision _)
    {
        Destroy(gameObject, collisionDeathTimer);
    }
}
