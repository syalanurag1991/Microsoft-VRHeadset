using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public Vector3 HealthBarCoordinates = new Vector3(0, 0, 0);
	public Vector3 HealthCubeCoordinates = new Vector3(0, 0, 0);
	public Color healthBarColor;
	public Color healthCubeColor;
	public Vector3 healthBarSize;
	public Vector3 healthCubeSize;
	float healthbBarSizeOld = 0f;
	float healthbCubeSizeOld = 0f;

	public Camera mainCamera;
	public Transform mainCameraTransform;
	public Vector3 mainCameraPosition;
	public Quaternion mainCameraRotation;
	public Vector3 gazeDirection;

	// Use this for initialization
	void Start () {
		SetHealthBarSize (160f);
		SetHealthCubeSize (160f);

		mainCamera = FindObjectOfType<Camera> ();
		mainCameraTransform = mainCamera.transform;
	}
	
	// Update is called once per frame
	void Update () {
		mainCameraPosition = mainCamera.transform.position;
		mainCameraRotation = mainCamera.transform.rotation;
		gazeDirection = mainCamera.transform.forward;
		Debug.Log (mainCameraPosition);
		Debug.Log (mainCameraRotation);
		Debug.Log (gazeDirection);
	}

	public void SetHealthBarCoordinates(float xValue, float yValue, float newSize){
		//float tempY =  (((newSize/125.0f)*1000.0f)-(1.0f*yValue))/200;
		//float deltaSize = newSize - healthbBarSizeOld;
		//float deltaY = deltaSize;
		//Debug.Log ("Delta Y: " + deltaY);
		//float tempY =  (300.0f + (newSize/1.5f) - (yValue*50f/newSize))/100;
		float tempY =  (600.0f + (newSize/1.5f) - (yValue))/100;
		//Debug.Log ("Get Y Scale       = " + (0.003f*newSize));
		//Debug.Log ("Get Y Coordinate = " + (yValue/200));
		//Debug.Log ("Set Y Coordinate = " + tempY);
		HealthBarCoordinates.x = (xValue-675.0f)/100;
		HealthBarCoordinates.y = tempY;
		HealthBarCoordinates.z = -1f*(newSize / 200f);

		healthbBarSizeOld = newSize;
	}

	public void SetHealthBarColor(Color newHealthBarColor){
		healthBarColor = newHealthBarColor;
	}

	public void SetHealthBarSize(float newSize){

		if (newSize > 125f) {
			newSize = 125f;	
		}

		healthBarSize.x = 0.0002f*125;
		healthBarSize.y = 0.0003f*125;
		healthBarSize.z = 1;
		//healthBarSize.x = 0.0002f*newSize;
		//healthBarSize.y = 0.0003f*newSize;
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetHealthCubeCoordinates(float xValue, float yValue, float newSize){

		//Debug.Log ("x: " + xValue);
		//Debug.Log ("y: " + yValue);

		// Camera feed parameters
		float feedWidth = 1280f;
		float feedHeight = 720f;


		float flipFactorX = 1f;
		float flipFactorY = 1f;

		//Mapping - Camera feed to Mixed Reality Worldspace
		//Offset X/Y to make cube appear above face and 
		//incline towards a side(20%/40% screewidth.height)
		//Account for Horizontal flip and Vertical flip

		/*
		// for width = 640 and height = 480
		float scaleY = 100f;
		float scaleX = 80f;
		float offsetXpercent = 0.05f;
		float offsetYpercent = 0.05f;
		*/

		// for width = 1280 and height = 720
		float scaleX = 128f;
		float scaleY = 72f;
		float offsetXpercent = 0.05f;
		float offsetYpercent = 0.05f;


		float originX = feedWidth / 2f;
		float originY = feedHeight / 2f;

		//Mapping detected facial coordinates to Worldspace
		float recenterX = flipFactorX*(xValue - originX);
		float recenterY = flipFactorY*(yValue - originY);

		float offsetX = offsetXpercent * feedWidth;
		float offsetY = offsetYpercent * feedHeight;

		//Normalizing final Coordinates
		float normalizeX = (recenterX + offsetX)/scaleX;
		float normalizeY = (recenterY + offsetY)/scaleY;

		HealthCubeCoordinates.x = normalizeX;
		HealthCubeCoordinates.y = normalizeY;
		HealthCubeCoordinates.z = 10f;

		healthbCubeSizeOld = newSize;
	}

	public void SetHealthCubeColor(Color newHealthCubeColor){
		healthCubeColor = newHealthCubeColor;
	}

	public void SetHealthCubeSize(float newSize){

		if (newSize > 125f) {
			newSize = 125f;	
		}

		healthCubeSize.x = 0.0002f*125;
		healthCubeSize.y = 0.0003f*125;
		healthCubeSize.z = 1;
		//healthCubeSize.x = 0.0002f*newSize;
		//healthCubeSize.y = 0.0003f*newSize;
	}
}
