using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	[AddComponentMenu("FootballStar/Marcaje Al Hombre")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class MarcajeAlHombre_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;
		
		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}
		
		public override float CalculateDesirability() { 
			mDesirability = 0;
			
			/*
			if ( transform.parent.gameObject.name != "Soccer-Visitante1" )
			if ( mData ) {
				if ( mData.Marcaje != null ) {
					desirability = 0.6f;
				}
				else {
					GameObject target = mData.SoccerNearest( mData.EnemigoNear );
					if ( target ) {
						ZonaAtaque zona = target.GetComponentInChildren<ZonaAtaque>();
						if ( zona.NumAttackers() == 0 ) {
							desirability = 0.6f;
						}
					}
				}
			}
			*/
			
			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( MarcajeAlHombre_Goal.New( gameObject ), this, mDesirability );
		}
	}
	
}
