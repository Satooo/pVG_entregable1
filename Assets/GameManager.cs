using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// SINGLETON
public class GameManager : MonoBehaviour
{
    public UnityAction<float> OnPlayerDamage;
    public UnityAction<float> OnEnemy1Damage;
    public static GameManager Instance { get; private set; }
    public bool pause = false;
    // Main player variables
    public float mainPlayerMaxHp = 100;
    public float mainPlayerCurrentHp = 100;

    // Enemy1 variables
    public float enemy1MaxHp = 50;
    public float enemy1CurrentHp = 50;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Ya hay un GameManager");
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDamage(float damage)
    {
        mainPlayerCurrentHp -= damage;
        OnPlayerDamage.Invoke(mainPlayerCurrentHp);
    }

    public void EnemyDamage(float damage)
    {
        enemy1CurrentHp -= damage;
        OnEnemy1Damage.Invoke(enemy1CurrentHp);
    }

    public void AddObserverMainPlayer(UnityAction<float> action)
    {
        OnPlayerDamage += action;
    }

    public void AddObserverEnemy1(UnityAction<float> action)
    {
        OnEnemy1Damage += action;
    }

}
