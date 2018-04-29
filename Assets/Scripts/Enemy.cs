using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	public GameObject king;

	public GameObject clearText;
	public GameObject spawnText;
	public GameObject spawnPause;

	public GameObject[] unit = new GameObject[5];
	public GameObject[] spawn = new GameObject[5];
	public Transform[] target = new Transform[5];
	public int[] amount = new int[5];

	int level;
	bool toSpawn;
	bool stopSpawn;
	bool auto;
	//PathGrid grid;

	// Use this for initialization
	void Start()
	{
		//var aStar = GameObject.Find("A*");
		//grid = aStar.GetComponent<PathGrid>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (toSpawn && !stopSpawn)
		{
			clearText.SetActive(true);
			Invoke("SpawnWave", 3f);
			stopSpawn = true;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SpawnClass(0, 1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SpawnClass(1, 1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SpawnClass(2, 1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SpawnClass(3, 1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SpawnClass(4, 1);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (auto)
			{
				auto = false;
				spawnPause.SetActive(true);
			}
			else
			{
				auto = true;
				spawnPause.SetActive(false);
			}
		}
		if (auto)
		{
			if (Unit.black.Count == 0)
			{
				toSpawn = true;
			}
		}
	}

	void SpawnWave()
	{
		clearText.SetActive(false);
		spawnText.SetActive(true);
		if (level < 5)
		{
			level++;
		}
		for (int i = 0; i < level; i++)
		{
			SpawnClass(i, amount[i]);
			if (level < 5)
			{
				amount[i]++;
			}
		}
		Invoke("Reset", 2f);
	}

	void Reset()
	{
		spawnText.SetActive(false);
		stopSpawn = false;
		toSpawn = false;
	}

	void SpawnClass(int index, int num)
	{
		for (int i = 0; i < num; i++)
		{
			//var pos = Pathfinder.NearestOpenPosition(spawn[index].transform.position, grid);
			var s = Instantiate(unit[index], spawn[index].transform.position, Quaternion.identity);
			var u = s.GetComponent<Unit>();
			u.Sentry = true;
			if (u.type == Unit.Type.KNIGHT)
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
						u.SeekAttack(white);
					else
						u.Move(target[index].position);
				}
				else
				{
					u.Move(target[index].position);
				}
			}
			else
			{
				var rand = Random.Range(0, 3);
				switch (rand)
				{
				case 0:
					u.Move(target[index].position);
					break;
				case 1:
					if (Unit.white.Count > 0)
					{
						int hp = 9999;
						GameObject white = null;
						foreach (Unit w in Unit.white)
						{
							if (w.hp < hp)
							{
								hp = w.hp;
								white = w.gameObject;
							}
						}
						if (white != null)
							u.SeekAttack(white);
						else
							u.Move(target[index].position);
					}
					else
					{
						u.Move(target[index].position);
					}
					break;
				case 2:
					if (Unit.white.Count > 0)
					{
						GameObject white = null;
						foreach (Unit w in Unit.white)
						{
							white = w.gameObject;
							break;
						}
						if (white != null)
							u.SeekAttack(white);
						else
							u.Move(target[index].position);
					}
					else
					{
						u.Move(target[index].position);
					}
					break;
				case 3:
					u.SeekAttack(king);
					break;
				}
			}
		}
	}
}
