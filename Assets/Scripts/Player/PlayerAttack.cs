using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform FirePositionDer;
    public Transform FirePositionIzq;
    public GameObject Fireball;
    // Start is called before the first frame update
    void Start()
    {
        FirePositionDer = this.transform.GetChild(1).transform;
        FirePositionIzq = this.transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.pause)
        {
            if (Input.GetMouseButtonDown(1))
            {
                var soundEffect = Random.Range(0f, 101.0f);
                if (soundEffect <= 10)
                {
                    SoundManagerScript.PlaySound(SoundManagerScript.SoundType.fire);
                }
                Fire();
            }
        }
        
    }
    public void Fire()
    {
        if (transform.localScale.x == 1)
        {
            Instantiate(Fireball, FirePositionDer.position, FirePositionDer.rotation);

        }
        if (transform.localScale.x == -1)
        {
            Instantiate(Fireball, FirePositionIzq.position, FirePositionIzq.rotation);

        }
        
    }
}
