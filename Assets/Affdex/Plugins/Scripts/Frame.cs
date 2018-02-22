using UnityEngine;

namespace Affdex
{

    /// <summary>
    /// A wrapper struct for images and their associated timestamps.
    /// </summary>
    public struct Frame
    {
        /// <summary>
        /// Representation of RGBA colors in 32 bit format.
        /// <para>
        /// Each color component is a byte value with a range from 0 to 255.
        /// </para><para>
        /// Components(r, g, b) define a color in RGB color space. Alpha component(a) defines transparency - alpha of 255 is completely opaque, alpha of zero is completely transparent.
        /// </para>
        /// </summary>
        public Color32[] rgba;

        /// <summary>
        /// The timestamp of the frame (in seconds). Can be used as an identifier of the frame.  If you use Time.timeScale to pause and use the same time units then you will not be able to process frames while paused.
        /// </summary>
        public float timestamp;

        /// <summary>
        /// Width of the frame. Value has to be greater than zero
        /// </summary>
        public int w;

        /// <summary>
        /// Height of the frame. Value has to be greater than zero
        /// </summary>
        public int h;

        /// <summary>
        /// Orientation of the frame
        /// Note : enum values match http://sylvana.net/jpegcrop/exif_orientation.html
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            /// Image's 0th row is at the top and 0th column is on the left side.
            /// </summary>
            Upright = 1,
            
            /// <summary>
            /// Image's 0th row is on the left side and 0th column is at the bottom.
            /// </summary>
            CW_90 = 8,

            /// <summary>
            /// Image's 0th row is at the bottom and 0th column is on the right side.
            /// </summary>
            CW_180 = 3,

            /// <summary>
            /// Image's 0th row is on the right side and 0th column is at the top.
            /// </summary>
            CW_270 = 6
        }

        /// <summary>
        /// Orientation of the frame
        /// </summary>
        public Orientation orientation;

        /// <summary>
        /// Representation of RGBA colors in 32 bit format.
        /// <para>
        /// Each color component is a byte value with a range from 0 to 255.
        /// </para><para>
        /// Components(r, g, b) define a color in RGB color space. Alpha component(a) defines transparency - alpha of 255 is completely opaque, alpha of zero is completely transparent.
        /// </para>
        /// </summary>
        /// <param name="rgba">Representation of RGBA colors in 32 bit format.</param>
        /// <param name="width">Width of the frame. Value has to be greater than zero</param>
		/// <param name="height">Height of the frame. Value has to be greater than zero</param>
        /// <param name="orientation">Orientation of the frame.</param>
        /// <param name="timestamp">The timestamp of the frame (in seconds). Can be used as an identifier of the frame.  If you use Time.timeScale to pause and use the same time units then you will not be able to process frames while paused.</param>
        public Frame(Color32[] rgba, int width, int height, Orientation orientation, float timestamp)
        {
            this.w = width;
            this.h = height;
            this.rgba = rgba;
            this.orientation = orientation;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Representation of RGBA colors in 32 bit format.  The orientation of the image must be upright.  For a rotated image, use the alternate constructor which allows specification of the frame orientation.
        /// <para>
        /// Each color component is a byte value with a range from 0 to 255.
        /// </para><para>
        /// Components(r, g, b) define a color in RGB color space. Alpha component(a) defines transparency - alpha of 255 is completely opaque, alpha of zero is completely transparent.
        /// </para>
        /// </summary>
        /// <param name="rgba">Representation of RGBA colors in 32 bit format.</param>
        /// <param name="width">Width of the frame. Value has to be greater than zero</param>
        /// <param name="height">Height of the frame. Value has to be greater than zero</param>
        /// <param name="timestamp">The timestamp of the frame (in seconds). Can be used as an identifier of the frame.  If you use Time.timeScale to pause and use the same time units then you will not be able to process frames while paused.</param>
        public Frame(Color32[] rgba, int width, int height, float timestamp) : this(rgba, width, height, Frame.Orientation.Upright, timestamp)
        {
        }
    }
}
