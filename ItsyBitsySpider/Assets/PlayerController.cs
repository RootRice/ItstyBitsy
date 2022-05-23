using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Controls controls;

    Vector2 movementDir;
    Vector2 jumpDir;

    Rigidbody2D mrigidbody;
    float appliedJumpForce;

    bool grounded;

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] bool jumpHeld;

    float boxHeight;
    int mask;
    void EnableControls()
    {
        controls = new Controls();
        controls.Enable();
        controls.Movement.Enable();
        controls.Movement.LeftRight.performed += ctx => Move(ctx.ReadValue<float>());
        controls.Movement.LeftRight.canceled += ctx => Move(0);
        controls.Movement.Jump.performed += ctx => Jump(true);
        controls.Movement.Jump.canceled += ctx => Jump(false);
    }
    private void Awake()
    {
        EnableControls();
        mrigidbody = GetComponent<Rigidbody2D>();
        boxHeight = GetComponent<BoxCollider2D>().bounds.extents.y;
        mask = LayerMask.GetMask("Floor");
    }


    private void FixedUpdate()
    {
        GroundCheck();
        if(grounded)
        {
            GroundMovement();
        }
        else
        {
            MidAirMovement();
        }
    }
    void Move(float movement)
    {
        movementDir.x = movement;
    }

    void Jump(bool pressed)
    {
        jumpHeld = pressed; 
    }

    bool GroundCheck()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, boxHeight * 1.1f, mask);
        return grounded;
    }

    void GroundMovement()
    {
        jumpDir = movementDir;
        Vector3 movement = movementDir * speed * Time.fixedDeltaTime;
        mrigidbody.position = transform.position + movement;
        if(jumpHeld)
        {
            mrigidbody.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);
        }
    }

    void MidAirMovement()
    {
        Vector3 movement = jumpDir * speed * Time.fixedDeltaTime;
        mrigidbody.position = transform.position + movement;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            jumpDir = movementDir;
        }
    }

}
