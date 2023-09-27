using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform playerTransform;
    public bool isChasing = false;
    public float chaseDistance = 20f;
    public float moveSpeed = 2f;


    void Update(){
        if(isChasing){
            if(transform.position.x > playerTransform.position.x){
                transform.localScale = new Vector3(-1,1,1);
                transform.position += Vector3.left*moveSpeed*Time.deltaTime;
            }else{
                transform.localScale = new Vector3(1,1,1);
                transform.position += Vector3.right*moveSpeed*Time.deltaTime;
            }
        }else{
            if(Vector2.Distance(transform.position, playerTransform.position)<chaseDistance){
                isChasing = true;
            }else{
                isChasing = false;
            }
        }
    }
}
