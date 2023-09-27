using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private void Awake() {
        slider = GetComponent<Slider>();
    }
    // Observer
    private void Start() {
        GameManager.Instance.AddObserverMainPlayer(ReduceHealth);
    }

    private void Update() {
        slider.maxValue = GameManager.Instance.mainPlayerMaxHp;
        slider.value = GameManager.Instance.mainPlayerCurrentHp;
    }

    private void ReduceHealth(float playerCurrentHp) 
    {
        slider.value = playerCurrentHp;
    }
   
}
