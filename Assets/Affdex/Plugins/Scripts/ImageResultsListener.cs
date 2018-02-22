using System.Collections.Generic;
using UnityEngine;

namespace Affdex
{
    /// <summary>
    /// Class that contains callback methods for asset results
    /// </summary>
    public abstract class ImageResultsListener : MonoBehaviour
    {
        /// <summary>
        /// Indicates image results are available.
        /// </summary>
        /// <param name="faces">The faces.</param>
        public abstract void onImageResults(Dictionary<int, Face> faces);

        /// <summary>
        /// Indicates that the face detector has started tracking a new face.
        /// <para>
        /// When the face tracker detects a face for the first time method is called.
        /// The receiver should expect that tracking continues until detection has stopped.
        /// </para>
        /// </summary>
        /// <param name="timestamp">Frame timestamp when new face was first observed.</param>
        /// <param name="faceId">Face identified.</param>
        public abstract void onFaceFound(float timestamp, int faceId);

        /// <summary>
        /// Indicates that the face detector has stopped tracking a face.
        /// <para>
        /// When the face tracker no longer finds a face this method is called. The receiver should expect that there is no face tracking until the detector is
        /// started.
        /// </para>
        /// </summary>
        /// <param name="timestamp">Frame timestamp when previously observed face is no longer present.</param>
        /// <param name="faceId">Face identified.</param>
        public abstract void onFaceLost(float timestamp, int faceId);
    }
}
