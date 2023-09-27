using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   
    private void Start() {
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        Enemy enemy = other.transform.GetComponent<Enemy>();
        Boss boss = other.transform.GetComponent<Boss>();

        if (enemy != null)
        {
            Debug.Log("IMPACTANDO CONTRA ENEMIGO");
            GameManager.Instance.PlayerDamage(enemy.damage);
        }else if (boss != null){
            Debug.Log("IMPACTANDO CONTRA BOSS");
            GameManager.Instance.PlayerDamage(boss.damage);
        }

    }
}
