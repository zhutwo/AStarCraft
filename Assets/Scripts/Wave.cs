using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
	public GameObject parent;
	private float speed = 4f;
	private int damage;
	private float range;
	private bool friendly;
	private List<Unit> units = new List<Unit>();

	public void Set(int d, bool f)
	{
		damage = d;
		friendly = f;
		Destroy(parent, 3f);
	}
	
	// Update is called once per frame
	void Update()
	{
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly != friendly)
			{
				units.Add(unit);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly != friendly)
			{
				units.Remove(unit);
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly != friendly && unit.type != Unit.Type.ROOK && unit.type != Unit.Type.KING)
			{
				if (!unit.Colliding())
				{
					unit.transform.position += transform.forward * speed * Time.deltaTime;
					unit.Knockback();
				}
			}
		}
	}

	IEnumerator Damage()
	{
		while (true)
		{
			foreach (Unit unit in units)
			{
				unit.TakeDamage(damage);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	void Start()
	{
		StartCoroutine(Damage());
	}
}
