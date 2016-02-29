using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {

	[AddComponentMenu("FootballStar/Celebrar")]
	[RequireComponent (typeof (SoccerThink))]
	[RequireComponent (typeof (SoccerData))]
	public class Celebrar_Evaluator : GoalEvaluator {
		private GoalThink mBrain;
		private float mDesirability;

		void Awake() {
			mBrain = gameObject.GetComponent<GoalThink>();
		}

		public override float CalculateDesirability() { 
			mDesirability = 0;

			if ( mBrain.Data.Ganador ) {
				mDesirability = 1f;
			}

			mDesirability *= characterBias;
			return mDesirability; 
		}
		
		public override void   SetGoal() {
			mBrain.RemoveAllSubgoals();
			mBrain.AddSubgoal ( Celebrar_Goal.New( gameObject ), this, mDesirability );
		}
	}

}
