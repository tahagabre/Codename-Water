using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class ActiveCameraManger : MonoBehaviour
{

    [SerializeField] private GameObject thirdPersonCamera;
    [SerializeField] private GameObject boatCamera;
    [SerializeField] private GameObject overheadCamera;
    [SerializeField] private GameObject aimThrowCamera;
    private List<GameObject> cameras = new List<GameObject>();
    private CinemachineBrain cinemachineBrain;
    
    // TODO: Make class that handles state as opposed to passing entire character class
    [SerializeField] private SimpleSampleCharacterControl character;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        cameras.Add(thirdPersonCamera);
        cameras.Add(boatCamera);
        cameras.Add(overheadCamera);
        cameras.Add(aimThrowCamera);

        ActivateCamera(thirdPersonCamera);
    }

    // Update is called once per frame
    void Update()
    {
        // Should read what the current state is, so it knows to return to that
        switch(character.state)
        {
            case SimpleSampleCharacterControl.State.LAND:
                ActivateCamera(thirdPersonCamera);
                break;
            case SimpleSampleCharacterControl.State.BOAT:
                ActivateCamera(boatCamera);
                break;
            case SimpleSampleCharacterControl.State.FISHING:
                ActivateCamera(aimThrowCamera);
                break;
            case SimpleSampleCharacterControl.State.MAP:
                ActivateCamera(overheadCamera);
                break;
        }
    }

    private void ActivateCamera(GameObject cam)
    {
        if (cam.GetComponent<ICinemachineCamera>() == cinemachineBrain.ActiveVirtualCamera) { return; } // Doesn't work, rhs is always cinemachinebrain
        cam.SetActive(true);

        var deactivate = from camera in cameras
                         where camera.tag != cam.tag
                         select camera;
        foreach(GameObject nonactive in deactivate) { nonactive.SetActive(false); }
    }
}
