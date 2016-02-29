using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Regatear")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Regatear_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.BallPropietary && mBrain.Data.Regatear ) {
				mDesirability = 0.5f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Regatear_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}

