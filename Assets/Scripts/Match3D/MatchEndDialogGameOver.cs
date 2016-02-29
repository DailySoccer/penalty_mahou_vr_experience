using UnityEngine;
using System;
using System.Collections;
using FootballStar.Common;
using ExtensionMethods;
using System.Collections.Generic;
using FootballStar.Audio;
using FootballStar.Manager.Model; 

namespace FootballStar.Match3D {

	public class MatchEndDialogGameOver : MonoBehaviour 
	{
		public GameObject EnergyStuffContainer;
		public UILabel EnergyChangedValueLabel;
				
		public Transform ContinueButtonHandle;
		public Transform ReloadButtonHandle;
		
		void Awake()
		{				
			GameObject gameModel = GameObject.FindGameObjectWithTag("GameModel");

			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchBridge = gameModel.GetComponent<MatchBridge>();
			mAudioInGameController = mMatchBridge.GetComponent<AudioInGameController>();
			mMainModel = gameModel.GetComponent<MainModel>();

			mPanel = GetComponent<UIPanel>();
		}
		
		void Start()
		{
			StartCoroutine(DoTheSequence());
		}
		
		IEnumerator DoTheSequence() 
		{
			// Esperamos N segundos antes de lanzarnos (ponernos visibles, etc)
			yield return new WaitForSeconds(1.5f);
			
			GetComponent<Animation>().Play();
			yield return null;
			mPanel.enabled = true;

			if (mMainModel != null)
			{
				// Si no tenemos energia o estamos en alguna fase del tutorial, no permitimos repetir
				if ( mMainModel.CanIPlayMatches() && mMainModel.Player.TutorialStage == TutorialStage.DONE )
					StartCoroutine(AnimateCoordX(ReloadButtonHandle, ReloadButtonHandle.localPosition.x + 190.0f, 0.150f));
			}
			else
			{
				StartCoroutine(AnimateCoordX(ReloadButtonHandle, ReloadButtonHandle.localPosition.x + 190.0f, 0.150f));
			}
			
			StartCoroutine(AnimateCoordX(ContinueButtonHandle, ContinueButtonHandle.localPosition.x - 190.0f, 0.150f));
		}
	
		public void OnContinueClick()
		{
			StartCoroutine(OnContinueCoroutine());
		}

		public void PlaySoundResult()
		{
			mAudioInGameController.PlayDefinition(SoundDefinitions.MATCH_RESULT, false);
		}
		
		private IEnumerator OnContinueCoroutine()
		{
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));
			
			// Hemos acabado el partido, vamos a volver al Manager en caso de que nos hayan arrancado desde el mismo
			// Esta es la ultima instruccion del partido, internamente se encarga de cargar otra escena
			mMatchBridge.SendMessage("GoBackToManager", SendMessageOptions.DontRequireReceiver);
			mAudioInGameController.StopAllActiveAudios(true);
		}

		public void OnReloadClick()
		{
			StartCoroutine(OnReloadCoroutine());
		}
				
		private IEnumerator OnReloadCoroutine()
		{
			if ( mMainModel != null ) {
				EnergyStuffContainer.SetActive(true);
				EnergyChangedValueLabel.color = Color.green;
				mMainModel.Player.AddEnergy(mMainModel.Player.EnergyCostPerMatch);
				EnergyChangedValueLabel.text = (mMainModel.Player.EnergyCostPerMatch > 0 ? "+" : "") + mMainModel.Player.EnergyCostPerMatch.ToString();
				EnergyChangedValueLabel.GetComponent<Animation>().Play();

				var uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UICamera>();
				uiCamera.enabled = false;
				yield return new WaitForSeconds(0.5f);
				uiCamera.enabled = true;
			}
			
			yield return StartCoroutine( CameraFade.FadeCoroutine(false, 1.0f, 0.0f) );
			
			StopAllCoroutines();
		
			// Mandamos el mensaje al GameModel (se envia a todos los componentes)
			mMatchBridge.SendMessage("OnMatchRepeat", SendMessageOptions.DontRequireReceiver);					
			// El matchmanager recrea todo aqui
			mMatchManager.Repeat();
		}

		IEnumerator AnimateCoordX(Transform who, float targetX, float time)
		{
			float coordXVel = 0.0f;
			
			while (Mathf.Abs(who.localPosition.x - targetX) > 1.0f)
			{
				var newX = Mathf.SmoothDamp(who.localPosition.x, targetX, ref coordXVel, time, Mathf.Infinity, Time.deltaTime);
				who.localPosition = new Vector3(newX, who.localPosition.y, who.localPosition.z);
				yield return null;
			}
		}

		private MatchManager mMatchManager;
		private MatchBridge mMatchBridge;
		private AudioInGameController mAudioInGameController;
		private MainModel mMainModel;
		
		private UIPanel mPanel;
	}
}
