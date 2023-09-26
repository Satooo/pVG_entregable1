using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float damage = 10f;

    private void Start() {
        GameManager.Instance.AddObserverMainPlayer(PlayerDamage);
    }

    private void PlayerDamage(float playerCurrentHp)
    {
        Debug.Log("El enemigo reacciono a danho a jugador");
    }
}
