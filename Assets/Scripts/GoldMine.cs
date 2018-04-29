using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{

	public bool occupied;
	public UnitSelection unitSel;

	// Use this for initialization
	void Start()
	{
		unitSel = Camera.main.GetComponent<UnitSelection>();
	}

	void OnMouseEnter()
	{
		unitSel.mouseOver = "gold";
		unitSel.mouseOverGold = this;
		unitSel.DrawPopup(true, "gold");
	}

	void OnMouseOver()
	{
		unitSel.DrawPopup(true, "gold");
	}

	void OnMouseExit()
	{
		unitSel.mouseOver = null;
		unitSel.mouseOverGold = null;
		unitSel.DrawPopup(false);
	}
}
