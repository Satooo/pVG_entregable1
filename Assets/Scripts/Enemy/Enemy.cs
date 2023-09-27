using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float damage = 10f;
    public float EnemyHp = 4f;
    
    private void Start() {
        EnemyHp = 4f;
        GameManager.Instance.AddObserverMainPlayer(PlayerDamage);
    }
    private void Update()
    {
        DieWatcher();
    }
    private void PlayerDamage(float playerCurrentHp)
    {
        // El enemigo reacciono a danho a jugador
    }
    public void SubstractDamage()
    {
        EnemyHp -= 1f;
    }
    private void DieWatcher()
    {
        if (EnemyHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
