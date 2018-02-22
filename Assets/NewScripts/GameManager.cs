using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject gameManager;
	public GameObject uiManager;

	public GameObject facialEmotionAnalyzerObject;
	public GameObject wordSentimentEmotionAnalyzerObject;
	public GameObject vocalEmotionAnalyzerObject;

	private FacialEmotionAnalyzer facialAnalyzer;
	// private WordSentimentEmotionAnalyzer wordAnalyzer;
	// private VocalEmotionAnalyzer vocalAnalyzer;

	// Flags for enabling and disabling certain emotion analysis features
	public bool useFacialEmotion = false;
	public bool useWordSentimentEmotion = false;
	public bool useVocalToneEmotion = false;

	public EmotionStruct currentCumulativeEmotion;
	private float emotionThreshold = 20.0f;

	// Use this for initialization
	void Start () {
		// Initialize the current emotion
		currentCumulativeEmotion = new EmotionStruct();

		// Find the script for facial emotion analysis
		try
		{
			facialAnalyzer = (FacialEmotionAnalyzer) facialEmotionAnalyzerObject.GetComponent(typeof(FacialEmotionAnalyzer)); // this seems to fail silently...
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to find facial emotion analyzer. This functionality will be disabled.");
			useFacialEmotion = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Calculate a synthesized emotional state for the user
		calculateCumulativeEmotion();
	}

	// Returns a final emotion based on the emotion input types that are specified using a simple weighted average.
	public void calculateCumulativeEmotion() {

		EmotionStruct emotionSum = new EmotionStruct();
		int numEmotionModes = 0;

		if (useFacialEmotion)
		{
			EmotionStruct facialEmotions = facialAnalyzer.getCurrentEmotions();
			emotionSum.joy += facialEmotions.joy;
			emotionSum.sadness += facialEmotions.sadness;
			emotionSum.anger += facialEmotions.anger;
			emotionSum.fear += facialEmotions.fear;
			emotionSum.disgust += facialEmotions.disgust;
			emotionSum.surprise += facialEmotions.surprise;

			numEmotionModes++;
		}
		if (useWordSentimentEmotion)
		{
			Debug.Log("Need to implement emotion from word sentiment.");
		}
		if (useVocalToneEmotion)
		{
			Debug.Log("Need to implement emotion from vocal tone.");
		}

		if (numEmotionModes > 0) 
		{
			currentCumulativeEmotion.joy = emotionSum.joy / (float) numEmotionModes;
			currentCumulativeEmotion.sadness = emotionSum.sadness / (float) numEmotionModes;
			currentCumulativeEmotion.anger = emotionSum.anger / (float) numEmotionModes;
			currentCumulativeEmotion.fear = emotionSum.fear / (float) numEmotionModes;
			currentCumulativeEmotion.disgust = emotionSum.disgust / (float) numEmotionModes;
			currentCumulativeEmotion.surprise = emotionSum.surprise / (float) numEmotionModes;
		}
			
	}

	// Returns a color to be used by the user interface based on the current synthesized emotion
	public Color getCurrentCumulativeEmotionColor()
	{
		if (currentCumulativeEmotion.joy > currentCumulativeEmotion.fear &&
			currentCumulativeEmotion.joy > currentCumulativeEmotion.disgust &&
			currentCumulativeEmotion.joy > currentCumulativeEmotion.sadness && 
			currentCumulativeEmotion.joy > currentCumulativeEmotion.anger && 
			currentCumulativeEmotion.joy > currentCumulativeEmotion.surprise &&
			currentCumulativeEmotion.joy > emotionThreshold)
		{
			return new Color(0.0f, 1.0f, 0.0f, 1.0f);		// green
		}
		else if (currentCumulativeEmotion.fear > currentCumulativeEmotion.disgust &&
				 currentCumulativeEmotion.fear > currentCumulativeEmotion.sadness &&
				 currentCumulativeEmotion.fear > currentCumulativeEmotion.anger &&
				 currentCumulativeEmotion.fear > currentCumulativeEmotion.surprise &&
				 currentCumulativeEmotion.fear > emotionThreshold)
		{
			return new Color(1.0f, 0.0f, 1.0f, 1.0f);		// magenta
		}
		else if (currentCumulativeEmotion.disgust > currentCumulativeEmotion.sadness &&
				 currentCumulativeEmotion.disgust > currentCumulativeEmotion.anger &&
				 currentCumulativeEmotion.disgust > currentCumulativeEmotion.surprise &&
				 currentCumulativeEmotion.disgust > emotionThreshold)
		{
			return new Color(1.0f, 1.0f, 0.0f, 1.0f);		// yellow
		}
		else if (currentCumulativeEmotion.sadness > currentCumulativeEmotion.anger &&
				 currentCumulativeEmotion.sadness > currentCumulativeEmotion.surprise &&
				 currentCumulativeEmotion.sadness > emotionThreshold)
		{
			return new Color(0.0f, 0.0f, 1.0f, 1.0f);		// blue
		}
		else if (currentCumulativeEmotion.anger > currentCumulativeEmotion.surprise &&
				 currentCumulativeEmotion.anger > emotionThreshold)
		{
			return new Color(1.0f, 0.0f, 0.0f, 1.0f);		// red
		}
		else if(currentCumulativeEmotion.surprise > emotionThreshold)
		{
			return new Color(1.0f, 1.0f, 1.0f, 1.0f);		// white
		}
		else
		{
			return new Color(0.0f, 0.0f, 0.0f, 1.0f);		// black
		}
	}

}
