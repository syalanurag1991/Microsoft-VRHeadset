using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Affdex;

public class FacialEmotionAnalyzer : ImageResultsListener {

	public EmotionStruct currentEmotions;
	private Vector3 moodTrackerParameters;

	// Use this for initialization
	void Start () {
		currentEmotions = new EmotionStruct();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public EmotionStruct getCurrentEmotions()
	{
		Debug.Log("Got current emotions.");
		return currentEmotions;
	}

	public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face");
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face");
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        Debug.Log("Got face results");

        foreach (KeyValuePair<int, Face> pair in faces) {
            int FaceId = pair.Key;  // The Face Unique Id.
            Face face = pair.Value;    // Instance of the face class containing emotions, and facial expression values.

            //Retrieve the Emotions Scores
            // face.Emotions.TryGetValue(Emotions.Contempt, out currentContempt);
            // face.Emotions.TryGetValue(Emotions.Valence, out currentValence);
			face.Emotions.TryGetValue(Emotions.Joy, out currentEmotions.joy);
            face.Emotions.TryGetValue(Emotions.Fear, out currentEmotions.fear);
            face.Emotions.TryGetValue(Emotions.Disgust, out currentEmotions.disgust);
            face.Emotions.TryGetValue(Emotions.Sadness, out currentEmotions.sadness);
            face.Emotions.TryGetValue(Emotions.Anger, out currentEmotions.anger);
            face.Emotions.TryGetValue(Emotions.Surprise, out currentEmotions.surprise);
            

            //Retrieve the Smile Score
            // face.Expressions.TryGetValue(Expressions.Smile, out currentSmile);


            //Retrieve the Interocular distance, the distance between two outer eye corners.
            float currentInterocularDistance = face.Measurements.interOcularDistance;

			//Retrieve the coordinates of the facial landmarks (face feature points)
			FeaturePoint[] featurePointsList = face.FeaturePoints;
			Measurements measurementsList = face.Measurements;

			moodTrackerParameters.x = featurePointsList [12].x;
			moodTrackerParameters.y = featurePointsList [12].y;
			moodTrackerParameters.z = measurementsList.interOcularDistance;

        }
    }

	public Vector3 GetMoodTrackerGeometry (){
		return moodTrackerParameters;
	}

}
