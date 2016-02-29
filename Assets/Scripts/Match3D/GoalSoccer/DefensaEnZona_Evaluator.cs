using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Defensa en Zona")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class DefensaEnZona_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		private BallMotor  mBallMotor;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
			
			GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			mBallMotor = balon.GetComponentInChildren<BallMotor>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( transform.parent.gameObject.name != "Soccer-Visitante1" )
			if ( /*data.BallNear &&*/ !mBrain.Data.BallPropietary && (transform.parent.gameObject != mBallMotor.NewPropietary) ) {
				mDesirability = 0.5f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( DefensaEnZona_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
