using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarEnemy1 : MonoBehaviour
{
    private Slider slider;
    private void Awake() {
        slider = GetComponent<Slider>();
    }
    // Observer
    private void Start() {
        GameManager.Instance.AddObserverEnemy1(ReduceHealth);
    }

    private void Update() {
        slider.maxValue = GameManager.Instance.enemy1MaxHp;
        slider.value = GameManager.Instance.enemy1CurrentHp;
    }

    private void ReduceHealth(float enemy1CurrentHp) 
    {
        slider.value = enemy1CurrentHp;
    }
}
