using System;
using UnityEngine;
using FootballStar.Manager.Model;
using FootballStar.Audio;
using System.Collections;

namespace FootballStar.Manager
{
	public class Intro : MonoBehaviour
	{
#if UNITY_WEBPLAYER
		public GameObject ParentalControl;
		public GameObject FullAgeAdvisor;
#endif

		void Awake()
		{
#if UNITY_WEBPLAYER
			FullAgeAdvisor.SetActive(false);
#endif
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			//mTouchToStartLabel = GameObject.Find("TouchToStart Label").GetComponent<UILabel>();
			
			mHeader = GameObject.Find("Header").GetComponent<Header>();
		}
		
		void Start()
		{
			// Cargamos el componente que controla la reproduccion de los sonidos.
			mAudioInGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
			
			// Ya la hemos mostrado 1 vez, evitamos que a la vuelta de los partidos volvamos a mostrarla
			mMainModel.Player.ShowIntro = false;
			
			//StartCoroutine(BlinkTouchToStart());
		}
		
		void OnYesButtonClick()
		{
			// Nos tenemos que desactivar mientras se hace el fade-out, etc
			if (enabled) {
				mAudioInGameController.PlayDefinition(SoundDefinitions.BUTTON_CONTINUE, false);
						
				if (mMainModel.Player.TutorialStage != TutorialStage.FIRST_MATCH)
					StartCoroutine(GoToMainScreen());
				else
					mMainModel.PlayTutorial();

				enabled = false;
			}
		}

		void OnNoButtonClick()
		{
#if UNITY_WEBPLAYER
			FullAgeAdvisor.SetActive(true);
			ParentalControl.SetActive(false);
#else
			// Nos tenemos que desactivar mientras se hace el fade-out, etc
			Application.Quit ();
#endif
		}

		void OnPoliticaPrivacidadClick()
		{
#if UNITY_WEBPLAYER
			Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouPolitica.html','Política de privacidad y protección de datos personales')");
#else
			Application.OpenURL("http://download.unusualwonder.com/mahou/MahouPolitica.html");
#endif
		}

		void OnCondicionesClick()
		{
#if UNITY_WEBPLAYER
			Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouCondiciones.html','Política de privacidad y protección de datos personales')");
#else
			Application.OpenURL("http://download.unusualwonder.com/mahou/MahouCondiciones.html");
#endif
		}

		void OnBasesLegalesClick()
		{
#if UNITY_WEBPLAYER
			Application.ExternalEval("window.open('http://download.unusualwonder.com/mahou/MahouBasesLegales.html','Política de privacidad y protección de datos personales')");
#else
			Application.OpenURL("http://download.unusualwonder.com/mahou/MahouBasesLegales.html");
#endif
		}

		IEnumerator GoToMainScreen()
		{

			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
			mHeader.GoToMainScreen();
		}

		//TODO: remove. Debug Only.
		IEnumerator GoToSelectionScreen()
		{
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
			mHeader.GoToSelectTeamScreen();
		}
		
		/*IEnumerator BlinkTouchToStart()
		{
			while(true)
			{
				mTouchToStartLabel.alpha = 1;
				yield return new WaitForSeconds(1.5f);
				mTouchToStartLabel.alpha = 0;
				yield return new WaitForSeconds(0.5f);
			}
		}*/
		
		MainModel mMainModel;
		Header mHeader;
		
		//UILabel mTouchToStartLabel;
		AudioInGameController mAudioInGameController;
	}
}

