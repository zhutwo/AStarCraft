using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour
{
	//public UnitSelection unitSel;

	public int gold;
	public int mineRate;
	public Text goldUI;
	public Text inspectText;
	public Image portrait;

	public GameObject[] unitPrefabs = new GameObject[5];
	public Transform[] unitSpawns = new Transform[5];
	public Text[] unitText = new Text[5];
	public Button[] unitButtons = new Button[5];
	public int[] unitCosts = new int[5];
	public int[] unitCounts = new int[5];
	public bool[] unitUnlock = new bool[5];
	public static int[] shrineCost = new int[5] { 9999, 200, 400, 500, 800 };

	private static int[] unitMax = new int[5] { 8, 4, 4, 2, 1 };

	PathGrid grid;

	void Start()
	{
		//unitSel = Camera.main.GetComponent<UnitSelection>();
		unitUnlock[0] = true;
		var a = GameObject.Find("A*");
		grid = a.GetComponent<PathGrid>();
		UpdateUI();
		//currentNode = aStar.GetComponent<PathGrid>().NodeFromWorldPoint(this.gameObject.transform.position);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			gold += 1000;
			UpdateUI();
		}
		if (UnitSelection.selectedObjects.Count > 0)
		{
			portrait.enabled = true;
			inspectText.enabled = true;
			SelectableUnit s = UnitSelection.selectedObjects.Last();

			Unit unit = s.gameObject.GetComponent<Unit>();
			/*
			if (unit.friendly)
			{
				inspectText.color = Color.green;
			}
			else
			{
				inspectText.color = Color.red;
			}
			*/
			portrait.sprite = unit.portrait;
			inspectText.text = ((unit.friendly) ? "Friendly " : "Enemy ") + unit.type.ToString()
			+ "\nHealth: " + unit.hp.ToString() + "/" + unit.maxhp.ToString()
			+ "\nCasts: " + unit.attackName
			+ "\n" + unit.otherInfo;
		}
		else
		{
			portrait.enabled = false;
			inspectText.enabled = false;
		}
	}

	public void UpdateUI()
	{
		goldUI.text = gold.ToString();
		for (int i = 0; i < 5; i++)
		{
			unitText[i].text = unitCounts[i].ToString() + "/" + unitMax[i].ToString();
			if (unitUnlock[i] && gold >= unitCosts[i] && unitCounts[i] < unitMax[i])
			{
				unitButtons[i].interactable = true;
			}
			else
			{
				unitButtons[i].interactable = false;
			}
		}
	}

	public bool BuyShrine(int i)
	{
		if (i < 1 || i > 4)
		{
			print("Player.BuyShrine index out of range");
			return false;
		}
		if (gold >= shrineCost[i])
		{
			unitUnlock[i] = true;
			gold -= shrineCost[i];
			UpdateUI();
			return true;
		}
		return false;
	}

	public void GetGold()
	{
		gold += mineRate;
		UpdateUI();
	}

	public void BuildUnit(int i)
	{
		Vector3 pos = Pathfinder.NearestOpenPosition(unitSpawns[i].position, grid);
		Instantiate(unitPrefabs[i], pos, Quaternion.identity);
		gold -= unitCosts[i];
		UpdateUI();
	}

}
