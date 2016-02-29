using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Entrada")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Entrada_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.Entrada || (/*mBrain.Data.BallNear &&*/ (mBrain.Data.CoordinatedTime > Time.time)) ) {
				mDesirability = 0.7f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Entrada_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
