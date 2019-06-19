using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class HqmUnity : MonoBehaviour {
	Text myText;

      void Start()
      {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                        myText = GameObject.Find("Text1").GetComponent<Text>();

                        myText.text = myText.text + "\nStarting HQM";
						AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

						AndroidJavaClass pluginClass = new AndroidJavaClass("io.humanteq.hqmunity.HqmUnity");

                        myText.text = myText.text + "\nIniting HQM";
						pluginClass.CallStatic("init", context,
							"38e44d7", // your key
							true,      // debug enabled
							true);     // slack messages enabled

                        myText.text = myText.text + "\nCollecting installed apps";
						pluginClass.CallStatic("getInstalledApps", context);

						/*
						Dictionary<string, string> Pairs = new Dictionary<string, string>();
						Pairs["test1"] = "value1";
						Pairs["test2"] = "value2";
						AndroidJavaObject javaMap = CreateJavaMapFromDictainary(Pairs);
						pluginClass.CallStatic("logEvent", "event_name", javaMap);
						*/

						myText.text = myText.text + "\nSending custom event";
						pluginClass.CallStatic("logEvent", "test", "{ 'test1': 'value1', 'test2': 'value2' }");

						String groupIdJson = pluginClass.CallStatic<string>("getGroupIdList");
						myText.text = myText.text + "\ngetGroupIdList:" + groupIdJson;

						String groupNameJson = pluginClass.CallStatic<string>("getGroupNameList");
						myText.text = myText.text + "\ngetGroupNameList:" + groupNameJson;
                }
            }
		}
    }

	public static AndroidJavaObject CreateJavaMapFromDictainary(IDictionary<string, string> parameters)
	{
		AndroidJavaObject javaMap = new AndroidJavaObject("java.util.HashMap");
		IntPtr putMethod = AndroidJNIHelper.GetMethodID(
			javaMap.GetRawClass(), "put",
				"(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

		object[] args = new object[2];
		foreach (KeyValuePair<string, string> kvp in parameters)
		{

			using (AndroidJavaObject k = new AndroidJavaObject(
				"java.lang.String", kvp.Key))
			{
				using (AndroidJavaObject v = new AndroidJavaObject(
					"java.lang.String", kvp.Value))
				{
					args[0] = k;
					args[1] = v;
					AndroidJNI.CallObjectMethod(javaMap.GetRawObject(),
							putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
				}
			}
		}

		return javaMap;
	}
}
