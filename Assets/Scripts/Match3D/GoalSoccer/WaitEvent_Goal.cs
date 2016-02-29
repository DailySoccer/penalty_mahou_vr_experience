using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class WaitEvent_Goal : Goal {
			
		public static WaitEvent_Goal New( GameObject _owner ) {
			WaitEvent_Goal goal = Create<WaitEvent_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
		}
		
		public override void Activate () {
			status = Status.Active;
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			status = Status.Completed;
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}
	}
	
}
