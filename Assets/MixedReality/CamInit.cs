///////////////////////////////////////////////
/// Author: Anurag Syal ///////////////////////
/// syal.anurag1991@gmail.com /////////////////
/// visit 360otg.com //////////////////////////
/// ///////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Affdex;

public class CamInit : MonoBehaviour {

	private bool camAvailable;
	private WebCamTexture backCam;
	private WebCamTexture frontCam;
	private WebCamTexture defaultCam;
	private Texture defaultBackground;
	private Renderer renderer;

	//public RawImage background;
	//public AspectRatioFitter aspectRatioFit;

	// Use this for initialization
	void Start () {
		//defaultBackground = background.texture;
		WebCamDevice[] cameraDevices = WebCamTexture.devices;

		Debug.Log ("Number of Cameras: " + cameraDevices.Length);
		if (cameraDevices.Length == 0) {
			Debug.Log ("Unable to detected to camera!!");
			camAvailable = false;
			return;
		}

		for (int i = 0; i < cameraDevices.Length; i++) {
			if (cameraDevices [i].isFrontFacing) {
				frontCam = new WebCamTexture (cameraDevices [i].name, Screen.width, Screen.height);
			} else {
				backCam = new WebCamTexture (cameraDevices [i].name, Screen.width, Screen.height);
			}
		}

		defaultCam = backCam;

		if (backCam == null) {
			Debug.Log ("Back Camera not available!!");
			defaultCam = frontCam;
		}

		defaultCam.Play ();

		renderer = gameObject.GetComponent<Renderer> ();
		renderer.material.mainTexture = defaultCam;

		//background.texture = defaultCam;
		camAvailable = true;


	}

	// Update is called once per frame
	void Update () {
		if (!camAvailable) {
			Debug.Log ("No cam");
			return;
		}

		//float ratio = (float)defaultCam.width / (float)defaultCam.height;
		//aspectRatioFit.aspectRatio = ratio;

		float flip = defaultCam.videoVerticallyMirrored ? 1f : -1f;
		gameObject.transform.localScale = new Vector3 (8f, flip*4.5f, 1f);

		//background.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);

		//int orient = -defaultCam.videoRotationAngle;
		//background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);
	}
}
