using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class ControlBalon_Goal : GoalComposite {
		
		private BallMotor mBalonMotor;
		private SoccerData mData;
		private float mTimeWaitingBall;

		public static new ControlBalon_Goal New( GameObject _owner ) {
			ControlBalon_Goal goal = Create<ControlBalon_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = balon.GetComponentInChildren<BallMotor>();				
				mData = gameObject.GetComponent<SoccerData>();
			}
		}
		
		public override void Activate () {
			status = Status.Active;
			mTimeWaitingBall = 0f;

			RemoveAllSubgoals();
			
			mData.Aim = mBalonMotor.gameObject;
			
			if ( (mBalonMotor.Estado == BallMotor.EEstado.Pase) && (mBalonMotor.NewPropietary != Entity) ) {
				mData.ControlBalon = false;
				status = Status.Completed;
			}
			else {
				mBalonMotor.Estado = BallMotor.EEstado.Propietario;
				mBalonMotor.NewPropietary = Entity;
				
				AddSubgoal ( FollowPath_Goal.New( gameObject ) );
			}
			
			//Debug.Log ( ToString () + " Balon: " + balon.ToString() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ ProcessSubgoals();
				float distance = Helper.DistanceInPlaneXZ( gameObject, mBalonMotor.gameObject );
				if ( distance <= 1f ) {
					mBalonMotor.AttachTo( Entity, "Pie" );
					mData.BallPropietary = true;
					mData.ControlBalon = false;
					status = Status.Completed;
				}
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			mData.Aim = null;
		}
	}
	
}