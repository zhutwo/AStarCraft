using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class Bishop : Unit
{
	public GameObject boltPrefab;
	private GameObject temp;

	public override void Fire()
	{
		Vector3 pos = new Vector3(attackTarget.transform.position.x, attackTarget.transform.position.y + 10f, attackTarget.transform.position.z);
		var bolt = Instantiate(boltPrefab, pos, Quaternion.identity);
		var script = bolt.GetComponent<LightningBoltScript>();
		script.EndObject = attackTarget;
		temp = attackTarget;
		Invoke("DelayedDamage", 1.2f);
	}

	void DelayedDamage()
	{
		if (temp != null)
			temp.GetComponent<Unit>().TakeDamage(damage);
	}
}
