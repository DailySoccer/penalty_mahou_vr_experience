using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Common
{	
	public class MatchLoader : MonoBehaviour
	{
		public GameObject CurrentStadium { get; set; }
		
		void Start()
		{
		}
		
		private IEnumerator StreamStadium(string unitySceneName)
		{
			mAsyncSceneName = unitySceneName;
			
			var prevPriority = Application.backgroundLoadingPriority;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			
			Debug.Log("StreamStadium: Level[" + mAsyncSceneName + "] loading level");
			
		    mAsync = Application.LoadLevelAdditiveAsync(mAsyncSceneName);
		    yield return mAsync;
			
			CurrentStadium = GameObject.FindGameObjectWithTag("Stadium");
			DontDestroyOnLoad(CurrentStadium);
			
			mStadiumLightmaps = LightmapSettings.lightmaps;
			
			Application.backgroundLoadingPriority = prevPriority;
			
			Debug.Log("StreamStadium: Level[" + mAsyncSceneName + "] loading complete");
		}

		public void LoadMatch()
		{	
			StartCoroutine(LoadMatchCoroutine());
		}
		
		public IEnumerator LoadMatchCoroutine()
		{
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 0.150f, 0.0f));
			
			// Las pantallas dejan el estadio desactivado, lo reactivamos
			CurrentStadium.SetActive(true);
						
			// Destruimos la MainScene para que no se quede funcionando por detras mientras cargamos.
			// El estadio se queda, pero no nos importa.
			var mainScene = GameObject.Find("MainScene");
			
			if (mainScene != null)
				Destroy(mainScene);
			
			// Subimos a prioridad maxima para cargar lo mas rapido posible. Lo unico que queda por detras
			// sera la musica, el loading y el estadio
			var prevPriority = Application.backgroundLoadingPriority;
			Application.backgroundLoadingPriority = ThreadPriority.High;
			
			mAsync = Application.LoadLevelAsync ("Match");
			yield return mAsync;
			
			// Entramos en el partido => Ponemos los lightmaps del estadio
			LightmapSettings.lightmaps = mStadiumLightmaps;
			
			// Dejamos la prioridad como estuviera
			Application.backgroundLoadingPriority = prevPriority;
		}
		
		public void LoadManager(int tierIdx)
		{
			Application.LoadLevel("FootballStar");
			
			// No funciona si hacemos el cambio de lightmap en el mismo frame despues de haber cargado, hay
			// que esperar al menos 1 frame
			StartCoroutine(SetLightmapCoroutine());
		}
		
		private IEnumerator SetLightmapCoroutine()
		{
			yield return null;
			LightmapSettings.lightmaps = mStadiumLightmaps;
		}
		
		public void LoadStadium(int tierIdx)
		{
			StartCoroutine(StreamStadium("BernabeuStadium"));
		}
		
		private AsyncOperation mAsync;
		private string mAsyncSceneName;
		
		LightmapData[] mStadiumLightmaps;
	}
}

