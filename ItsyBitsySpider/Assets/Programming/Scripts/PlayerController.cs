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
    float timeOfLastJump;

    public bool grounded = true;

    [SerializeField] float speed;
    [SerializeField] float swingPower;
    [SerializeField] float jumpForce;
    [SerializeField] [Range(1f, 2f)] float jumpHorizontalBoost;
    float movementMod = 1.0f;
    bool jumpHeld;

    float boxHeight;
    int mask;

    Animator mAnimator;
    int movingHash;
    int groundedHash;
    int doubleJumpHash;
    int holdingHash;
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
        controls.Movement.Freeze.performed += ctx => FreezeMovement(0);
        controls.Movement.Freeze.canceled += ctx => FreezeMovement(1);

    }
    void DisableControls()
    {
        controls.Enable();
        controls.Movement.Enable();
        controls.Movement.LeftRight.performed -= ctx => Move(ctx.ReadValue<float>());
        controls.Movement.LeftRight.canceled -= ctx => Move(0);
        controls.Movement.xJoy.performed -= ctx => Move(ctx.ReadValue<Vector2>().x);
        controls.Movement.xJoy.canceled -= ctx => Move(0);
        controls.Movement.Jump.performed -= ctx => Jump(true);
        controls.Movement.Jump.canceled -= ctx => Jump(false);
        controls.Movement.Freeze.performed -= ctx => FreezeMovement(0);
        controls.Movement.Freeze.canceled -= ctx => FreezeMovement(1);
        controls.Disable();
        controls.Movement.Disable();
    }
    private void Start()
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
        holdingHash = Animator.StringToHash("Holding");
        mAnimator.SetBool(holdingHash, false);
        GameplayManager.spiderHeight = transform.position.y;
    }

    void FreezeMovement(float m)
    {
        movementMod = m;
        if(m > 0)
        {
            mAnimator.SetBool(holdingHash, false);
                
        }
        else
        {
            mAnimator.SetBool(holdingHash, true);
        }
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
        GameplayManager.spiderHeight = transform.position.y;
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
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
        //if (pressed && !grounded && !doubleJump)
        //{
        //    mrigidbody.velocity = Vector2.zero;
        //    jumpDir = movementDir;
        //    ApplyJumpForce();
        //    doubleJump = true;
        //}
    }

    bool GroundCheck()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, boxHeight, Vector2.down, boxHeight * 0.1f, mask);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit)
            {
                if (hit.point.y < transform.position.y)
                {
                    grounded = true;
                    break;
                }
                else
                {
                    grounded = false;
                }
            }
            else
            {
                grounded = false;
            }
        }
        if(hits.Length == 0)
        {
            grounded = false;
        }
        
        
        mAnimator.SetBool(groundedHash, grounded);
        return grounded;
    }

    void GroundMovement()
    {
        doubleJump = false;
        jumpDir = movementDir;
        Vector3 movement = movementDir * speed * Time.fixedDeltaTime * movementMod;
        mrigidbody.position = transform.position + movement;
        if (jumpHeld && Time.timeSinceLevelLoad - timeOfLastJump > 0.25f)
        {
            timeOfLastJump = Time.timeSinceLevelLoad;
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

    private void OnDestroy()
    {
        DisableControls();
    }
}
