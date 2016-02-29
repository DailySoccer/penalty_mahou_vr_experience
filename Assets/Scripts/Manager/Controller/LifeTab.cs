using UnityEngine;
using System.Collections;
using System.Linq;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class LifeTab : ImprovementTab
	{	
		private GameObject mMessageOverlap;
		private MainModel mGameModel;

		void Awake()
		{
			mGameModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		}

		void Start()
		{
			GetComponentsInChildren<UIToggle>().Where(comp => comp.name == "Button01 Properties").FirstOrDefault().value = true;
			OnPropertiesClick();
		}

		void OnEnable()
		{
			if(!mGameModel.Player.VidaTutorialScreenAlreadyShown && mGameModel.Player.TutorialStage != TutorialStage.DONE )
			{
				mMessageOverlap = NGUITools.AddChild(this.gameObject, VidaTutorialScreenPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().functionName = "OnContinueAfterVistaScreenClick";

				mGameModel.Player.VidaTutorialScreenAlreadyShown = true;
				mGameModel.SaveDefaultGame();
			}
		}

		void OnPropertiesClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.PropertiesCategory);
		}
		
		void OnEventsClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.EventsCategory);
		}
		
		void OnVehiclesClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.VehiclesCategory);
		}

		void OnContinueAfterVistaScreenClick()
		{
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		public GameObject VidaTutorialScreenPrefab;
	}
}