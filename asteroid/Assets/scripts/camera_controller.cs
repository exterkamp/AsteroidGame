using UnityEngine;
using System.Collections;

public class camera_controller : MonoBehaviour {

	//handle if player dies
	public GameObject target;
	private float cameraDelta = 0f;
	public float scrollSpeed = 1000f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//print (Input.GetAxis("Mouse ScrollWheel"));
		cameraDelta = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

		Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - cameraDelta,1);
		this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y,-10f);

	}
}
