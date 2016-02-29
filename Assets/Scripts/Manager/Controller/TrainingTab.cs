using UnityEngine;
using System.Collections;
using System.Linq;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	public class TrainingTab : ImprovementTab
	{	
		private GameObject mMessageOverlap;
		private MainModel mGameModel;

		void Awake()
		{
			mGameModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		}

		void Start()
		{
			GetComponentsInChildren<UIToggle>().Where(comp => comp.name == "Button01 Gym").FirstOrDefault().value = true;
			OnGymClick();
		}


		void OnEnable()
		{
			if(!mGameModel.Player.EntrenarTutorialScreenAlreadyShown && mGameModel.Player.TutorialStage != TutorialStage.DONE )
			{
				mMessageOverlap = NGUITools.AddChild(this.gameObject, EntrenarTutorialScreenPrefab);
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().target = this.gameObject;
				mMessageOverlap.GetComponentInChildren<UIButtonMessage>().functionName = "OnContinueAfterEntrenarScreenClick";
				mMessageOverlap.SetActive(true);

				mGameModel.Player.EntrenarTutorialScreenAlreadyShown = true;
				mGameModel.SaveDefaultGame();
			}
		}

		void OnGymClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.GymCategory);
		}
		
		void OnBlackboardClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.BlackboardCategory);
		}
		
		void OnTechniqueClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.TechniqueCategory);
		}
		
		void OnLockerRoomClick()
		{
			ShowImprovementScreen(ImprovementsDefinition.LockerRoomCategory);
		}

		void OnContinueAfterEntrenarScreenClick()
		{
			Destroy(mMessageOverlap);
			mMessageOverlap = null;
		}

		public GameObject EntrenarTutorialScreenPrefab;
	}
}