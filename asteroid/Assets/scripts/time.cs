using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class time : MonoBehaviour {

	public List<ship_resource_manager> ships;
	private float lastTime = 0;
	public int dayLengthSeconds = 5;
	private bool initialized = false;
	private int day = 0;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		float curTime = Time.time;
		
		if (curTime - lastTime > dayLengthSeconds) {
			day++;
			print ("Day " + day);
			lastTime = curTime;
			foreach (ship_resource_manager ship in ships){
				ship.eat();
			}

		}

	}

	private void initialize(){
		ships = new List<ship_resource_manager>();
		initialized = true;
	}

	public void addManager(ship_resource_manager ship){
		if (!initialized) {
			initialize ();
		}
		ships.Add (ship);
	}

	//make randomized names and the time system

}
