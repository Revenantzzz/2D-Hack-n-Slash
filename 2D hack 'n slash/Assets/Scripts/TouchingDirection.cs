using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    CapsuleCollider2D touchingCol;
    Animator animator;
    ContactFilter2D contactFilter;

    float groundDistance = .01f;
    float wallDistance = .05f;
    float ceilDistance = .05f;

    public Vector2 wallDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    RaycastHit2D[] groundHit = new RaycastHit2D[5];
    RaycastHit2D[] wallHit = new RaycastHit2D[5];
    RaycastHit2D[] ceilHit = new RaycastHit2D[5];

    private bool _isGrounded;
    public bool isGrounded
    {
        get
        {
            return _isGrounded;
        }
        set
        {
            _isGrounded = value;
            animator.SetBool(AnimationString.isGrounded, value);
        }
    }

    private bool _isOnCeilling;
    public bool isOnCeilling 
    { 
        get
        {
            return _isOnCeilling;
        }
        set
        {
            _isOnCeilling = value;
            animator.SetBool(AnimationString.isOnCeilling, value);
        }
    }

    private bool _isOnWall;
    public bool isOnWall
    {
        get
        {
            return _isOnWall;
        }
        set 
        { 
            _isOnWall = value;
            animator.SetBool(AnimationString.isOnWall, value);
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        isGrounded = touchingCol.Cast(Vector2.down, contactFilter, groundHit, groundDistance) != 0;
        isOnCeilling = touchingCol.Cast(Vector2.up, contactFilter, ceilHit, ceilDistance) != 0;
        isOnWall = touchingCol.Cast(wallDirection, contactFilter, wallHit, wallDistance) > 0;
    }
}
