using System;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;
using System.Collections.Generic;
using ExtensionMethods;

namespace FootballStar.Manager
{
	public class SponsorsScreen : MonoBehaviour
	{
		public GameObject AutoBuySponsorScreenPrefab;

		void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mSmallTheater = GameObject.Find("SmallTheater").GetComponent<SmallTheater>();
			
			mButtonBackgrounds = gameObject.GetComponentsInChildren<UISprite>().Where (spr => spr.name == "BottomButtonBackground").OrderBy(spr => spr.transform.parent.name).ToList();
			mBackgroundTexture = transform.FindChild("Background Texture").GetComponentInChildren<UITexture>();
			
			mRequiresValueLabel = GameObject.Find ("RequiresValue").GetComponent<UILabel>();
			mBonusValueLabel = GameObject.Find ("BonusValue").GetComponent<UILabel>();
			
			mCurrentTierBackgrounds = new List<Texture2D>();
			
			mBuySponsorButton = GameObject.Find ("BuySponsor Button");
			mGotSponsorContainer =  transform.FindChild("LeftPanel").FindChild("YouGotSponsor Container").gameObject;
			mLowerPanel = GameObject.Find ("LowerPanel");
			mCurrentButtonIdx = 0;
		}
		
		void OnDestroy()
		{
			mMainModel.OnModelChanged -= HandleOnModelChanged;
		}
		
		void Start()
		{
			ForceUpdate();
		}
		
		void OnEnable()
		{
			mMainModel.OnModelChanged += HandleOnModelChanged;
			AutoBuySponsor();
			ForceUpdate();
		}

		void AutoBuySponsor() {
			// Buscar cual es el primero sin comprar
			var firstSponsorNotPurchased = mMainModel.Player.CurrentTier.Sponsors.GetFirstPurchaseableSponsorID();

			if (firstSponsorNotPurchased != -1) {
				mCurrentButtonIdx = firstSponsorNotPurchased;
				
				// Mostramos mensaje contratador
				mMessageOverlap = NGUITools.AddChild(this.gameObject, AutoBuySponsorScreenPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.transform.localPosition = new Vector3(0, 0, -10);
			}
		}

		void OnContinueAutoBuySponsorClick() {
			Destroy(mMessageOverlap);
			OnBuySponsorButtonClick();
		}
		
		void OnDisable()
		{
			mMainModel.OnModelChanged -= HandleOnModelChanged;
			mSmallTheater.HideCurrentObject();
		}
		
		void ForceUpdate()
		{
			var currentTierIdx = mMainModel.Player.CurrentTierIndex;
			RefreshBackgroundsForTier(currentTierIdx);
			RefreshButtonsForTier(currentTierIdx);
			RefreshContent();
		}

		void HandleOnModelChanged(object sender, EventArgs e)
		{
			RefreshBackgroundsForTier(mMainModel.Player.CurrentTierIndex);
			RefreshButtonsForTier(mMainModel.Player.CurrentTierIndex);
			RefreshContent();
		}
		
		void OnSponsor01Click()
		{
			OnClickButton(0);
		}
		
		void OnSponsor02Click()
		{
			OnClickButton(1);
		}
		
		void OnSponsor03Click()
		{			
			OnClickButton(2);
		}
		
		void OnClickButton(int idxButton)
		{
			if (mCurrentButtonIdx == idxButton)
				return;
			
			mCurrentButtonIdx = idxButton;			
			RefreshContent();
		}
				
		void OnBuySponsorButtonClick()
		{
			var currentSponsors = mMainModel.CurrentTier.Sponsors;
			var sponsorDefinition = currentSponsors.Definition.SponsorsDefinitions[mCurrentButtonIdx];
			
			if (currentSponsors.IsSponsorLocked(sponsorDefinition) || currentSponsors.IsSponsorAlreadyPurchased(sponsorDefinition))
				return;
			
			mMainModel.BuySponsor(sponsorDefinition);
		}
		
		void RefreshContent()
		{
			mBackgroundTexture.material = new Material(Shader.Find("Unlit/Texture"));
			mBackgroundTexture.mainTexture = mCurrentTierBackgrounds[mCurrentButtonIdx];
			
			var currentSponsors = mMainModel.CurrentTier.Sponsors;
			var sponsorDefinition = currentSponsors.Definition.SponsorsDefinitions[mCurrentButtonIdx];
			
			if (currentSponsors.IsSponsorLocked(sponsorDefinition)) {
				mSmallTheater.ShowObject("Locked");
				mLowerPanel.SetActive(true);
				mBuySponsorButton.SetActive(false);
				mGotSponsorContainer.SetActive(false);
			}
			else {
				mSmallTheater.HideCurrentObject();
				
				if (currentSponsors.IsSponsorAlreadyPurchased(sponsorDefinition))
				{
					mLowerPanel.SetActive(false);
					mGotSponsorContainer.SetActive(true);
				}
				else
				{
					mLowerPanel.SetActive(true);
					mBuySponsorButton.SetActive(true);
					mGotSponsorContainer.SetActive(false);
				}
			}
			
			mBonusValueLabel.text = sponsorDefinition.SponsorshipBonus.FormatAsMoney();
			mRequiresValueLabel.text = sponsorDefinition.RequiredFans.ToString() + " FANS";
		}
		
		void RefreshBackgroundsForTier(int tierIdx)
		{
			if (tierIdx == mCurrentLoadedTierIdx)
				return;
			
			foreach (var texture2D in mCurrentTierBackgrounds)
			{
				Resources.UnloadAsset(texture2D);
			}
				
			for (int c = 0; c < mButtonBackgrounds.Count; ++c)
			{
				var textureName = String.Format("SponsorTier{0}Background{1:00}", tierIdx + 1, c + 1);
				var texture2D = Resources.Load(textureName, typeof(Texture2D)) as Texture2D;
								
				if (texture2D != null)
					mCurrentTierBackgrounds.Add(texture2D);
				else
					Debug.Log("Sponsor Background Didn't Load... " + textureName);
			}
			
			mCurrentLoadedTierIdx = tierIdx;
		}
		
		void RefreshButtonsForTier(int tierIdx)
		{			
			for (int c = 0; c < mButtonBackgrounds.Count; ++c)
			{
				mButtonBackgrounds[c].spriteName = String.Format("SponsorTier{0}Button{1:00}", tierIdx + 1, c + 1);
			}
		}
		
		MainModel mMainModel;
		SmallTheater mSmallTheater;
			
		int mCurrentLoadedTierIdx = -1;
		int mCurrentButtonIdx = -1;
				
		List<UISprite> mButtonBackgrounds;
		UITexture mBackgroundTexture;
		
		UILabel mBonusValueLabel;
		UILabel mRequiresValueLabel;
		
		GameObject mBuySponsorButton;
		GameObject mGotSponsorContainer;
		GameObject mLowerPanel;
		
		List<Texture2D> mCurrentTierBackgrounds;

		GameObject mMessageOverlap;
	}
}

