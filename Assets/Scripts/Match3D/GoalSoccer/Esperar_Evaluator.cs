using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Esperar")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Esperar_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.Esperar ) {
				mDesirability = 0.2f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Esperar_Goal.New( gameObject, mBrain.Data.Tiempo ), this, mDesirability );
		}
	}
	
}
