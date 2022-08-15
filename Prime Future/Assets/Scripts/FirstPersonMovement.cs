using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public bool hasControl = true;

    [Header("Component References")]
    public Camera playerCam;

    [SerializeField]
    private PlayerAttack attack;
    private Rigidbody rb;
    private CapsuleCollider capsule;

    [Header("Movement Variables")]
    public float maxSpeed = 10f;
    public float airAccel = 100f;
    public float jumpHeight = 2f;
    public float gravity = -25f;
    public float airDragScalar = .2f;

    [Header("Ground Check Variables")]
    public bool isGrounded;
    public readonly float groundDistance = .075f;
    public LayerMask groundLayer;

    private Vector3 moveDir;
    private bool jump;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void Start()
    {
        attack = PlayerAttack._instance;
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        if (hasControl)
        {
            transform.rotation = Quaternion.Euler(0, playerCam.transform.rotation.eulerAngles.y, 0);

            if (Input.GetButtonDown("Jump") && isGrounded)
                jump = true;
        }
    }

    private void FixedUpdate()
    {
        //----------Ground Check----------
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down, groundDistance, groundLayer);

        //----------Jump Handling----------
        if (jump)
        {
            jump = false;

            rb.AddForce(Vector3.up * (Mathf.Sqrt(jumpHeight * (-2) * gravity)), ForceMode.Impulse);
        }

        if (isGrounded && rb.velocity.y <= 0)
            rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);

        //----------Ground Movement----------
        if (hasControl)
        {
            // Not using GetAxisRaw here because I'm using the sensitivity and gravity values of the input manager
            // to mimic a physical acceleration curve
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");

            moveDir = (playerCam.transform.right * xMove) + (transform.forward * zMove);

            if (isGrounded)
                rb.velocity = moveDir * maxSpeed;

            if (!isGrounded)
            {
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    rb.velocity = new Vector3(moveDir.x * maxSpeed, rb.velocity.y, moveDir.z * maxSpeed);
            }
            
            // Horizontal movement speed cap
            if ((rb.velocity - new Vector3(0, rb.velocity.y, 0)).magnitude > maxSpeed)
            {
                float vertVelocity = rb.velocity.y;

                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized * maxSpeed;
                rb.velocity += new Vector3(0, vertVelocity, 0);
            }
        }

        //Debug.Log(string.Format("Velocity: {0}", rb.velocity.magnitude));

        //----------Air Drag----------
        if (!isGrounded)
        {
            if (rb.velocity.x < 0)
                rb.AddForce(-rb.velocity * airDragScalar, ForceMode.Acceleration);
            else if (rb.velocity.x > 0)
                rb.AddForce(-rb.velocity * airDragScalar, ForceMode.Acceleration);

            if (rb.velocity.z < 0)
                rb.AddForce(-rb.velocity * airDragScalar, ForceMode.Acceleration);
            else if (rb.velocity.z > 0)
                rb.AddForce(-rb.velocity * airDragScalar, ForceMode.Acceleration);
        }
    }

    private void LateUpdate()
    {
        if (moveDir.magnitude != 0 && attack.SetAccuracy > .5f)
            attack.SetAccuracy = 1 / (1 + (rb.velocity.magnitude / maxSpeed));
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    // Events
    //-------------------------------------------------------------------------
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(transform.position + Vector3.down, groundDistance);
    //}
}
