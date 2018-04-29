using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
	GameObject parent;
	Text text;
	Vector2 screenPos;
	Vector3 worldPos;
	Camera cam;

	// Use this for initialization
	void Start()
	{
		cam = Camera.main;
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update()
	{
		screenPos = cam.WorldToScreenPoint(worldPos);
		text.rectTransform.anchoredPosition = new Vector2(screenPos.x, screenPos.y + 40f);
	}

	public void SetParent(GameObject obj)
	{
		parent = obj;
		worldPos = obj.transform.position;
	}

	public void SetPos(Vector3 pos)
	{
		worldPos = pos;
	}
}
