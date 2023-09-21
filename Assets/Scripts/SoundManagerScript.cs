using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip playerJump, playerDash;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        playerDash = Resources.Load<AudioClip>("whoosh");
        playerJump = Resources.Load<AudioClip>("jump");

        audioSrc= GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip){
        switch(clip){
            case "jump":
                audioSrc.PlayOneShot(playerJump);
                break;
            case "dash":
                audioSrc.PlayOneShot(playerDash);
                break;
        }
        
    }
}
