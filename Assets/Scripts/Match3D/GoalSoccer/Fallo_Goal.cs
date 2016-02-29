using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Fallo_Goal : Goal {
		
		private GameObject target;
		private float distanceToTarget;
		private SoccerData data;
		private GameObject balon;
		
		public static Fallo_Goal New( GameObject _owner ) {
			Fallo_Goal goal = Create<Fallo_Goal>(_owner);
			goal.Set();
			return goal;
		}
				
		protected void Set() {
			if ( InitializeGlobal ) {
				// data = owner.GetComponent<SoccerData>();
				// balon = GameObject.FindGameObjectWithTag ("Balon");
			}
		}
	
		public override void Activate () {
			status = Status.Active;
			
			/*
			balon.transform.parent = null;
			
			BallMotor ballMotor = balon.GetComponent<BallMotor>();
			ballMotor.ApplyImpulseToPosition ( owner.transform.position + (owner.transform.forward * 10f), 1 );
			
			data.BallPropietary = false;
			*/
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			ReactivateIfFailed();
			
			return status;
		}
		
		public override void Terminate () {
			status = Status.Inactive;
			
			// Debug.Log ( "Terminate: " + ToString () );
		}	
	}
	
}
