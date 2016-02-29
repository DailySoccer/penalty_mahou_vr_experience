using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	// [AddComponentMenu("FootballStar/Template")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Template_Evaluator : GoalEvaluator {
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
			mBrain.AddSubgoal ( Template_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
