using UnityEngine;
using System.Collections;

public class storage_container : MonoBehaviour {

	public bool loaded;
	public SpriteRenderer container;
	public Sprite fullContainer;


	public storage_container(bool b){
		loaded = b;
		if (!loaded) {
			container.sprite = new Sprite();
		}
	}

	void Start(){
		if (!loaded) {
			container.sprite = new Sprite ();
		} else {
			container.sprite = fullContainer;
		}
	}

	public void load(){
		loaded = true;
		container.sprite = fullContainer;
	}

	public void unload(){
		loaded = false;
		container.sprite = new Sprite ();
	}



}
