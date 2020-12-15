using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Cinemachine;


public class FishingMinigame : MonoBehaviour
{
    private bool _isPlayerFishing;
    [SerializeField] private float reelSpeed = 5f;
    [SerializeField] private GameObject player;
    [SerializeField] private BobberController bobber;
    private Rigidbody _bobberRb;
    [SerializeField] private float radiusDecay = 0.01f;
    [SerializeField] private int fishTimer = 30;
    [SerializeField] private int failTimer = 3;
    private float outsideCircleTimer;
    private GameObject _fish;
    private int circleIndex = 64;
    private LineRenderer lineRenderer;
    private Camera cam;
    private int clickForce = 100;
    [SerializeField] private GameTimer gameTimer;
    
    [Range(0.1f, 100f)]
    public float radius = 7f;

    private float scaledRadius = 5f;
    private float _scaledCurrentRadius;
    private float _scaledRadiusDecay = 0.01f;

    [Range(3, 256)]
    public int numSegments = 128;
    
    // Mouse Control Variables
    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;

    Vector3 dist;
    Vector3 startPos;
    float posX;
    float posZ;
    float posY;
    private bool dragging;
    void OnEnable()
    {
        print("Minigame Start");
        gameTimer.enabled = true;
        gameTimer.SetTimer(fishTimer);
        gameTimer.currentTime = fishTimer;
        gameTimer.SetFailTimer(failTimer);

        lineRenderer = GetComponent<LineRenderer>();
        transform.position = bobber.transform.position;
        scaledRadius = radius / transform.parent.localScale.x;
        _scaledCurrentRadius = scaledRadius;
        
        CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
        cam = brain.OutputCamera;
        
        _scaledRadiusDecay /= fishTimer / radiusDecay / transform.parent.localScale.x;

        InitRenderer();
        InvokeRepeating("CountTimer", 0f, 1f);
    }

    private void Update()
    {
        if (_scaledCurrentRadius >= 1 / transform.parent.localScale.x )
        {
            _scaledCurrentRadius -= _scaledRadiusDecay;
        }
        else
        {
            _scaledCurrentRadius = 1 / transform.parent.localScale.x;
        }
        DrawRenderer(_scaledCurrentRadius);
        if (Input.GetMouseButtonDown(1))
        {
            dragging = true;
            startPos = transform.position;
            dist = cam.WorldToScreenPoint(transform.position);
            posX = Input.mousePosition.x - dist.x;
            posY = Input.mousePosition.y - dist.y;
            posZ = Input.mousePosition.z - dist.z;
            
        }
        else if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
        }
        
        if (dragging)
        {
            float disX = Input.mousePosition.x - posX;
            float disY = Input.mousePosition.y - posY;
            float disZ = Input.mousePosition.z - posZ;
            Vector3 lastPos = cam.ScreenToWorldPoint(new Vector3(disX, disY, disZ));
            transform.position = new Vector3(lastPos.x, startPos.y, lastPos.z);
        }

        if (IsBobberOutsideCircle())
        {
            //lineRenderer.startColor = Color.red;
            outsideCircleTimer += Time.deltaTime;
            if (outsideCircleTimer > failTimer || gameTimer.GetTimer() <= 0)
            {
                CompleteMinigame(false);
            }
        }
        else
        {
            //lineRenderer.startColor = Color.black;
            outsideCircleTimer = 0f;
            if (gameTimer.GetTimer() <= 0)
            {
                CompleteMinigame(true);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var circleTarget = getPointOnRenderer(circleIndex);
        var bobberPosition = bobber.transform.position;

        bobberPosition = Vector3.MoveTowards(bobberPosition, new Vector3(circleTarget.x, transform.position.y, circleTarget.z), 0.5f * Time.fixedDeltaTime);
        bobber.transform.position = bobberPosition;
    }

    public void SetFish(GameObject fish)
    {
        _fish = fish;
    }

    public GameObject GetFish()
    {
        return _fish;
    }
    
    
    public void InitRenderer()
    {
        //lineRenderer.material = GetComponent<Material>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;
        DrawRenderer(scaledRadius);
    }
    
    public void DrawRenderer(float radiusRenderer)
    {
        var deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
        var theta = 0f;

        for (int i = 0 ; i < numSegments + 1 ; i++) {
            float x = radiusRenderer * Mathf.Cos(theta);
            float z = radiusRenderer * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 50, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
    
    public void ClearRenderer(){
        lineRenderer.positionCount = 0;
    }

    public void CountTimer()
    {
        gameTimer.Decrement();
    }
    

    public Vector3 getPointOnRenderer(int index)
    {
        if (index < lineRenderer.positionCount)
            return lineRenderer.GetPosition(index);
        return Vector3.zero;
    }

    public bool IsBobberOutsideCircle()
    {
        return (Mathf.Pow((bobber.transform.position.x - transform.position.x) / transform.parent.localScale.x,2) + Mathf.Pow((bobber.transform.position.z - transform.position.z) / transform.parent.localScale.x,2 )) > Mathf.Pow(_scaledCurrentRadius, 2);
    }

    public void CompleteMinigame(bool minigameSuccess)
    {
        CancelInvoke();
        ClearRenderer();
        bobber.SetMinigameSuccess(minigameSuccess);
        bobber.reelBobber();
        gameTimer.ResetTimer();
        gameTimer.enabled = false;
        enabled = false;
    }
}
