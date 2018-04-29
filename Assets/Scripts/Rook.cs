using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Unit
{
	public GameObject fissurePrefab;

	public override void Fire()
	{
		var fis = Instantiate(fissurePrefab, transform.position, Quaternion.identity);
		fis.GetComponent<Fissure>().StartHit(damage, range + 1, targetLayer);
	}
}
