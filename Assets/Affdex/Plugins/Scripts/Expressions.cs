using UnityEngine;

namespace Affdex
{

    /// <summary>
    /// Extension "On" method for the emotion and expression enums to simplify the mask
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension "On" method for the emotion to simplify the mask
        /// </summary>
        public static bool On(this Emotions emotion, Emotions value)
        {
            //The layermask in Unity uses bitmask values for the selection.
            //Find the value they mask with by pow(2, num);

            int unityVal = (int)Mathf.Pow(2, (int)value);
            
            return ((int)emotion & unityVal) == unityVal;
        }

        /// <summary>
        /// Extension "On" method for the expression enums to simplify the mask
        /// </summary>
        public static bool On(this Expressions expression, Expressions value)
        {
            int unityVal = (int)Mathf.Pow(2, (int)value);
            return ((int)expression & unityVal) == unityVal;
        }
    }

    // The following enums need to stay in sync with the values in AffdexWrapper.h

    /// <summary>
    /// Enum of currently supported expressions (based on FACS https://en.wikipedia.org/wiki/Facial_Action_Coding_System )
    /// </summary>
    public enum Expressions
    {
        /// <summary>
        /// Smile score range of the value is [0, 100]
        /// </summary>
        Smile = 0,
        /// <summary>
        /// Inner brow raise (AU01) score range of the value is [0, 100]
        /// </summary>
		InnerBrowRaise,
        /// <summary>
        /// Brow raise score range of the value is [0, 100]
        /// </summary>
		BrowRaise,
        /// <summary>
        /// Brow furrow score range of the value is [0, 100]
        /// </summary>
		BrowFurrow,
        /// <summary>
        /// Nose wrinkler (AU09) score range of the value is [0, 100]
        /// </summary>
		NoseWrinkle,
        /// <summary>
        /// Upper lip raiser (AU05) score range of the value is [0, 100]
        /// </summary>
		UpperLipRaise,
        /// <summary>
        /// Lip corner depressor (AU15) score range of the value is [0, 100]
        /// </summary>
		LipCornerDepressor,
        /// <summary>
        /// Chin raiser (AU17) score range of the value is [0, 100]
        /// </summary>
		ChinRaise,
        /// <summary>
        /// Lip suck (AU28) score range of the value is [0, 100]
        /// </summary>
		LipPucker,
        /// <summary>
        /// Lip pucker (AU18) score range of the value is [0, 100]
        /// </summary>
		LipPress,
        /// <summary>
        /// Lip suck (AU28) score range of the value is [0, 100]
        /// </summary>
		LipSuck,
        /// <summary>
        /// Mouth open (AU25) score range of the value is [0, 100]
        /// </summary>
		MouthOpen,
        /// <summary>
        /// Smirk score range of the value is [0, 100]
        /// </summary>
		Smirk,
        /// <summary>
        /// Eye closure score range of the value is [0, 100]
        /// </summary>
		EyeClosure,
        /// <summary>
        /// Attention score range of the value is [0, 100]
        /// </summary>
		Attention
    }

    /// <summary>
    /// Enum of currently supported emotions
    /// </summary>
    public enum Emotions
    {
        /// <summary>
        /// Joy score range of the value is [0, 100]
        /// </summary>
        Joy = 0,
        /// <summary>
        /// Fear score range of the value is [0, 100]
        /// </summary>
		Fear,
        /// <summary>
        /// Disgust score range of the value is [0, 100]
        /// </summary>
		Disgust,
        /// <summary>
        /// Sadness score range of the value is [0, 100]
        /// </summary>
		Sadness,
        /// <summary>
        /// Anger score range of the value is [0, 100]
        /// </summary>
		Anger,
        /// <summary>
        /// Suprise score range of the value is [0, 100]
        /// </summary>
		Surprise,
        /// <summary>
        /// Contempt score range of the value is [0, 100]
        /// </summary>
		Contempt,
        /// <summary>
        /// Valence (composite of negative and positive emotions) score range of the value is [-100, 100]
        /// </summary>
		Valence,
        /// <summary>
        /// Engagment score range of the value is [0, 100]
        /// </summary>
		Engagement
    }
}
