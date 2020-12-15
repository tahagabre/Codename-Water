using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Component = UnityEngine.Component;

public class BobberController : MonoBehaviour
{
    public Transform resetTransform;

    private Component joint;
    private Rigidbody rigidBody;
    //private HingeJoint hingeJoint;
    //private Rigidbody connectedBody;
    public bool isBobberCast;
    public float speed;
    private bool _isFishHooked;
    public GameObject _fish;
    [SerializeField] private int _fishDistance;
    [SerializeField] public Floater floater;
    [SerializeField] private GameObject fishingMiniGame;
    private FishingMinigame miniGame;
    private Transform _parentObject;
    private bool _miniGameSuccess;
    private Transform _fishParent;
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        floater.enabled = false;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        //hingeJoint = this.GetComponent<HingeJoint>();
        //connectedBody = hingeJoint.connectedBody;
        _isFishHooked = false;
        miniGame = fishingMiniGame.GetComponent<FishingMinigame>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void castBobber(Vector3 mousePosition)
    {
        //hingeJoint.connectedBody = null;
        //Destroy(hingeJoint);
        _parentObject = transform.parent;
        transform.parent = null;
        float step = speed * Time.deltaTime;
        transform.position = mousePosition;
        rigidBody.isKinematic = false;
        floater.enabled = true;
        
        rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        isBobberCast = true;

        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
        
        GetClosestFishable();
    }

    public GameObject reelBobber()
    {
        transform.parent = _parentObject;
        transform.position = resetTransform.position;
        rigidBody.isKinematic = true;
        //hingeJoint = gameObject.AddComponent<HingeJoint>();
        //hingeJoint.connectedBody = connectedBody;
        rigidBody.constraints = RigidbodyConstraints.None;
        isBobberCast = false;
        _isFishHooked = false;
        floater.enabled = false;

        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        if (transform.childCount > 1)
        {
            if (_miniGameSuccess)
            {
                if (_fish.GetComponent<SimpleFishController>() != null)
                {
                    _fish.GetComponent<SimpleFishController>().enabled = false;
                    _fish.transform.localPosition = Vector3.zero;
                }
                else if (_fish.GetComponent<Fishable>() != null)
                {

                    _fish.GetComponent<Fishable>().enabled = false;
                    //Destroy(fish);
                }
                return _fish;
            }
            else
            {
                _fish.transform.parent = _fishParent;
            }
            
        }

        return null;
    }

    public bool GetIsFishHooked()
    {
        return this._isFishHooked;
    }

    public void setIsFishHooked(bool isFishHooked)
    {
        this._isFishHooked = isFishHooked;
    }

    public void GetClosestFishable()
    {
        //TODO: could be improved with overlap sphere for colliders and finding objects with tags from that.
        //TODO: implement weighted collection. see: https://gamedev.stackexchange.com/questions/162976/how-do-i-create-a-weighted-collection-and-then-pick-a-random-element-from-it
        List<GameObject> floatingGameobjects;
        List<GameObject> challengeGameobjects;
        List<GameObject> allFishables;

        floatingGameobjects = GameObject.FindGameObjectsWithTag("FloatingObject").ToList();
        challengeGameobjects = GameObject.FindGameObjectsWithTag("ChallengeObject").ToList();
        floatingGameobjects.AddRange(challengeGameobjects);
        allFishables = floatingGameobjects;
        print("all Fishables:" + allFishables);

        if (allFishables.Count == 0) return;
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        List<GameObject> gosClosest = allFishables.Where(gameObject => Vector3.Distance(transform.position, gameObject.transform.position) < _fishDistance).ToList();
        if (gosClosest.Count == 0) return;
        foreach (GameObject objectClosest in gosClosest)
        {
            Vector3 directionToTarget = objectClosest.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closest = objectClosest;
            }
        }
        _fish = closest.GetComponent<Fishable>().gameObject;
    }

    public void SetMinigameSuccess(bool minigameSuccess)
    {
        _miniGameSuccess = minigameSuccess;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("fish"))
        {
            _fish = other.gameObject;
            _fishParent = other.transform.parent;
            other.transform.parent = transform;
            miniGame.enabled = true;
        }
    }
    
}
