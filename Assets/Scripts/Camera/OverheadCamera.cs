using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OverheadCamera : MonoBehaviour
{
    private bool changedPos;
    [SerializeField] private GameObject overheadCamera;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private SimpleSampleCharacterControl character;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveOverheadCamera();
    }
    private void MoveOverheadCamera()
    {
        if (character.state == SimpleSampleCharacterControl.State.MAP)
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            if (changedPos == false)
            {
                overheadCamera.GetComponent<CinemachineVirtualCamera>().LookAt = null;
                overheadCamera.GetComponent<CinemachineVirtualCamera>().Follow = null;
                changedPos = true;
            }
            overheadCamera.transform.localPosition += new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Z))
            {
                overheadCamera.transform.localPosition += new Vector3(0f, -1f, 0f) * zoomSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.X))
            {
                overheadCamera.transform.localPosition += new Vector3(0f, 1f, 0f) * zoomSpeed * Time.deltaTime;
            }
        }
        else
        {
            overheadCamera.GetComponent<CinemachineVirtualCamera>().LookAt = character.transform;
            overheadCamera.GetComponent<CinemachineVirtualCamera>().Follow = character.transform;
            changedPos = false;
        }
    }
}
