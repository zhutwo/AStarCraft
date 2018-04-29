using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Unit
{
	public GameObject wavePrefab;
	private List<Unit> heals = new List<Unit>();

	public override void Fire()
	{
		var wave = Instantiate(wavePrefab, transform.position, transform.rotation);
		var w = wave.GetComponentInChildren<Wave>();
		w.Set(damage, friendly);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly == friendly)
			{
				heals.Add(unit);
			}
			else
			{
				enemies.Add(unit);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly == friendly)
			{
				heals.Remove(unit);
			}
			else
			{
				enemies.Remove(unit);
			}
		}
	}

	public void StartHeal()
	{
		StartCoroutine(HealAura());
	}

	IEnumerator HealAura()
	{
		while (true)
		{
			var h = heals.ToArray();
			for (int i = 0; i < h.Length; i++)
			{
				if (h[i] == null)
				{
					heals.Remove(h[i]);
				}
				else
				{
					h[i].TakeHeal(damage);
				}
			}
			yield return new WaitForSeconds(2f);
		}
	}
}
