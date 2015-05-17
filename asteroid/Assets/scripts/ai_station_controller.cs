using UnityEngine;
using System.Collections;

public class ai_station_controller : MonoBehaviour {

	public GameObject orbitee;
	private Transform center;
	private Vector3 axis;
	public Vector3 desiredPosition;
	public float radius = 30f;
	public float radiusSpeed = 0.5f;
	public float rotationSpeed = 80.0f;
	
	void Start () {
		axis = new Vector3(0,0,1);
		center = orbitee.transform;
		transform.position = (transform.position - center.position).normalized * radius + center.position;
	}
	
	void Update () {
		transform.RotateAround (center.position, axis, rotationSpeed * Time.deltaTime);
		desiredPosition = (transform.position - center.position).normalized * radius + center.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
	}
}
