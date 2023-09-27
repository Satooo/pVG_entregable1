using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform playerTransform;
    private Rigidbody2D rb;
    private BoxCollider2D myBoxCollider;
    public bool isChasing = false;
    public float chaseDistance = 10f;
    public float moveSpeed = 2f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
    }
    void Update(){
        if (Boss.Instance.EnemyHp <= 95)
        {
            moveSpeed = 4f;
        }
        if (Boss.Instance.EnemyHp <= 80)
        {
            
        }
        if(isChasing){
            if(transform.position.x > playerTransform.position.x){
                transform.localScale = new Vector3(-1,1,1);
                transform.position += Vector3.left*moveSpeed*Time.deltaTime;
            }else{
                transform.localScale = new Vector3(1,1,1);
                transform.position += Vector3.right*moveSpeed*Time.deltaTime;
            }
        }else{
            transform.position += Vector3.right * 0 * Time.deltaTime;
        
        if (Vector2.Distance(transform.position, playerTransform.position)<chaseDistance){
                isChasing = true;
            }else{

                isChasing = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Platform") || collision.CompareTag("GameWall"))
    {
        // Cambia la dirección horizontal del boss al chocar con una plataforma o pared.
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}

}
