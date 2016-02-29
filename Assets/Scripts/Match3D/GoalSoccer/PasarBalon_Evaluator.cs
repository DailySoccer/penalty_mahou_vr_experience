using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Pasar Balon")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class PasarBalon_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.PasarBalon ) {
				if ( mBrain.Data.BallPropietary )
					mDesirability = 0.5f;
				else {
					// Es posible pasar sin tener el balón (cuando queremos hacer una pared con otro jugador)
					// desirability = 0.5f;
					
					/*
					Helper.Log (transform.parent.gameObject, "Pasar sin balon");
					*/
				}
			}
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			// brain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( PasarBalon_Goal.New( gameObject, mBrain.Data.Target, mBrain.Data.Tiempo ), this, mDesirability );
		}
	}
	
}
