using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Rematar")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Rematar_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.Rematar /*&& mBrain.Data.ActionContext*/ ) {
				mDesirability = 0.5f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			//brain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Rematar_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}