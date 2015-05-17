using UnityEngine;
using System.Collections;

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

	public const int AI_BEHAVIOR_HOLDING = 1;
	public const int AI_BEHAVIOR_GOING_TO_WAYPOINT = 2;
	public const int AI_BEHAVIOR_ESCAPING = 3;
	public const int AI_BEHAVIOR_DOGFIGHT = 4;
	public const int AI_BEHAVIOR_GOING_TO_PATROLPOINT = 5;
	public const int AI_BEHAVIOR_ORBIT_STAGE_ONE = 6;
	public const int AI_BEHAVIOR_ORBIT_STAGE_TWO = 7;

	public const int AI_SUBBEHAVIOR_CHASE = 1;
	public const int AI_SUBBEHAVIOR_BREAKOFF = 2;


	public const int AI_HOLDING_CW = 1;
	public const int AI_HOLDING_CCW = 2;

	public const int FACTION_RAIDER = 1;
	public const int FACTION_EMPIRE = 2;
	 
	public static readonly string[] TAGS_ACTORS_ALL = new string[]{"ship_fighter","port_planet","ship_transport","port_station"};
	public static readonly string[] TAGS_ACTORS_FACTIONABLE = new string[]{"ship_fighter","ship_transport","port_station"};//FACTIONABLE implies -> transponder and DC unit

}
