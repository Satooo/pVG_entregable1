using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
	GameObject player;
	public bool flip = false;


	public int attackDamage = 20;
	public int enragedAttackDamage = 40;
	public float damage = 10;
	public Vector3 attackOffset;
	public float attackRange = 3f;
	public LayerMask attackMask;
	public SwordAttack Sword;
	// Start is called before the first frame update
	void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	public void Flip()
	{
		Vector3 flipped = transform.localScale;
		flipped.z *= -1f;

		if (transform.position.x > player.transform.position.x && flip)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			flip = false;
		}
		else if (transform.position.x < player.transform.position.x && !flip)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			flip = true;
		}
	}
	public void Attack()
	{
		Sword.Attack();
	}
}
