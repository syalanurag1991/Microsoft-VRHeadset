using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	/*
	float health = 1.0f;
	Slider healthBar;
	GameObject healthbarParent;

	GameManager gameManager;
	public int playerID;

	void Start () {

		healthBar = gameObject.GetComponentInChildren<Slider>();
		healthbarParent = healthBar.gameObject;
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}
	
	public void SetHealth (int damage, int totalDamage)
	{
		health -= damage / 1000.0f;
		healthBar.value = health;

		if (health < 0) {
			//print (healthBar.gameObject.name + " Dead!!");
			gameManager.GameOver (playerID);
		}

	}

	public void DisableHealth ()
	{
		
		if (healthBar != null) {
			//print ("Balle balle");
			Destroy(gameObject.GetComponentInChildren<Slider>().gameObject);
			//print (healthBar.gameObject.name);
		} //else {
			//print("Still there!!");
		//}
	}
	*/
}
