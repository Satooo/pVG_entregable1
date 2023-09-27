using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PowerBar : MonoBehaviour
{
    private Slider slider;
    // Start is called before the first frame update
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    // Update is called once per frame
    private void Start()
    {
        GameManager.Instance.AddObserverMainPlayerPower(AddPower);
        slider.value = 0;
    }
    private void Update()
    {
        slider.maxValue = GameManager.Instance.mainPlayerMaxPower;
        slider.value = GameManager.Instance.mainPlayerCurrentPower;
    }
    private void AddPower(float PowerCount)
    {
        slider.value = PowerCount;
    }
}
