using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour
{
	public Player player;
	public Canvas canvas;
	public RectTransform selectBox;
	public string mouseOver;
	public Shrine mouseOverShrine;
	public Unit mouseOverUnit;
	public Text popup;
	public GameObject alertPrefab;
	public GameObject unitAlertPrefab;
	public GameObject statIconPrefab;
	public GoldMine mouseOverGold;

	bool isSelecting = false;
	bool click = false;
	Vector3 mousePosition1;
	Vector3 mousePosition2;
	Vector3 hitPos;
	GameObject hitObj;
	GameObject sbGO;

	public static List<SelectableUnit> selectedObjects = new List<SelectableUnit>();
	public static List<SelectableUnit> suc = new List<SelectableUnit>();

	Camera cam;
	Vector2 v2 = new Vector2();
	Vector2 v2f = new Vector2();

	void Start()
	{
		player = GetComponent<Player>();
		cam = Camera.main;
		sbGO = selectBox.gameObject;
		sbGO.SetActive(false);
	}

	void Update()
	{
		DrawStatusIcons();
	}

	void LateUpdate()
	{
		if (Input.GetMouseButtonUp(0) && !click)
		{
			if (mouseOver == "shrine" && !mouseOverShrine.bought)
			{
				if (player.BuyShrine(mouseOverShrine.UIindex))
				{
					mouseOverShrine.effects.SetActive(true);
					mouseOverShrine.bought = true;
					Alert("NEW UNIT AVAILABLE", Color.green, 2f);
				}
				else
				{
					Alert("INSUFFICIENT GOLD", Color.red);
				}
			}
			if (UnitSelection.selectedObjects.Count > 0)
			{
				if (mouseOver == "gold")
				{
					foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
					{
						Unit unit = selectedUnit.GetComponent<Unit>();
						if (unit.type == Unit.Type.PAWN)
						{
							if (mouseOverGold.occupied)
							{
								Alert("ALREADY MINING", Color.red);
							}
							else
							{
								selectedObjects.Remove(selectedUnit);
								selectedUnit.selectionMark.enabled = false;
								unit.SeekMine(mouseOverGold);
							}
							break;
						}
					}
				}
				else if (mouseOver == "enemy")
				{
					foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
					{
						Unit unit = selectedUnit.GetComponent<Unit>();
						unit.SeekAttack(mouseOverUnit.gameObject);
					}
				}
				else
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 100, 1 << 11))
					{
						hitPos = hit.point;
						foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
						{
							Unit unit = selectedUnit.GetComponent<Unit>();
							unit.Move(hitPos);
						}
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			click = false;
		}
		if (!isSelecting)
		{
			if (Input.GetMouseButtonDown(0))
			{
				mousePosition1 = Input.mousePosition;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 100, 1 << 8))
				{
					var obj = hit.collider.GetComponent<SelectableUnit>();
					selectedObjects.Add(obj);
					obj.selectionMark.enabled = true;
					click = true;
				}
			}
			else if (Input.GetMouseButtonDown(1))
			{
				if (mouseOver == "unit")
				{
					var unit = mouseOverUnit.GetComponent<SelectableUnit>();
					selectedObjects.Remove(unit);
					unit.selectionMark.enabled = false;
				}
				else
				{
					foreach (SelectableUnit unit in selectedObjects)
					{
						unit.selectionMark.enabled = false;
					}
					selectedObjects.Clear();
				}
			}
			else if (Input.GetKeyDown(KeyCode.Q))
			{
				foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
				{
					selectedUnit.GetComponent<Unit>().Sentry = false;
					selectedUnit.showIcon = false;
					selectedUnit.statIcon.SetActive(false);
				}
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
				{
					if (selectedUnit.GetComponent<Unit>().type != Unit.Type.KING)
					{
						selectedUnit.GetComponent<Unit>().Sentry = true;
						selectedUnit.showIcon = true;
						selectedUnit.statIcon.SetActive(true);
					}
				}
			}
			else if (Input.GetMouseButton(0))
			{
				mousePosition2 = Input.mousePosition;
				if (mousePosition1.x > mousePosition2.x + 10 || mousePosition1.x < mousePosition2.x - 10 || mousePosition1.y > mousePosition2.y + 10 || mousePosition1.y < mousePosition2.y - 10)
				{
					click = true;
					isSelecting = true;
					/*
					selectedObjects.Clear();
					for (int i = 0; i < suc.Count; i++)
					{
						suc[i].selectionMark.enabled = false;
					}
					*/
					sbGO.SetActive(true);
					DrawSelectBox();
				}
			}
		}
		else
		{
			for (int i = 0; i < suc.Count; i++)
			{
				if (IsWithinSelectionBounds(suc[i].gameObject))
				{
					suc[i].selectionMark.enabled = true;
				}
				else
				{
					suc[i].selectionMark.enabled = false;
				}
			}
			DrawSelectBox();

			if (Input.GetMouseButtonUp(0))
			{
				selectedObjects.Clear();
				for (int i = 0; i < suc.Count; i++)
				{
					if (IsWithinSelectionBounds(suc[i].gameObject))
					{
						selectedObjects.Add(suc[i]);
					}
				}

				sbGO.SetActive(false);
				isSelecting = false;
				mousePosition1 = mousePosition2 = new Vector3(0, 0, 0);
			}
		}
	}

	public void DrawPopup(bool b, string type = null)
	{
		if (b)
		{
			if (type == "shrine")
			{
				if (mouseOverShrine.bought)
				{
					popup.color = Color.grey;
					popup.text = mouseOverShrine.typeString;
				}
				else
				{
					if (mouseOverShrine.cost > player.gold)
					{
						popup.color = Color.red;
					}
					else
					{
						popup.color = Color.green;
					}
					popup.text = "Activate " + mouseOverShrine.typeString + "\n" + mouseOverShrine.cost.ToString() + " Gold\n" + mouseOverShrine.popupInfo;
				}
			}
			else if (type == "gold")
			{
				if (mouseOverGold.occupied)
				{
					popup.text = "Gold Ore\n<MINING>";
				}
				else
				{
					popup.text = "Gold Ore";
				}
				if (UnitSelection.selectedObjects.Count > 0)
				{
					foreach (SelectableUnit selectedUnit in UnitSelection.selectedObjects)
					{
						Unit unit = selectedUnit.GetComponent<Unit>();
						if (unit.type == Unit.Type.PAWN)
						{
							if (!mouseOverGold.occupied)
							{
								popup.text = "Gold Ore\n<MINE>";
							}
							break;
						}
					}
				}
				popup.color = Color.yellow;
				popup.enabled = true;
			}
			else if (type == "enemy")
			{
				popup.color = Color.red;
				if (UnitSelection.selectedObjects.Count > 0)
				{
					popup.text = "Enemy " + mouseOverUnit.type.ToString() + "\nHP: " + mouseOverUnit.hp + "/" + mouseOverUnit.maxhp + "\n<ATTACK>";
				}
				else
				{
					popup.text = "Enemy " + mouseOverUnit.type.ToString() + "\nHP: " + mouseOverUnit.hp + "/" + mouseOverUnit.maxhp;
				}
			}
			else if (type == "unit")
			{
				popup.color = Color.green;
				popup.text = "Friendly " + mouseOverUnit.type.ToString() + "\nHP: " + mouseOverUnit.hp + "/" + mouseOverUnit.maxhp;
			
			}
			popup.rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x + 80f, Input.mousePosition.y - 110f);
			popup.enabled = true;
		}
		else
		{
			popup.enabled = false;
		}
	}

	public void DrawSelectBox()
	{
		Vector3 screenPosition1 = mousePosition1, screenPosition2 = Input.mousePosition;

		screenPosition1.x -= Screen.width / 2;
		screenPosition1.y -= Screen.height / 2;

		v2.Set(screenPosition2.x - mousePosition1.x, mousePosition1.y - screenPosition2.y);
		v2f.x = Mathf.Abs(v2.x);
		v2f.y = Mathf.Abs(v2.y); 
		selectBox.sizeDelta = v2f;

		if (v2.x < 0)
			screenPosition1.x -= selectBox.sizeDelta.x / 2;
		else
			screenPosition1.x += selectBox.sizeDelta.x / 2;
		if (v2.y < 0)
			screenPosition1.y += selectBox.sizeDelta.y / 2;
		else
			screenPosition1.y -= selectBox.sizeDelta.y / 2;

		selectBox.localPosition = screenPosition1;
	}

	public bool IsWithinSelectionBounds(GameObject gameObject)
	{
		if (!isSelecting)
			return false;

		Bounds viewportBounds = Utils.GetViewportBounds(cam, mousePosition1, Input.mousePosition);

		return viewportBounds.Contains(cam.WorldToViewportPoint(gameObject.transform.position));
	}

	public void Alert(string s, Color c, float timeOut = 1f, float x = 80f, float y = 80f)
	{
		var alertObj = Instantiate(alertPrefab, canvas.transform.position, Quaternion.identity, canvas.transform);
		var alert = alertObj.GetComponent<Text>();
		alert.text = s;
		alert.color = c;
		alert.rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x + x, Input.mousePosition.y + y);
		Destroy(alertObj, timeOut);
	}

	public void UnitAlert(GameObject unit, string s, Color c, float timeOut = 1f)
	{
		Vector2 screenPos = cam.WorldToScreenPoint(unit.transform.position);
		var alertObj = Instantiate(unitAlertPrefab, canvas.transform.position, Quaternion.identity, canvas.transform);
		alertObj.GetComponent<Alert>().SetPos(unit.transform.position);
		var alert = alertObj.GetComponent<Text>();
		alert.text = s;
		alert.color = c;
		alert.rectTransform.anchoredPosition = new Vector2(screenPos.x, screenPos.y + 40f);
		Destroy(alertObj, timeOut);
	}

	void DrawStatusIcons()
	{
		foreach (SelectableUnit s in suc)
		{
			if (s.showIcon)
			{
				Vector3 screenPos = cam.WorldToScreenPoint(s.transform.position);
				s.statIcon.GetComponent<Text>().rectTransform.anchoredPosition = new Vector2(screenPos.x, screenPos.y + 20f);
			}
		}
	}
}