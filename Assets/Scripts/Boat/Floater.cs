using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    public float maxTilt = 45;
    public float maxHeightAboveWaves = 0.5f;

    private Waves waves;

    private void Start()
    {
        waves = FindObjectOfType<Waves>();
    }

    private void FixedUpdate()
    {
        // Prevent boat from flipping over
        float angle = Vector3.Angle(transform.parent.up, Vector3.up);
        if ( angle > maxTilt )
        {
            float yaw = rigidBody.rotation.eulerAngles.y;
            Quaternion aim = Quaternion.LookRotation(new Vector3(Mathf.Cos(yaw), 0, Mathf.Sin(yaw)), Vector3.up);
            rigidBody.MoveRotation(Quaternion.RotateTowards(rigidBody.rotation, aim, angle - maxTilt));
        }


        rigidBody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        float waveHeight = waves.GetHeight(transform.position.x, transform.position.z);
        float middleHeight = waves.GetHeight(rigidBody.position.x, rigidBody.position.z);
        if (rigidBody.position.y - middleHeight > maxHeightAboveWaves)
        {
            rigidBody.position = new Vector3(rigidBody.position.x, middleHeight + maxHeightAboveWaves, rigidBody.position.z);
        }

        if (transform.position.y < waveHeight)
        {

            float displacementMultiplier = Mathf.Clamp01((waveHeight-transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        
    }
}
