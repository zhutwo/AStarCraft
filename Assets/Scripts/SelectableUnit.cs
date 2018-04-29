using UnityEngine;
using UnityEngine.UI;

public class SelectableUnit : MonoBehaviour
{
	public Projector selectionMark;
	public GameObject statIcon;
	public bool showIcon;

	void Start()
	{
		var canvas = Camera.main.GetComponentInChildren<Canvas>();
		var sel = Camera.main.GetComponent<UnitSelection>();
		statIcon = Instantiate(sel.statIconPrefab, canvas.transform.position, Quaternion.identity, canvas.transform);
		statIcon.SetActive(false);
		statIcon.GetComponent<Text>().text = "@";
		UnitSelection.suc.Add(this);
	}

	void OnDestroy()
	{
		UnitSelection.suc.Remove(this);
		Destroy(statIcon);
	}
}