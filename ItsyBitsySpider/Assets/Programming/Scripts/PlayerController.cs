using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Controls controls;
    GrapplingRope myRope;

    Vector2 movementDir;
    Vector2 jumpDir;
    Vector2 jumpVector;

    bool doubleJump;

    Rigidbody2D mrigidbody;
    float appliedJumpForce;

    bool grounded;

    [SerializeField] float speed;
    [SerializeField] float swingPower;
    [SerializeField] float jumpForce;
    [SerializeField] [Range(1f, 2f)] float jumpHorizontalBoost;
    bool jumpHeld;

    float boxHeight;
    int mask;

    Animator mAnimator;
    int movingHash;
    int groundedHash;
    int doubleJumpHash;
    float scale;
    void EnableControls()
    {
        controls = new Controls();
        controls.Enable();
        controls.Movement.Enable();
        controls.Movement.LeftRight.performed += ctx => Move(ctx.ReadValue<float>());
        controls.Movement.LeftRight.canceled += ctx => Move(0);
        controls.Movement.xJoy.performed += ctx => Move(ctx.ReadValue<Vector2>().x);
        controls.Movement.xJoy.canceled += ctx => Move(0);
        controls.Movement.Jump.performed += ctx => Jump(true);
        controls.Movement.Jump.canceled += ctx => Jump(false);
    }
    private void Awake()
    {
        EnableControls();
        mrigidbody = GetComponent<Rigidbody2D>();
        boxHeight = GetComponent<BoxCollider2D>().bounds.extents.y;
        mAnimator = GetComponent<Animator>();
        myRope = GetComponentInChildren<GrapplingRope>();
        mask = LayerMask.GetMask("Floor");
        movingHash = Animator.StringToHash("Moving");
        groundedHash = Animator.StringToHash("Grounded");
        doubleJumpHash = Animator.StringToHash("DoubleJump");
        scale = transform.localScale.z;
    }


    private void FixedUpdate()
    {
        GroundCheck();
        if (grounded)
        {
            GroundMovement();
        }
        else if(myRope.isGrappling)
        {
            GrapplingMovement();
        }
        else
        {
            MidAirMovement();
        }
    }
    void Move(float movement)
    {
        bool moving = true;
        movementDir.x = Mathf.Round(movement);
        if (movementDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else if(movementDir.x < 0)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            moving = false;
        }
        mAnimator.SetBool(movingHash, moving);
    }

    void GrapplingMovement()
    {
        jumpDir = movementDir;
        mrigidbody.AddForce(movementDir * swingPower * Time.fixedDeltaTime);
    }

    void Jump(bool pressed)
    {
        jumpHeld = pressed;
        if (pressed && !grounded && !doubleJump)
        {
            mrigidbody.velocity = Vector2.zero;
            jumpDir = movementDir;
            ApplyJumpForce();
            doubleJump = true;
        }
    }

    bool GroundCheck()
    {
        grounded = Physics2D.CircleCast(transform.position, boxHeight*0.6f, Vector2.down, boxHeight * 0.6f, mask);
        mAnimator.SetBool(groundedHash, grounded);
        return grounded;
    }

    void GroundMovement()
    {
        doubleJump = false;
        jumpDir = movementDir;
        Vector3 movement = movementDir * speed * Time.fixedDeltaTime;
        mrigidbody.position = transform.position + movement;
        if (jumpHeld)
        {
            mrigidbody.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);
        }
        
    }

    void ApplyJumpForce()
    {
        mrigidbody.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);
        mAnimator.SetTrigger(doubleJumpHash);
    }

    void MidAirMovement()
    {
        Vector3 movement = jumpDir * jumpHorizontalBoost * speed * Time.fixedDeltaTime;
        mrigidbody.position = transform.position + movement;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            jumpDir = movementDir;
        }
    }

}
