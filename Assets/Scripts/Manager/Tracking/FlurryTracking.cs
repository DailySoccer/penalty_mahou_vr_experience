using UnityEngine;
using System.Collections;

public class FlurryTracking : MonoBehaviour {

	private NerdFlurry mFlurry = null;

	void Awake () {
		Debug.Log( "Flurry INIT" );

		mFlurry = new NerdFlurry();

#if UNITY_ANDROID
		string API_KEY = "QZF358M46MQ5B6GNFYK3";
		// FlurryAgent.Instance.onStartSession(API_KEY);
		mFlurry.StartSession( API_KEY );
#endif

#if (UNITY_IOS || UNITY_IPHONE)
		string API_KEY = "7S76WSR7T3V4J6Z7C3XX";
		// FlurryAgent.Instance.onStartSession(API_KEY);
		mFlurry.StartSession( API_KEY );
#endif
	}

	void Start () {
	}

	void End () {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE)
		// FlurryAgent.Instance.onEndSession();
		
		if ( mFlurry != null ) {
			mFlurry.EndSession();
			mFlurry = null;
		}
#endif
	}

	void OnDestroy()
	{
		Debug.Log( "Flurry DESTROY" );
		End ();
	}

	void OnApplicationQuit()
	{
		Debug.Log( "Flurry QUIT" );
		End ();
	}
}
