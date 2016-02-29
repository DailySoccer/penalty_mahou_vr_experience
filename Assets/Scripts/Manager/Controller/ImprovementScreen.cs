using System;
using System.Linq;
using UnityEngine;
using FootballStar.Manager.Model;
using ExtensionMethods;
using FootballStar.Common;
using System.Collections.Generic;

namespace FootballStar.Manager
{
	public class ImprovementScreen : MonoBehaviour
	{
		public ImprovementCategory ImprovementCategory 
		{ 
			get { return mImprovementCategory; }
			set { 
				if (mImprovementCategory != value) {
					mImprovementCategory = value;

					if (!mImprovementCategoryIndices.ContainsKey(mImprovementCategory))
						mImprovementCategoryIndices[mImprovementCategory] = FindFirstItemNotPurchased(mImprovementCategory);

					mNeedsUpdate = true;
				}
			}
		}
		
		void Awake()
		{				
			var theBars = gameObject.GetComponentsInChildren<ProgressBar>();
			mVisionBar = GetBar(theBars, "VisionBar");
			mPowerBar = GetBar(theBars, "PowerBar");
			mTechniqueBar = GetBar(theBars, "TechniqueBar");
			mReputationBar = GetBar(theBars, "ReputationBar");
			
			var theLabels = gameObject.GetComponentsInChildren<UILabel>();
			mVisionValueLabel = theLabels.Where(label => label.name == "VisionValue Label").FirstOrDefault();
			mPowerValueLabel = theLabels.Where(label => label.name == "PowerValue Label").FirstOrDefault();
			mTechniqueValueLabel = theLabels.Where(label => label.name == "TechniqueValue Label").FirstOrDefault();
			mReputationValueLabel = theLabels.Where(label => label.name == "ReputationValue Label").FirstOrDefault();
						
			mSpeechLabel = theLabels.Where(label => label.name == "Speech Label").FirstOrDefault();
			mTitleLabel = theLabels.Where(label => label.name == "Title Label").FirstOrDefault();
			
			mVisionDiffLabel = theLabels.Where(label => label.name == "VisionDiff Label").FirstOrDefault();
			mPowerDiffLabel = theLabels.Where(label => label.name == "PowerDiff Label").FirstOrDefault();
			mTechniqueDiffLabel = theLabels.Where(label => label.name == "TechniqueDiff Label").FirstOrDefault();
			mReputationDiffLabel = theLabels.Where(label => label.name == "ReputationDiff Label").FirstOrDefault();
						
			mPriceButtonLabel = theLabels.Where(label => label.name == "Price Label").FirstOrDefault();
			
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mSmallTheater = GameObject.Find("SmallTheater").GetComponent<SmallTheater>();
			mPerformanceTuner = mMainModel.GetComponent<PerformanceTuner>();
			
			mBackgroundTexture = gameObject.GetComponentsInChildren<UITexture>().Where(texture => texture.name == "BackgroundTexture").FirstOrDefault();
			mBuyButton = transform.FindChild("Left Panel").FindChild("Buy Button").gameObject;
			mUIBuyButton = mBuyButton.GetComponent<UIButton>();
			mSoldOutContainer  = transform.FindChild("Left Panel").FindChild("SoldOut Container").gameObject;
			mForbiddenContainer = transform.FindChild("Left Panel").FindChild("Forbidden Container").gameObject;
		}
		
		static ProgressBar GetBar(ProgressBar[] bars, string name)
		{
			return (from s in bars 
					where s.gameObject.name == name
					select s).FirstOrDefault();
		}

		int FindFirstItemNotPurchased(ImprovementCategory category) {
			int index = -1;
			int length = category.Items.Length;
			for (int i=0; i<length && index==-1; i++) {
				ImprovementItem item = category.Items[i];
				if (!mMainModel.Player.Improvements.IsItemAlreadyPurchased(item))
					index = i;
			}
			return (index!=-1) ? index : category.Items.Length-1;
		}

		void OnEnable()
		{
			mNeedsUpdate = true;
		}
		
		void OnDisable()
		{
			mSmallTheater.HideCurrentObject();
		}

		void Update()
		{
			if (mNeedsUpdate) {
				ForceUpdate();
				mNeedsUpdate = false;
			}
		}
		
		void OnRightButtonClick()
		{
			if (mImprovementCategoryIndices[ImprovementCategory] < ImprovementCategory.Items.Length-1)
				mImprovementCategoryIndices[ImprovementCategory]++;

			mNeedsUpdate = true;
		}

		void OnLeftButtonClick()
		{
			if (mImprovementCategoryIndices[ImprovementCategory] > 0)
				mImprovementCategoryIndices[ImprovementCategory]--;

			mNeedsUpdate = true;
		}
		
		void OnBuyButtonClick()
		{
			mMainModel.BuyImprovement(CurrentItem);
			
			mNeedsUpdate = true;
		}

		ImprovementItem PrevItem { get { 
				int currentIdx = mImprovementCategoryIndices[ImprovementCategory];
				return ( currentIdx > 0 ) ? ImprovementCategory.Items[mImprovementCategoryIndices[ImprovementCategory]-1] : null;
			} }

		ImprovementItem CurrentItem { get { return ImprovementCategory.Items[mImprovementCategoryIndices[ImprovementCategory]]; } }

		void ChangeStateButton( UIButton button, bool enabled ) {
			mUIBuyButton.isEnabled = enabled;

			UISprite[] sprites = mUIBuyButton.GetComponentsInChildren<UISprite>();
			foreach( UISprite sprite in sprites ) {
				sprite.alpha = enabled ? 1f : 0.5f;
			}

			UILabel[] labels = mUIBuyButton.GetComponentsInChildren<UILabel>();
			foreach( UILabel label in labels ) {
				label.alpha = enabled ? 1f : 0.5f;
			}
		}

		void ForceUpdate()
		{
			if (ImprovementCategory == null)
				return;

			var theItem = CurrentItem;
			bool canAfford = theItem.Price <= mMainModel.Player.Money;
			
			mSpeechLabel.text = ImprovementCategory.Description;
			mTitleLabel.text = theItem.Name;

			if (mMainModel.Player.Improvements.IsItemAlreadyPurchased(theItem))
			{
				FillValue(mVisionBar, mVisionValueLabel, mVisionDiffLabel, mMainModel.Player.Vision, 0);
				FillValue(mPowerBar, mPowerValueLabel, mPowerDiffLabel, mMainModel.Player.Power, 0);
				FillValue(mTechniqueBar, mTechniqueValueLabel, mTechniqueDiffLabel, mMainModel.Player.Technique, 0);
				FillValue(mReputationBar, mReputationValueLabel, mReputationDiffLabel, mMainModel.Player.Motivation, 0);
				
				mBuyButton.SetActive(false);
				mSoldOutContainer.SetActive(true);
				mForbiddenContainer.SetActive(false);
				SetBackground(false, theItem);
			}
			else
			{
				FillValue(mVisionBar, mVisionValueLabel, mVisionDiffLabel, mMainModel.Player.Vision, theItem.VisionDiff);
				FillValue(mPowerBar, mPowerValueLabel, mPowerDiffLabel, mMainModel.Player.Power, theItem.PowerDiff);
				FillValue(mTechniqueBar, mTechniqueValueLabel, mTechniqueDiffLabel, mMainModel.Player.Technique, theItem.TechniqueDiff);
				FillValue(mReputationBar, mReputationValueLabel, mReputationDiffLabel, mMainModel.Player.Motivation, theItem.MotivationDiff);

				bool locked = false;

				ImprovementItem prevItem = PrevItem;
				if ( prevItem != null ) {
					locked = !mMainModel.Player.Improvements.IsItemAlreadyPurchased(prevItem);
					mBuyButton.SetActive( !locked );
					mForbiddenContainer.SetActive( locked );
				}
				else {
					mBuyButton.SetActive(true);
					mForbiddenContainer.SetActive(false);
				}

				ChangeStateButton( mUIBuyButton, canAfford );

				mSoldOutContainer.SetActive(false);
				mPriceButtonLabel.text = theItem.Price.FormatAsMoney();
			
				SetBackground(locked, theItem);
			}
		}


		private void SetBackground(bool isLocked, ImprovementItem theItem)
		{
			if (theItem.Background != null)
			{
				mBackgroundTexture.enabled = true;

				if (isLocked)
					mSmallTheater.ShowObject("Locked");
				else
					mSmallTheater.HideCurrentObject();

				if ( mMainModel.Player.Improvements.IsItemAlreadyPurchased(theItem) || (!isLocked && theItem.Price <= mMainModel.Player.Money) )
					mBackgroundTexture.material = new Material(Shader.Find("Unlit/Texture"));
				else if (mPerformanceTuner.OwnQualityLevel > 2)
					mBackgroundTexture.material = new Material(Shader.Find("Hidden/Locked Effect"));
					
				mBackgroundTexture.mainTexture = theItem.Background;
			}
			else
			{
				mBackgroundTexture.enabled = false;
			}
		}

		static void FillValue(ProgressBar bar, UILabel valueLabel, UILabel diffLabel, float val, float diff)
		{
			if (bar != null)
			{
				// Max = Valor Inicial + Cuanto podemos mejorar la habilidad
				const float valueInitial = 0.025f;
				const float max = valueInitial + 0.250f;
				
				if (diff >= 0)
				{
					bar.Percent = val / max;
					bar.MiddlePercent = bar.Percent + diff / max;
					bar.MiddleColor = Color.yellow;
				}
				else if (diff < 0)
				{
					bar.MiddlePercent = val / max;
					bar.MiddleColor = Color.red;			
					bar.Percent = bar.MiddlePercent + diff / max;
				}
			}
			
			valueLabel.text = MainModel.FormatVal(val);
			diffLabel.text = MainModel.FormatDiff(diff);
			
			ColorLabel(diffLabel, diff);
		}
		
		static void ColorLabel(UILabel label, float val)
		{
			if (val < 0)
				label.color = Color.red;
			else
				label.color = Color.green;
		}
		
		MainModel mMainModel;
		SmallTheater mSmallTheater;
		
		ProgressBar mVisionBar;
		UILabel     mVisionValueLabel;
		UILabel     mVisionDiffLabel;		
		ProgressBar mPowerBar;
		UILabel     mPowerValueLabel;
		UILabel     mPowerDiffLabel;		
		ProgressBar mTechniqueBar;
		UILabel     mTechniqueValueLabel;
		UILabel     mTechniqueDiffLabel;		
		ProgressBar mReputationBar;
		UILabel     mReputationValueLabel;
		UILabel     mReputationDiffLabel;		
				
		UILabel 	mSpeechLabel;
		UILabel 	mTitleLabel;
		UILabel 	mPriceButtonLabel;
		
		UITexture   mBackgroundTexture;
		GameObject  mBuyButton;
		UIButton 	mUIBuyButton;
		GameObject  mSoldOutContainer;
		GameObject  mForbiddenContainer;

		ImprovementCategory mImprovementCategory;
		Dictionary<ImprovementCategory, int> mImprovementCategoryIndices = new Dictionary<ImprovementCategory, int>();
		
		PerformanceTuner mPerformanceTuner;

		bool mNeedsUpdate = true;
	}
}
