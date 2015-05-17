using UnityEngine;
using System.Collections;

public class ship_engine : MonoBehaviour {

	private Rigidbody2D mRigidBody;

	public float maxSpeed;
	public float maxThrust;
	public float maxLateralThrust;
	public float maxReverseThrust;
	public float inertialDampenerPower;
	public float inertialSlipDampenerPower;





	// Use this for initialization
	void Start () {
		mRigidBody = this.GetComponent<Rigidbody2D> ();
	}

	public void applyEnginePower(int forwardMotion, int lateralMotion, float percentPower, float percentLateralPower){
		Vector2 forwardNormal = this.transform.up;
		float currSpeed = Vector2.Dot (Vector2.Dot (forwardNormal, mRigidBody.velocity) * forwardNormal,forwardNormal);
		float desiredSpeed = 0;
		float desiredTorque = 0;
		if (forwardMotion == ship_library.DIRECTION_UP){
			desiredSpeed = maxThrust * percentPower;
		}
		else if(forwardMotion ==  ship_library.DIRECTION_DOWN){
			desiredSpeed = -maxReverseThrust * percentPower;
		}

		if (lateralMotion == ship_library.DIRECTION_LEFT) {
			desiredTorque = maxLateralThrust * percentLateralPower;
		} else if (lateralMotion == ship_library.DIRECTION_RIGHT) {
			desiredTorque = -maxLateralThrust * percentLateralPower;
		}

		float force = 0;
		if (desiredSpeed != 0) {
			if (desiredSpeed > currSpeed) {
				force = maxThrust;
			} else if (desiredSpeed < currSpeed) {
				force = -maxReverseThrust;
			}
		}



		mRigidBody.AddForce (force * forwardNormal,ForceMode2D.Impulse);
		mRigidBody.AddTorque (desiredTorque, ForceMode2D.Impulse);
		lateralIntertialDampener();
	}

	public Vector2 getLateralVelocity(){
		Vector2 currentRightNormal = this.transform.right;
		return Vector2.Dot (currentRightNormal, mRigidBody.velocity) * currentRightNormal;
	}

	public void lateralIntertialDampener(){
		Vector2 impulse = mRigidBody.mass * -getLateralVelocity ();
		
		if (impulse.magnitude > inertialDampenerPower) {
			impulse *= inertialDampenerPower / impulse.magnitude;
		}
		
		mRigidBody.AddForce (impulse, ForceMode2D.Impulse);
		mRigidBody.AddTorque (inertialSlipDampenerPower * mRigidBody.inertia * -mRigidBody.angularVelocity);
		
	}



}
