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
    [SerializeField] private float fallSpeed = 0.4f;
    [Header("Timer")][SerializeField] private float wallTimerSlip = 1f;
    [Header("Teleport")]
    [SerializeField] private float TpLength = 5;
    [SerializeField] private float rayDistance;
    [SerializeField] private Transform rayCastPoint;
    public Tilemap platforms;
    public bool CanTP;
    public GameObject canvas;

    // Initialized on Start()
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private readonly SoundManagerScript soundManagerScript; // Not initialized



    private float moveDirection = 0f;
    private float wallJumpTimer = 0.5f;
    private float wallTimer = 0f;
    private float attackTimer = 1f;
    private bool isMovingRight = true;
    private bool isTouchingWall = false;
    private bool isInTheAir = true;
    private bool isWallJumping = false;
    private bool isOnTopOfWall = false;
    private bool isAttacking = false;






    // MonoBehaviour class methods (Start and Update)
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        
    }
    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
    }
    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.Instance.pause)
        {
            Run();
            FlipSprite();
            CallTP();
            Dash();
            //CheckHP();
            CheckPower();
            if (isInTheAir)
            {
                // Player reaches highest place during jump
                if (Mathf.Abs(rb.velocity.y) < Mathf.Epsilon)
                {
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFallingIdle", true);
                }
                // Player is falling idle
                /* else if (rb.velocity.y < Mathf.Epsilon)
                {
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFallingIdle", true);
                } */
            }else{
                animator.SetBool("IsFallingIdle", false);
            }

            if (isTouchingWall)
            {
                Debug.Log("touchingwall");
                 wallTimer -= Time.deltaTime;
                if (wallTimer > 0)
                {
                    rb.gravityScale = fallSpeed;
                    animator.SetBool("IsFalling", true);
                }else{
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsFallingIdle", true);
                    isTouchingWall = false;
                }
            }

            if (isWallJumping && !isOnTopOfWall)
            {
               
                Debug.Log(isWallJumping);
                wallJumpTimer -= Time.deltaTime;
                 Debug.Log(wallJumpTimer);
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



    // Key movements
    private void OnMove(InputValue value)
    {
        moveDirection = value.Get<float>();
    }
    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            // Player can only jump if is touching any ColliderLayerMask
            if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                animator.SetBool("IsJumping", true);
                rb.velocity += new Vector2(0f, jumpSpeed);
                isInTheAir = true;
                SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Jump);
            }
            else if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            {
                animator.SetBool("IsWallJumping", true);
                animator.SetBool("IsFalling", false);

                rb.velocity += new Vector2(1f, jumpSpeed / 2);
                isWallJumping = true;
                wallJumpTimer = 0.8f;
                isInTheAir = true;
                SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Jump);
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



    // Collisions function
    private void OnCollisionEnter2D(Collision2D other)
    {


        if (other.transform.name == "BottomWall")
        {
            Die();
        }


        if (other.transform.CompareTag("Platform"))
        {
            Debug.Log("CHOCA CONTRA PLATAFORMA");
            foreach (ContactPoint2D contact in other.contacts)
            {
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
                }
                else
                {
                    // Calculate the direction of the collision
                    Vector2 collisionDirection = (transform.position - other.transform.position).normalized;

                    // Reflect the player's velocity
                    Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionDirection);

                    // Set the player's velocity to the reflected velocity
                    rb.velocity = reflectedVelocity;

                    // Add a downward force to simulate the player falling
                    rb.AddForce(new Vector2(0f, -5f), ForceMode2D.Impulse);
                }
                // ELSE: Collision with any other part that is not the bottom of Y
            }
        }








        else if (other.transform.CompareTag("GameWall"))
        {
            Debug.Log("CHOCA CONTRA GAMEWALL");
            // wallTimer = wallTimerSlip;
            // isTouchingWall = true;

            // isInTheAir = false;
            // if (other.transform.name == "BottomWall")
            // {
            //     Die();
            // }


            foreach (ContactPoint2D contact in other.contacts)
            {
                // Choco con cabeza o pies
                if (contact.point.y < transform.position.y || contact.point.y > transform.position.y)
                {
                    Debug.Log("TOCANDO VERTICAL");
                    isTouchingWall = true;
                    //isOnTopOfWall = true;
                    isInTheAir = false;
                    wallTimer=0.8f;
                }
                else
                {
                    Debug.Log("TOCANDO LADO");
                }
            }
        }


















        else if (other.transform.CompareTag("Enemy"))
        {
            Debug.Log("CHOCA CONTRA ENEMY");
            rb.velocity = new Vector2(0, jumpSpeed / 2);
            isInTheAir = true;
        }































    }
    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     Debug.Log("ENTRAMOS AFUERA");
        
        
    //     // Lanza rayos hacia la izquierda y hacia la derecha desde el centro del jugador.
    //     RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.1f);
    //     RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.1f);

    //     // Comprueba si el jugador todavía toca el GameWall desde los lados horizontales.
    //     bool touchingLeft = (hitLeft.collider != null && hitLeft.collider.CompareTag("GameWall"));
    //     bool touchingRight = (hitRight.collider != null && hitRight.collider.CompareTag("GameWall"));
    //     bool isTouchingSide = touchingLeft || touchingRight;

    //     if (!isTouchingSide)
    //     {
    //         // El jugador dejó de tocar el GameWall desde los lados horizontales
    //         isTouchingWall = false;
    //         isOnTopOfWall = false;
    //         isInTheAir = true;
    //     }else{
    //         isOnTopOfWall = true;

    //     }
        
        
        
        
        
        
        
        
    //     if (other.transform.CompareTag("GameWall"))
    //     {
    //         isOnTopOfWall = true;
    //     }
    //     // {
    //     //     // Lanza un rayo desde el centro del jugador hacia la izquierda.
    //     //     RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.4f);

    //     //     // Lanza un rayo desde el centro del jugador hacia la derecha.
    //     //     RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.4f);

    //     //     bool touchingLeft = (hitLeft.collider != null && hitLeft.collider.CompareTag("GameWall"));
    //     //     bool touchingRight = (hitRight.collider != null && hitRight.collider.CompareTag("GameWall"));
    //     //     bool isTouchingSide = touchingLeft || touchingRight;

    //     //     if (!isTouchingSide)
    //     //     {
    //     //         // El jugador dejó de tocar el GameWall desde los lados horizontales
    //     //         isTouchingWall = false;
    //     //         isInTheAir = true;
    //     //     }
    //     // }

    // }



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
            if (CanTP == true)
            {
                animator.SetBool("TP", true);
                Invoke("Teleport", 0.2f);
            }

        }

    }
    public void Teleport()
    {
        if (CanTP == true)
        {
            if (transform.localScale.x == 1)
            {
                transform.position = new Vector3(transform.position.x + TpLength, transform.position.y, transform.position.z);
                GameManager.Instance.mainPlayerCurrentPower = 0;
                CanTP = false;
            }
            else if (transform.localScale.x == -1)
            {
                transform.position = new Vector3(transform.position.x - TpLength, transform.position.y, transform.position.z);
                animator.SetTrigger("TP");
                GameManager.Instance.mainPlayerCurrentPower = 0;
                CanTP = false;

            }
            animator.SetBool("TP", false);
            Collider2D hit = Physics2D.OverlapPoint(this.transform.position, LayerMask.GetMask("Ground"));
            if (hit != null)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("No se puede realizar Teleport");
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
        SoundManagerScript.PlaySound(SoundManagerScript.SoundType.Death);
        StartCoroutine(passiveMe(0.5f));
        
    }
    public void CheckHP() {
        if (GameManager.Instance.mainPlayerCurrentHp == 0)
        {
            Die();
        }
    }
    public void CheckPower()
    {
        if (GameManager.Instance.mainPlayerCurrentPower == 50)
        {
            CanTP = true;
        }
    }
    

IEnumerator passiveMe(float secs)
    {
        yield return new WaitForSeconds(secs);
        canvas.transform.GetChild(2).gameObject.SetActive(true);
    }
}
