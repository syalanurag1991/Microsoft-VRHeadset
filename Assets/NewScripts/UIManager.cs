using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Affdex;

public class UIManager : MonoBehaviour {

	// Import Game Manager object
	public GameObject gameManagerObject;
	private GameManager gameManagerScript;
	[Space(10)]

	// Import Webcam input object
	public Camera mainCamera;
	public GameObject inputDeviceCamera;
	public GameObject webcamRenderPlane;
	private CameraInput camInputScript;

	// Use this for initialization
	void Start () {
		gameManagerScript = gameManagerObject.GetComponent<GameManager>();
		camInputScript = inputDeviceCamera.GetComponent<CameraInput>();
		planeRenderer = webcamRenderPlane.GetComponent<Renderer> ();

		// Camera feed parameters
		if (camInputScript.Texture == null) {
			Debug.Log ("Camera not started");
			feedWidth = camInputScript.targetWidth;
			feedHeight = camInputScript.targetHeight;
			camReady = false;
		}
			
		SetFeed ();

		// Initalize the colors
		previousEmotionColor = new Color();
		currentEmotionColor = new Color();

		// Start the background emotion updater
		StartCoroutine(RequestEmotionUpdate());
	}
	
	// Update is called once per frame
	void Update () {
		
		// Display the webcam input
		planeRenderer.material.mainTexture = camInputScript.Texture;
	
		if (!camReady) {
			if (camInputScript.Texture == null) {
				Debug.Log ("Camera not started");
				feedWidth = camInputScript.targetWidth;
				feedHeight = camInputScript.targetHeight;
				camReady = false;
			} else {
				Debug.Log ("Camera is Working");
				feedWidth = camInputScript.Texture.width;
				feedHeight = camInputScript.Texture.height;
				camReady = true;
				SetFeed ();
			}
		}
			
	}
		
////////////////////////////////////////////////// BACKGROUND COLOR UPDATE START //////////////////////////////////////////////////////
	// Coroutine enumerator for updating the current emotion color using linear interpolation over a predefined amount of time
	/*
	private IEnumerator UpdateBackgroundColor()	{		
		
		// Debug.Log("Entered UPDATE BACKGROUND COLOR COROUTINE.");
		float t = 0;
		while (t < 1) {
			// Now the loop will execute on every end of frame until the condition is true
			mainCamera.backgroundColor = Color.Lerp(previousEmotionColor, currentEmotionColor, t);
			t += Time.deltaTime / lerpTime;
			yield return new WaitForEndOfFrame();
		}


	}
	*/
////////////////////////////////////////////////// BACKGROUND COLOR UPDATE START //////////////////////////////////////////////////////

////////////////////////////////////////////////// EMOTION UPDATE START ///////////////////////////////////////////////////////////////
	private IEnumerator RequestEmotionUpdate() {

		// Debug.Log("Entered REQUEST EMOTION UPDATE COROUTINE.");
		while (true) {

			yield return new WaitForSeconds(colorUpdateTime);

			previousEmotionColor = currentEmotionColor;
			currentEmotionColor = gameManagerScript.getCurrentCumulativeEmotionColor();

			//StartCoroutine(UpdateBackgroundColor());
		}
	}
////////////////////////////////////////////////// EMOTION UPDATE END ///////////////////////////////////////////////////////////////


/////////////////////////////////////////// SET MOOD TRACKER ATTRIBUTES  START //////////////////////////////////////////////////////
	[HideInInspector] public Vector3 normalizedMoodTrackerCoordinates;
	[HideInInspector] public Vector3 moodTrackerSize;
	[HideInInspector] public Color moodTrackerColor;

	// Define moodtracker scaling and offset by hit-and-trial
	[Space(10)]
	public float offsetX = 0f;
	public float offsetY = 8f;
	public float scaleXpercent  = 0.1f;
	public float scaleYpercent  = 0.1f;

	// Update the colors on-screen every X seconds
	public float colorUpdateTime = 0.5f;
	public float lerpTime = 0.25f;
	[Space(10)]

	private Color currentEmotionColor;
	private Color previousEmotionColor;

	public void SetMoodTrackerGeometry (Vector3 moodTrackerCoordinates){

		float flipTrackerX = flipHorizontal ? -1f : 1f;
		float flipTrackerY = flipVertical ? 1f : -1f;

		float xValue = moodTrackerCoordinates.x;
		float yValue = moodTrackerCoordinates.y;
		float interOcularDistance = moodTrackerCoordinates.z;

		// Debug.Log ("xValue: " + xValue + " yValue: " + yValue + " IOD: " + interOcularDistance);
		// Mapping - Camera feed to Mixed Reality Worldspace
		// Offset X/Y to make cube appear above face and 
		// incline towards a side(20% or 40% screen width or height)
		// Account for Horizontal flip and Vertical flip
		// Works good for width = 1280 and height = 720

		// Mapping detected facial coordinates to Worldspace
		float originX = feedWidth / 2f;
		float originY = feedHeight / 2f;

		float recenterX = flipTrackerX * (xValue - originX);
		float recenterY = flipTrackerY * (yValue - originY);

		// Normalizing final Coordinates
		float scaleX = scaleXpercent * feedWidth;
		float scaleY = scaleYpercent * feedHeight;

		float offsetXpercent = offsetX * interOcularDistance / originX;
		float offsetYpercent = offsetY * interOcularDistance / originY;

		float normalizeX = (recenterX / scaleX) + offsetXpercent;
		float normalizeY = (recenterY / scaleY) + offsetYpercent;

		// Assigning values
		normalizedMoodTrackerCoordinates.x = normalizeX;
		normalizedMoodTrackerCoordinates.y = normalizeY;
		normalizedMoodTrackerCoordinates.z = 10f;


	}

	public Color GetMoodTrackerColor(){
		return currentEmotionColor;
	}
/////////////////////////////////////////// SET MOOD TRACKER ATTRIBUTES  END ////////////////////////////////////////////////////////

//////////////////////////////////////////////// SET CAMERA FEED  START /////////////////////////////////////////////////////////////
	// Configure Webcam output object
	[Space(10)]
	public float displayHeight = 0.54f;
	public bool flipHorizontal = false;
	public bool flipVertical = true;

	private Renderer planeRenderer;
	private float feedWidth;
	private float feedHeight;
	private bool camReady;

	public void SetFeed (){

		float flipDisplayX = flipHorizontal ? 1f : -1f;
		float flipDisplayY = flipVertical ? 1f : -1f;

		// Set the webcam-Render-Plane to have the same aspect ratio as the video feed
		float aspectRatio = feedWidth / feedHeight;
		webcamRenderPlane.transform.localScale = new Vector3 (flipDisplayX*aspectRatio*displayHeight, 1.0f, flipDisplayY*displayHeight);

		Debug.Log (" Feed Width: " + feedWidth + " Feed Height: " + feedHeight + " Aspect Ratio: " + aspectRatio);

	}

///////////////////////////////////////////////// SET CAMERA FEED  END //////////////////////////////////////////////////////////////
}
