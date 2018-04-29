using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shrine : MonoBehaviour
{
	UnitSelection unitSel;

	public int cost;
	public GameObject effects;
	public Type type;
	public bool bought;
	public string typeString;
	public string popupInfo;

	public enum Type
	{
		FIRE,
		WATER,
		EARTH,
		WIND
	}

	public int UIindex;

	// Use this for initialization
	void Start()
	{
		unitSel = Camera.main.GetComponent<UnitSelection>();
		switch (type)
		{
		case Type.FIRE:
			UIindex = 1;
			cost = Player.shrineCost[UIindex];
			typeString = "FIRE SHRINE";
			popupInfo = "Unlocks Knight";
			break;
		case Type.WIND:
			UIindex = 2;
			cost = Player.shrineCost[UIindex];
			typeString = "WIND SHRINE";
			popupInfo = "Unlocks Bishop";
			break;
		case Type.EARTH:
			UIindex = 3;
			cost = Player.shrineCost[UIindex];
			typeString = "EARTH SHRINE";
			popupInfo = "Unlocks Rook";
			break;
		case Type.WATER:
			UIindex = 4;
			cost = Player.shrineCost[UIindex];
			typeString = "WATER SHRINE";
			popupInfo = "Unlocks Queen";
			break;
		}
	}

	void OnMouseEnter()
	{
		unitSel.mouseOver = "shrine";
		unitSel.mouseOverShrine = this;
	}

	void OnMouseOver()
	{
		unitSel.DrawPopup(true, "shrine");
	}

	void OnMouseExit()
	{
		unitSel.mouseOver = null;
		unitSel.mouseOverShrine = null;
		unitSel.DrawPopup(false);
	}
}
