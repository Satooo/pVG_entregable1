using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    // Variables that can be change from unity
    [Header("Speeds")]
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float jumpSpeed = 12f;
    [SerializeField] private float fallSpeed = 2f;
    [Header("Timer")][SerializeField] private float wallTimerSlip = 1f;
    [Header("Teleport")][SerializeField] private float TpLength = 5;



    // Initialized on Start()
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private readonly SoundManagerScript soundManagerScript; // Not initialized



    private float moveDirection = 0f;
    private float leftWall = -9.1394f;
    private float wallJumpTimer = 0.5f;
    private float wallTimer = 0f;
    private float attackTimer = 1f;
    private bool isMovingRight = true;
    private bool isTouchingWall = false;
    private bool isInTheAir = true;
    private bool isWallJumping = false;
    private bool isAttacking = false;
    private bool canTP = false;





    // MonoBehaviour class method
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }


    // Key movements
    private void OnMove(InputValue value)
    {
        moveDirection = value.Get<float>();
    }
    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Jump);
            // Player can only jump if is touching any ColliderLayerMask
            if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                Debug.Log("IS TOUCHING GROUND TO JUMP");
                // change animation idle/running->IsJumping
                animator.SetBool("IsJumping", true);
                // push to jump
                rb.velocity += new Vector2(0f, jumpSpeed);
                isInTheAir = true;
            }
            else if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            {
                Debug.Log("IS WALL GROUND TO JUMP");
                // Change animation jumping->IsWallJumping
                animator.SetBool("IsWallJumping", true);
                // push to jump
                rb.velocity += new Vector2(1f, jumpSpeed / 2);
                isWallJumping = true;
                wallJumpTimer = 0.5f;
                isInTheAir = true;
            }
        }
    }
    private void OnAttack()
    {
        attackTimer = 1f;
        isAttacking = true;
        animator.SetBool("IsAttacking", true);
        if (isMovingRight)
        {
            rb.velocity += new Vector2(15f, 0f);
        }
        else
        {
            rb.velocity -= new Vector2(15f, 0f);
        }
    }


    // MonoBehaviour class method
    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.Instance.pause)
        {
            Run();
            FlipSprite();
            CallTP();
            Dash();

            if (isInTheAir)
            {
                // Player reaches highest place during jump
                if (Mathf.Abs(rb.velocity.y) < Mathf.Epsilon)
                {
                    rb.gravityScale *= 2f;
                }
                // Player is falling idle
                else if (rb.velocity.y < Mathf.Epsilon)
                {
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFallingIdle", true);
                }
            }

            if (isTouchingWall)
            {
                Debug.Log(wallTimer);
                wallTimer -= Time.deltaTime;
                if (wallTimer > 0)
                {
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFalling", true);
                }
                else
                {
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsFallingIdle", true);
                    isTouchingWall = false;
                }
            }

            if (isWallJumping)
            {
                wallJumpTimer -= Time.deltaTime;
                if (wallJumpTimer >= 0)
                {
                    if (isMovingRight)
                    {
                        rb.velocity -= new Vector2(5f, 0f);
                        transform.localScale = new Vector3(
                            Mathf.Sign(rb.velocity.x),
                            1f,
                            1f
                        );
                    }
                    else
                    {
                        rb.velocity += new Vector2(5f, 0f);
                        transform.localScale = new Vector3(
                            Mathf.Sign(rb.velocity.x),
                            1f,
                            1f
                        );
                    }
                }
            }
            else
            {
                isWallJumping = false;
            }

            if (isAttacking)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    animator.SetBool("IsAttacking", false);
                    isAttacking = false;
                }
            }
        }
    }


    // Collisions function
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in other.contacts)
            {
                Debug.Log("CHOCO CON:"+contact.point.y);
                // Check if the collision point is below the player's center (feet).
                // Only Y axis is checked
                if (contact.point.y < transform.position.y)
                {
                    Debug.Log("CHOCO CON LOS PIES :)");
                    // End jumping animation
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsWallJumping", false);
                    animator.SetBool("IsFallingIdle", false);
                    rb.gravityScale = 1f;

                    isInTheAir = false;
                }
                else{
                    Debug.Log("CHOCO CON LA CABEZA :)");
                }
                // ELSE: Collision with any other part that is not the bottom of Y
            }
        }
        else if (other.transform.CompareTag("GameWall"))
        {
            wallTimer = wallTimerSlip;
            isTouchingWall = true;

            isInTheAir = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("GameWall"))
        {
            Debug.Log("SE SEPARO");
            // The player has stopped touching the GameWall
            isTouchingWall = false;
            isInTheAir = true;
        }
    }



    // Player movement functions
    private void Run()
    {
        // Conditional to change animation idle->running
        if (moveDirection == 0f)
        {
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsRunning", true);
        }

        // Player movement on X axis
        rb.velocity = new Vector2(
            runSpeed * moveDirection,
            rb.velocity.y
        );

        // X axis movement side
        if (moveDirection > 0)
        {
            isMovingRight = true;
        }
        else if (moveDirection < 0)
        {
            isMovingRight = false;
        }
    }
    private void FlipSprite()
    {
        // If the player is in movement, it's body direction will change
        if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        {
            // Change the direction of the player's body
            transform.localScale = new Vector3(
                Mathf.Sign(rb.velocity.x), // the function returns -1 or +1 if the values are negative or positive
                1f,
                1f
            );
        }
    }
    public  void CallTP()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Teleport();
        }
    }
    public  void Teleport()
    {
        if (transform.localScale.x == 1)
        {
            transform.position = new Vector3(transform.position.x + TpLength, transform.position.y, transform.position.z);


        }
        else if (transform.localScale.x == -1)
        {
            transform.position = new Vector3(transform.position.x - TpLength, transform.position.y, transform.position.z);

        }
    }
    public  void Dash()
    {
        if (Input.GetKey("x"))
        {
            SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Dash);
            OnAttack();
        }
    }

}
