using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Presion Sobre Contrario")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class PresionSobreContrario_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		// private BallMotor mBalonMotor;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		
			/*
			GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			mBalonMotor = balon.GetComponentInChildren<BallMotor>();
			*/
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			if ( transform.parent.gameObject.name != "Soccer-Visitante1" )
			if ( mBrain.Data && mBrain.Data.BallNear ) {
				mDesirability = 0.6f;
				/*
				GameObject target = mBalonMotor.NewPropietary;
				if ( target != null ) {
					ZonaAtaque zonaAtaque = target.GetComponentInChildren<ZonaAtaque>();
					if ( zonaAtaque != null && zonaAtaque.GetAttackPosition( gameObject ) != null ) {
						desirability = 0.6f;
					}
				}
				*/
			}
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( PresionSobreContrario_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
