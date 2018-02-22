using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

using Affdex;

public class PlayerEmotions :  ImageResultsListener {
    
	public GameManager gameManager;

	// public float currentSmile;
    // public float currentInterocularDistance;
    // public float currentValence;

    // Emotions
    public float currentJoy;
    public float currentFear;
    public float currentDisgust;
    public float currentSadness;
    public float currentAnger;
    public float currentSurprise;
    // public float currentContempt;

    public FeaturePoint[] featurePointsList;
	public Measurements measurementsList;

	// UI Variables
    //public Camera mainCamera;
    //public Text currentEmotionText;

 	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {


		// Update the UI to reflect the emotion currently being detected.

		Color newColor;

		if (currentJoy > currentFear &&
			currentJoy > currentDisgust &&
			currentJoy > currentSadness && 
			currentJoy > currentAnger && 
			currentJoy > currentSurprise)
		{
			//currentEmotionText.text = "Joy";
			//mainCamera.backgroundColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);		// green
			Debug.Log("Joy");
			newColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);		// green
		}
		else if (currentFear > currentDisgust &&
				 currentFear > currentSadness &&
				 currentFear > currentAnger &&
				 currentFear > currentSurprise)
		{
			//currentEmotionText.text = "Fear";
			//mainCamera.backgroundColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);		// magenta
			Debug.Log("Fear");
			newColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);		// magenta
		}
		else if (currentDisgust > currentSadness &&
				 currentDisgust > currentAnger &&
				 currentDisgust > currentSurprise)
		{
			//currentEmotionText.text = "Disgust";
			//mainCamera.backgroundColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);		// yellow
			Debug.Log("Disgust");
			newColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);		// yellow
		}
		else if (currentSadness > currentAnger &&
				 currentSadness > currentSurprise)
		{
			//currentEmotionText.text = "Sadness";
			//mainCamera.backgroundColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);		// blue
			Debug.Log("Sad");
			newColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);		// blue
		}
		else if (currentAnger > currentSurprise)
		{
			//currentEmotionText.text = "Anger";
			//mainCamera.backgroundColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);		// red
			Debug.Log("Anger");
			newColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);		// red
		}
		else
		{
			//currentEmotionText.text = "Surprise";
			//mainCamera.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);		// white
			Debug.Log("Surprise");
			newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);		// white
		}

		//mainCamera.backgroundColor = newColor;
		gameManager.GetComponent<GameManager> ().SetHealthBarColor (newColor);
		gameManager.GetComponent<GameManager> ().SetHealthCubeColor (newColor);

		Debug.Log ("Player Emotion Color Detected: " + newColor);

	}

    public override void onFaceFound(float timestamp, int faceId)
    {
        //Debug.Log("Found the face");
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        //Debug.Log("Lost the face");
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        Debug.Log("Got face results");

        foreach (KeyValuePair<int, Face> pair in faces)
        {
            int FaceId = pair.Key;  // The Face Unique Id.
            Face face = pair.Value;    // Instance of the face class containing emotions, and facial expression values.
			//Debug.Log(face);

            //Retrieve the Emotions Scores
            // face.Emotions.TryGetValue(Emotions.Contempt, out currentContempt);
            // face.Emotions.TryGetValue(Emotions.Valence, out currentValence);
			face.Emotions.TryGetValue(Emotions.Joy, out currentJoy);
            face.Emotions.TryGetValue(Emotions.Fear, out currentFear);
            face.Emotions.TryGetValue(Emotions.Disgust, out currentDisgust);
            face.Emotions.TryGetValue(Emotions.Sadness, out currentSadness);
            face.Emotions.TryGetValue(Emotions.Anger, out currentAnger);
            face.Emotions.TryGetValue(Emotions.Surprise, out currentSurprise);
            
			//Retrieve the coordinates of the facial landmarks (face feature points)
            featurePointsList = face.FeaturePoints;
			measurementsList = face.Measurements;

			float xCoordinate = featurePointsList [12].x;
			float yCoordinate = featurePointsList [12].y;
			float interOcularDistance = measurementsList.interOcularDistance;

			//Debug.Log ("x-coordinate: " + xCoordinate);
			//Debug.Log ("y-coordinate: " + yCoordinate);

			//Debug.Log ("Eye Detection - " + featurePointsList[12].y);
			//Debug.Log ("Inter-Ocular: " + interOcularDistance);

			gameManager.SetHealthBarCoordinates (xCoordinate, yCoordinate, interOcularDistance);
			gameManager.SetHealthBarSize (interOcularDistance);

			gameManager.SetHealthCubeCoordinates (xCoordinate, yCoordinate, interOcularDistance);
			gameManager.SetHealthCubeSize (interOcularDistance);
        }

		//Debug.Log (featurePointsList);
    }
}
