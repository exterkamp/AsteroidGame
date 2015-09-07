using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Raycasts Hit Triggers: false 
 * Raycasts Start in Collider: false
 * Changes Stops Callback: false
 */
public class ship_library : MonoBehaviour {

	public const int DIRECTION_UP = 1;
	public const int DIRECTION_DOWN = 2;
	public const int DIRECTION_LEFT = 3;
	public const int DIRECTION_RIGHT = 4;

	public const int AI_ORDERS_GUARD_WAYPOINT = 1;
	public const int AI_ORDERS_PATROL = 2;
	public const int AI_ORDERS_TRANSPORT = 3;
	public const int AI_ORDERS_ESCORT = 4;
	public const int AI_ORDERS_ORBIT = 5;
	public const int AI_ORDERS_DOCK = 6;

	public const int AI_BEHAVIOR_HOLDING = 1;
	public const int AI_BEHAVIOR_GOING_TO_WAYPOINT = 2;
	public const int AI_BEHAVIOR_ESCAPING = 3;
	public const int AI_BEHAVIOR_DOGFIGHT = 4;
	public const int AI_BEHAVIOR_GOING_TO_PATROLPOINT = 5;
	public const int AI_BEHAVIOR_ORBIT_STAGE_ONE = 6;
	public const int AI_BEHAVIOR_ORBIT_STAGE_TWO = 7;
	public const int AI_BEHAVIOR_DOCKED = 8;

	public const int AI_SUBBEHAVIOR_CHASE = 1;
	public const int AI_SUBBEHAVIOR_BREAKOFF = 2;


	public const int AI_HOLDING_CW = 1;
	public const int AI_HOLDING_CCW = 2;

	public const int FACTION_RAIDER = 1;
	public const int FACTION_EMPIRE = 2;
	 
	public static readonly string[] TAGS_ACTORS_ALL = new string[]{"ship_fighter","port_planet","ship_transport","port_station"};
	public static readonly string[] TAGS_ACTORS_FACTIONABLE = new string[]{"ship_fighter","ship_transport","port_station"};//FACTIONABLE implies -> transponder and DC unit

	public const string RESOURCE_MONEY = "cash";
	public const string RESOURCE_METAL = "metal";
	public const string RESOURCE_FUEL = "fuel";
	public const string RESOURCE_AMMO = "ammunition";
	public const string RESOURCE_WPNS = "weaponry";
	public const string RESOURCE_FOOD = "food";
	public const string RESOURCE_GOODS = "goods";
	public const string RESOURCE_LUX = "luxuryGoods";

	public const int BASE_PRICE_METAL = 10;
	public const int BASE_PRICE_FUEL = 5;
	public const int BASE_PRICE_AMMO = 2;
	public const int BASE_PRICE_WPNS = 250;
	public const int BASE_PRICE_FOODS = 1;
	public const int BASE_PRICE_GOODS = 25;
	public const int BASE_PRICE_LUX = 100;

	public static readonly string[] LIST_RESOURCES = new string[]{RESOURCE_MONEY,RESOURCE_METAL,RESOURCE_FUEL,RESOURCE_AMMO,RESOURCE_WPNS,RESOURCE_FOOD,RESOURCE_GOODS,RESOURCE_LUX};

	/*public const string CREW_CIV_CAPTAIN = "Captain - civ";
	public const string CREW_CIV_FIRSTMATE = "First Mate";
	public const string CREW_CIV_CREWCHIEF = "Chief";
	public const string CREW_CIV_CREW = "Crew";
	public const string CREW_CIV_CIVILIAN = "Civilian";

	public const string CREW_MIL_ADMIRAL = "Admiral";
	public const string CREW_MIL_CAPTAIN = "Captain - mil";
	public const string CREW_MIL_XO = "Executive Officer";
	public const string CREW_MIL_LIEUTENANT = "Lieutenant";
	public const string CREW_MIL_ENSIGN = "Ensign";
	public const string CREW_MIL_WARRANT = "Warrant Officer";
	public const string CREW_MIL_CHIEF = "Chief";
	public const string CREW_MIL_PETTY = "Petty Officer";
	public const string CREW_MIL_SEAMEN = "Seaman";*/

	public enum Ranks{
		CREW_CIV_CIVILIAN,
		CREW_CIV_CREW,
		CREW_CIV_CREWCHIEF,
		CREW_CIV_FIRSTMATE,
		CREW_CIV_CAPTAIN,
		CREW_MIL_SEAMEN,
		CREW_MIL_PETTY,
		CREW_MIL_CHIEF,
		CREW_MIL_WARRANT,
		CREW_MIL_ENSIGN,
		CREW_MIL_LIEUTENANT,
		CREW_MIL_XO,
		CREW_MIL_CAPTAIN,
		CREW_MIL_ADMIRAL,
	}

	public enum Roles
	{
		GENERIC,
		DEFENSE,
		DC,
		MARINE,
		MEDICAL,
		ENGINEER,
		FLIGHT,
		COMMAND
	}

	/*public const string CREW_JOB_COMMAND = "Command";//captain,xo, lt
	public const string CREW_JOB_FLIGHT = "Flight Line";//lt,ensign
	public const string CREW_JOB_ENGINEER = "Engineering";//chief,PO,seaman
	public const string CREW_JOB_MEDICAL = "Medical";//lt,ensign
	public const string CREW_JOB_MARINE = "Marine";//chief,PO, seaman
	public const string CREW_JOB_DEFENSE = "Defence";//chief,PO, seaman
	public const string CREW_JOB_DC = "Damage Control";//chief,PO, seaman
	public const string CREW_JOB_GENERIC = "Generic";//all civ ranks*/

	static private string[] firstNames = new string[]{"Shane","Rob","Robert","Carl","Smith","Charles","Charlie","Hank","Harold"};
	static private string[] lastNames = new string[]{"Alberts","Carrie","Smith","Rivers","Gibbons"};
	static private string[][] generalNames = new string[][]{firstNames,lastNames};

	static private string[] firstNamesAlien = new string[]{"Tzyuz","Ytzix","Jiyuz","Grizyuz","Ghuaz","Qeq","Kyzuz","Tgrunqti","Hdu"};
	static private string[] lastNamesAlien = new string[]{"Jutyizq","Zuauz","Huzua","Tzhich","Gruzyuza"};
	static private string[][] AlienNames = new string[][]{firstNamesAlien,lastNamesAlien};

	static private string[] firstNamesArabic = new string[]{"Abdul","Mohommad","Ahmed","Hadi","Malik","Halim","Hamid","Latif","Mubdi"};
	static private string[] lastNamesArabic = new string[]{"Kassis","Kouri","Nassar","Atiyeh","Totah"};
	static private string[][] ArabicNames = new string[][]{firstNamesArabic,lastNamesArabic};

	static private string[] firstNamesAsian = new string[]{"Aki","Chung","Lee","Dara","Chin","Decha","Haru","Hiro","Ichiro"};
	static private string[] lastNamesAsian = new string[]{"Nguyen","Patel","Lee","Yang","Zhang"};
	static private string[][] AsianNames = new string[][]{firstNamesAsian,lastNamesAsian};
	
	static private List<string[][]> compiledNames = new List<string[][]>(){generalNames,AlienNames,ArabicNames,AsianNames};

	static private string[] middleNames = new string[]{"A.","B.","C.","D.","E.","F.","G.","H.","I.","J.","K.","L.","M.","N.","O.","P.","Q.","R.","S.","T.","U.","V.","W.","X.","Y.","Z."};
	static private string[] nickNames = new string[]{"Killer","Crazy","Insane",
		"Whiskey","Bourbon","Wine","Moonshine","Tequila","Whisky","Margarita","Beer",
		"Buzzsaw","Bolt","Fork","Shanks","Knife","Spoon","Gear","Tool",
		"Papers","Stapler"};


	static public string makeRandomName(){
		string name = "";
		string[][] names = compiledNames[Random.Range (0, compiledNames.Count)];

		name = names[0][Random.Range (0, names[0].Length)];
		if (Random.Range (0, 10) > 2) {
			name = name + " " + middleNames [Random.Range (0, middleNames.Length)];
		} else {
			name = name + " \"" + nickNames [Random.Range (0, nickNames.Length)] + "\"";
		}
		name = name + " " + names[1][Random.Range(0,names[1].Length)];

		return name;
	}


}
