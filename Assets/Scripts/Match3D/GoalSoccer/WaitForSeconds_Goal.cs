using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class WaitForSeconds_Goal : Goal {
		
		private float seconds = 0;
		
		public static WaitForSeconds_Goal New( GameObject _owner, float _seconds ) {
			WaitForSeconds_Goal goal = Create<WaitForSeconds_Goal>(_owner);
			goal.Set( _seconds );
			return goal;
		}
		
		protected void Set( float _seconds ) {
			seconds = _seconds;
		}
		
		public override void Activate () {
			status = Status.Active;
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			seconds -= Time.deltaTime;
			if ( seconds < 0 ) {
				status = Status.Completed;
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}
	}
	
}
	