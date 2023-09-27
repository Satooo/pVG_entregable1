using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Transform rayCastPoint;
    private Vector2 direction = Vector2.right;
    private Transform player;
    private Rigidbody2D rb;
    private BoxCollider2D myBoxCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            rayDistance
        );

        Debug.DrawRay(
            transform.position,
            transform.right*3f,
            Color.red
        );

        if (hit) 
        {
            player = hit.collider.transform;
            Attack();
        }
        if(ShouldFall())
        {
            rb.velocity = new Vector2(
                0f,
                rb.velocity.y
            );
        }
        if(IsFacingRight()){
            rb.velocity = new Vector2(speed, 0f);
        }else{
            rb.velocity = new Vector2(-speed, 0f);
        }
    }

    private void Attack()
    {
        rb.velocity = new Vector2(
            speed,
            rb.velocity.y
        );
    }

    private bool IsFacingRight(){
        return transform.localScale.x > Mathf.Epsilon;
    }
    private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Platform"))
    {
        // Aquí puedes realizar las acciones que desees cuando este enemigo deja de colisionar con una "platform".
        // Por ejemplo, puedes cambiar su dirección o comportamiento, etc.
        transform.localScale = new Vector2(-Math.Sign(rb.velocity.x), transform.localScale.y);
    }
    
    
    
}


    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        transform.localScale = new Vector2(-Math.Sign (rb.velocity.x), transform.localScale.y);
    }
    else if (collision.CompareTag("GameWall"))
    {
        transform.localScale = new Vector2(-Math.Sign (rb.velocity.x), transform.localScale.y);
    }
}



    private bool ShouldFall()
    {
        Vector2 dir = new Vector2(1f,-1f);
        RaycastHit2D hit = Physics2D.Raycast(
            rayCastPoint.position,
            dir.normalized,
            2f
        );
        Debug.DrawRay(
            rayCastPoint.position,
            dir.normalized * 2f,
            Color.blue
        );
        return !hit;
    }
}
