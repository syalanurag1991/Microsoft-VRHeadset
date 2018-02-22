using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

namespace Affdex
{
    internal class AndroidNativePlatform : NativePlatform
    {
        // declarations for listeners
        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            delegate void ImageResults (IntPtr i);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            delegate void FaceResults (Int32 i);

        [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
            delegate void LogCallback (IntPtr l);

        // declarations for the exported C functions in the C/C++ SDK for Affdex
        [DllImport ("affdex-native")]
        private static extern int affdexRegisterListeners (IntPtr handle,
                                                     [MarshalAs (UnmanagedType.FunctionPtr)] ImageResults imageCallback,
                                                     [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults foundCallback, 
                                                     [MarshalAs (UnmanagedType.FunctionPtr)] FaceResults lostCallback);

        [DllImport ("affdex-native")]
        private static extern int affdexProcessFrame (IntPtr handle, IntPtr rgba, Int32 w, Int32 h, Int32 orientation, float timestamp);

        [DllImport ("affdex-native")]
        private static extern int affdexStart (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern void affdexRelease (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern int affdexStop (IntPtr handle);

        [DllImport ("affdex-native")]
        private static extern void affdexSetExpressionState (IntPtr handle, int expression, int state);

        [DllImport ("affdex-native")]
        private static extern void affdexSetEmotionState (IntPtr handle, int emotion, int state);

        [DllImport ("affdex-native")]
        private static extern IntPtr affdexInitialize (int discrete, string affdexDataPath);

        private const string dataVersionPrefKey = "affdexDataVersion";
        private IntPtr nativeHandle;
        private string affdexDataDir;
        
        public override IEnumerator Initialize (Detector detector, int discrete)
        {
            AndroidNativePlatform.detector = detector;

            // assemble the destination path where the data files will go
            affdexDataDir = Path.Combine(Application.persistentDataPath, "affdex-data");

            // Copy data files out of StreamingAssets to the destination path (if necessary)
            // Wait for it to complete before continuing
            yield return StartCoroutine(CopyAssets());

            // native call
            nativeHandle = affdexInitialize(discrete, affdexDataDir);

            // allocate and pin memory for the listeners
            FaceResults faceFound = new FaceResults(this.onFaceFound);
            FaceResults faceLost = new FaceResults(this.onFaceLost);
            ImageResults imageResults = new ImageResults(this.onImageResults);

            h1 = GCHandle.Alloc(faceFound, GCHandleType.Pinned);
            h2 = GCHandle.Alloc(faceLost, GCHandleType.Pinned);
            h3 = GCHandle.Alloc(imageResults, GCHandleType.Pinned);

            // native call
            affdexRegisterListeners(nativeHandle, imageResults, faceFound, faceLost);
        }

        public override void ProcessFrame (byte[] rgba, int w, int h, Frame.Orientation orientation, float timestamp)
        {
            try
            {
                IntPtr addr = Marshal.AllocHGlobal (rgba.Length);
                Marshal.Copy (rgba, 0, addr, rgba.Length);
                // native call
                affdexProcessFrame(nativeHandle, addr, w, h, (int)orientation, Time.realtimeSinceStartup);
                Marshal.FreeHGlobal (addr);
            } catch (Exception e) {
                Debug.LogError (e.Message + " " + e.StackTrace);
            }
        }

        public override void SetExpressionState (int expression, bool state)
        {
            int intState = (state) ? 1 : 0;
            // native call
            affdexSetExpressionState(nativeHandle, expression, intState);
        }

        public override void SetEmotionState (int emotion, bool state)
        {
            int intState = (state) ? 1 : 0;
            // native call
            affdexSetEmotionState(nativeHandle, emotion, intState);
        }

        public override int StartDetector ()
        {
            // native call
            return affdexStart(nativeHandle);
        }

        public override void StopDetector ()
        {
            // native call
            affdexStop(nativeHandle);
        }

        public override void Release ()
        {
            // native call
            affdexRelease(nativeHandle);

            // release memory for the listeners
            h1.Free();
            h2.Free();
            h3.Free();
        }

        // Copy assets out of StreamingAssets to the app's private data folder.
        private IEnumerator CopyAssets()
        {
            string srcDir = Path.Combine(Application.streamingAssetsPath, "affdex-data-3");

            // load the file listing
            WWW fileList = new WWW(Path.Combine(srcDir, "fileList.txt"));

            // wait for the file to be read in
            yield return fileList;

            // read the file line-by-line.  The first line is a version identifier;
            // the remaining lines are relative paths of files to be copied.
            using (StringReader sr = new StringReader(fileList.text))
            {
                // read the version identifier
                string dataVersion;
                if ((dataVersion = sr.ReadLine()) != null)
                {
                    // fetch the version of the data files we've previously copied (if any)
                    string previousDataVersion = PlayerPrefs.GetString(dataVersionPrefKey);

                    // if different, copy the files
                    if (previousDataVersion != dataVersion)
                    {
                        // first delete the previously copied ones, if any
                        if (previousDataVersion != "")
                        {
                            Directory.Delete(affdexDataDir, true);
                        }

                        // create the destination dir
                        if (!Directory.Exists(affdexDataDir))
                        {
                            Directory.CreateDirectory(affdexDataDir);
                        }

                        // copy the files one at a time, yielding with each copy
                        string fileToCopy;
                        while ((fileToCopy = sr.ReadLine()) != null)
                        {
                            yield return StartCoroutine(CopyAssetFile(fileToCopy, srcDir, affdexDataDir));
                        }

                        // remember the version we just copied
                        PlayerPrefs.SetString(dataVersionPrefKey, dataVersion);
                    }
                }
            }
        }

        private IEnumerator CopyAssetFile(String relPath, String srcRoot, String destRoot)
        {
            // load the file to be copied
            WWW file = new WWW(Path.Combine(srcRoot, relPath));

            // wait for file to be read in
            yield return file;

            // create the dest dir for this file, if necessary
            string destPath = Path.Combine(destRoot, relPath);
            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // write out the file
            File.WriteAllBytes(destPath, file.bytes);
        }
    }
}
