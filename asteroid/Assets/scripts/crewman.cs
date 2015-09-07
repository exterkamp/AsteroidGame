using UnityEngine;
using System.Collections;

public class crewman : MonoBehaviour{

	public string DispName;
	private bool init = false;
	public bool enlistment;
	public ship_library.Roles role;
	public ship_library.Ranks rank;

	public crewman(string name_in, bool enlist, ship_library.Roles role_in, ship_library.Ranks rank_in){
		DispName = name_in;
		enlistment = enlist;
		role = role_in;
		rank = rank_in;
	}

	void Start(){
		if (!init) {
			DispName = ship_library.makeRandomName ();
			init = true;
		}
	}

	void Update(){}

	public string getDispName(){
		if (!init) {
			DispName = ship_library.makeRandomName();
			init = true;
		}
		return DispName;
	}
}
