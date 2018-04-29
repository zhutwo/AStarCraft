using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
	public ParticleSystem flame;

	public override void Fire()
	{
		flame.Play();
		RaycastHit[] hits;
		hits = Physics.RaycastAll(muzzle.transform.position, transform.forward, range, targetLayer);
		for (int i = 0; i < hits.Length; i++)
		{
			RaycastHit hit = hits[i];
			var unit = hit.transform.GetComponent<Unit>();
			unit.TakeDamage(damage);
		}
	}
}
