using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder_Anim : MonoBehaviour
{
    [SerializeField] private Animator myAnimationCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bobber"))
        {
            myAnimationCollider.SetBool("Buoy", true);
            print("This is working");
        }
    }
}
