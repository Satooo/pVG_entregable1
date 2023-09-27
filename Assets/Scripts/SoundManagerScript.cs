using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    private static AudioClip playerJump, playerDash, PlayerFire, PlayerDeath;
    private static AudioSource audioSrc;
    public static AudioSource BackMusic;
    public enum SoundType
    {
        Jump,
        Dash,
        fire,
        Death
    }
    

    // MonoBehaviour class method
    // Start is called before the first frame update
    void Start()
    {
        // ShortAudios
        playerDash = Resources.Load<AudioClip>("whoosh");
        playerJump = Resources.Load<AudioClip>("jump");
        PlayerFire = Resources.Load<AudioClip>("Fire");
        PlayerDeath = Resources.Load<AudioClip>("Fatality");
        BackMusic = transform.GetChild(0).GetComponent<AudioSource>();
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
            case SoundType.fire:
                audioSrc.PlayOneShot(PlayerFire);
                break;
            case SoundType.Death:
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(PlayerDeath);
                BackMusic.Stop();
                break;
        }
        
    }
}
