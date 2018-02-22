// This stores common code
using UnityEngine;
using System;

namespace Affdex
{
    /// <summary>
    /// Class of utilities implemented as static methods.
    /// </summary>
    public static class AffdexUnityUtils
    {
        /// <summary>
        /// Checks to see if Application.platform is supported by Affectiva's plugin
        /// </summary>
        /// <returns>True or False</returns>
        public static bool ValidPlatform ()
        {
            if (RuntimePlatform.WindowsEditor == Application.platform || RuntimePlatform.WindowsPlayer == Application.platform ||
                RuntimePlatform.OSXEditor == Application.platform || RuntimePlatform.OSXPlayer == Application.platform ||
                // Not ready to move XBox code to Master
                //RuntimePlatform.XboxOne == Application.platform || 
                RuntimePlatform.IPhonePlayer == Application.platform ||
                RuntimePlatform.Android == Application.platform)
            {
                return true;
            }
            Debug.Log (Application.platform + " is not currently supported by Affectiva's Unity Asset!");
            return false;
        }
    }
}

