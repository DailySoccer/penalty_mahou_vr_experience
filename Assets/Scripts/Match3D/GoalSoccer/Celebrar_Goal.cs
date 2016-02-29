using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {

	public class Celebrar_Goal : GoalComposite {

		private MatchManager mMatchManager;
		private SoccerData mData;
		private Animator mAnimator;
		private SoccerMotor mMotor;
		
		public static new Celebrar_Goal New( GameObject _owner ) {
			Celebrar_Goal goal = Create<Celebrar_Goal>(_owner);
			goal.Set();
			return goal;
		}
		
		protected void Set() {
			if ( InitializeGlobal ) {
				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = Entity.GetComponent<Animator>();
				mMotor = Entity.GetComponentInChildren<SoccerMotor>();
			}
		}

		private IEnumerator UpdateState() {
			// Esperar a terminar cualquier posible Chut o Remate
			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Chut ) && !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Rematar ) );
			} ) );

			mData.Aim = null;
			mMotor.ForcedZeroY = false;

			// Si tenemos el balón, deslinkarlo antes de dar botes de alegría
			if ( mData.BallPropietary ) {
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				BallMotor ballMotor = balon.GetComponent<BallMotor>();
				ballMotor.UnAttach();
			}

			mAnimator.SetBool( AnimatorID.MainCharacter, (mMatchManager.InteractiveActions.MainCharacter == Entity) && (mData.ActionSuccess) );
			// mAnimator.SetBool( AnimatorID.FinalPartido, mMatchManager.IsLastPlay ); // CACA
			mAnimator.SetInteger( AnimatorID.Celebracion, UnityEngine.Random.Range(1, 4) );

			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Celebracion ) );
			} ) );
			
			mAnimator.SetInteger( AnimatorID.Celebracion, 0 );

			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Celebracion ) );
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
