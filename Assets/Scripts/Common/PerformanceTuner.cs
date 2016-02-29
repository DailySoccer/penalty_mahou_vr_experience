using UnityEngine;
using System.Collections;

namespace FootballStar.Common
{
	public class PerformanceTuner : MonoBehaviour
	{
		public Material[] MarmosetMaterials;
		
		public bool FastBloomEnabled = false;
		public bool ColorCorrectionEnabled = false;
		
		public int OwnQualityLevel { get { return mOwnQualityLevel; }	}
		
		void Awake()
		{			
			mOwnQualityLevel = 3;
			
			// Determinamos si iOS o Android en tiempo de compilacion
			#if UNITY_IPHONE
			if (!Application.isEditor)
				CalculateQualityLevelIOS();
			#endif

			#if UNITY_ANDROID
			if (!Application.isEditor)
				CalculateQualityLevelAndroid();
			#endif
			
			// En el editor (todas las plataformas), siempre al maximo
			if (Application.isEditor)
				mOwnQualityLevel = 5;
				
			ApplyQualityLevel();
		}
		
		void ApplyQualityLevel()
		{
			if (mOwnQualityLevel == 0) {
				QualitySettings.SetQualityLevel(0);
				DowngradeMarmosetMaterials();
			}
			else if (mOwnQualityLevel == 1 || mOwnQualityLevel == 2) {
				QualitySettings.SetQualityLevel(1);			
				DowngradeMarmosetMaterials();
			}
			else if (mOwnQualityLevel == 3 || mOwnQualityLevel == 4) {
				QualitySettings.SetQualityLevel(3);
			}
			else {
				QualitySettings.SetQualityLevel(3);	// TODO: Cuando tengamos un 5s/Nexus 5, probar el 4 con AA
			}

			ApplyFogQualityLevel();
			AppyPostProcessQualityLevel();
			
			if (Debug.isDebugBuild)
				Debug.Log("ApplyQualityLevel: " + mOwnQualityLevel);
		}

		void AppyPostProcessQualityLevel() {
			if (mOwnQualityLevel >= 4)
				EnablePostProcess();
		}

		void ApplyFogQualityLevel() {
			if (mOwnQualityLevel >= 3)
				RenderSettings.fog = true;
		}
		
		void DowngradeMarmosetMaterials()
		{
			// Tocamos los materiales directamente en el disco, pero solo es un problema si estamos en el editor
			foreach(Material mat in MarmosetMaterials)
			{
				mat.shader = Shader.Find("Mobile/Diffuse");
			}
			
			RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.6f);
		}
		
		void OnLevelWasLoaded(int levelIdx)
		{
			// Si es el FootballStar.unity o el partido, reaplicamos los settings de quality
			if (levelIdx == 0 || levelIdx == 1)
				ApplyQualityLevel();
		}

		#if UNITY_ANDROID
		void CalculateQualityLevelAndroid()
		{
			Debug.Log (string.Format("CalculateQualityLevelAndroid: {0} || {1} || {2} || {3} || {4} || {5} || {6} || {7}", 
			                         SystemInfo.deviceName, SystemInfo.deviceModel,
		    	                     SystemInfo.processorType, SystemInfo.processorCount, 
			                         SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVendor, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsPixelFillrate));

			// Nexus 5 & Galaxy S4 & HTC ONE
			if (SystemInfo.deviceModel.Contains("Nexus 5") || SystemInfo.deviceModel.Contains("GT-I9505") || SystemInfo.deviceModel.Contains("HTC One"))
			{
				mOwnQualityLevel = 4;
			}
			else
			{
				mOwnQualityLevel = 3;
			}
		}
		#endif
	
		#if UNITY_IPHONE
		void CalculateQualityLevelIOS()
		{
			// http://docs.unity3d.com/Documentation/ScriptReference/iPhoneGeneration.html
			var iOSGen = iPhone.generation;
						
			// Good is 3, Fastest 0
			if (iOSGen < iPhoneGeneration.iPhone4) 			// TODO: Prohibir si < iPhone4
			{
	        	mOwnQualityLevel = 0;
			}
	    	else if (iOSGen < iPhoneGeneration.iPhone4S)	// iPhone4, iPodTouch4Gen, iPad2
			{
	        	mOwnQualityLevel = 1;
			}
	    	else if (iOSGen < iPhoneGeneration.iPhone5)		// iPhone4S, iPad3Gen
			{
				mOwnQualityLevel = 3;
			}
			else if (iOSGen <= iPhoneGeneration.iPhone5C)	// iPhone5, iPodTouch5Gen, iPadMini1Gen, iPad4Gen
			{
				mOwnQualityLevel = 4;
			}
			else 
			{
				mOwnQualityLevel = 5;
			}
					
			if (Debug.isDebugBuild)
				Debug.Log("CalculateQualityLevelIOS: " + mOwnQualityLevel + " seleccionado para iOSGen " + iOSGen.ToString());
		}
		#endif

		public void SetCutsceneQuality(bool cutscenePlaying) {
			if (cutscenePlaying) {
				RenderSettings.fog = false;
			}
			else {
				ApplyFogQualityLevel();
			}
		}
		
		void EnablePostProcess()
		{
			GameObject mainCamera = GameObject.Find("Main Camera");
			
			if (ColorCorrectionEnabled)
			{
				var ccCurves = (mainCamera.GetComponent("ColorCorrectionCurves") as MonoBehaviour);
				
				if (ccCurves != null)
					ccCurves.enabled = true;
			}
			
			if (FastBloomEnabled)
			{
				var fastBloom = (mainCamera.GetComponent("FastBloom") as MonoBehaviour);
				
				if (fastBloom != null)
					fastBloom.enabled = true;
			}
		}
		
		PerformanceTunerStadium mPerformanceTunerStadium;
		int mOwnQualityLevel = 3;
	}
}