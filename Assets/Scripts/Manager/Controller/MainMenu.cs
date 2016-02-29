using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Manager.Model;
using FootballStar.Audio;

namespace FootballStar.Manager
{
	public class MainMenu : MonoBehaviour
	{
		public GameObject TapHereToPlay;
		public GameObject TapHereToTrain;
		public GameObject CompetitionExplanationPrefab;
		public GameObject YouNeedToImprovePrefab;
		public GameObject GenetsisPrefab;
		public GameObject GenetsisButton;


		void Awake()
		{
			mScreenStackController = GameObject.Find("Header").GetComponent<ScreenStack>();
			
			mGameModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mAudioController = mGameModel.GetComponent<AudioInGameController>();
			
			mYouNeedToImproveAlreadyShown = false;
#if UNITY_WEBPLAYER
			GenetsisButton.SetActive(false);
#endif
		}
		
		void OnEnable()
		{
			if (mGameModel.Player.TutorialStage == TutorialStage.DONE)
			{
				TapHereToPlay.SetActive(false);
				TapHereToTrain.SetActive(false);
			}
			else
			if (mGameModel.Player.TutorialStage == TutorialStage.FRIENDLY_EXPLANATION)
			{
				TapHereToTrain.SetActive(false);
			}
			else
			if (mGameModel.Player.TutorialStage == TutorialStage.CUP_EXPLANATION)
			{
				TapHereToTrain.SetActive(false);
				mMessageOverlap = NGUITools.AddChild(this.gameObject, CompetitionExplanationPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
			}
			else
			if (mGameModel.Player.TutorialStage == TutorialStage.YOU_NEED_TO_IMPROVE)
			{
				if (!mYouNeedToImproveAlreadyShown)
				{
					TapHereToPlay.SetActive(false);
					mMessageOverlap = NGUITools.AddChild(this.gameObject, YouNeedToImprovePrefab);
					mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
					
					mYouNeedToImproveAlreadyShown = true;
				}
				else
				{
					TapHereToPlay.SetActive(false);
					TapHereToTrain.SetActive(false);
				}
			}
		}
		
		void Start()
		{
			mAudioController.PlayDefinition(SoundDefinitions.THEME_MAIN, true);
		}

		void OnContinueCompetitionExplanationClick() {
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "COMPETICION"} });
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnContinueYouNeedToImproveClick() {
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "MEJORA"} });
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnTrainingClick()
		{
			mScreenStackController.PushScreenController("TrainingScreen");
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Training Screen"} });
		}
		
		void OnLifeClick()
		{
			mScreenStackController.PushScreenController("LifeScreen");
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Life Screen"} });
		}
		
		void OnPlayClick()
		{
			mScreenStackController.PushScreenController("PlayScreen");
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Play Screen"} });
		}

		void OnSponsorsClick()
		{
			mScreenStackController.PushScreenController("SponsorsScreen");
			//MixPanel.SendEventToMixPanel(AnalyticEvent.SCREEN_VIEW, new Dictionary<string, object>{ {"Screen Name", "Sponsors Screen"} });
		}

		/***** Genetsis Controller methods *****/
		public void OnCloseGenetsisControllerClick(object sender, EventArgs e) {
			mMessageOverlap.GetComponentInChildren<GenetsisMahouController>().OnGenetsisClose -= OnCloseGenetsisControllerClick;
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnRedeemPinCodeClick()
		{
			// Creamos nuestro mensajito explicativo
			mMessageOverlap = NGUITools.AddChild(this.gameObject.transform.parent.gameObject, GenetsisPrefab);
			mMessageOverlap.GetComponentInChildren<GenetsisMahouController>().OnGenetsisClose += OnCloseGenetsisControllerClick;
		}
		/***** *****/
			
		MainModel mGameModel;
		GameObject mMessageOverlap;

		ScreenStack mScreenStackController;
		AudioInGameController mAudioController;
		
		bool mYouNeedToImproveAlreadyShown;
	}

}
