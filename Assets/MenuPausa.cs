using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa Instance { get; private set; }
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    private bool juegoPausado=false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausa();
            }

        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Ya hay un Menu");
        }
    }
    public void Pausa()
    {
        juegoPausado = true;
        Time.timeScale = 0f;
        GameManager.Instance.pause = true;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }
    public void Reanudar()
    {
        GameManager.Instance.pause = false;
        juegoPausado = false;
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }
    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Cerrar()
    {
        Debug.Log("Se cerró el juego");
        Application.Quit();
    }
}
