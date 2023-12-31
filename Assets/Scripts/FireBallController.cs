using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public float speed;
    private Rigidbody2D RbFireBall;
    private float Timer;
    public float timeToDestroy = 3f;
    public GameObject Explosion;
    // Start is called before the first frame update
    void Start()
    {
        speed = 10;
        RbFireBall = this.GetComponent<Rigidbody2D>();
        RbFireBall.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > timeToDestroy)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            enemy.SubstractDamage();
            GameManager.Instance.mainPlayerCurrentPower += 10f;
            ExplosionCreation();
            Destroy(gameObject);
        }
        if (collision.CompareTag("Boss"))
        {
            var enemy = collision.GetComponent<Boss>();
            enemy.SubstractDamage();
            GameManager.Instance.mainPlayerCurrentPower += 10f;
            ExplosionCreation();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Platform"))
        {
            ExplosionCreation();
            Destroy(gameObject);
        }
    }
    public void ExplosionCreation()
    {
        GameObject jose = Instantiate(Explosion, transform.position, Quaternion.identity);
    }
}
