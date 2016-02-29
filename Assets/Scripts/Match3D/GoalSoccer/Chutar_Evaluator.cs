using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Chutar")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Chutar_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( mBrain.Data.Chutar ) {
				if ( mBrain.Data.BallPropietary )
					mDesirability = 0.5f;
				else {
					/*
					GameObject partido = GameObject.FindGameObjectWithTag("Partido");
					if ( partido ) {
						SoccerGame soccerGame = partido.GetComponentInChildren<SoccerGame>();
						soccerGame.RecordEvent( gameObject, "" );
					}
					Helper.Log (transform.parent.gameObject, "Chutar sin balon");
					*/
				}
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			// brain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Chutar_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
