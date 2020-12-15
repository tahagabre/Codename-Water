using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBridge : MonoBehaviour {

    [SerializeField] private GameObject bridge;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (bridge != null)
            {
                Destroy(bridge);
            }
        }
    }
}
