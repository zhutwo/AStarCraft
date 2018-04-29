using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fissure : MonoBehaviour
{
	public void StartHit(int d, float r, int l)
	{
		StartCoroutine(Hit(d, r, l));
		Destroy(this.gameObject, 5f);
	}

	IEnumerator Hit(int damage, float range, int layer)
	{
		yield return new WaitForSeconds(0.5f);
		var cols = Physics.OverlapSphere(transform.position, range);
		foreach (var col in cols)
		{
			if (1 << col.gameObject.layer == layer)
			{
				var unit = col.gameObject.GetComponent<Unit>();
				unit.TakeDamage(damage);
			}
		}
	}
}
