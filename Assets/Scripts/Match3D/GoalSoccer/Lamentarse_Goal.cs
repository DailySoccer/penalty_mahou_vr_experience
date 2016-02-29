using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {

	public class Lamentarse_Goal : GoalComposite {
		
		private SoccerData mData;
		private Animator mAnimator;
		private SoccerMotor mMotor;

		public static new Lamentarse_Goal New( GameObject _owner ) {
			Lamentarse_Goal goal = Create<Lamentarse_Goal>(_owner);
			goal.Set();
			return goal;
		}
		
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = Entity.GetComponent<Animator>();
				mMotor = Entity.GetComponentInChildren<SoccerMotor>();
			}
		}

		private IEnumerator UpdateState() {
			mData.Aim = null;
			mMotor.ForcedZeroY = false;

			// Si tenemos el balón, deslinkarlo antes de ponernos a llorar
			if ( mData.BallPropietary ) {
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				BallMotor ballMotor = balon.GetComponent<BallMotor>();
				ballMotor.UnAttach();
			}

			mAnimator.SetInteger( AnimatorID.Derrota, UnityEngine.Random.Range(1, 3) );
			
			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Derrota ) );
			} ) );
			
			mAnimator.SetInteger( AnimatorID.Derrota, 0 );

			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Derrota ) );
			} ) );
		}

		public override void Activate () {
			status = Status.Active;

			RemoveAllSubgoals();

			StartCoroutine ( UpdateState () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ ProcessSubgoals();
				
				// status = Status.Completed;
				
				ReactivateIfFailed();
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}

	}
}
