using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{

	private List<Unit> heals = new List<Unit>();
	int heal = 20;

	void Start()
	{
		StartCoroutine(HealAura());
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			heals.Add(unit);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			heals.Remove(unit);
		}
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
					h[i].TakeHeal(heal);
				}
			}
			yield return new WaitForSeconds(2f);
		}
	}
}
