using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knight_Player_Controller : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    TouchingDirection touchingDir;


    private Vector2 moveInput = Vector2.zero;

    [SerializeField]
    private float walkSpeed = 8f;
    [SerializeField]
    private float jumpImpulse = 8f;
    [SerializeField]
    private float airSpeed = 5f;
    [SerializeField]
    private float rollImpulse = 3f;


    private bool _isFacingRight = true;
    public bool isFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    private bool _isMoving = false;
    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
        set
        {
            animator.SetBool(AnimationString.isMoving, value);
            _isMoving = value;
        }
    }

    public float yVelocity
    {
        get
        {
            return rb.velocity.y;
        }
        set
        {
            animator.SetFloat(AnimationString.yVelocity, value);
        }
    }
    private void switchDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
        }
        if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
        }
    }
    private float currentSpeed()
    {
        if(touchingDir.isGrounded) 
        {
            return walkSpeed;
        }
        else
        {
            return airSpeed;
        }
        
    }

    private void Move()
    {
        if (isMoving)
        {
            rb.velocity = new Vector2(currentSpeed() * moveInput.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDir = GetComponent<TouchingDirection>();
    }

    private void FixedUpdate()
    {  
        Move();
        yVelocity = rb.velocity.y;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.x != 0;
        switchDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started && touchingDir.isGrounded)
        {
            animator.SetTrigger(AnimationString.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if(context.started && touchingDir.isGrounded)
        {
            animator.SetTrigger(AnimationString.roll);
            rb.velocity = new Vector2(rollImpulse, rb.velocity.y);
        }
    }

    public void OnShield(InputAction.CallbackContext context)
    {
        if(context.started && touchingDir.isGrounded)
        {
            animator.SetBool(AnimationString.shield, true);
            
        }
        if (context.canceled)
        {
            animator.SetBool(AnimationString.shield, true);
        }
    }
}
