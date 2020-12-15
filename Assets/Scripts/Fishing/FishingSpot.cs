using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour
{
    private Vector3 Min;
    private  Vector3 Max;
    private  float _xAxis;
    private  float _yAxis;
    private  float _zAxis; //If you need this, use it
    private Vector3 _randomPosition;
    public GameObject fish;
    private Collider fishingSpotCollider;

    private float lastSpawnTimeS = -1;
    public int maxSpawn = 5;
    public int numSpawned = 0;
    public float spawnDelayS = 5f;
    private bool isColliding;
    private void Start()
    {
        fishingSpotCollider = GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update () {
        if (isColliding)
        {
            if (numSpawned < maxSpawn)
            {
                if (lastSpawnTimeS < 0) {
                    lastSpawnTimeS = Time.time;
                    print ("spawn timer fire");
                    GameObject spawned = Instantiate(fish, transform.position, Quaternion.identity) as GameObject;
                    spawned.transform.parent = transform;
                    //Vector3 pos = new Vector3(Random.Range(-maxSpawnPos.x, maxSpawnPos.x), Random.Range(-maxSpawnPos.y, maxSpawnPos.y), 0);
                    //spawned.transform.localPosition = pos;
                    numSpawned += 1;
                } else if (lastSpawnTimeS >= 0 && Time.time - lastSpawnTimeS > spawnDelayS) {
                    lastSpawnTimeS = -1;
                }
            }
        }
        
    }
    
    public Color GizmosColor = new Color(1.0f, 0f, 0f, 0.5f);

    void OnDrawGizmos()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(fishingSpotCollider.bounds.center, fishingSpotCollider.bounds.size);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Bobber")
        {
            isColliding = true;
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Bobber")
        {
            isColliding = false;
        }
    }
}
