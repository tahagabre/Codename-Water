using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishable : MonoBehaviour
{
    [SerializeField] private Transform objectToUse;
    [SerializeField] private BobberController bobber;
    [SerializeField] private SimpleSampleCharacterControl character;
    [SerializeField] private float smoothSpeed = .3f;
    [SerializeField] private float speedMultiplier = .5f;

    private bool _isFished;
    private Collider _collider;

    //[SerializeField] private float _distanceToFish;
    // Start is called before the first frame update
    void Start()
    {
        bobber = FindObjectOfType<BobberController>();
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bobber.isBobberCast)
        {
            _isFished = false;
        }

        if (_isFished)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position = Vector3.Lerp(transform.position, (transform.position + character.transform.forward * speedMultiplier), smoothSpeed);
            }

            else if (Input.GetKey(KeyCode.A))
            {
                transform.position = Vector3.Lerp(transform.position, (transform.position - character.transform.right * speedMultiplier) , smoothSpeed);
            }

            else if (Input.GetKey(KeyCode.S))
            {
                transform.position = Vector3.Lerp(transform.position, (transform.position - character.transform.forward * speedMultiplier), smoothSpeed);
            }

            else if (Input.GetKey(KeyCode.D))
            {
                transform.position = Vector3.Lerp(transform.position, (transform.position + character.transform.right * speedMultiplier), smoothSpeed);
            }
        }
    }

    public void setIsFished(bool isFished)
    {
        _isFished = isFished;
    }

    public bool getIsFished()
    {
        return _isFished;
    }

    public void fishObject()
    {
        if (gameObject.CompareTag("FloatingObject")) {
            bobber.setIsFishHooked(true);
            objectToUse.transform.parent = bobber.transform;
            _collider.enabled = false;
            transform.localPosition = Vector3.zero;
        }

        if (gameObject.CompareTag("ChallengeObject")) {
            //print("hit challenge object");
            bobber.transform.position = transform.position;
            bobber.floater.enabled = false;

            _isFished = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("ChallengeObject") && collision.gameObject.CompareTag("Bobber"))
        {
            print("hit challenge object");
            bobber.transform.position = transform.position;
            bobber.floater.enabled = false;

            _isFished = true;
        }

        if (gameObject.CompareTag("Boulder") && collision.gameObject.CompareTag("Bobber"))
        {
            print("hit challenge object");
            bobber.transform.position = transform.position;
            bobber.floater.enabled = false;

            _isFished = true;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (gameObject.CompareTag("ChallengeObject") && collision.gameObject.CompareTag("Bobber"))
        {
            print("exit challenge object");
            bobber.floater.enabled = true;

            _isFished = false;
        }
    }

}
