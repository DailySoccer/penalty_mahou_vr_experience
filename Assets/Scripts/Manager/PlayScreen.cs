using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using FootballStar.Common;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class PlayScreen : MonoBehaviour
	{
		public GameObject RightBox;
		public GameObject LeftBox;
		
		public GameObject YouNeedEnergyScreen;

		void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
			mRightBox = (Instantiate(RightBox) as GameObject).GetComponent<PlayScreenRightBox>();
			mRightBox.transform.parent = this.transform;
			
			var leftBox = Instantiate(LeftBox) as GameObject;
			leftBox.transform.parent = this.transform;
			
			var theLabels = gameObject.GetComponentsInChildren<UILabel>();
			mMatchCountTitle = theLabels.Where(label => label.name == "MatchCount Title").FirstOrDefault();
			
			mSmallTheater = GameObject.Find("SmallTheater").GetComponent<SmallTheater>();
			
			// Nos subscribimos a los botones siguiente partido / partido anterior
			foreach (var comp in GetComponentsInChildren<UIButtonMessage>())
				comp.target = this.gameObject;

			PostAwake(leftBox, theLabels);
		}
				
		void Start()
		{
			ForceUpdate();		// TODO: ??????
		}
		
		void OnEnable()
		{
			ForceUpdate();
			mSmallTheater.ShowObject(SmallTheaterTrophyName);
		}
		
		void OnDisable()
		{
			mSmallTheater.HideCurrentObject();
		}
		
		void OnPlayMatchClick()
		{
			PlayMatch();
		}

	protected virtual void PlayMatch()
		{
			if ( mMainModel.CanIPlayMatches() )
			{
				mMainModel.PlayMatch(MatchToPlay);			
			}
			else
			{
				// Creamos nuestro mensajito explicativo
				mMessageOverlap = NGUITools.AddChild(this.gameObject, YouNeedEnergyScreen);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().LastPlay = mMainModel.Player.LastEnergyUse;
				mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().OnEnergyCountdownEnds += HandleOnEnergyCountdownEnds;
				mMessageOverlap.transform.localPosition = new Vector3(0, 0, -1000);
			}
		}

		void HandleOnEnergyCountdownEnds(object sender, EventArgs e)
		{
			mMessageOverlap.GetComponentInChildren<YouNeedEnergy>().OnEnergyCountdownEnds -= HandleOnEnergyCountdownEnds;
			OnContinueYouNeedEnergyClick();
		}
		
		void OnContinueYouNeedEnergyClick()
		{
			Destroy(mMessageOverlap);
		}
		
		void OnLeftButtonClick()
		{
			GotoPrevMatch();
			ForceUpdate();
		}
		
		void OnRightButtonClick()
		{
			GotoNextMatch();
			ForceUpdate();
		}
		
		protected virtual void ForceUpdate()
		{
			mMatchCountTitle.text = MatchCount;
			mRightBox.MatchToDisplay = MatchToPlay;
		}

		protected virtual string SmallTheaterTrophyName { get { return ""; } }			// Amistoso, CupTrophy01...
		protected virtual Match  MatchToPlay { get { return null; } }					// Partido q vamos a jugar cuando Click on Play
		protected virtual string MatchCount { get { return ""; } }
		
		// Nuestros hijos comunicaran al MatchBrowser a que partido queremos ir
		protected virtual void GotoNextMatch() {}
		protected virtual void GotoPrevMatch() {}
		
		// Para que nuestros hijos se puedan enganchar
		protected virtual void PostAwake(GameObject leftBox, UILabel[] theLabels) {}
		
		protected MainModel mMainModel;

		PlayScreenRightBox mRightBox;
		SmallTheater mSmallTheater;
		
		UILabel mMatchCountTitle;
		
		GameObject mMessageOverlap;
	}
}

