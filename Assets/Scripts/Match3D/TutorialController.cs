using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using FootballStar.Audio;
using FootballStar.Common;
using System.Collections.Generic;
using FootballStar.Manager.Model;

namespace FootballStar.Match3D {
	
	public class TutorialController : MonoBehaviour {
		
		public GameObject   TutorialInterface;
		public GameObject[] AllMsgs;
		public UISprite 	IconTap;
		public GameObject   BlackBackground;
		public GameObject   MatchEndStarter;
		public GameObject   TimelinePlay;

				
		void Start()
		{
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();

			if (!mMatchManager.IsTutorialMatch) {
				gameObject.SetActive(false);
				return;
			}

			mGameModel = GameObject.FindGameObjectWithTag("GameModel");
			mMainModel = mGameModel.GetComponent<MainModel>();

			mAnchor = TutorialInterface.GetComponent<UIAnchor>();

			mTimeScale = mMatchManager.GetComponent<TimeScale>();
			mInterfaceAnimation = TutorialInterface.GetComponent<Animation>();
									
			mMatchManager.InteractiveActions.OnEvent += OnInteractiveAction;
			mMatchManager.OnNewPlay += OnNewPlay;
			mMatchManager.OnMatchEnd += OnMatchEnd;

			// En el tutorial no queremos dialogo fin de partido ni timeline
			Destroy(MatchEndStarter);
			Destroy(TimelinePlay);

			StartCoroutine(TutorialSequence());
			StartCoroutine(BlinkIconTap());
		}

		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e)
		{
			// Paramos TutorialSequence, que se queda pillada esperando a esto. Solo se puede salir por aqui, asi que no hay problema en dejar
			// una coroutina pillada
			StopAllCoroutines();

			StartCoroutine(MatchEndSequence());
		}
		
		IEnumerator MatchEndSequence()
		{
			yield return new WaitForSeconds(2.0f);

			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "P01 End"} });

			yield return StartCoroutine(ActivateMsg("FinalMessage", true, UIAnchor.Side.Center));
			yield return StartCoroutine(WaitForAlphaAnimationEnd());
			yield return StartCoroutine(WaitForContinueButton());
			yield return StartCoroutine(CameraFade.FadeCoroutine(false, 1.0f, 0.0f));

			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "FICHADO"} });

			// Igual que en el MatchEndDialog...
			mGameModel.GetComponent<MatchBridge>().SendMessage("GoBackToManager", SendMessageOptions.DontRequireReceiver);
			mGameModel.GetComponent<AudioInGameController>().StopAllActiveAudios(true);
		}
	
		IEnumerator TutorialSequence()
		{
			mMatchManager.InteractiveActions.InputEnabled = false;

			// Mensaje inicial (sobre negro)
			BlackBackground.SetActive(true);
			CameraFade.Enabled = false;								// Se pinta por encima de todo -> desconectamos
			yield return StartCoroutine(ActivateMsg("MessagePanel01", true, UIAnchor.Side.Center));
			yield return StartCoroutine(WaitForContinueButton());

			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J01 Start"} });

			CameraFade.Enabled = true;
			BlackBackground.SetActive(false);
			mCurrentMsg.SetActive(false);
			mMatchManager.OnTutorialStart();						// Soltamos al MatchManager para que comience la cutscene

			// Primer mensaje (Simplemente en verde)
			yield return StartCoroutine(ShowMessageOnGreen("SmallMsg01", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J01 Step01"} });
			
			// Segundo mensaje (Circulos en perfect)
			yield return StartCoroutine(ShowMessageOnPerfect("SmallMsg02", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J01 Step02"} });
			
			// Chut
			yield return StartCoroutine(ShowMessageOnPerfect("SmallMsg03", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J01 Step03"} });
			
			// Segunda jugada (muy bien, esta jugada es un poco mas complicada)
			yield return StartCoroutine(ShowMessageOnNewPlay("SmallMsg04", UIAnchor.Side.Bottom));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J02 Start"} });

			// Regate 1
			yield return StartCoroutine(ShowMessageOnGreen("SmallMsg05", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J02 Step01"} });

			// Regate 2, en perfect
			yield return StartCoroutine(ShowMessageOnPerfect("SmallMsg06", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J02 Step02"} });

			// Chut
			yield return StartCoroutine(ShowMessageOnPerfect("SmallMsg07", UIAnchor.Side.BottomLeft));
			// MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J02 Step03"} });

			mMatchManager.InteractiveActions.InputEnabled = true;

			// You are on your own...
			yield return StartCoroutine(ShowMessageOnNewPlay("SmallMsg08", UIAnchor.Side.Bottom));
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J03 Start"} });

			// Nos quedamos pillados esperando a que llegue un MatchEnd y se haga un StopAllCoroutines
			while(true) {
				yield return StartCoroutine(WaitForNewPlay());
				MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "J03 Replay"} });
			}
		}

		IEnumerator ShowMessageOnGreen(string msg, UIAnchor.Side align)
		{
			yield return StartCoroutine(WaitForQTEShow());
			yield return StartCoroutine(ActivateMsg(msg, true, align));
			StartCoroutine(WaitForPauseTime(0.5f));
			yield return StartCoroutine(WaitForInteractiveActionSuccess());
			mCurrentMsg.SetActive(false);
			StartCoroutine(WaitForRestoreTime(0.3f));
		}

		IEnumerator ShowMessageOnPerfect(string msg, UIAnchor.Side align)
		{
			yield return StartCoroutine(WaitForQTEShow());
			StartCoroutine(WaitForPauseTime(1.0f));	// El perfect tardara menos en llegar que el tiempo congelado...
			yield return StartCoroutine(WaitForInteractiveActionPerfect());
			mTimeScale.factor = 0.0f;				// ... por esto, paramos ahora. Tb, hacer eso fuerza a la coroutine a retornar
			yield return StartCoroutine(ActivateMsg(msg, false, align));
			yield return StartCoroutine(WaitForInteractiveActionSuccess());
			mCurrentMsg.SetActive(false);
			StartCoroutine(WaitForRestoreTime(0.2f));
		}

		IEnumerator ShowMessageOnNewPlay(string msg, UIAnchor.Side align)
		{
			yield return StartCoroutine(WaitForNewPlay());
			yield return new WaitForSeconds(1.0f);
			yield return StartCoroutine(ActivateMsg(msg, true, align));
			yield return StartCoroutine(WaitForAlphaAnimationEnd());
			yield return StartCoroutine(WaitForPauseTime(0.2f));
			yield return StartCoroutine(WaitForContinueButton());
			mCurrentMsg.SetActive(false);
			yield return StartCoroutine(WaitForRestoreTime(0.1f));
		}
		
		IEnumerator ActivateMsg(string name, bool fadeIn, UIAnchor.Side align)
		{
			mAnchor.side = align;

			// Solo tenemos mensajes al BottomLeft y al centro
			if (align == UIAnchor.Side.BottomLeft)
				mAnchor.relativeOffset = new Vector2(0.33f, 0.16f);
			else if (align == UIAnchor.Side.Bottom)
				mAnchor.relativeOffset = new Vector2(0.0f, 0.16f);
			else
				mAnchor.relativeOffset = Vector2.zero;

			// Esperamos un frame para darle tiempo al UIAnchor procesar
			yield return null;

			if (fadeIn) {
				TutorialInterface.GetComponent<UIPanel>().alpha = 0.0f;
				TutorialInterface.GetComponent<AnimatedAlpha>().alpha = 0.0f;
				TutorialInterface.GetComponent<Animation>().Play();
			}

			foreach (GameObject theMsg in AllMsgs) {
				if (theMsg.name == name) {
					theMsg.SetActive(true);
					mCurrentMsg = theMsg;
				}
				else
					theMsg.SetActive(false);
			}
		}

		
		void OnNewPlay (object sender, EventArgs e)
		{
			mNewPlay = true;
		}
		
		IEnumerator BlinkIconTap() 
		{
			while (gameObject.activeSelf) {
				if (mShowingIconTap) {
					var elapsedTime = Time.realtimeSinceStartup - mTimeIconTap;
					
					if (elapsedTime < 1.0f)
						IconTap.enabled = false;
					else if (elapsedTime < 2.0f)
						IconTap.enabled = true;
					else
						mTimeIconTap = Time.realtimeSinceStartup;
				}
				else {
					IconTap.enabled = false;
				}
				yield return null;
			}
		}

		
		void ShowIconTap(bool showIt) 
		{
			mShowingIconTap = showIt;
			
			if (mShowingIconTap) {
				mTimeIconTap = Time.realtimeSinceStartup;
			}
		}

		private float mTimeScaleVel = 1.0f;
		private float mLastPauseTime;
		
		IEnumerator WaitForPauseTime(float pauseTime) 
		{
			mLastPauseTime = Time.realtimeSinceStartup - 0.033f;
			while(mTimeScale.factor > 0.05) {
				mTimeScale.factor = Mathf.SmoothDamp(mTimeScale.factor, 0.0f, ref mTimeScaleVel, pauseTime, Mathf.Infinity, Time.realtimeSinceStartup - mLastPauseTime);
				mLastPauseTime = Time.realtimeSinceStartup;
				yield return null;
			}
			mTimeScale.factor = 0.0f;
		}
		
		IEnumerator WaitForRestoreTime(float restoreTime) 
		{
			mLastPauseTime = Time.realtimeSinceStartup - 0.033f;
			while(mTimeScale.factor < 0.95) {
				mTimeScale.factor = Mathf.SmoothDamp(mTimeScale.factor, 1.0f, ref mTimeScaleVel, restoreTime, Mathf.Infinity, Time.realtimeSinceStartup - mLastPauseTime);
				mLastPauseTime = Time.realtimeSinceStartup;
				yield return null;
			}
			mTimeScale.factor = 1.0f;
		}

		void OnContinueButton()
		{
			mOnContinue = true;
		}
		IEnumerator WaitForContinueButton()
		{
			mOnContinue = false;
			while (!mOnContinue)
				yield return null;
			mOnContinue = false;
		}
				
		void OnInteractiveAction (object sender, EventInteractiveActionArgs e) 
		{
			if (e.State == InteractionStates.BEGIN)
				mQTEShow = true;
			else if (e.State == InteractionStates.PERFECT)
				mInteractiveActionPerfect = true;
			else if (e.State == InteractionStates.END)
				mInteractiveActionSuccess = true;
		}
		IEnumerator WaitForInteractiveActionPerfect() 
		{
			while (!mInteractiveActionPerfect)
				yield return null;
			mInteractiveActionPerfect = false;
		}
		IEnumerator WaitForInteractiveActionSuccess() 
		{
			mMatchManager.InteractiveActions.InputEnabled = true;
			mInteractiveActionSuccess = false;
			while (!mInteractiveActionSuccess)
				yield return null;
			mInteractiveActionSuccess = false;
			mMatchManager.InteractiveActions.InputEnabled = false;
		}

		IEnumerator WaitForQTEShow() 
		{
			mQTEShow = false;
			while (!mQTEShow)
				yield return null;
			mQTEShow = false;
		}
				
		IEnumerator WaitForAlphaAnimationEnd() 
		{			
			while (mInterfaceAnimation.IsPlaying("AlphaAnim"))
				yield return null;
		}
		
		IEnumerator WaitForNewPlay() 
		{
			mNewPlay = false;
			while (!mNewPlay)
				yield return null;
			mNewPlay = false;
		}

		void OnCompartirFichajeEnFacebook()
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
			Debug.Log( string.Format("FB.Init completed: Is user logged in? {0} FB ID:{1}", FB.IsLoggedIn, FB.UserId) );
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
				PublicaFichajeEnFB();
		}
		
		void FBLoginCallback(FBResult result)
		{
			string mLastFBResponse = "";
			if (result.Error != null)
				mLastFBResponse = "Error Response:\n" + result.Error;
			else if (!FB.IsLoggedIn) {
				mLastFBResponse = "Login cancelled by Player";
			}
			else {
				PublicaFichajeEnFB();
			}
			
			Debug.Log( String.Format("Respuesta del login: {0}, Resultado del login: {1}", mLastFBResponse, result.Text) );
		}
		
		void PublicaFichajeEnFB()
		{
			FB.Feed(
				"", //ToID
				"", //Link
				FacebookUtils.PUBLISH_TITLE_FOR_SIGNING, // Link Name
				" ", // Link Caption
				FacebookUtils.PUBLISH_DESCRIPTION_FOR_SGNING, // linkDescription
				FacebookUtils.PUBLISH_PICTURE_FOR_SINGNING, // picture
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

		GameObject mGameModel;
		MainModel mMainModel;

		MatchManager mMatchManager;
		GameObject mCurrentMsg;

		Animation mInterfaceAnimation;
		TimeScale mTimeScale;

		bool mQTEShow = false;
		bool mOnContinue = false;
		bool mInteractiveActionPerfect = false;
		bool mInteractiveActionSuccess = false;
		bool mNewPlay = false;
		
		bool mShowingIconTap = false;
		float mTimeIconTap = 0.0f;

		UIAnchor mAnchor;

	}
}