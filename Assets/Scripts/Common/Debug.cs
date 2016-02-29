/*

using UnityEngine;
using System.Collections;

//
// http://forum.unity3d.com/threads/38720-Debug-Log-and-needless-spam/page2
//
public class Debug
{


    public static bool isDebugBuild
	{		
		get { return UnityEngine.Debug.isDebugBuild; }
	}

	public static void Log (object message)		
	{   
		if (isDebugBuild) {
			UnityEngine.Debug.Log (message);

#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif
		}
	}
	
	public static void Log (object message, UnityEngine.Object context)
	{
		if (isDebugBuild) {
			UnityEngine.Debug.Log (message, context);

#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif	
		}
	}
	
	public static void LogError (object message)		
	{   
		if (isDebugBuild) {
			UnityEngine.Debug.LogError (message);

#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif
		}
	}

	public static void LogError (object message, UnityEngine.Object context)
	{   
		if (isDebugBuild) {
			UnityEngine.Debug.LogError (message, context);

#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif
		}
	}

	public static void LogWarning (object message)
	{   
		if (isDebugBuild) {
			UnityEngine.Debug.LogWarning (message.ToString ());

#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif
		}
	}
	public static void LogWarning (object message, UnityEngine.Object context)
	{   
		if (isDebugBuild) {
			UnityEngine.Debug.LogWarning (message.ToString (), context);
#if UNITY_WEBPLAYER
			Application.ExternalEval ("console.log(': " + message + "')");
#endif
		}
	}

	public static void DrawLine(Vector3 a, Vector3 b, Color color, float duration=0.0f )
	{
		if (isDebugBuild)
			UnityEngine.Debug.DrawLine(a, b, color, duration);
	}

    public static void Break()
    {
        if (isDebugBuild)
            UnityEngine.Debug.Break();
    }

}
*/