using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PasarBalon_Goal : GoalComposite {
		
		private MatchManager mMatchManager;
		private GameObject  mTarget;
		private float		mTimeAfter;
		private Vector3 	mTargetPosition;
		private SoccerData mData;
		private Animator mAnimator;
		private AnimationContext mAnimationContext;
		private GameObject mBalon;
		private BallMotor mBallMotor;
		private Transform mPieTransform;
		private bool mTaskCompleted = false;
		
		public static PasarBalon_Goal New( GameObject _owner, GameObject _target, float _timeAfter ) {
			PasarBalon_Goal goal = Create<PasarBalon_Goal>(_owner);
			goal.Set( _target, _timeAfter );
			return goal;
		}
		
		protected void Set( GameObject _target, float _timeAfter ) {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = Entity.GetComponent<Animator>();
				mAnimationContext = Entity.GetComponent<AnimationContext>();
				mBalon = GameObject.FindGameObjectWithTag ("Balon");
				mBallMotor = mBalon.GetComponent<BallMotor>();
				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
				mPieTransform = Helper.FindTransform( Entity.transform, /*"H_Balon_AA"*/ "Pie" );
			}
			
			mTarget = _target;
			mTargetPosition = Vector3.zero;
			mTimeAfter = _timeAfter;
		}	
	
		private void UpdateInfoAnimator() {
			mAnimator.SetFloat( AnimatorID.Altura, mBallMotor.TargetPosition.y );
			
			float timeToTarget = mBallMotor.TimeToTarget;
			if ( !mData.BallPropietary ) {
				float dist = Helper.ZeroY( mBallMotor.TargetPosition - mPieTransform.position ).magnitude;
				float speed = mAnimator.GetFloat ( AnimatorID.Speed );
				float tiempoHastaBalon = speed > 0f ? dist / speed : 1f;
				if ( tiempoHastaBalon > timeToTarget ) {
					// Helper.Log ( Entity, "toTarget: " + timeToTarget + " toBall: " + tiempoHastaBalon );
					timeToTarget = tiempoHastaBalon;
				}
			}
			mAnimator.SetFloat( AnimatorID.Tiempo, timeToTarget );
            mAnimator.SetFloat(AnimatorID.DireccionBalon, 0);
            float distancia = 20.0f;
			mAnimator.SetFloat( AnimatorID.Distancia, distancia );					
			float distanciaBalon = mData.BallPropietary ? 0f : Helper.ZeroY( mBallMotor.transform.position - mPieTransform.position ).magnitude;
			mAnimator.SetFloat( AnimatorID.DistanciaBalon, distanciaBalon );
			// AnimatorID.Log( Entity );
		}
		
		private void SetupAnimationContext() {
			if ( mData.ActionSuccess ) {
                mAnimationContext.Direccion = 0;// AngleToTarget();
			}
			else {
				Quaternion rot = Quaternion.FromToRotation( transform.forward, mData.ActionDirection );
				mAnimationContext.Direccion = rot.eulerAngles.y;
			}

            float distancia = 20.0f;
			mAnimationContext.Distancia = distancia;			
			mAnimationContext.Tiempo = mData.TiempoTrayectoria;			
			mAnimationContext.SetContext( ActionType.PASE );
		}
		
		private IEnumerator UpdateState() {
			// float timeIni = Time.time;
			
			yield return StartCoroutine( Helper.WaitCondition( () => { 
				return (mBallMotor.NewPropietary == Entity); 
			} ) );			
			
			mAnimator.SetBool( AnimatorID.Pasar, true );
			if ( !mData.BallPropietary ) 
				mAnimator.SetBool( AnimatorID.Pared, true );
			UpdateInfoAnimator();
			SetupAnimationContext();
			
			mData.Aim = mTarget;
			
			// Asegurarnos de que se ha activado la animación y se indica que tenemos el balón
			yield return StartCoroutine( Helper.WaitCondition( () => { 
				UpdateInfoAnimator ();
				SetupAnimationContext();
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Pasar ) );
			} ) );			
			
			mAnimator.SetBool( AnimatorID.Pasar, false );
			mAnimator.SetBool( AnimatorID.Pared, false );

            mMatchManager.InteractiveActions.EvaluateAction();
            // Esperar a NO tener el balón
            yield return StartCoroutine( Helper.WaitCondition( () => {
                if ( mBalon.transform.parent != null && mAnimator.GetFloat( AnimatorID.Unlink ) > 0 ) {
					mBalon.transform.parent = null;
				}
				if ( !mData.BallPropietary ) {
					float distanciaBalon = Helper.ZeroY( mBallMotor.transform.position - mPieTransform.position ).magnitude;
					if ( (distanciaBalon < 0.5f) || !mBallMotor.MovingToPosition ) {
						mBallMotor.AttachTo( Entity, /*"H_Balon_AA"*/ "Pie" );
						mData.BallPropietary = true;
						mData.ControlBalon = false;
					}
				}
                return mAnimator.GetFloat(AnimatorID.Balon)>0.001f;
			} ) );
            mData.Aim = mBalon;

            var res = mMatchManager.InteractiveActions.CurrentAction.Result;
            
            if (res) mTargetPosition = DirectionGesture.mLastTarget;
            else mTargetPosition = transform.position + transform.forward * 10f;

            // Miramos que jugador va a recoger la pelota.
            float  minAttackerDiff = float.MaxValue;
            Entrenador minAttacker = null;            
            foreach (var player in mMatchManager.Customization.Attackers) {
                float d;
                if (this.transform.parent.gameObject != player.Target && player.CanReach(mTargetPosition, mData.TiempoTrayectoria * 1.5f, out d)) {
                    if(d < minAttackerDiff) {
                        minAttackerDiff = d;
                        minAttacker = player;
                    }
                }
            }

            float minDefenderDiff = float.MaxValue;
            Entrenador minDefender = null;
            foreach (var player in mMatchManager.Customization.Defenders) {
                float d;
                if (player.CanReach(mTargetPosition, mData.TiempoTrayectoria * 1.5f, out d))
                    if (d < minDefenderDiff) {
                        minDefenderDiff = d;
                        minDefender = player;
                    }
            }
            Entrenador bestReceiver = null;
            Entrenador bestPresser = null;

            bool goodLuck = Random.value > 0.5f;

            if ( minAttackerDiff < minDefenderDiff || goodLuck) {
                bestReceiver = minAttacker;
                bestPresser = minDefender;
            }
            else {
                bestReceiver = minDefender;
                bestPresser = minAttacker;
            }

            if (bestReceiver != null)
            {
                Debug.Log(">>> Se manda al jugador " + bestReceiver.name + " a por la pelota.");
                bestReceiver.Chaseball();
            }
            else {
                Debug.Log(">>> Nadie coje la pelota... " );
                mMatchManager.FinDeJugada(false, false); // Termina la jugada de pase.
            }

//            if (bestPresser != null)
//                bestPresser.Chaseball();

			mMatchManager.InteractiveActions.MsgBallKick( ActionType.PASE );
	
			mBalon.transform.parent = null;

			mBallMotor.ApplyImpulseToPosition ( mTargetPosition + new Vector3(0, mData.Altura, 0), mData.TiempoTrayectoria ); 
			//mBallMotor.NewPropietary = bestReceiver!=null?bestReceiver.Target:null;
			mBallMotor.Estado = BallMotor.EEstado.Pase;

			// Esperar a terminar la animacion
			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Pasar ) );
			} ) );

            // Marca el fin de la juagda
            // mMatchManager.FinDeJugada(false); // Lo quito porque sera el receptor el que termine la jugada...
            mData.PasarBalon = false;
			mData.CentrarBalon = false;
			mData.BallPropietary = false;
			mData.ControlBalon = false;
		
			// status = Status.Completed;
			mTaskCompleted = true;
		}

        public override void Activate () {
			status = Status.Active;
			mTaskCompleted = false;
			RemoveAllSubgoals();
			StartCoroutine ( UpdateState () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
//			if ( mTaskCompleted ) status = Status.Completed;
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
		}
	}
	
}
