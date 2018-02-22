using UnityEngine;
using Affdex;

public class ViewCam : MonoBehaviour {

    public Affdex.CameraInput cameraInput;
    public Affdex.VideoFileInput movieInput;

	// Use this for initialization
	void Start () {
		if (! AffdexUnityUtils.ValidPlatform ())
			return;

        Texture texture = movieInput != null ? movieInput.Texture : cameraInput.Texture;

        if (texture == null)
            return;

        this.GetComponent<MeshRenderer>().material.mainTexture = texture;

        // rotate the image to be upright on the display
		if (cameraInput != null) {
			float videoRotationAngle = -cameraInput.videoRotationAngle;
			transform.rotation = transform.rotation * Quaternion.AngleAxis (videoRotationAngle, Vector3.forward);
		}

        float wscale = (float)texture.width / (float)texture.height;
       
        transform.localScale = new Vector3(transform.localScale.y * wscale, transform.localScale.y, 1);
	}
}