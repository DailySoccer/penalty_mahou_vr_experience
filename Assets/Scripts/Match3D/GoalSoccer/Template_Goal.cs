using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Template_Goal : GoalComposite {
		
		private SoccerData mData;
		
		public static new Template_Goal New( GameObject _owner ) {
			Template_Goal goal = Create<Template_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mData.ToString();	// <---- Remove WARNING!!
			}
		}
		
		public override void Activate () {
			status = Status.Active;
			
			RemoveAllSubgoals();
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

