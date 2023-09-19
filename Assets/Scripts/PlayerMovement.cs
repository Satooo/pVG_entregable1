using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float runSpeed = 10f;
    [SerializeField]
    private float jumpSpeed = 4f;

    private float moveDirection = 0f;
    public bool isInTheAir = true;
    private bool touchingWall = false;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;

    private float leftWall = -9.1394f;

    public float fallSpeed = 10f;

    private bool wallJumping=false;


    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();   
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnMove(InputValue value)
    {
        moveDirection = value.Get<float>();
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (
                capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) 
            ){
                // Saltar
                animator.SetBool("IsJumping", true);
                rb.velocity += new Vector2(0f, jumpSpeed);
                isInTheAir = true;
            }else if(capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Wall"))){
                animator.SetBool("IsWallJumping", true);
                animator.SetBool("IsFalling", false);
                /* transform.localScale = new Vector3(
                    Mathf.Sign(rb.velocity.x),
                    1f,
                    1f
                ); */
                rb.velocity += new Vector2(1f, jumpSpeed/2);

                wallJumping=true;
                
                isInTheAir = true;
            }
        }
    }

    private void Update()
    {
        Run();
        FlipSprite();
        //Debug.Log(isInTheAir);
       // Debug.Log((Mathf.Abs(rb.velocity.y) < Mathf.Epsilon));
       if(isInTheAir){
            if(Mathf.Abs(rb.velocity.y) < Mathf.Epsilon){
                rb.gravityScale = 2f;
                animator.SetBool("IsFallingIdle", true);
            }
        }else{
            animator.SetBool("IsFallingIdle", false);
        }
    }

    private void bounceWall(){
       

    }

    private void FlipSprite()
    {
        if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(rb.velocity.x),
                1f,
                1f
            );
        }
    }

    private void Run()
    {
        if (moveDirection == 0f)
        {
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsRunning", true);
        }
        
        rb.velocity = new Vector2(
            runSpeed * moveDirection,
            rb.velocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.transform.CompareTag("Platform"))
        {
            // Finalizo el salto
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsWallJumping", false);
            isInTheAir = false;
            rb.gravityScale = 1f;
        }else if(other.transform.CompareTag("GameWall")){
            isInTheAir = false;
            touchingWall=true;
            rb.gravityScale = fallSpeed;
            animator.SetBool("IsFalling", true);
        }
    }
}
