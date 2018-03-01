using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCube : MonoBehaviour {

	UIManager uiManager;

	// Mood Tracker Attributes
	Renderer moodTrackerRenderer;

	// Use this for initialization
	void Start () {
		uiManager = FindObjectOfType<UIManager> ();
		moodTrackerRenderer = gameObject.GetComponent<Renderer> ();

		/*
		//To stop memory leak, delete previous material
		if (assignMaterial != null) {
			UnityEditor.AssetDatabase.DeleteAsset (UnityEditor.AssetDatabase.GetAssetPath (assignMaterial));
		}

		//Add material to cube if not assigned
		assignMaterial = new Material(Shader.Find("Diffuse"));
		*/
	}

	// Update is called once per frame
	void Update () {

		//Update uiManager-cube position
		Vector3 moodTrackerPosition = new Vector3 (uiManager.normalizedMoodTrackerCoordinates.x, uiManager.normalizedMoodTrackerCoordinates.y, uiManager.normalizedMoodTrackerCoordinates.z);
		gameObject.transform.localPosition = moodTrackerPosition;

		//Update mood-cube color
		Color moodColor = uiManager.GetMoodTrackerColor();
		moodTrackerRenderer.material.color = moodColor;

		//Debug.Log ("Color: " + moodColor);


	}
}
