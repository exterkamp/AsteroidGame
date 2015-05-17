using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ship_CAMod : MonoBehaviour {


	//list may be comodified when the registeredShips are destroyed

	public List<GameObject> registeredShips = new List<GameObject>();
	public float standardCutoff = 30f;
	public bool DRAW_RAYS = false;


	void Update(){
		/*foreach (GameObject g in registeredShips){
			Debug.DrawLine(this.transform.position,g.transform.position,Color.yellow,0.1f);
		}*/


	}


	public float[] activateAvoidance(){

		if (checkIfAlive ()) {


			float[] args = new float[4];

			//find average point of all registered ships
			if (registeredShips.Count > 0) {
				Vector2 averagePosition = new Vector2 (0, 0);
				foreach (GameObject g in registeredShips) {
					averagePosition.x += g.transform.position.x;
					averagePosition.y += g.transform.position.y;
				}
				averagePosition.x /= (float)registeredShips.Count;
				averagePosition.y /= (float)registeredShips.Count;

				//invert to escape
				averagePosition = averagePosition - new Vector2 (this.transform.position.x, this.transform.position.y);

				//figure out what angle that is
				float desiredAngle = Mathf.Atan2 (averagePosition.x, averagePosition.y) * Mathf.Rad2Deg;
				//what the difference is
				float angleDifference = Mathf.DeltaAngle (desiredAngle, Mathf.Atan2 (this.transform.up.x, this.transform.up.y) * Mathf.Rad2Deg);

				args [0] = ship_library.DIRECTION_UP;



				if (angleDifference > -20.0f) {
					args [1] = ship_library.DIRECTION_RIGHT;
				} else {
					args [1] = ship_library.DIRECTION_LEFT;
				}
				if (Vector2.Distance(this.transform.position, averagePosition) > 5f){
					args [2] = 0.75f;

				}else{
					args [2] = 0.1f;
					print ("DANGER CLOSE");
				}

				args [3] = 1f;
				if (DRAW_RAYS){Debug.DrawRay (this.transform.position, averagePosition.normalized * 3.0f, Color.yellow, 1, false);}
				return args;
			} else {
				return null;
			}
		} else {
			return null;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		//ship_library.
		if (ship_library.TAGS_ACTORS_ALL.Contains(other.gameObject.tag)) {
			registeredShips.Add (other.gameObject);
		}
	}
	void OnTriggerStay2D(Collider2D other) {

	}
	void OnTriggerExit2D(Collider2D other) {
		if (ship_library.TAGS_ACTORS_ALL.Contains(other.gameObject.tag)) {
			registeredShips.Remove (other.gameObject);
		}
		//print ("TAG: " + other.gameObject.tag);
		//print ("TAG EXISTS: " + ship_library.TAGS_ACTORS_ALL.Contains(other.gameObject.tag));
	}

	bool checkIfAlive(){
		bool oneAlive = false;
		List<GameObject> objectsToDelete = new List<GameObject>();
		foreach (GameObject g in registeredShips) {
			if (g != null){
				if (Vector2.Distance(this.transform.position,g.transform.position) < standardCutoff){
					oneAlive = true;
				}else{
					//registeredShips.Remove(g);
					objectsToDelete.Add(g);
					//print ("Too far away.");
				}
			}else{
				//registeredShips.Remove(g);
				objectsToDelete.Add(g);
				//print("Exiting range");
			}
		}
		foreach (GameObject g in objectsToDelete) {
			registeredShips.Remove(g);
		}

		return oneAlive;
	}


}
