using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class PlayerMovement : MonoBehaviour
{
    // Variables that can be change from unity
    [Header("Speeds")]
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float jumpSpeed = 12f;
    [SerializeField] private float fallSpeed = 2f;
    [Header("Timer")] [SerializeField] private float wallTimerSlip = 1f;
    [Header("Teleport")]
    [SerializeField] private float TpLength = 5;
    [SerializeField] private float rayDistance;
    private Vector2 direction;
    [SerializeField] private Transform rayCastPoint;
    public Tilemap platforms;
    private bool CanDie;


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
    private bool isFallingIdle = false;
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
                // change animation idle/running->IsJumping
                animator.SetBool("IsJumping", true);
                // push to jump
                rb.velocity += new Vector2(0f, jumpSpeed);
                isInTheAir = true;
            }
            else if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            {
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
        direction = new Vector2(transform.localScale.x, 0);
        if (!GameManager.Instance.pause)
        {
            //SetRayCastPos();
            RaycastHit2D hit = Physics2D.Raycast(
            rayCastPoint.position,
            direction,
            rayDistance
        );

            Debug.DrawRay(
                rayCastPoint.position,
                direction * rayDistance,
                Color.red
            );
            if (hit)
            {
                //Debug.Log(hit.collider.transform.name);
                if (hit.collider.transform.CompareTag("Platform"))
                {
                    //Debug.Log("MORIR");
                    CanDie = true;

                }
                else
                {
                    CanDie = false;
                }

            }
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
                    if (isFallingIdle == false)
                    {
                        animator.SetBool("IsJumping", false);
                        animator.SetBool("IsFallingIdle", true);
                        isFallingIdle = true;
                    }
                }
            }

            if (isTouchingWall)
            {
                //Debug.Log(wallTimer);
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
                //Debug.Log("HAHAHAHAHAHAHAHAHAHA" + contact);
                // Check if the collision point is below the player's center (feet).
                // Only Y axis is checked
                if (contact.point.y < transform.position.y)
                {
                    // End jumping animation
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsWallJumping", false);
                    animator.SetBool("IsFallingIdle", false);
                    rb.gravityScale = 1f;

                    isInTheAir = false;
                    isFallingIdle = false;
                }
                // ELSE: Collision with any other part that is not the bottom of Y
            }
        }
        else if (other.transform.CompareTag("GameWall"))
        {
            wallTimer = wallTimerSlip;
            isTouchingWall = true;

            isInTheAir = false;
            isFallingIdle = false;
            if (other.transform.name == "BottomWall")
            {
                Die();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("GameWall"))
        {
            //Debug.Log("SE SEPARO");
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
    public void CallTP()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Teleport();
        }
    }
    public void Teleport()
    {
        if (transform.localScale.x == 1)
        {
            transform.position = new Vector3(transform.position.x + TpLength, transform.position.y, transform.position.z);


        }
        else if (transform.localScale.x == -1)
        {
            transform.position = new Vector3(transform.position.x - TpLength, transform.position.y, transform.position.z);

        }
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position,LayerMask.GetMask("Ground"));
        if (hit != null)
        {
            Debug.Log("DEBE MORIR");
            Die();
        }
    }
    

    public void Dash()
    {
        if (Input.GetKey("x"))
        {
            SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Dash);
            OnAttack();
        }
    }
 
    public void Die()
    {
        animator.SetTrigger("Death");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
