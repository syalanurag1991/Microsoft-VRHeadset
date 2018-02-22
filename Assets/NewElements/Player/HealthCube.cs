using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCube : MonoBehaviour {

	GameManager gameManager;
	//Image healthCube;
	Material healthCubeMaterial;
	Renderer healthCubeRenderer;

	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		healthCubeMaterial = gameObject.GetComponent<Renderer> ().material;
		healthCubeRenderer = gameObject.GetComponent<Renderer> ();
		//healthbarBG.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);


		//assignMaterial = gameObject.GetComponent<Renderer> ().material;
		/*
		//To stop memory leak, delete previous material
		if (assignMaterial != null) {
			UnityEditor.AssetDatabase.DeleteAsset (UnityEditor.AssetDatabase.GetAssetPath (assignMaterial));
		}

		//Add material to cube if not assigned
		assignMaterial = new Material(Shader.Find("Diffuse"));
		*/
		//assignMaterial.color = new Color(0.05f, 0.05f, 0.05f, 1.0f); // black	
	}

	// Update is called once per frame
	void Update () {
		//Update health-bar position
		Vector3 healthCubePosition = new Vector3 (gameManager.HealthCubeCoordinates.x, gameManager.HealthCubeCoordinates.y, gameManager.HealthCubeCoordinates.z);
		gameObject.transform.localPosition = healthCubePosition;
		//gameObject.transform.position = healthCubePosition;

		//Update health-bar color
		//healthCubeMaterial.color = gameManager.healthCubeColor;
		Color healthColor = gameManager.healthCubeColor;
		Debug.Log (healthColor);
		healthCubeRenderer.material.color = healthColor;



		//Update health-bar size
		//Vector3 healthCubeNewSize = new Vector3 (gameManager.healthCubeSize.x, gameManager.healthCubeSize.y, gameManager.healthCubeSize.z);
		//gameObject.transform.localScale = healthCubeNewSize;
	}
}
