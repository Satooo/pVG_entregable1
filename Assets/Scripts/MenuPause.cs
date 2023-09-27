using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPause : MonoBehaviour
{
    public void Pausa()
    {
        Time.timeScale = 0f;
    }
}
