using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoalManager : MonoBehaviour
{
	int type;
	int index;
	Unit unit;
	Enemy enemy;

	// Use this for initialization
	void Start()
	{
		var e = GameObject.Find("Enemy");
		enemy = e.GetComponent<Enemy>();
		unit = gameObject.GetComponent<Unit>();
		switch (unit.type)
		{
		case Unit.Type.PAWN:
			index = 0;
			break;
		case Unit.Type.KNIGHT:
			index = 1;
			break;
		case Unit.Type.BISHOP:
			index = 2;
			break;
		case Unit.Type.ROOK:
			index = 3;
			break;
		case Unit.Type.QUEEN:
			index = 4;
			break;
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (unit.state == Unit.State.IDLE)
		{
			if (unit.type == Unit.Type.KNIGHT)
			{
				if (Unit.white.Count > 0)
				{
					GameObject white = null;
					foreach (Unit w in Unit.white)
					{
						if (w.type == Unit.Type.PAWN)
						{
							white = w.gameObject;
						}
					}
					if (white != null)
						unit.SeekAttack(white);
					else
						unit.Move(enemy.target[index].position);
				}
				else
				{
					unit.Move(enemy.target[index].position);
				}
			}
		}
	}
}