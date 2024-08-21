using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigid;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] Vector2 deathKich = new Vector2(0f, 10f);
    Animator animator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;
    float gravityscaleAtstart;

    bool isAlive = true;

    [Header("Weapon")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider=GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        gravityscaleAtstart= myRigid.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        Run();
        FlipDprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void Die()
    {
        bool isBodyTouchingEnemy = bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"));
        bool isFeetTouchingEnemy = feetCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"));
        if (isBodyTouchingEnemy || isFeetTouchingEnemy) { 
        isAlive = false;
            animator.SetTrigger("Dying");

            myRigid.velocity = deathKich;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void ClimbLadder()
    {
       
        if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigid.gravityScale = gravityscaleAtstart;
            animator.SetBool("isClimbing", false);
            return;
        }

        bool playerHasVerticalSpeed = Mathf.Abs(myRigid.velocity.y) > Mathf.Epsilon;

        Vector2 climbVelocity = new Vector2(myRigid.velocity.x, moveInput.y * climbSpeed);
        myRigid.velocity = climbVelocity;
        myRigid.gravityScale = 0f;
        
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void FlipDprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigid.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
        transform.localScale = new Vector2 (Mathf.Sign(myRigid.velocity.x), 1f);
        }
        
    }

    void Run()
    {
        if (!isAlive)
        {
            return;
        }
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigid.velocity.x) > Mathf.Epsilon;
        Vector2 playerVelocity = new Vector2(moveInput.x* runSpeed, myRigid.velocity.y);
        myRigid.velocity = playerVelocity;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void OnJump(InputValue inputValue)
    {
        if (!isAlive)
        {
            return;
        }
        bool isTouchingGround = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!isTouchingGround)
        {
            return;
        }
        if (inputValue.isPressed)
        {
            animator.SetBool("isClimbing", false);
            myRigid.velocity += new Vector2(0f, jumpSpeed);
        }
       
    }

    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

}
