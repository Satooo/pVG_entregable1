using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    // Start is called before the first frame update
    private bool spawned = true;
    public GameObject player;
    public GameObject Boss;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist =  transform.position.x - player.transform.position.x;
        Debug.Log("    " + dist);
        if (dist >= 18f && dist <= 21f && spawned)
        {
            Debug.Log("ME ALEJO: " +  dist);
            Instantiate(Boss, this.transform.position, this.transform.rotation);
            spawned = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && spawned == false)
        {
            spawned = true;
            Debug.Log("spawn");
        }
    }
}
