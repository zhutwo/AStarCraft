using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Unit
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
}
