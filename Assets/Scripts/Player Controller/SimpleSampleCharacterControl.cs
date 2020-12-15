using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;

public class SimpleSampleCharacterControl : MonoBehaviour
{
    private enum ControlMode
    {
        /// <summary>
        /// Up moves the character forward, left and right turn the character gradually and down moves the character backwards
        /// </summary>
        Tank,
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// </summary>
        Direct
    }

    [SerializeField] private float m_moveSpeed = 8;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 10;

    [SerializeField] private Rigidbody m_rigidBody = null;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_sprintScale = 2.5f;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;

    private bool m_isGrounded;

    private List<Collider> m_collisions = new List<Collider>();

    public FishingRod fishingRod;
    private boatController boatController;
    [SerializeField] private GameObject boatCharacter;
    public enum State
    {
        LAND,
        BOAT,
        FISHING,
        MAP
    }
    public State state;
    public State previousState;

    //Camera Fade
    public float speedScale = 3f;
    public Color fadeColor = Color.black;
    public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 1),
        new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));
    public bool startFadedOut = false;


    private float alpha = 0f;
    private Texture2D texture;
    private int direction = 0;
    private float time = 0f;

    public void Start()
    {
        if (startFadedOut) alpha = 1f; else alpha = 0f;
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
        texture.Apply();
    }
    //

    private void Awake()
    {
        state = State.LAND;
        boatController = GameObject.FindObjectOfType<boatController>();
        fishingRod = GameObject.FindObjectOfType<FishingRod>();

        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        if (!m_jumpInput && Input.GetKey(KeyCode.Space))
        {
            m_jumpInput = true;
        }

        GetFishingInputs();

        if (Input.GetKeyDown(KeyCode.K))
        {
            print("FIXED UPDATE: K pressed");
            if (state == State.BOAT)
            {
                print("Starting transition to Land");

                previousState = state;
                state = State.LAND;
                ToggleCharacters();
                //print("current: " + state);
                //print("previousState: " + previousState);

                StartCoroutine(FadeOut());
                StartCoroutine(FadeIn());
            }

            else if (state == State.LAND)
            {

                print("Starting transition to Boat");

                previousState = state;
                state = State.BOAT;
                ToggleCharacters();
                //print("current: " + state);
                //print("previousState: " + previousState);

                StartCoroutine(FadeOut());
                StartCoroutine(FadeIn());
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (state == State.MAP)
            {
                state = previousState;
            }

            else
            {
                previousState = state;
                state = State.MAP;
            }
        }

        switch (state)
        {
            case State.LAND:
                boatController.enabled = false;
                switch (m_controlMode)
                {
                    case ControlMode.Direct:
                        DirectUpdate();
                        break;

                    case ControlMode.Tank:
                        TankUpdate();
                        break;

                    default:
                        Debug.LogError("Unsupported state");
                        break;
                }
                break;
            case State.BOAT:
                boatController.enabled = true;
                break;
            case State.FISHING:
                boatController.enabled = false;
                if (!fishingRod.GetIsBobberCast())
                {
                    switch (m_controlMode)
                    {
                        case ControlMode.Direct:
                            DirectUpdate();
                            break;

                        case ControlMode.Tank:
                            TankUpdate();
                            break;

                        default:
                            Debug.LogError("Unsupported state");
                            break;
                    }
                }
                Fish();
                break;
        }
        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }

    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftControl);
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        if (v < 0)
        {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        }
        else if (walk)
        {
            v *= m_walkScale;
        }
        else if (sprint)
        {
            v *= m_sprintScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = transform.forward * m_currentV + transform.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }
    }

    private void GetFishingInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (state == State.FISHING)
            {
                State temp = state;
                state = previousState;
                previousState = temp;
                fishingRod.reelIn();
            }

            else
            {
                print("Entering fishing state");
                previousState = state;
                state = State.FISHING;
            }
        }
    }

    private void Fish()
    {
        if (Input.GetMouseButtonDown(0) && !fishingRod.GetIsBobberCast())
        {
            CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
            Camera camera = brain.OutputCamera;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("water") || hit.collider.gameObject.layer == 11)
                {
                    //print("Hit point: " + hit.point);
                    fishingRod.castRod(hit.point);
                    //print("rod casted");
                }
            }
        }

        //TODO: implement ending minigame when reelIn();
        else if (Input.GetMouseButtonDown(0) && fishingRod.GetIsBobberCast())
        {
            if (!fishingRod.isFish())
                fishingRod.reelIn();
        }
    }

    //Camera Fader
    IEnumerator FadeOut()
    {
        alpha = 0f;
        time = 1f;
        direction = -1;
        yield return null;
    }

    IEnumerator FadeIn()
    {
        alpha = 1f;
        time = 0f;
        direction = 1;
        yield return null;
    }

    public void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        if (direction != 0)
        {
            time += direction * Time.deltaTime * speedScale * 0.09f;
            alpha = Curve.Evaluate(time);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            texture.Apply();
            if (alpha <= 0f || alpha >= 1f) direction = 0;
        }
    }

    // When we transition to boat or land, disable the corresponding character
    private void ToggleCharacters()
    {
        boatCharacter.SetActive(!boatCharacter.activeSelf);

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject childGO = transform.GetChild(i).gameObject;
            childGO.SetActive(!childGO.activeSelf);
        }
    }
}
