using UnityEngine;
using System.Collections;
using System.Linq;

public class ai_ship_vision : MonoBehaviour {

	public GameObject ai_fighter;
	private ai_ship_controller brain;

	// Use this for initialization
	void Start () {
		brain = ai_fighter.GetComponent<ai_ship_controller> ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (ship_library.TAGS_ACTORS_FACTIONABLE.Contains(other.gameObject.tag) && other.gameObject.GetComponent<ship_transponder>()) {
			if (other.gameObject.GetComponent<ship_transponder>().faction != GetComponentInParent<ship_transponder>().faction){
				brain.registerThreat (other.gameObject);
			}
		}
	}
	void OnTriggerStay2D(Collider2D other) {
		if (brain.threat == null) {
			if (ship_library.TAGS_ACTORS_FACTIONABLE.Contains(other.gameObject.tag) && other.gameObject.GetComponent<ship_transponder>()) {
				if (other.gameObject.GetComponent<ship_transponder>().faction != GetComponentInParent<ship_transponder>().faction){
					brain.registerThreat (other.gameObject);
				}
			}
		}

		brain.incDesperation ();
	}
	void OnTriggerExit2D(Collider2D other) {
		brain.deregisterThreat (other.gameObject);
	}
}
