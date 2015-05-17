using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ai_station_controller : MonoBehaviour {

	public GameObject orbitee;
	private Transform center;
	private Vector3 axis;
	public Vector3 desiredPosition;
	public float radius = 30f;
	public float radiusSpeed = 0.5f;
	public float rotationSpeed = 80.0f;

	public float fighters = 2f;
	public Transform launchPoint;
	public Transform approachPoint;
	public GameObject fighterType;
	public bool readyTolaunch  = true;

	public List<GameObject> activeFighters = new List<GameObject>();


	private float minOrbit = 30f;
	private float maxOrbit = 50f;


	void Start () {
		axis = new Vector3(0,0,1);
		center = orbitee.transform;
		transform.position = (transform.position - center.position).normalized * radius + center.position;

	}
	
	void Update () {
		transform.RotateAround (center.position, axis, rotationSpeed * Time.deltaTime);
		desiredPosition = (transform.position - center.position).normalized * radius + center.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
		if (fighters > 0) {
			if (readyTolaunch){
				StartCoroutine(launchFighter());

			}

		}
	}

	//docking procedure, send all orbiters away and bring ship wanting to dock in



	IEnumerator launchFighter(){
		readyTolaunch = false;
		fighters--;
		GameObject new_fighter = (GameObject)Instantiate(fighterType, launchPoint.position, this.transform.rotation);
		new_fighter.GetComponent<Rigidbody2D>().AddForce(this.transform.up.normalized * 1000f);
		new_fighter.GetComponentInChildren<ship_transponder> ().faction = this.GetComponent<ship_transponder> ().faction;
		ai_ship_orders orders = new_fighter.GetComponent<ai_ship_orders>();
		orders.TYPE = ship_library.AI_ORDERS_ORBIT;
		orders.minDistanceFromFollowee = Random.Range(minOrbit,maxOrbit);
		orders.maxDistanceFromWaypoint = 100f;
		orders.followee = this.gameObject;
		activeFighters.Add (new_fighter);

		yield return new WaitForSeconds(3f);
		readyTolaunch = true;
	}

}
