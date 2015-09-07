using UnityEngine;
using System.Collections;

public class ai_ship_controller : MonoBehaviour {

	private ai_ship_orders orders;
	private Vector2 currentWaypoint;
	public GameObject threat;   //need to check when dogfighting that it isnt gone | this probably needs to be a list so we can pick the closest/most threatening one
	public int currentBehavior;
	private int subBehavior;
	private int holdingPattern;
	private ship_engine engine;
	public GameObject engine_fx;

	private ship_resource_manager res;
	//private transport_bay bay;
	//private crew_quarters quarters;

	public float distance;//15f
	public float desperation = 0f;
	public bool enlisted = false;


	private float propensityToFight;
	//private float propensityToFire;
	private float maxDistanceFromWaypoint;
	private bool docking = false;
	private bool sortee = false;
	private int sorteeMillis = 1000;
	private float sorteeBeginTime;


	public bool DEBUG_RAYS = false;

	private ship_weapon_laser laser;



	// Use this for initialization
	void Start () {
		initialize ();
	}

	public void initialize(){
		currentWaypoint = this.transform.position;
		currentBehavior = ship_library.AI_BEHAVIOR_HOLDING;
		holdingPattern = ship_library.AI_HOLDING_CCW;
		engine = this.GetComponent<ship_engine> ();
		laser = this.GetComponentInChildren<ship_weapon_laser> ();
		orders = this.GetComponent<ai_ship_orders>();

		//bay = this.GetComponentInChildren<transport_bay> ();
		//bay.initialize ();

		//quarters = this.GetComponentInChildren<crew_quarters> ();
		//crewman captain = new crewman ("The Captain", enlisted, ship_library.CREW_JOB_COMMAND, ship_library.CREW_CIV_CAPTAIN);
		//quarters.initialize (enlisted);

		res = this.GetComponentInChildren<ship_resource_manager> ();
		res.initialize (enlisted);

		propensityToFight = orders.propensityToFight;
		//propensityToFire = orders.propensityToFire;
		maxDistanceFromWaypoint = orders.maxDistanceFromWaypoint;
		analyzeInitialOrders ();
		//naive linear function to plot some lateral forces perfect for 20, good for about 5-25 @ 20 thrust
		//may need to be set manually for each class of ship
		
		this.GetComponent<Rigidbody2D> ().centerOfMass = new Vector3 (0,0,0);
		
		distance = (engine.maxLateralThrust > 25)? 20f :(engine.maxLateralThrust-31.25f)/-0.75f;
	}

	
	// Update is called once per frame
	void Update () {
		//figure out where we are supposed to be going
		gatherOrderData();


		//figure out what kind of state we are in based on the orders
		switch (orders.TYPE){
		case (ship_library.AI_ORDERS_ORBIT):
			//if we are not escaping or in combat
			if (currentBehavior != ship_library.AI_BEHAVIOR_ESCAPING && currentBehavior != ship_library.AI_BEHAVIOR_DOGFIGHT) {

				//calculate next point in circle
				//5 degree difference

				//i for inner
				Vector2 iBaseDir = orders.followee.transform.right;
				float iCurrentAngle = Mathf.Atan2(iBaseDir.x, iBaseDir.y) * Mathf.Rad2Deg;
				Vector2 iDesiredDirection =  new Vector2(this.transform.position.x,this.transform.position.y) - currentWaypoint;
				float iDesiredAngle = Mathf.Atan2(iDesiredDirection.x, iDesiredDirection.y) * Mathf.Rad2Deg;
				float iAngleDifference = Mathf.DeltaAngle( iDesiredAngle, iCurrentAngle );
				iAngleDifference = (iAngleDifference + 360f) % 360f;

				//angle is degrees CW from the forward of the center Piece




				if (DEBUG_RAYS){
					//print ("current angle: " + iAngleDifference);
					Debug.DrawRay(this.transform.position,iBaseDir * 2.0f,Color.cyan,1,false);
					Debug.DrawRay(this.transform.position,iDesiredDirection.normalized * 2.0f,Color.magenta,1,false);
				}
				//radius = distance



				if (currentBehavior == ship_library.AI_BEHAVIOR_ORBIT_STAGE_ONE){
					//if we are supposed to be at a waypoint but we are too far away
					if (Vector2.Distance (this.transform.position, currentWaypoint) < orders.minDistanceFromFollowee) {
						//STAGE 1
						//set current point
						currentBehavior = ship_library.AI_BEHAVIOR_ORBIT_STAGE_TWO;
					}
				}
				if (currentBehavior == ship_library.AI_BEHAVIOR_ORBIT_STAGE_TWO){
					//5 degree next point
					float nextAngle = iAngleDifference + orders.followee.transform.eulerAngles.z + 10f;
					Vector2 nextPosition = new Vector2(Mathf.Cos(nextAngle * Mathf.Deg2Rad), Mathf.Sin(nextAngle * Mathf.Deg2Rad)) * orders.minDistanceFromFollowee;

					currentWaypoint = nextPosition + (Vector2)orders.followee.transform.position;

					if (DEBUG_RAYS){
						//print (nextAngle);
						Debug.DrawRay(orders.followee.transform.position,nextPosition,Color.red,1,false);
					}



					if(Vector2.Distance (this.transform.position, orders.followee.transform.position) > orders.minDistanceFromFollowee + 3f){
						currentBehavior = ship_library.AI_BEHAVIOR_ORBIT_STAGE_ONE;
					}
				}

				//
				if (Vector2.Distance (this.transform.position, currentWaypoint) > orders.minDistanceFromFollowee) {
					//STAGE 1
					//set current point
					currentBehavior = ship_library.AI_BEHAVIOR_ORBIT_STAGE_ONE;
				}




			}
			else if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING){
				if (desperation > propensityToFight){
					currentBehavior = ship_library.AI_BEHAVIOR_DOGFIGHT;
					subBehavior = ship_library.AI_SUBBEHAVIOR_CHASE;
				}
			}
			//too far away
			if (Vector2.Distance(this.transform.position, currentWaypoint) > maxDistanceFromWaypoint){
				//goto STAGE 1
				currentBehavior = ship_library.AI_BEHAVIOR_ORBIT_STAGE_ONE;
				subBehavior = 0;
				desperation = 0f;
				threat = null;
			}

			
			
			break;
		case (ship_library.AI_ORDERS_GUARD_WAYPOINT):
			//if we are not escaping or in combat
			if (currentBehavior != ship_library.AI_BEHAVIOR_ESCAPING && currentBehavior != ship_library.AI_BEHAVIOR_DOGFIGHT) {

				//if we are supposed to be at a waypoint but we are too far away
				if (Vector2.Distance (this.transform.position, currentWaypoint) > distance) {
					//print ("Too far away from waypoint, seeking");
					currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				} else {
					currentBehavior = ship_library.AI_BEHAVIOR_HOLDING;
				}
			}
			else if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING){
				if (desperation > propensityToFight){
					currentBehavior = ship_library.AI_BEHAVIOR_DOGFIGHT;
					subBehavior = ship_library.AI_SUBBEHAVIOR_CHASE;
				}
			}
			if (Vector2.Distance(this.transform.position, currentWaypoint) > maxDistanceFromWaypoint){
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				subBehavior = 0;
				desperation = 0f;
				threat = null;
			}


			break;
		case (ship_library.AI_ORDERS_PATROL):
			//if we are not escaping or in combat
			if (currentBehavior != ship_library.AI_BEHAVIOR_ESCAPING && currentBehavior != ship_library.AI_BEHAVIOR_DOGFIGHT) {
				
				//if we reached the waypoint, go to the next one
				if (Vector2.Distance (this.transform.position, currentWaypoint) < distance) {
					orders.incPatrol();
				}
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			}
			else if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING){
				if (desperation > propensityToFight){
					currentBehavior = ship_library.AI_BEHAVIOR_DOGFIGHT;
					subBehavior = ship_library.AI_SUBBEHAVIOR_CHASE;
				}
			}
			if (Vector2.Distance(this.transform.position, currentWaypoint) > maxDistanceFromWaypoint){
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				subBehavior = 0;
				desperation = 0f;
				threat = null;
			}
			
			
			break;
		case (ship_library.AI_ORDERS_ESCORT):
			//if we are not escaping or in combat
			if (currentBehavior != ship_library.AI_BEHAVIOR_ESCAPING && currentBehavior != ship_library.AI_BEHAVIOR_DOGFIGHT) {

				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				
			}
			else if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING){
				if (desperation > propensityToFight){
					currentBehavior = ship_library.AI_BEHAVIOR_DOGFIGHT;
					subBehavior = ship_library.AI_SUBBEHAVIOR_CHASE;
				}
			}
			if (Vector2.Distance(this.transform.position, currentWaypoint) > maxDistanceFromWaypoint){
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				subBehavior = 0;
				desperation = 0f;
				threat = null;
			}
			
			
			break;
		case (ship_library.AI_ORDERS_DOCK):

			if (orders.docking){
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
				docking = true;
			} else if (orders.docked){
				currentBehavior = ship_library.AI_BEHAVIOR_DOCKED;
				this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}

			//if (orders.docked){
			//	this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			//}







			break;

		}



		Vector2 currForwardNormal = this.transform.up;
		float currentAngle = Mathf.Atan2(currForwardNormal.x, currForwardNormal.y) * Mathf.Rad2Deg;
		float angleDifference = 0f;
		Vector2 desiredDirection;
		float desiredAngle;

		int forwardDirection = 0;
		int lateralDirection = 0;
		float power = 1f;
		float lateralPower = 1f;

		switch (currentBehavior) {
		///////////////////////////////
		///   ORBIT STAGE 1         ///
		/////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_ORBIT_STAGE_ONE):
			//find vector to the waypoint
			desiredDirection = currentWaypoint - new Vector2(this.transform.position.x,this.transform.position.y);
			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;
			
			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );
			//print (angleDifference);
			
			//if the angle difference is less than 2 degrees then start to buffer out the lateral rotation
			float RRpercentAngleWasOff = (Mathf.Abs(angleDifference) > 2)? 1f : Mathf.Abs(angleDifference) / 180f;
			
			
			if (angleDifference > 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
				lateralDirection = ship_library.DIRECTION_LEFT;
				power = 1f;
				lateralPower = 1f * RRpercentAngleWasOff;  //make lateral power proportional to the amount of angle left
			}else if(angleDifference < 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
				lateralDirection = ship_library.DIRECTION_RIGHT;
				power = 1f;
				lateralPower = 1f * RRpercentAngleWasOff;
			}
			forwardDirection = ship_library.DIRECTION_UP;
			//if our orders have a minDistanceToFollow slow down if we are in that
			if (Vector2.Distance(this.transform.position,currentWaypoint) < 5f && orders.TYPE == ship_library.AI_ORDERS_ESCORT){
				power *= 0f;
			}
			
			
			
			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,desiredDirection.normalized * 2.0f,Color.green,1,false);
				Debug.DrawRay(this.transform.position,this.transform.up * 2.0f,Color.blue,1,false);
			}

			break;

		///////////////////////////////
		///   ORBIT STAGE 2         ///
		/////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_ORBIT_STAGE_TWO):
			//find the desired direction
			desiredDirection = currentWaypoint - new Vector2(this.transform.position.x,this.transform.position.y);

			//Vector2 nextPosition = currentWaypoint - (Vector2)orders.followee.transform.position;
			//find tangent to circle vector DEPRECATED
			//Vector2 perp = new Vector2(-nextPosition.y, nextPosition.x) / Mathf.Sqrt(Mathf.Pow(nextPosition.x,2f) + Mathf.Pow(nextPosition.y,2f));

			Vector2 desiredDirectionCombo = desiredDirection.normalized;


			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;
			
			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );



			if (angleDifference > 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
				lateralDirection = ship_library.DIRECTION_LEFT;
				//print ("LEFT" + desiredAngle);
			}else if(angleDifference < 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
				lateralDirection = ship_library.DIRECTION_RIGHT;
				//print ("RIGHT" + desiredAngle);
			}





			lateralPower = 1f;
			//Vector3 localVel = transform.InverseTransformDirection(this.GetComponent<Rigidbody2D>().velocity);
			/*if (localVel.y > 0){
				forwardDirection = ship_library.DIRECTION_DOWN;
			}
			else{*/
				forwardDirection = ship_library.DIRECTION_UP;
			//}
			power = 1f;
			//print (localVel.y);

			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,desiredDirection.normalized * 2.0f,Color.green,1,false);
				Debug.DrawRay(this.transform.position,desiredDirectionCombo.normalized * 2.0f,Color.grey,1,false);
				//DEPRECATED//Debug.DrawRay(this.transform.position,perp * 2.0f,Color.white,1,false);
			}
			break;
		///////////////////////////////
		///   HOLDING CIRCLE        ///
		/////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_HOLDING):

			desiredDirection = currentWaypoint - new Vector2(this.transform.position.x,this.transform.position.y);
			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;

			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );

			if (angleDifference < 0 && holdingPattern == ship_library.AI_HOLDING_CCW){
				holdingPattern = ship_library.AI_HOLDING_CW;
			}else if (angleDifference > 0 && holdingPattern == ship_library.AI_HOLDING_CW){
				holdingPattern = ship_library.AI_HOLDING_CCW;
			}

			power = 0.5f;
			//print (angleDifference);
			if (holdingPattern == ship_library.AI_HOLDING_CCW){
				if (angleDifference < 90f){
					power += 0.15f;
				}else if (angleDifference > 90f){
					power -= 0.15f;
				}
			}else if (holdingPattern == ship_library.AI_HOLDING_CW){
				if (angleDifference > -90f){
					power += 0.15f;
				}else if (angleDifference < -90f){
					power -= 0.15f;
				}
			}



			if (holdingPattern == ship_library.AI_HOLDING_CCW){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,power,1f);
				lateralDirection = ship_library.DIRECTION_LEFT;
				lateralPower = 1f;
			}else if(holdingPattern == ship_library.AI_HOLDING_CW){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,power,1f);
				lateralDirection = ship_library.DIRECTION_RIGHT;
				lateralPower = 1f;
			}
			forwardDirection = ship_library.DIRECTION_UP;
			//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,0.5f);

			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,this.transform.up * 5.0f,Color.blue,1,false);
				Debug.DrawRay(this.transform.position,desiredDirection,Color.green,1,false);
			//Debug.DrawRay(this.transform.position,this.transform.up * 2.0f,Color.red,10,false);
			}
			break;

		///////////////////////////////
		///   GOING TO WAYPOINT     ///
	    /////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT):
			//find vector to the waypoint
			desiredDirection = currentWaypoint - new Vector2(this.transform.position.x,this.transform.position.y);
			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;

			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );
			//print (angleDifference);

			//if the angle difference is less than 2 degrees then start to buffer out the lateral rotation
			float percentAngleWasOff = (Mathf.Abs(angleDifference) > 2)? 1f : Mathf.Abs(angleDifference) / 180f;


			if (angleDifference > 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
				lateralDirection = ship_library.DIRECTION_LEFT;
				power = 1f;
				lateralPower = 1f * percentAngleWasOff;  //make lateral power proportional to the amount of angle left
			}else if(angleDifference < 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
				lateralDirection = ship_library.DIRECTION_RIGHT;
				power = 1f;
				lateralPower = 1f * percentAngleWasOff;
			}
			forwardDirection = ship_library.DIRECTION_UP;
			//if our orders have a minDistanceToFollow slow down if we are in that
			if (Vector2.Distance(this.transform.position,currentWaypoint) < 5f && orders.TYPE == ship_library.AI_ORDERS_ESCORT){
				power *= 0.5f;
			}



			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,desiredDirection.normalized * 2.0f,Color.green,1,false);
				Debug.DrawRay(this.transform.position,this.transform.up * 2.0f,Color.blue,1,false);
			}
			break;

		///////////////////////////////
		///   ESCAPING              ///
		/////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_ESCAPING):
			desiredDirection = new Vector2(threat.transform.position.x,threat.transform.position.y) - new Vector2(this.transform.position.x,this.transform.position.y);
			desiredDirection = desiredDirection * -1.0f;
			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;

			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );
			//print (angleDifference);
			if (angleDifference > 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
				lateralDirection = ship_library.DIRECTION_LEFT;
				power = 1f;
				lateralPower = 1f;
			}else if(angleDifference < 0){
				//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
				lateralDirection = ship_library.DIRECTION_RIGHT;
				power = 1f;
				lateralPower = 1f;
			}
			forwardDirection = ship_library.DIRECTION_UP;

			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,desiredDirection.normalized * 2.0f,Color.green,1,false);
				Debug.DrawRay(this.transform.position,this.transform.up * 2.0f,Color.blue,1,false);
			}

			break;

		///////////////////////////////
		///   DOGFIGHTING           ///
		/////////////////////////////// 
		case (ship_library.AI_BEHAVIOR_DOGFIGHT):
			if (threat == null){
				currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
				subBehavior = 0;
				desperation = 0f;
				threat = null;
				break;
			}

			desiredDirection = new Vector2(threat.transform.position.x,threat.transform.position.y) - new Vector2(this.transform.position.x,this.transform.position.y);
			desiredAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.y) * Mathf.Rad2Deg;
			
			angleDifference = Mathf.DeltaAngle( desiredAngle, currentAngle );
			float distanceToTarget = Vector2.Distance(threat.transform.position, this.transform.position);

			//lets use this info

			//if we are too close, break off to re-engage
			if (distanceToTarget < 10f){
				subBehavior = ship_library.AI_SUBBEHAVIOR_BREAKOFF;
			}




			if (subBehavior == ship_library.AI_SUBBEHAVIOR_CHASE){

				if (angleDifference > 0){
					//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
					lateralDirection = ship_library.DIRECTION_LEFT;
				}else if(angleDifference < 0){
					//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
					lateralDirection = ship_library.DIRECTION_RIGHT;
				}
				if (distanceToTarget <= laser.range && /*propensityToFire > Random.Range(0f,1f) &&*/ Mathf.Abs(angleDifference) < 10f){
					//if sortee success breakoff
					float curTime = Time.time * 1000;
					if (!sortee){
						sorteeBeginTime = curTime;
						sortee = true;
					}
					if ((curTime - sorteeBeginTime) > sorteeMillis){//laser.fire()){
						subBehavior = ship_library.AI_SUBBEHAVIOR_BREAKOFF;
						sortee = false;
					}else{
						//print ("ai_fire");
						laser.fire ();
					}
				}
			}else if (subBehavior == ship_library.AI_SUBBEHAVIOR_BREAKOFF){//problem if is messed up in process, or not behind player, is alittle weird
				if (angleDifference < 90 && angleDifference > 0){
					//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_RIGHT,1f,1f);
					lateralDirection = ship_library.DIRECTION_RIGHT;
				}else if (angleDifference > -90 && angleDifference < 0){
					//engine.applyEnginePower(ship_library.DIRECTION_UP,ship_library.DIRECTION_LEFT,1f,1f);
					lateralDirection = ship_library.DIRECTION_LEFT;
				}else{
					subBehavior = ship_library.AI_SUBBEHAVIOR_CHASE;
				}
			}

			forwardDirection = ship_library.DIRECTION_UP;
			power = 1f;
			lateralPower = 1f;

			if (DEBUG_RAYS){
				Debug.DrawRay(this.transform.position,desiredDirection.normalized * 3.0f,Color.green,1,false);
				Debug.DrawRay(this.transform.position,this.transform.up * 3.0f,Color.blue,1,false);
			}

			break;
		}

		//AUTOMATED COLLISION AVOIDANCE
		if (this.GetComponentInChildren<ship_CAMod> () != null && !docking) {
			float[] args  = this.GetComponentInChildren<ship_CAMod>().activateAvoidance();
			if (args != null){
				forwardDirection =  (int)args[0];
				lateralDirection = (int)args[1];
				power = args[2];
				lateralPower = args[3];
			}
		}



		engine.applyEnginePower(forwardDirection,lateralDirection,power,lateralPower);
		//if engine power is going then enable engine_fx



	}

	void gatherOrderData(){
		//currentWaypoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currentWaypoint = orders.nextAction (this.transform.position);
	}

	void analyzeInitialOrders(){
		switch (orders.TYPE){
		case (ship_library.AI_ORDERS_GUARD_WAYPOINT):
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			break;
		case (ship_library.AI_ORDERS_PATROL):
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			break;
		case (ship_library.AI_ORDERS_TRANSPORT):
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			break;
		case (ship_library.AI_ORDERS_ESCORT):
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			break;
		case (ship_library.AI_ORDERS_DOCK):
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			break;
		}


	}

	public void registerThreat(GameObject threat){
		this.threat = threat;
		currentBehavior = ship_library.AI_BEHAVIOR_ESCAPING;
	}

	public void incDesperation(){
		if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING) {
			desperation += 0.01f;
		}
	}

	public void deregisterThreat(GameObject threat){
		if (currentBehavior == ship_library.AI_BEHAVIOR_ESCAPING) {
			currentBehavior = ship_library.AI_BEHAVIOR_GOING_TO_WAYPOINT;
			desperation = 0f;
			threat = null;
		}
	}


}
