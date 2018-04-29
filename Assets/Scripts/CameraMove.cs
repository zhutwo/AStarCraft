using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed;
	public float maxX, minX, maxY, minY, maxZ, minZ;

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.W) && transform.position.x != maxX)
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S) && transform.position.x != minX)
		{
			transform.position -= transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A) && transform.position.z != maxZ)
		{
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D) && transform.position.z != minZ)
		{
			transform.position -= transform.forward * speed * Time.deltaTime;
		}
		transform.position += transform.up * Input.mouseScrollDelta.y;
		if (transform.position.x > maxX)
		{
			transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
		}
		else if (transform.position.x < minX)
		{
			transform.position = new Vector3(minX, transform.position.y, transform.position.z);
		}
		if (transform.position.y > maxY)
		{
			transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
		}
		else if (transform.position.y < minY)
		{
			transform.position = new Vector3(transform.position.x, minY, transform.position.z);
		}
		if (transform.position.z > maxZ)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
		}
		else if (transform.position.z < minZ)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
		}
	}
}
