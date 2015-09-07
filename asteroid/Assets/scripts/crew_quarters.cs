using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class crew_quarters : MonoBehaviour {

	private manifest_crew crew;
	private bool initialized = false;

	public string captain;
	public List<string> commandCrew;
	public List<string> flightCrew;
	public List<string> Engineering;
	public List<string> Medical;
	public List<string> Marine;
	public List<string> Defense;
	public List<string> DC;
	public List<string> Generic;



	// Use this for initialization
	void Start () {
		commandCrew = new List<string>();
		flightCrew = new List<string> ();
		Engineering = new List<string> ();
		Medical = new List<string> ();
		Marine = new List<string> ();
		Defense = new List<string> ();
		DC = new List<string> ();
		Generic = new List<string> ();
		updateLists ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void updateLists(){
		if (initialized) {
			convertToNames(commandCrew,crew.look(ship_library.Roles.COMMAND));
			convertToNames(flightCrew,crew.look (ship_library.Roles.FLIGHT));
			convertToNames(Engineering,crew.look (ship_library.Roles.ENGINEER));
			convertToNames(Medical,crew.look (ship_library.Roles.MEDICAL));
			convertToNames(Marine,crew.look (ship_library.Roles.MARINE));
			convertToNames(Defense,crew.look (ship_library.Roles.DEFENSE));
			convertToNames(DC,crew.look (ship_library.Roles.DC));
			convertToNames(Generic,crew.look (ship_library.Roles.GENERIC));
			captain = crew.lookCaptain().getDispName();
		}
	}

	private void convertToNames(List<string> names, List<crewman> enumerated){
		foreach (crewman person in enumerated) {
			names.Add(person.getDispName());
		}
	}

	/*public void initialize(bool mil, crewman cap){
		if (!initialized) {
			crew = new manifest_crew(mil,cap);
			initialized = true;
			updateLists ();
		}
	}*/

	public void initialize(bool mil){
		crewman[] crewUnder = GetComponentsInChildren<crewman> ();
		if (!initialized) {
			crew = new manifest_crew(mil,crewUnder);
			initialized = true;
			updateLists ();
		}
	}

	public void addCrew(crewman newGuy){
		crew.addCrew (newGuy);
		updateLists ();
	}

	public void removeCrew(crewman goneGuy){
		crew.removeCrew (goneGuy);
		updateLists ();
	}

	public int getCount(){
		return crew.getCrewSize();
	}

}
