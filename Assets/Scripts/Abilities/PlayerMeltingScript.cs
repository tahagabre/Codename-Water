using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeltingScript : MonoBehaviour
{
    public Transform PlayerCamera;

    public float abilityRange;
    public float abilityCooldown;
    protected float cooldownTimer;

    public float minDist;

    public bool debugRay;

    void Start() {cooldownTimer = 0;}
    void Update() {
        cooldownTimer -= Time.deltaTime;
        if ( Input.GetKeyDown(KeyCode.M) ) UseMelt();
    }

    public void UseMelt() {
        Vector3 raycastStart = PlayerCamera.position;
        Vector3 raycastDirection = PlayerCamera.forward;
        
        if (cooldownTimer <= 0f ) {
            cooldownTimer = abilityCooldown;
            if (debugRay) Debug.DrawRay( raycastStart + minDist * raycastDirection, Vector3.Normalize(raycastDirection) * abilityRange, Color.red, 0.5f );
            RaycastHit hit;
            if( Physics.Raycast( raycastStart + minDist * raycastDirection, raycastDirection, out hit, abilityRange) ) 
            {
                ObjectMeltingScript target = hit.transform.GetComponent<ObjectMeltingScript>();
                if (target != null) target.onObjectMelt.Invoke( hit.point );
            }
        }

    }

}
