using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class manifest_resources {

	Dictionary<string,int> resourceDictionary;



	public manifest_resources(){
		int inc = 0;
		int[] initialAmounts  = new int[]{
			0,//RESOURCES_MONEY
			0,//RESOURCE_METAL
			0, //RESOURCES_FUEL
			0, //RESOURCE_AMMO
			0, //RESOURCE_WPNS
			0, //RESOURCE_FOOD
			0, //RESOURCE_GOODS
			0};//RESOURCE_LUX
		resourceDictionary = new Dictionary<string, int> ();
		foreach (string s in ship_library.LIST_RESOURCES){
			resourceDictionary.Add(s,initialAmounts[inc]);
			inc++;
		}



	}

	public void add(string item, int i){
		resourceDictionary[item] = resourceDictionary[item] + i;
	}

	public int remove(string item, int i){
		int amount = resourceDictionary [item];
		if (i <= amount) {
			resourceDictionary[item] = resourceDictionary[item] - i;
			return i;
		} else {
			resourceDictionary[item] = 0;
			return amount;
		}
	}

	public bool isEmpty(){
		foreach (int i in resourceDictionary.Values) {
			if (i > 0){
				return false;
			}
		}
		return true;
	}

	public void dump(){
		foreach (string s in resourceDictionary.Keys) {
			resourceDictionary[s] = 0;
		}
	}

	public void look(){
		foreach (string s in resourceDictionary.Keys) {
			Debug.Log(s + " : " + resourceDictionary[s]);
		}
	}

	public int look(string s){
		return resourceDictionary [s];
	}
	

}
