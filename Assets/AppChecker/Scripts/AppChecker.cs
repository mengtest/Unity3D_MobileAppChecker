// Author: Salvis Poišs (poisins92@gmail.com)
// Feel free to use and modify (and leave some credits :) )!

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Poisins.AppChecker
{
    [AddComponentMenu("Poisins/APP Checker")]
    [DisallowMultipleComponent]
    public class AppChecker : MonoBehaviour
    {
        #region Instance
        public static AppChecker Instance = null;
        //Awake is always called before any Start functions
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

#if UNITY_IOS
            MapApplicationSchemes();
            MapApplicationIDs();
#endif
        }
        #endregion

        // Uses my own MessageWindows created in uGUI. Not included with plugin, but you can get the point about usage...
        // This was just for testing, so I commented them to avoid compiler errors

        //     public void CheckApplicationTest(string appID)
        //     {
        //         string str = IsApplicationInstalled(appID) ? string.Format("Application {0} installed", appID) : string.Format("Application {0} not found", appID);
        //         MessageWindow.ShowWindow(str, MessageWindow.VisibleButtons._1_Button,
        //             new MessageWindow.ButtonProperties("Close", UiManager.Colors.Red, MessageWindow.HideWindow));
        //     }

        //     public void OpenApplicationTest(string appID)
        //     {
        //         tempAppId = appID;
        //         string str = string.Format("Open application {0}?",appID);
        //         MessageWindow.ShowWindow(str, MessageWindow.VisibleButtons._2_Buttons,
        //             new MessageWindow.ButtonProperties("Open!", UiManager.Colors.Blue, TempOpenApp),
        //             new MessageWindow.ButtonProperties("Close", UiManager.Colors.Red, MessageWindow.HideWindow));
        //     }

        //     string tempAppId;
        //     private void TempOpenApp()
        //     {
        //         OpenApplication(tempAppId);
        //MessageWindow.HideWindow();
        //     }

        //     public void OpenInMarketTest(string appID)
        //     {
        //         tempAppId = appID;
        //         string str = string.Format("Open application {0} in Market?", appID);
        //         MessageWindow.ShowWindow(str, MessageWindow.VisibleButtons._2_Buttons,
        //             new MessageWindow.ButtonProperties("Open!", UiManager.Colors.Blue, TempOpenInMarket),
        //             new MessageWindow.ButtonProperties("Close", UiManager.Colors.Red, MessageWindow.HideWindow));
        //     }

        //     private void TempOpenInMarket()
        //     {
        //         OpenInMarket(tempAppId);
        //MessageWindow.HideWindow();
        //     }

        #region IOS Specific
#if UNITY_IOS
        private Dictionary<string, string> applicationSchemesMap;
        public static Dictionary<string, string> ApplicationSchemesMap
        {
            get { return Instance.applicationSchemesMap; }
        }

        // Same as in PostProcessBuild script
        /// <summary>
        /// Map application bundle IDs with iOS URL schemes
        /// </summary>
        public void MapApplicationSchemes()
        {
            applicationSchemesMap = new Dictionary<string, string>();
			applicationSchemesMap.Add("com.poisins.App1", "cp.app1");
			applicationSchemesMap.Add("com.poisins.App2", "cp.app2");
        }

        private Dictionary<string, string> applicationIDMap;
        public static Dictionary<string, string> ApplicationIDMap
        {
            get { return Instance.applicationIDMap; }
        }

        /// <summary>
        /// Map application bundle IDs with iTunes store ID
        /// </summary>
        public void MapApplicationIDs()
        {
            applicationIDMap = new Dictionary<string, string>();
			applicationIDMap.Add("com.poisins.App1", "1234567890"); // 123456789 is application ID from iTunes Connect
			applicationIDMap.Add("com.poisins.App2", "0987654321");
        }
#endif
        #endregion

        /// <summary>
        /// Open application on mobile device, if available
        /// </summary>
        /// <param name="applicationBundleID">Application BundleID (in platforms application store)</param>
        public static void OpenApplication(string applicationBundleID)
        {
            if (IsApplicationInstalled(applicationBundleID))
            {
#if UNITY_ANDROID
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            Debug.Log("Checking for app availability on system.");
            AndroidJavaObject launchIntent = null;
            //if the app is installed, no errors. Else, doesn't get past next line
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", applicationBundleID);
                ca.Call("startActivity", launchIntent);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Error checking app availability: {0}", ex.Message));
            }

            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
#elif UNITY_IOS
                if (ApplicationSchemesMap.ContainsKey(applicationBundleID))
                    Application.OpenURL(string.Format("{0}://", ApplicationSchemesMap[applicationBundleID]));
                else
					Debug.Log(string.Format("Error trying to open unavailable application: {0}", applicationBundleID));
#endif
            }
            else
            {
                Debug.Log(string.Format("Error trying to open unavailable application: {0}", applicationBundleID));
            }
        }

        /// <summary>
        /// Open application in platform's market
        /// </summary>
        /// <param name="applicationBundleID">Application BundleID (in platforms application store)</param>
        public static void OpenInMarket(string applicationBundleID)
        {
#if UNITY_ANDROID
            Application.OpenURL(string.Format("market://details?id={0}", applicationBundleID));
#elif UNITY_IOS
            if(ApplicationIDMap.ContainsKey(applicationBundleID))
                Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/id{0}", ApplicationIDMap[applicationBundleID]));
#endif
        }

        /// <summary>
        /// Checks if application is available on mobile device and performs action based on result
        /// </summary>
        /// <param name="applicationBundleID">Application BundleID (in platforms application store)</param>
        /// <param name="onSuccessAction">Action to perform if application is available</param>
        /// <param name="onFailAction">Action to perform if application is not available</param>
        public static void CheckApplicationAvailability(string applicationBundleID, Action onSuccessAction, Action onFailAction)
        {
            if (IsApplicationInstalled(applicationBundleID))
            {
                if (onSuccessAction != null)
                    onSuccessAction();
            }
            else
            {
                if (onFailAction != null)
                    onFailAction();
            }
        }

        /// <summary>
        /// Checks if application is available on mobile device
        /// </summary>
        /// <param name="applicationBundleID">Application BundleID (in platforms application store)</param>
        public static bool IsApplicationInstalled(string applicationBundleID)
        {
            //#if UNITY_EDITOR
            //            return false;
            //#elif UNITY_ANDROID
#if UNITY_ANDROID
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            Debug.Log("Checking for app availability on system.");
            AndroidJavaObject launchIntent = null;
            //if the app is installed, no errors. Else, doesn't get past next line
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", applicationBundleID);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Error checking app availability: {0}", ex.Message));
            }

            bool result = (launchIntent != null);

            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();

			Debug.Log(string.Format("Application {0} found: {0}", applicationBundleID, result));
            return result;
#elif UNITY_IOS
            bool result = false;
			if (ApplicationSchemesMap.ContainsKey(applicationBundleID)){
				Debug.Log(string.Format("Direct app request iOS: {0}", iOSApplicationAvailable(ApplicationSchemesMap[applicationBundleID])));
				result = iOSApplicationAvailable(ApplicationSchemesMap[applicationBundleID]);
			}
            else
                result = false;

			Debug.Log(string.Format("Application {0} found: {0}", applicationBundleID, result));
            return result;
#else
            return false;
#endif
        }

        #region Native plugin imports
#if UNITY_IOS
        // On iOS and Xbox 360 plugins are statically linked into
        // the executable, so we have to use __Internal as the
        // library name.
		[DllImport("__Internal")]
		private static extern int checkApp(string urlScheme);

		/// <summary>
		/// Checks whether interested application is available on iOS device
		/// </summary>
		/// <param name="interestedApplication">Interested Application's URL scheme</param>
		private static bool iOSApplicationAvailable(string interestedApplication)
		{
			int status = checkApp (interestedApplication);
			return (status == 1);
		}
#endif
        #endregion

    }
}