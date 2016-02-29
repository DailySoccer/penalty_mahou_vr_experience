using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Regatear_Goal : GoalComposite {

		private MatchManager mMatchManager;

		private SoccerData mData;
		private Animator mAnimator;
        private GameObject mBalon;
        private BallMotor mBalonMotor;

        public static new Regatear_Goal New( GameObject _owner ) {
			Regatear_Goal goal = Create<Regatear_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = Entity.GetComponent<Animator>();
				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
                mBalon = GameObject.FindGameObjectWithTag("Balon");
                mBalonMotor = mBalon.GetComponent<BallMotor>();
            }
        }

		private IEnumerator UpdateState() {
			// Estamos realizando una accion coordinada?
			if ( mData.CoordinatedTarget ) {
				SoccerData dataTarget = mData.CoordinatedTarget.GetComponentInChildren<SoccerData>();
				if ( dataTarget ) {
					dataTarget.Entrada = true;
					dataTarget.CoordinatedTime = 0f;
				}
			}
            // Hacer en la maquina de estados la opcion de fallar el regate a tropezar
			mAnimator.SetBool( AnimatorID.Regatear, true );
				
			GameObject target = mData.CoordinatedTarget;
			if ( target ) {
				Vector3 dirToTarget = Helper.ZeroY ( target.transform.position - transform.position );
				Quaternion rot = Quaternion.FromToRotation( transform.forward, dirToTarget );
				mAnimator.SetFloat( AnimatorID.DireccionBalon, rot.eulerAngles.y );
				float distancia = Helper.DistanceInPlaneXZ( gameObject, target );
				mAnimator.SetFloat ( AnimatorID.Distancia, distancia );
			}

            mMatchManager.InteractiveActions.EvaluateAction();
            // Esperar a comenzar la animacion
            yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Regatear ) );
			} ) );

            mAnimator.SetBool( AnimatorID.Regatear, false );
            // Esperar a terminar la evalaucion del gesto.
            yield return StartCoroutine(Helper.WaitCondition(() => {
                return mMatchManager.InteractiveActions.CurrentAction.State == TweenAction.EState.None;
            } ) );
            var regate = mMatchManager.InteractiveActions.CurrentAction.Result;
            if (!regate) {
                mAnimator.SetBool(AnimatorID.Regatear, false);
                mAnimator.SetBool(AnimatorID.Tropezar, true);
                yield return StartCoroutine(Helper.WaitCondition(() => {
                    return (Helper.IsAnimationPlaying(mAnimator, AnimatorID.Tropezar));
                }));
                mAnimator.SetBool(AnimatorID.Tropezar, false);

                mBalonMotor.UnAttach();
                GameObject balon = GameObject.FindGameObjectWithTag("Balon");
                Rigidbody rigidBody = balon.AddComponent<Rigidbody>();
                rigidBody.AddForce(Vector3.up * 0.3f + (Entity.transform.right + Entity.transform.forward) * 6f, ForceMode.Impulse);
/*
                yield return StartCoroutine(Helper.WaitCondition(() => {
                    return (!Helper.IsAnimationPlaying(mAnimator, AnimatorID.Tropezar));
                }));
                Debug.Log("!!!!! TERMINAD ANIMACION DE TROPEZAR!!!");
                mAnimator.SetBool(AnimatorID.Tropezar, false);
*/
            }
/*
            else
            {
                yield return StartCoroutine(Helper.WaitCondition(() => {
                    return (!Helper.IsAnimationPlaying(mAnimator, AnimatorID.Regatear));
                }));
                mAnimator.SetBool(AnimatorID.Regatear, false);

            }
*/
            mData.Regatear = false;
            status = Status.Completed;
            mMatchManager.FinDeJugada( regate, false);
        }
		
		public override void Activate () {
			status = Status.Active;			
			RemoveAllSubgoals();			
			StartCoroutine ( UpdateState () );			
		}
		
		public override Status Process () {
			ActivateIfInactive();			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ // ProcessSubgoals();				
				ReactivateIfFailed();
			}			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();			
			mData.CoordinatedTarget = null;
		}
	}
	
}

