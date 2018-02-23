using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Affdex;

public class UIManager : MonoBehaviour {

	// Import Game Manager object
	public GameObject gameManagerObject;
	private GameManager gameManagerScript;

	public Camera mainCamera;
	[Space(10)]

	// Import Webcam input object
	public GameObject inputDeviceCamera;
	private CameraInput camInputScript;
	[Space(10)]

	// Import and Configure Webcam output object
	public GameObject webcamRenderPlane;
	public float displayHeight = 0.5625f;
	public bool flipHorizontal;
	public bool flipVertical;
	private WebCamTexture defaultCamTexture;
	private Renderer planeRenderer;
	[Space(10)]

	// Update the colors on-screen every X seconds
	public float colorUpdateTime = 0.5f;
	public float lerpTime = 0.25f;

	private Color currentEmotionColor;
	private Color previousEmotionColor;

	// Use this for initialization
	void Start () {
		gameManagerScript = gameManagerObject.GetComponent<GameManager>();
		camInputScript = inputDeviceCamera.GetComponent<CameraInput>();
		planeRenderer = webcamRenderPlane.GetComponent<Renderer> ();
		
		// Set the webcamRenderPlane to have the same aspect ratio as the video feed
		float aspectRatio = camInputScript.targetWidth / (float) camInputScript.targetHeight;
		webcamRenderPlane.transform.localScale = new Vector3(aspectRatio, 1.0f, 1.0f);
		Debug.Log (aspectRatio);

		float flipX = flipHorizontal ? 1f : -1f;
		float flipY = flipVertical ? 1f : -1f;

		webcamRenderPlane.transform.localScale = new Vector3 (flipX*aspectRatio*displayHeight, 1.0f, flipY*displayHeight);
		defaultCamTexture = camInputScript.Texture as WebCamTexture;

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
		//planeRenderer.material.mainTexture = defaultCamTexture;
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
	/*
	[HideInInspector] public Vector3 moodTrackerCoordinates;
	[HideInInspector] public Vector3 moodTrackerSize;
	[HideInInspector] public Color moodTrackerColor;
	*/

	public Vector3 normalizedMoodTrackerCoordinates;
	public Vector3 moodTrackerSize;
	public Color moodTrackerColor;

	public void SetMoodTrackerGeometry (Vector3 moodTrackerCoordinates){

		float xValue = moodTrackerCoordinates.x;
		float yValue = moodTrackerCoordinates.y;
		float interOcularDistance = moodTrackerCoordinates.z;

		// Debug.Log ("xValue: " + xValue + " yValue: " + yValue + " IOD: " + interOcularDistance);
		// Mapping - Camera feed to Mixed Reality Worldspace
		// Offset X/Y to make cube appear above face and 
		// incline towards a side(20% or 40% screen width or height)
		// Account for Horizontal flip and Vertical flip
		// Works good for width = 1280 and height = 720

		// Camera feed parameters
		float feedWidth = 1280f;
		float feedHeight = 720f;

		float flipFactorX = 1f;
		float flipFactorY = -1f;

		// Define scaling by hit-and-trial
		float offsetX = 0f;
		float offsetY = 8f;
		float scaleXpercent  = 0.1f;
		float scaleYpercent  = 0.1f;

		// Mapping detected facial coordinates to Worldspace
		float originX = feedWidth / 2f;
		float originY = feedHeight / 2f;

		float recenterX = flipFactorX * (xValue - originX);
		float recenterY = flipFactorY * (yValue - originY);


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

	/*
	public void SetMoodTrackerColor(Color newMoodTrackerColor){
		moodTrackerColor = newMoodTrackerColor;
	}
	*/

	public Color GetMoodTrackerColor(){
		return currentEmotionColor;
	}
/////////////////////////////////////////// SET MOOD TRACKER ATTRIBUTES  END ////////////////////////////////////////////////////////

}
