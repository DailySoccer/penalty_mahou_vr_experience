using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class EmmaTracking : MonoBehaviour {

#if (UNITY_IOS || UNITY_IPHONE)
	[DllImport("__Internal", CharSet = CharSet.Ansi)]
	private static extern void eMMaUnity_startSession([In, MarshalAs(UnmanagedType.LPStr)]string apiKey);
#endif

	void Awake () {
		const string API_KEY = "mahouZYyW9kxPp";

		Debug.Log( "eMMa INIT" );

#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android ) {

			AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			using (AndroidJavaClass emma = new AndroidJavaClass("com.emma.android.eMMa")) {
				emma.CallStatic( "starteMMaSession", activityContext, API_KEY );

				string emmaVersion = emma.CallStatic<string>( "getSDKVersion" );
				Debug.Log( "eMMa Version: " + emmaVersion );
			}

			/*
			using (AndroidJavaClass ad4trackTESTConstant = new AndroidJavaClass("com.ad4screen.sdk.Constants")) {
				
				string androidId = ad4trackTESTConstant.CallStatic<string>( "SDK_VERSION" );
				Debug.Log( "a4sTrackingConstant Version: " + androidId );
		
			}
			using (AndroidJavaClass ad4track = new AndroidJavaClass("com.ad4screen.sdk.A4S")) {

				string androidId = ad4track.CallStatic<string>( "getAndroidId" );
				Debug.Log( "a4sTracking Version: " + androidId );

				AndroidJavaClass ad4object = ad4track.Call<AndroidJavaClass>( "get", activityContext);
				ad4object.Call("startActivity", activityContext);
			}
			*/


		}
#endif

#if (UNITY_IOS || UNITY_IPHONE)
		if ( Application.platform == RuntimePlatform.IPhonePlayer ) {
			eMMaUnity_startSession(API_KEY);
		}
#endif

		Debug.Log( "eMMa END" );
	}

	void onDestroy()
	{

		/*
		#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android ) {
			
			AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
						
			using (AndroidJavaClass ad4track = new AndroidJavaClass("com.ad4screen.sdk.A4S")) {
				AndroidJavaClass ad4object = ad4track.Call<AndroidJavaClass>( "get", activityContext);
				ad4object.Call("stopActivity", activityContext);				
			}
		}
		#endif
		*/
	}

	void Start () {
	}
}