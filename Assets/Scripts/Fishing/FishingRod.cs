using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public Transform pivot;
    public GameObject fishingLine;
    private LineRenderer lineRender;
    public BobberController bobber;
    private float startingAngle;
    private bool isRodCast;
    public float angle;
    [SerializeField] private GameObject inventory;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void castRod(Vector3 mousePosition)
    {
        print("Casting Rod...");
        bobber.castBobber(mousePosition);
    }

    public void reelIn()
    {
        print("Reeling in...");
        GameObject fish = bobber.reelBobber();
    }

    public bool GetIsBobberCast()
    {
        return bobber.isBobberCast;
    }

    public bool isFish()
    {
        if (bobber._fish != null)
            return bobber._fish.CompareTag("fish");
        return false;
    }
}