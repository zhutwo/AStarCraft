using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public bool friendly;
	public int damage;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly != friendly)
			{
				unit.TakeDamage(damage);
				Destroy(this.gameObject);
			}
		}
	}
}
