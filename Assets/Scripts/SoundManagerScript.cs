using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    private static AudioClip playerJump, playerDash;
    private static AudioSource audioSrc;
    public enum SoundType
    {
        Jump,
        Dash
    }

    
    // MonoBehaviour class method
    // Start is called before the first frame update
    void Start()
    {
        // ShortAudios
        playerDash = Resources.Load<AudioClip>("whoosh");
        playerJump = Resources.Load<AudioClip>("jump");
        // AudioSource
        audioSrc = GetComponent<AudioSource>();
    }

    // MonoBehaviour class method
    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(SoundType clip){
        switch(clip){
            case SoundType.Jump:
                audioSrc.PlayOneShot(playerJump);
                break;
            case SoundType.Dash:
                audioSrc.PlayOneShot(playerDash);
                break;
        }
        
    }
}
