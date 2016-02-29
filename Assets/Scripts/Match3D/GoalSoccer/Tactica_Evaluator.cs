using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Tactica")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Tactica_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponentInChildren<GoalThink>();
		}
		
		public override float CalculateDesirability() {
			mDesirability = 0;
			
			/*
			if ( !mBrain.Data.BallPropietary && mBrain.Data.ControlBalon ) {
				mDesirability = 0.5f;
			}
			*/
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Tactica_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
