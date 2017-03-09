// Author: Salvis Poišs (poisins92@gmail.com)
// Feel free to use and modify (and leave some credits :) )!

// Map project bundle identifiers with your custom iOS URL scheme.
// iOS app needs it's own URL scheme (CFBundleURLSchemes) which is used to call this app from another one
// and query schemes (LSApplicationQueriesSchemes) - URL scheme whitelist required by iOS 9+ to know which apps
// you can call. iOS 8 and lower will just ignore this whitelist.

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections.Generic;

namespace Poisins.AppChecker
{
    public class PostProcessBuild
    {
        [PostProcessBuild]
        public static void SetApplicationQueriesSchemes(BuildTarget buildTarget, string pathToBuiltProject)
        {
            // This is needed only for iOS
            if (buildTarget == BuildTarget.iOS)
            {
                // Map application bundle IDs with URL schemes
                Dictionary<string, string> applicationBundlesMap = new Dictionary<string, string>();
                applicationBundlesMap.Add("com.poisins.App1", "cp.app1");
                applicationBundlesMap.Add("com.poisins.App2", "cp.app2");

                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;

                // Set URL schemes app is able to respond
                PlistElementArray bundleUrlTypes = rootDict.CreateArray("CFBundleURLTypes"); // create dictionary of Types
                PlistElementDict urlTypesDict = bundleUrlTypes.AddDict();
                PlistElementArray bundleUrlSchemes = urlTypesDict.CreateArray("CFBundleURLSchemes"); // create URL schemes in Types dictionary

                // Set URL schemes app is allowed to query
                PlistElementArray appQuerySchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");

                // Process all schemes
                foreach(KeyValuePair<string,string> pair in applicationBundlesMap)
                {
                    // if current project, then add to CFBundleURLSchemes array
                    if (pair.Key == Application.bundleIdentifier)
                        bundleUrlSchemes.AddString(pair.Value);
                    //else LSApplicationQueriesSchemes array
                    else
                        appQuerySchemes.AddString(pair.Value);
                }

                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}