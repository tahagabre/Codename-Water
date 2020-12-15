using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PushPullController : MonoBehaviour
{
    private Camera cam;
    private Rigidbody rigidbody;
    private TrailRenderer trailRenderer;

    [SerializeField] private float maxDistanceStep = 1f;
    [SerializeField] private float speed;
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private float pullDistance = 3f;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject pushPull = Instantiate(planePrefab, transform.position + (cam.transform.forward.normalized * pullDistance), transform.rotation);
            rigidbody = pushPull.GetComponent<Rigidbody>();
            trailRenderer = pushPull.GetComponent<TrailRenderer>();

            Vector3 force = Vector3.MoveTowards(pushPull.transform.position, transform.position - (cam.transform.forward.normalized * speed), maxDistanceStep);
            trailRenderer.enabled = true;
            rigidbody.AddForce(force);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject pushPull = Instantiate(planePrefab, transform.position, transform.rotation);
            rigidbody = pushPull.GetComponent<Rigidbody>();
            trailRenderer = pushPull.GetComponent<TrailRenderer>();

            Vector3 force = Vector3.MoveTowards(pushPull.transform.position, cam.transform.forward.normalized * speed, maxDistanceStep);
            trailRenderer.enabled = true;
            rigidbody.AddForce(force);
        }
    }
}
