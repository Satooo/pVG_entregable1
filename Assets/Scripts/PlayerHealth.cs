using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{   private Animator damageAnimation;
    private void Start() {
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        Enemy enemy = other.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            GameManager.Instance.PlayerDamage(enemy.damage);
        }
    }
}
