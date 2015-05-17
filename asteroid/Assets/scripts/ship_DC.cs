using UnityEngine;
using System.Collections;

public class ship_DC : MonoBehaviour {

	private ship_engine engine;

	// Use this for initialization
	void Start () {
		engine = GetComponentInParent<ship_engine> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void takeHit(float damage){
		engine.maxSpeed = (engine.maxSpeed - 5f > 0f)? engine.maxSpeed - 5 : 0f ;
		engine.maxThrust = (engine.maxThrust - 2.5f > 0f)? engine.maxThrust - 2.5f : 0f ;

		if (engine.maxThrust == 0) {
			//DIE!

			//may need to disable object so it breaks ties with CAM to prevent co modification
			GameObject.Destroy(engine.gameObject);
		}

	}
}
