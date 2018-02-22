using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Affdex
{
    /// <summary>
    /// Structure containing the measurements
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Measurements
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        float[] orientation;
        
        /// <summary>
        /// Distance between the two outer eye corners in pixels
        /// </summary>
        public float interOcularDistance;
 
        /// <summary>
        /// Head orientation angles
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return Quaternion.Euler(orientation[0], orientation[1], orientation[2]);
            }
        }
    }

    /// <summary>
    /// Structure containing basic feature point coordinates.
    /// <para>
    /// Coordinate system is an x,y system with the top-left pixel center at ( x=0, y=0 )
    /// and the bottom right pixel at ( x=width-1, y=height-1 )
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FeaturePoint
    {
        /// <summary> Point identifier. </summary>
        public Int32 id;

        /// <summary> X-coordinate of point</summary>
        public float x;

        /// <summary> Y-coordinate of point</summary>
        public float y;
    }

    /// <summary>
    /// Structure containing value of a specific expression
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ExpressionPoint
    {
        /// <summary>
        /// Expression identifier
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Expressions expression;

        /// <summary>
        /// Current expression's value.  Range of the expression's value is [0.0, 100.0]
        /// </summary>
        public float value;
    }

    /// <summary>
    /// Structure containing value of a specific emotion
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EmotionPoint
    {
        /// <summary>
        /// Emotion identifier
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Emotions emotion;

        /// <summary>
        /// Current expression's value.  Range of the expression's value is [-100, 100]
        /// </summary>
        public float value;
    }

    /// <summary>
    /// Structure containing value of current face values
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FaceData
    {
        /// <summary>
        /// Structure containing the measurements
        /// </summary>
        public Measurements measurements;

        /// <summary>
        /// Array containing current emotions
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public EmotionPoint[] emotions;

        /// <summary>
        /// Array containing current expressions
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public ExpressionPoint[] expressions;

        /// <summary>
        /// Array containing basic feature point coordinates.
        /// <para>
        /// Coordinate system is an x,y system with the top-left pixel center at ( x=0, y=0 )
        /// and the bottom right pixel at ( x=width-1, y=height-1 )
        /// </para>
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 34)]
        public FeaturePoint[] featurePoints;
    }

    /// <summary>
    /// Represents a face found within a processed Frame.
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Structure containing the measurements
        /// </summary>
        public Measurements Measurements { get; private set; }

        /// <summary>
        /// Array containing basic feature point coordinates.
        /// <para>
        /// Coordinate system is an x,y system with the top-left pixel center at ( x=0, y=0 )
        /// and the bottom right pixel at ( x=width-1, y=height-1 )
        /// </para>
        /// </summary>
        public FeaturePoint[] FeaturePoints { get; private set; }

        /// <summary>
        /// Dictionary of set expressions (based on FACS https://en.wikipedia.org/wiki/Facial_Action_Coding_System )
        /// </summary>
        public Dictionary<Expressions, float> Expressions { get; private set; }

        /// <summary>
        /// Dictionary of set emotions
        /// </summary>
        public Dictionary<Emotions, float> Emotions { get; private set; }

        internal Face(FaceData data)
        {
            Measurements = data.measurements;
            FeaturePoints = data.featurePoints;
           
            Expressions = new Dictionary<Affdex.Expressions, float>();
            for(int i = 0; i < data.expressions.Length; i++)
            {
                Expressions[data.expressions[i].expression] = data.expressions[i].value;
            }
         
            Emotions = new Dictionary<Affdex.Emotions, float>();
            for (int i = 0; i < data.emotions.Length; i++)
            {
                Emotions[data.emotions[i].emotion] = data.emotions[i].value;
            }
        }

        /// <summary>
        /// Outputs face data to a string (can be used for debugging)
        /// </summary>
        public override string ToString()
        {
            string s = "Affdex Face\n";
            s += String.Format("Measurements: {0:F2}, {1:F2}, {2:F2} : {3:F2}\n", Measurements.Orientation.x, Measurements.Orientation.y, Measurements.Orientation.z, Measurements.interOcularDistance);
            s += "Expressions\n";
            for(int i = 0; i < Expressions.Count; i++)
            {
                s += String.Format("{0} : {1:F2}\n", (Affdex.Expressions)i, Expressions[(Affdex.Expressions)i]);
            }
            s += "Emotions\n";
            for(int i = 0; i < Emotions.Count; i++)
            {
                s += String.Format("{0} : {1:F2}\n", (Affdex.Emotions)i, Emotions[(Affdex.Emotions)i]);
            }
            return s;
        }
    }
}
