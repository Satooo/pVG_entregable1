using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float damage = 10f;
    public float EnemyHp = 40f;
    public static Boss Instance { get; private set; }

    private void Start() {
        EnemyHp = 100f;
        GameManager.Instance.AddObserverMainPlayer(PlayerDamage);
        
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Ya hay un GameManager
        }
    }
    private void Update()
    {
        DieWatcher();
        if (EnemyHp <= 60)
        {
            damage = 15f;
        }
        else if (EnemyHp <= 40)
        {
            damage = 20f;
        }
        else if (EnemyHp <= 20)
        {
            damage = 25f;
        }

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
        if (EnemyHp == 0)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
