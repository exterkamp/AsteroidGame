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
	public Transform dockingPoint;
	public GameObject fighterType;
	public bool readyTolaunch  = true;

	public List<GameObject> activeFighters = new List<GameObject>();
	public GameObject dockedShip;
		
	public bool transport_dock_open = true;
	
	public ai_station_controller[] tradeNetwork;

	public int loadRate = 10;
	private int millisLoad = 500;
	public bool loading = false;

	public int population = 0;


	//private
	private float minOrbit = 30f;
	private float maxOrbit = 50f;

	private ship_resource_manager res;

	//private transport_bay bay;
	//private crew_quarters crew;



	void Start () {
		axis = new Vector3(0,0,1);
		center = orbitee.transform;
		transform.position = (transform.position - center.position).normalized * radius + center.position;
		population = Random.Range (100,500);

		res = GetComponentInChildren<ship_resource_manager> ();
		res.initialize (false,
		                Random.Range (1000000, 5000000),
		                Random.Range (50, 200),
		                Random.Range (500, 2500),
		                Random.Range (10, 100),
		                Random.Range (1, 50),
		                Random.Range (1000, 10000),
		                Random.Range (25, 200),
		                Random.Range (0, 25)
		                );

		//bay = GetComponentInChildren<transport_bay> ();
		//bay.initialize (Random.Range (1000000, 5000000),
						//Random.Range (50, 200),
						//Random.Range (500, 2500),
						//Random.Range (10, 100),
						//Random.Range (1, 50),
						//Random.Range (1000, 10000),
						//Random.Range (25, 200),
		                //Random.Range (0, 25));
		//crew = GetComponentInChildren<crew_quarters> ();
		//crew.initialize (false);

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
		if (dockedShip != null) {
			dockedShip.transform.position = dockingPoint.position;

			//launch a mission!

			if (!loading){
				//figure out what we have most of -->
				string mostAmt = "";
				int most = 0;
				foreach (string item in ship_library.LIST_RESOURCES){
					if (!item.Equals(ship_library.RESOURCE_MONEY)){
						int i = res.look (item);
						if (i > most){
							most = i;
							mostAmt = item;
						}
					}
				}

				StartCoroutine(loadTransport(mostAmt,100));
				loading = true;
			}


		}
	}


	public bool requestDocking(){
		if (transport_dock_open) {
			return true;
		} else {
			return false;
		}
	}

	public bool dockShip(GameObject ship){
		if (Vector3.Distance (ship.transform.position, dockingPoint.position) < 2) {
			ship.transform.position = dockingPoint.position;
			ship.transform.rotation = dockingPoint.rotation;
			dockedShip = ship;
			return true;
		}
		return false;
	}

	public void releaseDockedShip(){
		dockedShip.GetComponent<ai_ship_orders> ().docked = false;
		dockedShip = null;
		transport_dock_open = true;
	}



	//docking procedure, send all orbiters away and bring ship wanting to dock in

	IEnumerator loadTransport(string res, int amt){
		//print ("loading!");
		ship_resource_manager ship_res = dockedShip.GetComponentInChildren<ship_resource_manager>();
		int loadedCargo = 0;

		//ai_station_resources ai = GetComponent<ai_station_resources> ();
		for (int i = 0; loadedCargo < amt; i+=1) {
			int modLoad = loadRate;
			if ((loadedCargo + loadRate) > amt){
				modLoad = amt - loadedCargo;
			}
			this.res.unload(res,modLoad);
			ship_res.load (res, modLoad);
			loadedCargo += modLoad;
			yield return new WaitForSeconds(millisLoad/1000f);
		}
		dockedShip.GetComponent<ai_ship_orders>().followee = tradeNetwork[0].gameObject;
		dockedShip.GetComponent<ai_ship_controller>().initialize();
		releaseDockedShip();
		loading = false;
	}


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
