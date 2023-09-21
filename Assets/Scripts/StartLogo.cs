using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLogo : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas CanvasObject;
    private float initialTime =5f;
    private bool finishedLoading =false;
    void Start()
    {
        CanvasObject = GetComponent<Canvas> ();
    }

    // Update is called once per frame
    void Update()
    {
        if(finishedLoading=false){
            initialTime-=Time.deltaTime;
        }
        
        if(initialTime<0){
            CanvasObject.enabled=false;
            finishedLoading=true;
        }
    }
}
