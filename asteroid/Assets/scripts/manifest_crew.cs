using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class manifest_crew{

	public bool military;

	crewman captain;
	List<crewman> commandCrew;
	List<crewman> flightCrew;
	List<crewman> Engineering;
	List<crewman> Medical;
	List<crewman> Marine;
	List<crewman> Defense;
	List<crewman> DC;
	List<crewman> Generic;
	
	/*public manifest_crew(bool mil, crewman cap){
		military = mil;
		captain = cap;
		initLists ();
	}

	public manifest_crew(bool mil, crewman cap, crewman[] crew){
		military = mil;
		captain = cap;
		initLists ();
		foreach (crewman member in crew){
			filterIn(member);
		}
	}*/

	public manifest_crew (bool mil, crewman[] crew){
		military = mil;
		initLists ();
		foreach (crewman member in crew) {
			filterIn(member);
		}
	}

	private void initLists(){
		commandCrew = new List<crewman>();
		flightCrew = new List<crewman>();
		Engineering = new List<crewman> ();
		Medical = new List<crewman> ();
		Marine = new List<crewman> ();
		Defense = new List<crewman> ();
		DC = new List<crewman> ();
		Generic = new List<crewman> ();
	}

	public void addCrew(crewman newbie){
		filterIn (newbie);
	}

	public void removeCrew(crewman remove){
		switch (remove.role){
		case (ship_library.Roles.COMMAND):
			commandCrew.Remove(remove);
			break;
		case (ship_library.Roles.FLIGHT):
			flightCrew.Remove(remove);
			break;
		case(ship_library.Roles.ENGINEER):
			Engineering.Remove(remove);
			break;
		case(ship_library.Roles.MEDICAL):
			Medical.Remove(remove);
			break;
		case(ship_library.Roles.MARINE):
			Marine.Remove(remove);
			break;
		case(ship_library.Roles.DEFENSE):
			Defense.Remove(remove);
			break;
		case(ship_library.Roles.DC):
			DC.Remove(remove);
			break;
		case(ship_library.Roles.GENERIC):
			Generic.Remove(remove);
			break;
		}
		if (remove.Equals (captain)) {
			captain = null;
		}
	}

	private void filterIn(crewman member){
		switch (member.role){
		case (ship_library.Roles.COMMAND):
			if (captain != null){
				if (member.rank > captain.rank){
					commandCrew.Add(captain);
					captain = member;
				}else{
					commandCrew.Add(member);
				}
			}else{
				captain = member;
			}
			break;
		case (ship_library.Roles.FLIGHT):
			flightCrew.Add(member);
			break;
		case(ship_library.Roles.ENGINEER):
			Engineering.Add(member);
			break;
		case(ship_library.Roles.MEDICAL):
			Medical.Add (member);
			break;
		case(ship_library.Roles.MARINE):
			Marine.Add(member);
			break;
		case(ship_library.Roles.DEFENSE):
			Defense.Add(member);
			break;
		case(ship_library.Roles.DC):
			DC.Add(member);
			break;
		case(ship_library.Roles.GENERIC):
			Generic.Add(member);
			break;
		}
	}

	public List<crewman> look(ship_library.Roles job){
		switch (job) {
		case(ship_library.Roles.COMMAND):
			return(commandCrew);
		case(ship_library.Roles.FLIGHT):
			return(flightCrew);
		case(ship_library.Roles.ENGINEER):
			return(Engineering);
		case(ship_library.Roles.MEDICAL):
			return(Medical);
		case(ship_library.Roles.MARINE):
			return(Marine);
		case(ship_library.Roles.DEFENSE):
			return(Defense);
		case(ship_library.Roles.DC):
			return(DC);
		case(ship_library.Roles.GENERIC):
			return(Generic);
		}
		return commandCrew;
	}

	public crewman lookCaptain(){
		return captain;
	}

	public int getCrewSize(){
		int total = 0;
		if (captain != null) {
			total++;
		}
		total += commandCrew.Count;
		total += flightCrew.Count;
		total += Engineering.Count;
		total += Medical.Count;
		total += Marine.Count;
		total += Defense.Count;
		total += DC.Count;
		total += Generic.Count;
		return total;
	}


}
