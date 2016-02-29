using UnityEngine;
using System;
using System.Collections;
using FootballStar.Common;
using ExtensionMethods;
using System.Collections.Generic;
using FootballStar.Audio;
using FootballStar.Manager.Model; 

namespace FootballStar.Match3D {

	public class MatchEndDialog : MonoBehaviour 
	{
		public UISprite BadgeRightSprite;
		public UISprite BadgeLeftSprite;
		
		public UILabel LeftResultLabel;
		public UILabel RightResultLabel;
		
		public UILabel RewardValueLabel;
		public UILabel BonusValueLabel;
		public UILabel TotalValueLabel;
		
		public UILabel VictoryLabel;

		public UIImageButton ShareInFacebookButton;
		
		public GameObject EnergyStuffContainer;
		public UILabel EnergyChangedValueLabel;
		
		public List<GameObject> Balls = new List<GameObject>();
		public List<UILabel> FansLabels = new List<UILabel>();
		public List<AnimatedColor> FansAnimatedColors = new List<AnimatedColor>();
		
		public Transform ContinueButtonHandle;
		public Transform ReloadButtonHandle;
		
		void Awake()
		{				
			GameObject gameModel = GameObject.FindGameObjectWithTag("GameModel");
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchBridge = gameModel.GetComponent<MatchBridge>();
			mAudioInGameController = mMatchBridge.GetComponent<AudioInGameController>();
			mMainModel = gameModel.GetComponent<MainModel>();
									
			for (int c = 0; c < 3; ++c)
			{
				Balls[c].SetActive(false);
				FansLabels[c].gameObject.SetActive(false);	
			}
			
			mPanel = GetComponent<UIPanel>();
		}
		
		void Start()
		{

			var matchResult = mMatchBridge.ReturnMatchResult;
			
			if (matchResult.PlayerLost)
				VictoryLabel.text = "DERROTA";
			else if (matchResult.PlayerTied)
				VictoryLabel.text = "EMPATE";
			
			BadgeRightSprite.spriteName = mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small";
			BadgeLeftSprite.spriteName 	= mMatchBridge.CurrentMatchDefinition.MyBadgeName + "_Small";
			
			LeftResultLabel.text = matchResult.PlayerGoals.ToString();
			RightResultLabel.text = matchResult.OppGoals.ToString();
		
			if (mMatchBridge.IsResultFirstTimeWon) {
				RewardValueLabel.text = mMatchBridge.CurrentMatchDefinition.Reward.FormatAsMoney();
				BonusValueLabel.text = mMatchBridge.SponsorshipBonus.FormatAsMoney();
				TotalValueLabel.text = mMatchBridge.TotalMoneyEarned.FormatAsMoney();
			}
			else if (mMatchBridge.ReturnMatchResult.PlayerWon) {
				RewardValueLabel.text = "0";
				BonusValueLabel.text = mMatchBridge.SponsorshipBonusRepeatMatch.FormatAsMoney();
				TotalValueLabel.text = BonusValueLabel.text;
			}
			else {
				RewardValueLabel.text = "0";
				BonusValueLabel.text = "0";
				TotalValueLabel.text = "0";
			}

			StartCoroutine(DoTheSequence());
		}
		
		IEnumerator DoTheSequence() 
		{
			// Esperamos N segundos antes de lanzarnos (ponernos visibles, etc)
			yield return new WaitForSeconds(1.5f);
			
			GetComponent<Animation>().Play();
			yield return null;
			mPanel.enabled = true;
			
			// N segundos mas hasta q hacemos la animacion de los balones / fans
			yield return new WaitForSeconds(1.5f);
			
			if (mMainModel != null)
			{
				// Si no tenemos energia o estamos en alguna fase del tutorial, no permitimos repetir
				if ( mMainModel.CanIPlayMatches() && mMainModel.Player.TutorialStage == TutorialStage.DONE )
				{
					StartCoroutine(AnimateCoordX(ReloadButtonHandle, ReloadButtonHandle.localPosition.x + 190.0f, 0.150f));
				}
			}
			else
			{
				StartCoroutine(AnimateCoordX(ReloadButtonHandle, ReloadButtonHandle.localPosition.x + 190.0f, 0.150f));
			}
			
			StartCoroutine(AnimateCoordX(ContinueButtonHandle, ContinueButtonHandle.localPosition.x - 190.0f, 0.150f));

			ShareInFacebookButton.gameObject.SetActive (mMatchBridge.ReturnMatchResult.PlayerWon);
			// Cogemos el resultado directamente del MatchManager pq queremos mostrar bien la precision incluso cuando no 
			// nos cargan desde el manager
			int nuBalls = mMatchManager.MatchResult.NumPrecisionBallsEndOfMatch;
			int numPrevBalls = 0;
			int sumFans = 0;
			
			// Bolas previas solo si nos cargan desde el manager y el partido ya tenia resultado
			if (mMatchBridge != null && mMatchBridge.CurrentMatchPrevResult != null)
				numPrevBalls = mMatchBridge.CurrentMatchPrevResult.NumPrecisionBallsEndOfMatch;

			for (int c = 0; c < nuBalls; ++c) {
				// The ball
				Balls[c].SetActive(true);
				yield return StartCoroutine(AnimateBallScale(Balls[c].transform));
				PlaySoundBall(c);
				
				// No damos fans si no mejoramos
				if (c > numPrevBalls - 1)
					sumFans += MatchResult.FANS_PER_PRECISION_BALL;
				
				FansLabels[c].gameObject.SetActive(true);
				FansLabels[c].text = "+" + sumFans.ToString();
				yield return StartCoroutine(AnimateCoordX(FansLabels[c].transform, 125.0f, 0.125f));
				
				if (c != nuBalls - 1)
					StartCoroutine(DismissFansLabel(FansLabels[c].transform, FansAnimatedColors[c]));
			}
			
			// Perder o empatar => 0 fans
			if (nuBalls == 0) {
				FansLabels[2].gameObject.SetActive(true);
				FansLabels[2].text = "+0";
				
				yield return StartCoroutine(AnimateCoordX(FansLabels[2].transform, 125.0f, 0.125f));
			}
		}
	
		public void OnContinueClick()
		{
			StartCoroutine(OnContinueCoroutine());
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
		
		public void PlaySoundSlide()
		{
			mAudioInGameController.PlayDefinition(SoundDefinitions.COUNTER_SLIDES, false);
		}
		
		public void PlaySoundCoins()
		{
			if( mMatchBridge.CurrentMatchDefinition.Reward > 0 )
				mAudioInGameController.PlayDefinition(SoundDefinitions.COINS, false);
		}
		
		public void PlaySoundBall(int c)
		{	
			switch(c)
			{
				case 0:
					mAudioInGameController.PlayDefinition(SoundDefinitions.RESULT_BALL_1, false);
				break;
				case 1:
					mAudioInGameController.PlayDefinition(SoundDefinitions.RESULT_BALL_2, false);
				break;
				case 2:
					mAudioInGameController.PlayDefinition(SoundDefinitions.RESULT_BALL_3, false);
				break;
			}
		}
		
		public void PlaySoundScore()
		{
			mAudioInGameController.PlayDefinition(SoundDefinitions.SCORE_SOUND, false);
		}
		
		public void PlaySoundResult()
		{
			mAudioInGameController.PlayDefinition(SoundDefinitions.MATCH_RESULT, false);
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
				
		IEnumerator DismissFansLabel(Transform label, AnimatedColor animColor) {
			float alphaVel = 0.0f;
			float targetX = 215.0f;
			float coordXVel = 350.0f;

			while (Mathf.Abs(animColor.color.a - 0.0f) > 0.01f)
			{
				var newAlpha = Mathf.SmoothDamp(animColor.color.a, 0.0f, ref alphaVel, 0.125f, Mathf.Infinity, Time.deltaTime);
				animColor.color = new Color32(255, 190, 0, (byte)(newAlpha * 255));
				
				var newX = Mathf.SmoothDamp(label.localPosition.x, targetX, ref coordXVel, 0.075f, Mathf.Infinity, Time.deltaTime);
				label.localPosition = new Vector3(newX, label.localPosition.y, label.localPosition.z);
				
				yield return null;
			}
		}
				
		IEnumerator AnimateBallScale(Transform ball) {			
			float scaleVel = 1.0f;
			ball.localScale = new Vector3(1.7f, 1.7f, 1.0f);
						
			while (Mathf.Abs(ball.localScale.x - 1.0f) > 0.05f)
			{
				var newScale = Mathf.SmoothDamp(ball.localScale.x, 1, ref scaleVel, 0.125f, Mathf.Infinity, Time.deltaTime);
				ball.localScale = new Vector3(newScale, newScale, 1);
				yield return null;
			}
			ball.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}

		void OnCompartirVictoriaEnFacebook()
		{
			if (!mMainModel.FacebookInitialized)                                                                                                                         
				FBInit();
			else
				CallFBLogin();
		}
		
		private void FBInit()
		{
			FB.Init (OnFBInitComplete, OnHideUnity);
		}
		
		private void OnFBInitComplete()
		{
			//Debug.Log( string.Format("FB.Init completed: Is user logged in? {0} FB ID:{1}", FB.IsLoggedIn, FB.UserId) );
			mMainModel.FacebookInitialized = true;
			CallFBLogin();
		}
		
		private void OnHideUnity(bool isGameShown)
		{
			Debug.Log("Is game showing? " + isGameShown);
		}
		
		public void CallFBLogin()
		{
			if (!FB.IsLoggedIn)
				FB.Login (FacebookUtils.FB_PERMISSIONS, FBLoginCallback);
			else
				PublicaVictoriaEnFB();
		}
		
		void FBLoginCallback(FBResult result)
		{
			mLastFBResponse = "";
			if (result.Error != null)
				mLastFBResponse = "Error Response:\n" + result.Error;
			else if (!FB.IsLoggedIn) {
				mLastFBResponse = "Login cancelled by Player";
			}
			else {
				PublicaVictoriaEnFB();
			}
			
			if(mLastFBResponse != "") 
				Debug.Log( String.Format("Respuesta del login: {0}", mLastFBResponse) );
		}
		
		void PublicaVictoriaEnFB()
		{
			FB.Feed(
				"", //ToID
				"", //Link
				FacebookUtils.PUBLISH_TITLE_FOR_WINNING_MATCH, // Link Name
				" ", // Link Caption
				FacebookUtils.PUBLISH_DESCRIPTION_FOR_WINNING_MATCH, // linkDescription
				FacebookUtils.PUBLISH_PICTURE_FOR_WINNING_MATCH, // picture
				"", // media source
				"", //action name
				"", // action link
				"", //reference
				null, //properties
				LogCallback // callback
			);
		}

		void LogCallback(FBResult response) {
			Debug.Log(response.Text);
		}

		private MatchManager mMatchManager;
		private MatchBridge mMatchBridge;
		private AudioInGameController mAudioInGameController;
		private MainModel mMainModel;
		
		private UIPanel mPanel;

		private string mLastFBResponse = "";
	}
}
