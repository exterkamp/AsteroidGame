using UnityEngine;
using System.Collections;

public class transport_bay : MonoBehaviour{

	private manifest_resources manifest;
	private bool initialized = false;

	
	public int money = 0;
	public int metal = 0;
	public int fuel = 0;
	public int ammo = 0;
	public int weapons = 0;
	public int food = 0;
	public int goods = 0;
	public int luxuryGoods = 0;
	
	public float[] price_multipliers = {1,1,1,1,1,1,1,1};
	public float metal_price;
	public float fuel_price;
	public float ammo_price;
	public float weapons_price;
	public float food_price;
	public float goods_price;
	public float luxuryGoods_price;

	void Start(){
		manifest = new manifest_resources ();
		updateNums ();
	}

	// Use this for initialization
	public void initialize(int mo,int me,int f,int a,int w,int fo,int g, int l) {
		if (!initialized) {
			manifest = new manifest_resources ();
			/*
		manifest.add (ship_library.RESOURCE_MONEY,Random.Range(1000000,5000000));
		manifest.add (ship_library.RESOURCE_METAL,Random.Range (50, 200));
		manifest.add (ship_library.RESOURCE_FUEL,Random.Range (500, 2500));
		manifest.add (ship_library.RESOURCE_AMMO,Random.Range (10, 100));
		manifest.add (ship_library.RESOURCE_WPNS,Random.Range (1, 50));
		manifest.add (ship_library.RESOURCE_FOOD,Random.Range (1000, 10000));
		manifest.add (ship_library.RESOURCE_GOODS,Random.Range (25, 200));
		manifest.add (ship_library.RESOURCE_LUX,Random.Range (0, 25));
		*/
			manifest.add (ship_library.RESOURCE_MONEY, mo);
			manifest.add (ship_library.RESOURCE_METAL, me);
			manifest.add (ship_library.RESOURCE_FUEL, f);
			manifest.add (ship_library.RESOURCE_AMMO, a);
			manifest.add (ship_library.RESOURCE_WPNS, w);
			manifest.add (ship_library.RESOURCE_FOOD, fo);
			manifest.add (ship_library.RESOURCE_GOODS, g);
			manifest.add (ship_library.RESOURCE_LUX, l);
		
			updateNums ();
			initialized = true;
		}
	}

	// Use this for initialization
	public void initialize() {
		if (!initialized) {
			//print ("blank init");

			manifest = new manifest_resources ();

			manifest.add (ship_library.RESOURCE_MONEY, 0);
			manifest.add (ship_library.RESOURCE_METAL, 0);
			manifest.add (ship_library.RESOURCE_FUEL, 0);
			manifest.add (ship_library.RESOURCE_AMMO, 0);
			manifest.add (ship_library.RESOURCE_WPNS, 0);
			manifest.add (ship_library.RESOURCE_FOOD, 0);
			manifest.add (ship_library.RESOURCE_GOODS, 0);
			manifest.add (ship_library.RESOURCE_LUX, 0);

		
			updateNums ();
			initialized = true;
		}
	}
	
	private void updateNums(){
		money = manifest.look (ship_library.RESOURCE_MONEY);
		metal = manifest.look(ship_library.RESOURCE_METAL);
		fuel = manifest.look(ship_library.RESOURCE_FUEL);
		ammo = manifest.look(ship_library.RESOURCE_AMMO);
		weapons = manifest.look(ship_library.RESOURCE_WPNS);
		food = manifest.look(ship_library.RESOURCE_FOOD);
		goods = manifest.look(ship_library.RESOURCE_GOODS);
		luxuryGoods = manifest.look(ship_library.RESOURCE_LUX);
		metal_price = ship_library.BASE_PRICE_METAL * price_multipliers[1];
		fuel_price = ship_library.BASE_PRICE_FUEL * price_multipliers[2];
		ammo_price = ship_library.BASE_PRICE_AMMO * price_multipliers[3];
		weapons_price = ship_library.BASE_PRICE_WPNS * price_multipliers[4];
		food_price = ship_library.BASE_PRICE_FOODS * price_multipliers[5];
		goods_price = ship_library.BASE_PRICE_GOODS * price_multipliers[6];
		luxuryGoods_price = ship_library.BASE_PRICE_LUX * price_multipliers[7];
	}
	
	public int unload(string s, int i){
		int i2 =  manifest.remove (s, i);
		updateNums ();
		return i2;
	}
	
	public void load(string s, int i){
		manifest.add (s, i);
		updateNums ();
	}
	
	public int look(string s){
		return manifest.look (s);
	}

}
