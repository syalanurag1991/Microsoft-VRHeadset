using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	GameManager gameManager;
	Image healthbar;

	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		healthbar = gameObject.GetComponentInChildren<Image> ();
		//healthbarBG.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		//Update health-bar position
		Vector3 healthBarPosition = new Vector3 (gameManager.HealthBarCoordinates.x, gameManager.HealthBarCoordinates.y, gameManager.HealthBarCoordinates.z);
		gameObject.transform.position = healthBarPosition;

		//Update health-bar color
		healthbar.color = gameManager.healthBarColor;

		//Update health-bar size
		Vector3 healthBarNewSize = new Vector3 (gameManager.healthBarSize.x, gameManager.healthBarSize.y, gameManager.healthBarSize.z);
		gameObject.transform.localScale = healthBarNewSize;
	}
}
