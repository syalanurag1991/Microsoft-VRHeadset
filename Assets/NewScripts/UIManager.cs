using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Affdex;

public class UIManager : MonoBehaviour {

	public GameObject gameManagerObject;
	private GameManager gameManagerScript;

	public Camera mainCamera;
	public GameObject inputDeviceCamera;
	public GameObject webcamRenderPlane;

	private CameraInput camInputScript;
	private Renderer planeRenderer;

	private Color currentEmotionColor;
	private Color previousEmotionColor;

	public float colorUpdateTime = 0.5f;	// Update the colors on-screen every X seconds
	private float lerpTime = 0.25f;

	// Use this for initialization
	void Start () {
		gameManagerScript = (GameManager) gameManagerObject.GetComponent(typeof(GameManager));
		camInputScript = (CameraInput) inputDeviceCamera.GetComponent<CameraInput>();
		planeRenderer = (Renderer) webcamRenderPlane.GetComponent<Renderer>();
		
		// Set the webcamRenderPlane to have the same aspect ratio as the video feed
		float aspectRatio = camInputScript.targetWidth / (float) camInputScript.targetHeight;
		webcamRenderPlane.transform.localScale = new Vector3(aspectRatio, 1.0f, 1.0f);

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
	}

	// Coroutine enumerator for updating the current emotion color using linear interpolation over a predefined amount of time
	private IEnumerator UpdateBackgroundColor()
	{		
		// Debug.Log("Entered UPDATE BACKGROUND COLOR COROUTINE.");
		float t = 0;
		while (t < 1)
		{
			// Now the loop will execute on every end of frame until the condition is true
			mainCamera.backgroundColor = Color.Lerp(previousEmotionColor, currentEmotionColor, t);
			t += Time.deltaTime / lerpTime;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator RequestEmotionUpdate()
	{
		// Debug.Log("Entered REQUEST EMOTION UPDATE COROUTINE.");
		while (true) 
		{
			yield return new WaitForSeconds(colorUpdateTime);
			previousEmotionColor = currentEmotionColor;
			currentEmotionColor = gameManagerScript.getCurrentCumulativeEmotionColor();
			StartCoroutine(UpdateBackgroundColor());
		}
	}
}
