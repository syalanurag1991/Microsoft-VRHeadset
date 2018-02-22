// Unity derives Debug Feature Viewer component's UI from this file
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Affdex
{
    /// <summary>
    /// Shows features for the detector on a Quad primitive.  Must be attached to a Quad GameObject or will not function properly.
    /// </summary>
    public class DebugFeatureViewer : MonoBehaviour
    {
        
        internal Face face;

        /// <summary>
        /// Texture to be used by the DebugFeatureViewer class
        /// </summary>
        public Texture debugTexture;

        /// <summary>
        /// Creates a texture if there is not already one defined.
        /// </summary>
        /// <returns>debugTexture</returns>
        public Texture DebugTexture
        { 
            get
            {
               
                if(debugTexture == null)
                {
                    Detector d = GameObject.FindObjectOfType<Detector>();
                    IDetectorInput input = d.gameObject.GetComponent<IDetectorInput>();
                    if (input != null)
                        debugTexture = input.Texture;
                }

              

                return debugTexture;     

            }
        }

        /// <summary>
        /// Get a normalized position based on the input texture
        /// </summary>
        /// <param name="fp">Feature point to normalize</param>
        /// <returns>A normalized local point with XY coordinates</returns>
        Vector3 NormalizePoint(FeaturePoint fp)
        {

            if (DebugTexture == null)
                return Vector3.zero;

            return new Vector3(fp.x / (float)DebugTexture.width - 0.5f, (1-fp.y / (float)DebugTexture.height) - 0.5f, 0);
        }

        /// <summary>
        /// Get the world position of a local space normalized point of a Quad
        /// </summary>
        /// <param name="fp"></param>
        /// <returns></returns>
        Vector3 GetWorldPoint(FeaturePoint fp)
        {
            Vector3 fpLocal = NormalizePoint(fp);


           return transform.TransformPoint(fpLocal);
        }

        /// <summary>
        /// Unity callback for drawing gizmos, used to draw Affectiva debug information
        /// </summary>
        public void OnDrawGizmos()
        {
            
            if (!Application.isPlaying)
                return;

            if ( DebugTexture == null || face == null)
                return;

            //Draw bridge
            Gizmos.DrawLine(GetWorldPoint(face.FeaturePoints[11]), GetWorldPoint(face.FeaturePoints[12]));
            //draw nose tip
            Gizmos.DrawWireSphere(GetWorldPoint(face.FeaturePoints[12]), 0.1f);

            Gizmos.DrawLine(GetWorldPoint(face.FeaturePoints[24]), GetWorldPoint(face.FeaturePoints[20]));


            // Draw all features as red dots
            Gizmos.color = Color.red;
            for(int i = 0; i < face.FeaturePoints.Length; i++)
            {
                Gizmos.DrawSphere(GetWorldPoint(face.FeaturePoints[i]), 0.05f);
            }

        }

        /// <summary>
        /// Set the face to show in editor.
        /// </summary>
        /// <param name="face">Face to show features of</param>
        public void ShowFace(Face face)
        {
            this.face = face;
        }
    }
}
