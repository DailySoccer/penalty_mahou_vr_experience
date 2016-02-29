using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Libero")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Libero_Evaluator : GoalEvaluator {
		// private GoalThink brain;
		private float mDesirability;
		
		void Awake() {
			// brain = gameObject.GetComponentInChildren<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			/*
			if ( !data.BallPropietary && data.ControlBalon ) {
				desirability = 0.5f;
			}
			*/
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			// brain.RemoveAllSubgoals();
			// brain.AddSubgoal ( Libero_Goal.New( gameObject ), this, desirability );
		}
	}
	
}
