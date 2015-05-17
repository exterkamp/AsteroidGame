using UnityEngine;
using System.Collections;

public class ship_controls : MonoBehaviour {

	private ship_engine engine;


	// Use this for initialization
	void Start () {
		engine = this.GetComponent<ship_engine> ();
	}
	
	// Update is called once per frame
	void Update () {
		int forwardMotion = 0;
		int lateralMotion = 0;
		float power = 1f;
		float lateralPower = 1f;

		if (Input.GetAxis("Vertical") > 0){
			forwardMotion = ship_library.DIRECTION_UP;
		}else if (Input.GetAxis("Vertical") < 0){
			forwardMotion = ship_library.DIRECTION_DOWN;
		}
		
		if (Input.GetAxis("Horizontal") < 0){
			lateralMotion = ship_library.DIRECTION_LEFT;
		}else if (Input.GetAxis("Horizontal") > 0){
			lateralMotion = ship_library.DIRECTION_RIGHT;
		}

		if (Input.GetButtonDown("ShipFire")) {
			if (this.GetComponentInChildren<ship_weapon_laser>() != null){
				this.GetComponentInChildren<ship_weapon_laser>().fire();
			}
		}

		//AUTOMATED COLLISION AVOIDANCE
		if (this.GetComponentInChildren<ship_CAMod> () != null) {
			float[] args  = this.GetComponentInChildren<ship_CAMod>().activateAvoidance();
			if (args != null){
				forwardMotion =  (int)args[0];
				lateralMotion = (int)args[1];
				power = args[2];
				lateralPower = args[3];
			}
		}

		
		engine.applyEnginePower (forwardMotion,lateralMotion,power,lateralPower);
	}
}
