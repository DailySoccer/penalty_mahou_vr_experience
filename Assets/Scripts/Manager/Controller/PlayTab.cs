using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using FootballStar.Manager.Model;
using System.Collections.Generic;


namespace FootballStar.Manager
{
	public class PlayTab : MonoBehaviour
	{	
		public FriendlyScreen FriendlyScreenPrefab;
		public LeagueScreen LeagueScreenPrefab;
		public CupScreen CupScreenPrefab;
		public EuroScreen EuroScreenPrefab;
		
		public GameObject MatchExplanationPrefab;
		public GameObject AllCompetitionsOpenedPrefab;
		public GameObject EuroUnlockPrefab;
		
		void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
			mIconLockedFriendly = GameObject.Find("IconLocked Friendly");
			mIconLockedLeague = GameObject.Find("IconLocked League");			
			mIconLockedCup = GameObject.Find("IconLocked Cup");
			mIconLockedEuro = GameObject.Find("IconLocked Euro");			
			
			mCheckboxFriendly = GameObject.Find("Button01 Friendly").GetComponent<UIToggle>();
			mCheckboxLeague = GameObject.Find("Button02 League").GetComponent<UIToggle>();
			mCheckboxCup = GameObject.Find("Button03 Cup").GetComponent<UIToggle>();
			mCheckboxEuro = GameObject.Find("Button04 Euro").GetComponent<UIToggle>();
		}
				
		void Start()
		{
		}

		void OnEnable()
		{
			mEnableOnUpdate = true;
		}

		void Update()
		{
			// Tenemos que hacer este fix por como funcionan los UIToggles. Los UIToggles se meten/sacan del grupo en el OnEnable/OnDisable. 
			// Por esto, cuando estamos en el OnEnable todavia no se han metido todos los hermanos en el grupo.
			if (mEnableOnUpdate) {
				RefreshLocked();
				RefreshClicked();
				ShowMessages();
				
				// Reseteamos los partidos que mostramos cada vez que entramos en esta seccion
				mMainModel.CurrentTier.MatchBrowser.ResetBrowsingToLast();
				
				// Hasta el siguiente OnEnable...
				mEnableOnUpdate = false;
			}
		}

		void RefreshClicked()
		{			
			if (mMainModel.Player.TutorialStage == TutorialStage.DONE)
			{
				mCheckboxFriendly.value = true;
				OnFriendlyClick();
			}
			else if (mMainModel.Player.TutorialStage == TutorialStage.FRIENDLY_EXPLANATION)
			{
				mCheckboxFriendly.value = true;
				OnFriendlyClick();
			}
			else if (mMainModel.Player.TutorialStage == TutorialStage.CUP_EXPLANATION)
			{
				mCheckboxCup.value = true;
				OnCupClick();
			}
			else if (mMainModel.Player.TutorialStage == TutorialStage.YOU_NEED_TO_IMPROVE)
			{
				mCheckboxFriendly.value = true;
				OnFriendlyClick();
			}
		}

		void ShowMessages() {
			if (mMainModel.Player.TutorialStage == TutorialStage.FRIENDLY_EXPLANATION) {
				// Mensaje en el primer amistoso, durante el tutorial
				mMessageOverlap = NGUITools.AddChild(this.gameObject, MatchExplanationPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.transform.localPosition = new Vector3(0, 0, -900);
			}
			else if (mMainModel.Player.TutorialStage == TutorialStage.YOU_NEED_TO_IMPROVE) {
				// Mensaje de final de tutorial
				mMessageOverlap = NGUITools.AddChild(this.gameObject, AllCompetitionsOpenedPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.transform.localPosition = new Vector3(0, 0, -900);
				
				// Y acabamos...
				mMainModel.NextStageTutorial();
			}
			else if (mMainModel.CurrentTier.MatchBrowser.LastLeagueIdx == Tier.EURO_MATCH_UNLOCK_LEAGUE_IDX && 
			        !mMainModel.Player.EuroUnlockScreenAlreadyShown) {

				// Se ha abierto Europa
				mMessageOverlap = NGUITools.AddChild(this.gameObject, EuroUnlockPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.transform.localPosition = new Vector3(0, 0, -900);

				mMainModel.Player.EuroUnlockScreenAlreadyShown = true;
				mMainModel.SaveDefaultGame();
			}
		}

		// Bloquemos los partidos no disponibles segun el Tier y segun TutorialStage
		void RefreshLocked()
		{
			if (mMainModel.Player.TutorialStage == TutorialStage.DONE || mMainModel.Player.TutorialStage == TutorialStage.YOU_NEED_TO_IMPROVE) {
				LockButtons (false, false, !mMainModel.CurrentTier.AreCupMatchesAvailable, !mMainModel.CurrentTier.AreEuroMatchesAvailable);
			}
			else {
				if (mMainModel.Player.TutorialStage == TutorialStage.FRIENDLY_EXPLANATION) {
					LockButtons(false, true, true, true);	// Todo bloqueado menos los amistosos
				}
				else
				if (mMainModel.Player.TutorialStage == TutorialStage.CUP_EXPLANATION) {
					LockButtons (true, true, false, true);	// Todo bloqueado menos copa
				}
			}
		}

		void LockButtons(bool lockFriendly, bool lockLeague, bool lockCup, bool lockEuro)
		{
			mIconLockedFriendly.SetActive(lockFriendly);
			mCheckboxFriendly.enabled = !lockFriendly;

			mIconLockedLeague.SetActive(lockLeague);
			mCheckboxLeague.enabled = !lockLeague;
			
			mIconLockedCup.SetActive(lockCup);
			mCheckboxCup.enabled = !lockCup;
			
			mIconLockedEuro.SetActive(lockEuro);
			mCheckboxEuro.enabled = !lockEuro;
		}

		void ShowScreen(GameObject screen)
		{
			if (mCurrentScreen != null && mCurrentScreen.GetType() != screen.GetType())
				return;

			NGUITools.Destroy(mCurrentScreen);			
			mCurrentScreen = NGUITools.AddChild(this.gameObject, screen);
		}
		

		// Continue del mensaje explicativo tutorial
		void OnContinueMatchExplanationClick() {
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "AMISTOSO"} });
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnContinueAllCompetitionsOpenedClick() {
			MixPanel.SendEventToMixPanel(AnalyticEvent.TUTORIAL, new Dictionary<string, object>{ {"Tutorial Stage", "TODO DESBLOQUEADO"} });
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnContinueEuroUnlockClick() {
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		void OnFriendlyClick()
		{			
			if (mCheckboxFriendly.enabled)
				ShowScreen(FriendlyScreenPrefab.gameObject);
		}
		
		void OnLeagueClick()
		{			
			if (mCheckboxLeague.enabled)
				ShowScreen(LeagueScreenPrefab.gameObject);
		}
		
		void OnCupClick()
		{			
			if (mCheckboxCup.enabled)
				ShowScreen(CupScreenPrefab.gameObject);
		}
		
		void OnEuroClick()
		{	
			if (mCheckboxEuro.enabled)
				ShowScreen(EuroScreenPrefab.gameObject);
		}
		
		MainModel mMainModel;
		GameObject mCurrentScreen;
		
		GameObject mIconLockedFriendly;
		GameObject mIconLockedLeague;
		GameObject mIconLockedCup;
		GameObject mIconLockedEuro;
		
		UIToggle mCheckboxFriendly;
		UIToggle mCheckboxLeague;
		UIToggle mCheckboxCup;
		UIToggle mCheckboxEuro;
		
		GameObject mMessageOverlap;

		bool mEnableOnUpdate = false;
	}
}