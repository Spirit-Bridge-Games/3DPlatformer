using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float decceleration;
    public float velPower;

    [Space]

    public float frictionAmount;

    [Space]

    [Header("Jump")]
    public float jumpForce;

    [Range(0, 1)]
    public float jumpCutMultiplier;

    [Space]

    public float jumpCoyoteTime;
    public float jumpBufferTime;

    [Space]
    
    public float fallGravityMultiplier;

    [Space]

    public Transform groundCheckPoint;
    public Vector3 groundcheckSize;

    public string groundLayer;

    [Space]

    public Animator animator;

    float lastGroundedTime;
    float lastJumpTime;
    bool isJumping;
    
    float moveInput;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        #region Timer
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        #endregion

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpInput();
        }


        #region Jump
        if (CanJump())
        {
            Jump();
        }
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateAnimator();

        Vector3 movement = Vector3.zero;
        #region Run
        movement.z = Movement("Vertical", rb.velocity.z);
        #endregion

        #region Strafe
        movement.x = Movement("Horizontal", rb.velocity.x);
        #endregion

        #region Friction
        if (moveInput < 0.01f)
        {
            Friction(rb.velocity.z, transform.forward);
            Friction(rb.velocity.x, transform.right);
        }
        #endregion

        rb.AddForce(movement);
    }

    private float Movement(string axis, float vel)
    {
        moveInput = Input.GetAxis(axis);

        //Calculate the direction we want to move in and ouur desired velocity
        float targetSpeed = moveInput * moveSpeed;
        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - vel;
        //Change acceleration rate depending on situation
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        /*Applies acceleration to speed difference, the raises to a set power so acceleration increases
         with higher speeds finally multiplies by sign to reapply direction */
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        
        return movement;
    }

    private void Friction(float vel, Vector3 dir)
    {
        float amount = Mathf.Min(Mathf.Abs(vel), Mathf.Abs(frictionAmount));
        
        amount *= Mathf.Sign(vel);

        rb.AddForce(dir * -amount, ForceMode.Impulse);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastGroundedTime = 0;
        lastJumpTime = 0;
        isJumping = true;
        //jumpInputReleased = false;
    }

    private void OnJumpInput()
    {
        lastJumpTime = jumpBufferTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(groundLayer.ToString()))
        {
            lastGroundedTime = jumpCoyoteTime;
            isJumping = false;
            Debug.Log(lastGroundedTime);
        }
    }

    private bool CanJump()
    {
        return lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("Jump", isJumping);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));
    }
}