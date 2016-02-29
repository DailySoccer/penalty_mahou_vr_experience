using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Portero Despejar")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class PorteroDespejar_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		private BallMotor mBalonMotor;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
			
			GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			mBalonMotor = balon.GetComponentInChildren<BallMotor>();	
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( (mBalonMotor.Estado == BallMotor.EEstado.Chut) && mBalonMotor.MovingToPosition ) {
				mDesirability = 0.5f;
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( PorteroDespejar_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}