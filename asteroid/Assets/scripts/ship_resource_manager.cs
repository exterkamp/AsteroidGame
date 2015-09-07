using UnityEngine;
using System.Collections;

public class ship_resource_manager : MonoBehaviour {

	public int rationAmount = 2;
	private transport_bay bay;
	private crew_quarters crew;
	private bool initialized = false;


	// Use this for initialization
	void Start () {
		Camera.main.GetComponent<time> ().addManager(this);
	}

	public void eat(){

		int count = crew.getCount ();

		count = count * rationAmount;
		//print (count);
		bay.unload (ship_library.RESOURCE_FOOD, count);

	}

	public void initialize(bool mil){
		if (!initialized) {
			crew = GetComponentInChildren<crew_quarters> ();
			bay = GetComponentInChildren<transport_bay> ();
			bay.initialize ();
			crew.initialize (mil);
			initialized = true;
		}
	}

	public void initialize(bool mil, int mo,int me,int f,int a,int w,int fo,int g, int l) {
		if (!initialized) {
			crew = GetComponentInChildren<crew_quarters> ();
			bay = GetComponentInChildren<transport_bay> ();
			bay.initialize (mo, me, f, a, w, fo, g, l);
			crew.initialize (mil);
			initialized = true;
		}
	}

	public int look(string s){
		return bay.look (s);
	}

	public int unload(string s, int i){
		return bay.unload (s,i);
	}

	public void load(string s, int i){
		bay.load (s, i);
	}


}
