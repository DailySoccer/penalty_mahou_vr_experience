using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Libero_Goal : GoalComposite {
		
		// private SoccerData data;
		
		public static new Libero_Goal New( GameObject _owner ) {
			Libero_Goal goal = Create<Libero_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				// data = gameObject.GetComponent<SoccerData>();
			}
		}
		
		public override void Activate () {
			status = Status.Active;
			
			RemoveAllSubgoals();
			
			//Debug.Log ( ToString () + " Balon: " + balon.ToString() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ ProcessSubgoals();
				
				status = Status.Completed;
			
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}
	}
	
}
