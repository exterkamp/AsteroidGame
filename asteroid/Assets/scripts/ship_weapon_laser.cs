using UnityEngine;
using System.Collections;
using System.Linq;

public class ship_weapon_laser : MonoBehaviour {

	public float range;
	public float accuracyAngle;
	public float accuracy;
	public float damage;

	public bool showAngle = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (showAngle) {



			Vector2 currentAim = this.transform.up;
			float currentAngle = Mathf.Atan2 (currentAim.x, currentAim.y) * Mathf.Rad2Deg;

			float angleDifference = -Mathf.DeltaAngle( Mathf.Atan2 (Vector2.right.x, Vector2.right.y) * Mathf.Rad2Deg, currentAngle );
			currentAngle = angleDifference;
			//print (angleDifference);
			float deltaAngle = accuracyAngle;
			float maxLeft = currentAngle + deltaAngle;
			float maxRight = currentAngle - deltaAngle;

			maxLeft *= Mathf.Deg2Rad;
			maxRight *= Mathf.Deg2Rad;
			Vector2 LEFT = new Vector2 (Mathf.Cos(maxLeft),Mathf.Sin (maxLeft));
			Vector2 RIGHT = new Vector2 (Mathf.Cos(maxRight),Mathf.Sin (maxRight));
			Debug.DrawRay(this.transform.position,LEFT.normalized * range,Color.yellow,0.01f,false);
			Debug.DrawRay(this.transform.position,RIGHT.normalized * range,Color.yellow,0.01f,false);

		}
	}

	public bool fire(){
		Vector2 currentAim = this.transform.up;
		float currentAngle = Mathf.Atan2 (currentAim.x, currentAim.y) * Mathf.Rad2Deg;

		float angleDifference = -Mathf.DeltaAngle( Mathf.Atan2 (Vector2.right.x, Vector2.right.y) * Mathf.Rad2Deg, currentAngle );
		currentAngle = angleDifference;

		float deltaAngle = accuracyAngle * Random.Range(0f,1f);
		deltaAngle = (Random.Range(0f,1f) > 0.5f) ? deltaAngle : -deltaAngle;
		//print ("orig angle: " + currentAngle);
		//print ("delt angle: " + deltaAngle);

		currentAngle = currentAngle + deltaAngle;

		currentAngle = currentAngle * Mathf.Deg2Rad;
		currentAim = new Vector2 (Mathf.Cos(currentAngle),Mathf.Sin (currentAngle));
		//currentAim.Normalize();

		//print ("final angle: " + currentAngle);

		//print (currentAngle);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, currentAim,range);




		if (hit.collider != null) {
			if (ship_library.TAGS_ACTORS_FACTIONABLE.Contains(hit.collider.gameObject.tag)){
				//print ("hit");
				Debug.DrawLine (transform.position, hit.point, Color.green);
				GameObject enemy = hit.collider.gameObject;
				enemy.GetComponent<Rigidbody2D>().AddForce(currentAim.normalized * damage,ForceMode2D.Impulse);
				enemy.GetComponent<Rigidbody2D>().AddTorque(damage,ForceMode2D.Impulse);

				ship_DC DC_unit = enemy.GetComponentInChildren<ship_DC>();
				if (DC_unit != null){
					DC_unit.takeHit(damage);
				}
				else{
					print ("Damage target lost?");
				}


			}
			return true;

		} else {
			Debug.DrawRay(this.transform.position,currentAim * range,Color.red,0.01f,false);
			return false;
		}
	}

}
