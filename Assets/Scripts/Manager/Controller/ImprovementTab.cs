using System;
using System.Collections.Generic;
using UnityEngine;
using FootballStar.Manager.Model;


namespace FootballStar.Manager
{
	public class ImprovementTab : MonoBehaviour
	{	
		public ImprovementScreen ImprovementScreen;
		
		void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
		}
		
		protected void ShowImprovementScreen(ImprovementCategory improvementCategory)
		{
			if (mCurrentImprovementScreen != null && mCurrentImprovementScreen.ImprovementCategory == improvementCategory)
				return;

			if (mCurrentImprovementScreen == null)
				mCurrentImprovementScreen = NGUITools.AddChild(gameObject, ImprovementScreen.gameObject).GetComponent<ImprovementScreen>();

			mCurrentImprovementScreen.ImprovementCategory = improvementCategory;
		}

		protected MainModel mMainModel;
		private ImprovementScreen mCurrentImprovementScreen;
	}
}

