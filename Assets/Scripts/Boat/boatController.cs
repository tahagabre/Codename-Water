using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatController : MonoBehaviour

{
    //cool-down for speed boost
    public bool cooldown = false;
    public int cooldownTime = 30;

    //visible Properties
    public Transform Motor;
    public float SteerPower = 100f;
    public float Power;
    public float MaxSpeed;
    public float Drag = 0.1f;


    //used Components
    public Rigidbody Rigidbody;
    private GameObject Sail;
    private float SailSpeedModifier;
    protected Quaternion StartRotation;
    protected Camera Camera;

    //internal Properties
    protected Vector3 CamVel;
    private float OriginalPower;
    private float OriginalSpeed;

    //public void Update()
    //{
    //    The variable which modifies how much speed is added to the boat, FROM the sail
    //     SailSpeedModifier = ;
    //}

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Sail = GameObject.Find("Sail");
        StartRotation = Motor.localRotation;
        Camera = Camera.main;
        OriginalSpeed = MaxSpeed;
        OriginalPower = Power;
    }

    public static float SailSpeedMod()
    {
        float SailSpeedModifier = GameObject.Find("Sail").GetComponent<SailSpeed>().sailSpeed;
        return SailSpeedModifier;
    }

    public static void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        if (force == 0 || velocity.magnitude == 0)
            return;

        float speedMod = SailSpeedMod();

        velocity = velocity + velocity.normalized * 0.2f * rigidbody.drag;

        force = Mathf.Clamp(force * speedMod, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }

    public void FixedUpdate()
    {
        //default direction
        var forceDirection = transform.forward;
        var steer = 0;

        //steer direction [-1,0,1]
        if (Input.GetKey(KeyCode.A))
            steer = 1;
        if (Input.GetKey(KeyCode.D))
            steer = -1;


        //Rotational Force
        // / 135f below w SteerPower
        Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower/10, Motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        var backward = Vector3.Scale(new Vector3(-1, 0, -1), transform.forward);
        var targetVel = Vector3.zero;

        //forward/backward power
        if (Input.GetKey(KeyCode.W))
        {
            //print("move forward");
            ApplyForceToReachVelocity(Rigidbody, forward * MaxSpeed, Power);
        }

        if (cooldown == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                MaxSpeed *= 1.65f;
                Power *= 1.5f;
                StartCoroutine(boostPeriod());
                cooldown = true;
                StartCoroutine(cooldownPeriod());
            }
        }


        var movingForward = Vector3.Cross(transform.forward, Rigidbody.velocity).y < 0;
        Rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * Rigidbody.velocity;

        if (Input.GetKey(KeyCode.S))
            ApplyForceToReachVelocity(Rigidbody, backward * (MaxSpeed * 0.75f), Power);
        Rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.down) * Rigidbody.velocity;
    }

    IEnumerator cooldownPeriod ()
    {
        yield return new WaitForSeconds(cooldownTime);
        revertCooldown();
       
    }

    IEnumerator boostPeriod ()
    {
        yield return new WaitForSeconds(cooldownTime/5);
        revertSpeed();
    }

    void revertCooldown ()
    {
        cooldown = false;
    }

    void revertSpeed ()
    {
    Power = 150f;
    MaxSpeed = 150f;
}

}