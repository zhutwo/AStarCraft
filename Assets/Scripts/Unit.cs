using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	public bool friendly;
	public Type type;
	public Sprite portrait;
	public GameObject explodePrefab;
	public Transform muzzle;

	public float speed;
	public float range;
	public float cooldown;
	public int damage;
	public int hp;
	public int maxhp;
	public string attackName;
	public string otherInfo;

	public static List<Unit> white = new List<Unit>();
	public static List<Unit> black = new List<Unit>();

	protected List<Unit> enemies = new List<Unit>();
	protected GameObject attackTarget;
	protected int targetLayer;
	protected float timer;
	protected float recover;
	protected float dist;

	Player player;
	UnitSelection unitSel;
	Vector3 target;
	Vector3[] path;
	public State state = State.IDLE;
	GoldMine activeGold;
	Node currentNode;
	protected LayerMask rayLayers;
	GameObject attacker;
	PathGrid grid;
	LineRenderer line;

	int knockbackIdx;
	bool knockback;
	int obstacles;
	int typenum;
	int targetIndex;
	bool seekGold;
	public bool seekAttack;
	bool transcendent;
	protected bool sentry = false;


	public enum State
	{
		MOVE,
		ATTACK,
		MINE,
		IDLE
	}

	public enum Type
	{
		PAWN,
		KNIGHT,
		BISHOP,
		ROOK,
		QUEEN,
		KING
	}

	void Start()
	{
		player = Camera.main.GetComponent<Player>();
		unitSel = Camera.main.GetComponent<UnitSelection>();
		var aStar = GameObject.Find("A*");
		grid = aStar.GetComponent<PathGrid>();
		currentNode = grid.NodeFromWorldPoint(this.gameObject.transform.position);
		currentNode.occupied = true;
		rayLayers = (1 << 8 | 1 << 9 | 1 << 12);
		recover = cooldown - 1f;
		switch (type)
		{
		case Type.PAWN:
			typenum = 0;
			break;
		case Type.KNIGHT:
			typenum = 1;
			recover = 1f;
			break;
		case Type.BISHOP:
			typenum = 2;
			transcendent = true;
			break;
		case Type.ROOK:
			typenum = 3;
			transcendent = true;
			break;
		case Type.QUEEN:
			typenum = 4;
			this.GetComponent<Queen>().StartHeal();
			break;
		default:
			break;
		}
		if (friendly)
		{
			line = GetComponent<LineRenderer>();
			line.enabled = false;
			white.Add(this);
			targetLayer = 1 << 12;
			if (type != Type.KING)
			{
				player.unitCounts[typenum]++;
				player.UpdateUI();
			}
		}
		else
		{
			black.Add(this);
			targetLayer = 1 << 8;
		}
	}

	void Update()
	{
		UpdateLinePath();
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		if (timer <= recover)
		{
			if (sentry && !seekAttack)
			{
				if (enemies.Count > 0)
				{
					dist = 999f;
					float d;
					var e = enemies.ToArray();
					for (int i = 0; i < e.Length; i++)
					{
						if (e[i] == null)
						{
							enemies.Remove(e[i]);
						}
						else
						{
							d = Vector3.Distance(this.transform.position, e[i].transform.position);
							if (d < dist)
							{
								dist = d;
								attackTarget = e[i].gameObject;
							}
						}
					}
					SeekAttack(attackTarget);
				}
			}
			else if (state == State.IDLE)
			{
				if (enemies.Count > 0)
				{
					dist = 999f;
					float d;
					var e = enemies.ToArray();
					for (int i = 0; i < e.Length; i++)
					{
						if (e[i] == null)
						{
							enemies.Remove(e[i]);
							if (e.Length == 1)
							{
								return;
							}
						}
						else
						{
							d = Vector3.Distance(this.transform.position, e[i].transform.position);
							if (d < dist)
							{
								dist = d;
								attackTarget = e[i].gameObject;
							}
						}
					}
					var lookDir = attackTarget.transform.position;
					lookDir.y = 0f;
					transform.LookAt(lookDir, Vector3.up);
					if (timer <= 0f)
					{
						Attack();
						state = State.IDLE;
					}
				}
			}
			else if (seekAttack)
			{
				if (attackTarget == null)
				{
					seekAttack = false;
					state = State.IDLE;
				}
				else
				{
					dist = Vector3.Distance(this.transform.position, attackTarget.transform.position);
					if (dist > range)
					{
						if (state != State.MOVE)
						{
							StopCoroutine("FollowPath");
							RequestPath(attackTarget.transform.position);
						}
					}
					else
					{
						var lookDir = attackTarget.transform.position;
						lookDir.y = 0f;
						transform.LookAt(lookDir, Vector3.up);
						if (timer <= 0f)
						{
							Attack();
						}
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		if (knockbackIdx > 0)
		{
			knockbackIdx--;
		}
		if (knockback)
		{
			if (knockbackIdx == 0)
			{
				knockback = false;
				var pos = Pathfinder.NearestOpenPosition(transform.position, grid);
				RequestPath(pos);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
		{
			var unit = other.gameObject.GetComponent<Unit>();
			if (unit.friendly != friendly)
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
			if (unit.friendly != friendly)
			{
				enemies.Remove(unit);
			}
		}
	}

	public void SeekAttack(GameObject t)
	{
		if (friendly)
		{
			line.positionCount = 0;
			line.enabled = false;
		}
		attackTarget = t;
		seekGold = false;
		seekAttack = true;
		dist = Vector3.Distance(this.transform.position, attackTarget.transform.position);
		if (dist > range)
		{
			StopCoroutine("FollowPath");
			RequestPath(attackTarget.transform.position);
		}
		StopCoroutine("Mine");
	}

	public virtual void Attack()
	{
		if (!transcendent)
		{
			bool blocked = false;
			var cols = Physics.OverlapBox(transform.forward * ((dist + 1f) / 2) + transform.position, new Vector3(0.5f, 0.5f, dist / 2.1f), transform.rotation, rayLayers);
			foreach (var col in cols)
			{
				if (col.gameObject.layer == this.gameObject.layer || col.gameObject.layer == 9)
				{
					blocked = true;
					break;
				}
			}
			if (blocked)
			{
				if (state != State.MOVE)
				{
					RequestPath(attackTarget.transform.position);
					return;
				}
				else
				{
					return;
				}
			}
		}
		if (state == State.MOVE)
		{
			var node = grid.NodeFromWorldPoint(this.gameObject.transform.position);
			if (node.occupied)
			{
				return;
			}
			StopCoroutine("FollowPath");
			currentNode.occupied = false;
			currentNode = node;
			currentNode.occupied = true;
		}
		Fire();
		timer = cooldown;
		state = State.ATTACK;

		/*
		RaycastHit hit;
		if (Physics.Raycast(muzzle.transform.position, transform.forward, out hit, range, rayLayers))
		{
			if (hit.collider.gameObject == attackTarget)
			{
				StopCoroutine("FollowPath");
				Fire();
				timer = cooldown;
			}
		}
		*/
	}

	/*
	IEnumerator Attack()
	{
		while (attackTarget != null)
		{
			state = State.ATTACK;
			var lookDir = attackTarget.transform.position;
			lookDir.y = 0;
			transform.LookAt(lookDir, Vector3.up);
			RaycastHit hit;
			if (Physics.Raycast(muzzle.transform.position, transform.forward, out hit, range, rayLayers))
			{
				if (hit.collider.gameObject == attackTarget)
				{
					timer = cooldown;
					if (Fire())
					{
						seekAttack = false;
						state = State.IDLE;
						yield break;
					}
					yield return new WaitForSeconds(cooldown);
				}
				else
				{
					yield break;
				}
			}

		}
	}
	*/

	public virtual bool Sentry {
		get { return sentry; }
		set { sentry = value; }
	}

	public virtual void Fire()
	{
		
	}

	public void Knockback()
	{
		knockbackIdx = 2;
		knockback = true;
	}

	public bool TakeDamage(int d)
	{
		unitSel.UnitAlert(this.gameObject, d.ToString(), Color.red, 0.5f);
		hp -= d;
		if (hp <= 0)
		{
			if (friendly)
			{
				var s = this.gameObject.GetComponent<SelectableUnit>();
				UnitSelection.selectedObjects.Remove(s);
				UnitSelection.suc.Remove(s);
				if (friendly && type != Type.KING)
				{
					player.unitCounts[typenum]--;
					player.UpdateUI();
				}
				white.Remove(this);
			}
			black.Remove(this);
			Instantiate(explodePrefab, transform.position, transform.rotation);
			transform.position = transform.position + transform.up * -999f;
			Destroy(this.gameObject);
			return true;
		}
		if (sentry && !seekAttack)
		{
			bool found = false;
			foreach (Unit enemy in friendly ? black : white)
			{
				if (enemy.attackTarget == this.gameObject)
				{
					attacker = enemy.gameObject;
					found = true;
					break;
				}
			}
			if (found && attacker != null)
			{
				SeekAttack(attacker);
			}
		}
		return false;
	}

	public void TakeHeal(int h)
	{
		if (hp == maxhp)
		{
			return;
		}
		var old = hp;
		hp += h;
		if (hp > maxhp)
		{
			hp = maxhp;
		}
		old = hp - old;
		unitSel.UnitAlert(this.gameObject, old.ToString(), Color.green, 0.5f);
	}

	public void SeekMine(GoldMine g)
	{
		if (friendly)
		{
			line.positionCount = 0;
			line.enabled = false;
		}
		if (g != activeGold && activeGold != null)
		{
			activeGold.occupied = false;
		}
		activeGold = g;
		target = g.GetComponent<Transform>().position;
		RequestPath(target);
		activeGold.occupied = true;
		seekAttack = false;
		seekGold = true;
	}

	IEnumerator Mine()
	{
		state = State.MINE;
		while (true)
		{
			player.GetGold();
			unitSel.UnitAlert(this.gameObject, "+" + player.mineRate.ToString() + "G", Color.yellow);
			yield return new WaitForSeconds(2f);
		}
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful, Node curr)
	{
		if (pathSuccessful)
		{
			state = State.MOVE;
			path = newPath;
			targetIndex = 0;
			currentNode.occupied = false;
			currentNode = curr;
			StopCoroutine("FollowPath");
			StopCoroutine("Mine");
			StartCoroutine("FollowPath");
			if (!seekGold && activeGold != null)
			{
				activeGold.occupied = false;
				activeGold = null;
			}
			SetLinePath();
		}
	}

	public virtual void Move(Vector3 _target)
	{
		target = _target;
		seekGold = seekAttack = false;
		PathRequestManager.RequestPath(transform.position, _target, OnPathFound);
	}

	public virtual void RequestPath(Vector3 _target)
	{
		PathRequestManager.RequestPath(transform.position, _target, OnPathFound);
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];
		while (true)
		{
			if (timer <= recover)
			{
				if (transform.position == currentWaypoint)
				{
					targetIndex++;

					if (targetIndex >= path.Length)
					{
						if (seekGold)
						{
							seekGold = false;
							StartCoroutine("Mine");
						}
						else
						{
							state = State.IDLE;
						}
						if (friendly)
						{
							line.positionCount = 0;
							line.enabled = false;
						}
						yield break;
					}
					if (friendly)
					{
						line.positionCount = path.Length - targetIndex + 1;
						line.SetPosition(0, transform.position);
						for (int i = 0; i < path.Length - targetIndex; i++)
						{
							line.SetPosition(i + 1, path[i + targetIndex]);
						}
					}
					currentWaypoint = path[targetIndex];
				}
				else
				{
					if (type == Type.KNIGHT)
					{
						transform.LookAt(currentWaypoint, Vector3.up);
					}
					transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
				}
			}
			yield return null;
		}
	}

	void OnMouseEnter()
	{
		unitSel.mouseOverUnit = this;
		if (friendly)
		{
			unitSel.mouseOver = "unit";
		}
		else
		{
			unitSel.mouseOver = "enemy";
		}
	}

	void OnMouseOver()
	{
		if (friendly)
		{
			unitSel.DrawPopup(true, "unit");
		}
		else
		{
			unitSel.DrawPopup(true, "enemy");
		}
	}

	void OnMouseExit()
	{
		unitSel.mouseOverUnit = null;
		unitSel.mouseOver = null;
		unitSel.DrawPopup(false);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == 9)
		{
			obstacles++;
		}
	}

	void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == 9)
		{
			obstacles--;
		}
	}

	public bool Colliding()
	{
		if (obstacles > 0)
		{
			return true;
		}
		return false;
	}

	public void OnDrawGizmos()
	{
		if (path != null && state == State.MOVE)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				if (seekAttack)
					Gizmos.color = Color.red;
				else if (seekGold)
					Gizmos.color = Color.yellow;
				else
					Gizmos.color = Color.green;
				
				Gizmos.DrawSphere(path[i], 0.2f);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}

	void SetLinePath()
	{
		if (friendly)
		{
			if (path != null && state == State.MOVE)
			{
				if (seekAttack)
				{
					line.startColor = line.endColor = Color.red;
				}
				else if (seekGold)
				{
					line.startColor = line.endColor = Color.yellow;
				}
				else
				{
					line.startColor = line.endColor = Color.green;
				}
				line.positionCount = path.Length + 1;
				line.SetPosition(0, transform.position);
				for (int i = 0; i < path.Length; i++)
				{
					line.SetPosition(i + 1, path[i]);
				}
				line.enabled = true;
			}
		}
	}

	void UpdateLinePath()
	{
		if (friendly)
		{
			if (line.enabled)
				line.SetPosition(0, transform.position);
		}
	}
}