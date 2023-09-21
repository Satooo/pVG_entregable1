using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas CanvasObject;
    public float initialTime =3f;
    private bool finishedLoading =false;
    void Start()
    {
        CanvasObject = GetComponent<Canvas> ();
    }

    // Update is called once per frame
    void Update()
    {
        if(finishedLoading==false){
            initialTime-=Time.deltaTime;
            Debug.Log(initialTime);
        }
        
        if(initialTime<0){
            CanvasObject.enabled=false;
            finishedLoading=true;
        }
    }
}
