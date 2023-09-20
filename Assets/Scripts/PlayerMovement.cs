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
    private float jumpSpeed = 10f;

    private float moveDirection = 0f;
    public bool isInTheAir = true;
    private bool touchingWall = false;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;

    private float leftWall = -9.1394f;

    public float fallSpeed = 10f;

    private bool wallJumping=false;

    private float wallJumpTimer = 0.5f;

    private float wallTimer = 0.5f;

    private float attackTimer = 1f;

    private bool attacking = false;

    private bool movingRight = true;

    [Header("Teleport")]
    [SerializeField] private float TpLength = 4;
    private bool CanTP = false;
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

    private void OnAttack(){
        attackTimer=1f;
        attacking=true;
        animator.SetBool("IsAttacking", true);
        if(movingRight){
            rb.velocity += new Vector2(15f, 0f);
        }else{
            rb.velocity -= new Vector2(15f, 0f);
            
        }
        
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
                wallJumpTimer=0.5f;
                
                isInTheAir = true;
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.pause == true)
        {
            
        }
        else
        {
            CallTP();
            Run();
            FlipSprite();
            if (GameManager.Instance.pause == false)
            {
                CallTP();
            }
        

           if(isInTheAir){
                if(Mathf.Abs(rb.velocity.y) < Mathf.Epsilon){
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFallingIdle", true);
                }
            }else{
                animator.SetBool("IsFallingIdle", false);
            }

            if(touchingWall){
                wallTimer-=Time.deltaTime;
                if(wallTimer>0){
                    rb.gravityScale = fallSpeed;
                    animator.SetBool("IsFalling", true);
                }else{
                    rb.gravityScale = 2f;
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsFallingIdle", true);
                    touchingWall=false;
                }  
            }

            if(wallJumping){
                wallJumpTimer-=Time.deltaTime;
                if(wallJumpTimer>=0){
                    if(movingRight){
                        rb.velocity -= new Vector2(5f, 0f);
                        transform.localScale = new Vector3(
                            Mathf.Sign(rb.velocity.x),
                            1f,
                            1f
                        );
                    }else{
                        rb.velocity += new Vector2(5f, 0f);
                        transform.localScale = new Vector3(
                            Mathf.Sign(rb.velocity.x),
                            1f,
                            1f
                        );
                    }
                }
            }
        }

        if(Input.GetKey("x")){
            OnAttack();
        }
        if(attacking){
            attackTimer-=Time.deltaTime;
            if(attackTimer<=0){
                animator.SetBool("IsAttacking", false);
            }
        }
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
        Debug.Log(moveDirection);
        Debug.Log(movingRight);
        if(moveDirection>0){
            movingRight=true;
        }else if(moveDirection<0){
            movingRight=false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.transform.CompareTag("Platform"))
        {
            // Finalizo el salto
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsFallingIdle", false);
            animator.SetBool("IsWallJumping", false);
            isInTheAir = false;
            rb.gravityScale = 1f;
        }else if(other.transform.CompareTag("GameWall")){
            isInTheAir = false;
            touchingWall=true;
            wallTimer=0.5f;         
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
    }
    public void CallTP()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Teleport();
        }
    }
}
