using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Unit
{
	public GameObject bulletPrefab;
	public float shotForce;

	public override void Fire()
	{
		var shot = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);
		var bullet = shot.GetComponent<Bullet>();
		bullet.friendly = friendly;
		bullet.damage = damage;
		var rb = shot.GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * shotForce);
		float v = (shotForce / rb.mass) * Time.fixedDeltaTime;
		Destroy(shot, (range + 1f) / v);
	}

	void Update()
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		if (seekAttack && timer <= recover)
		{
			if (attackTarget == null)
			{
				seekAttack = false;
				state = State.IDLE;
			}
			else
			{
				dist = Vector3.Distance(this.transform.position, attackTarget.transform.position);
				if (dist <= range)
				{
					var lookDir = attackTarget.transform.position;
					lookDir.y = 0f;
					transform.LookAt(lookDir, Vector3.up);
					if (timer <= 0f)
					{
						Attack();
					}
				}
			}
		}
	}

	public override bool Sentry {
		get { return sentry; }
	}

	public override void Attack()
	{
		bool blocked = false;
		dist = Vector3.Distance(this.transform.position, attackTarget.transform.position);
		var cols = Physics.OverlapBox(transform.forward * ((dist + 1f) / 2f) + transform.position, new Vector3(0.5f, 0.5f, dist / 2.1f), transform.rotation, rayLayers);
		foreach (var col in cols)
		{
			if (col.gameObject.layer == this.gameObject.layer || col.gameObject.layer == 9)
			{
				blocked = true;
				break;
			}
		}
		if (blocked)
		{
			return;
		}
		Fire();
		timer = cooldown;
		state = State.ATTACK;
	}

	public override void Move(Vector3 _target)
	{
		
	}

	public override void RequestPath(Vector3 _target)
	{
		
	}
}
