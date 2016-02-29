using System;
using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	/// <summary>
	/// Lanza el balón hacia la portería
	/// Condición: Posesión del balón.
	/// </summary>
	public class Rematar_Goal : GoalComposite {
		
		private MatchManager mMatchManager;
		
		private SoccerData mData;
		private Animator mAnimator;
		private SoccerMotor mMotor;
		private GameObject mBalon;
		private BallMotor mBallMotor;
		private GameObject mPorteria;
		private Vector3 mPuntoImpacto;
		
		private bool mWaitingBall;
			
		public static new Rematar_Goal New( GameObject _owner ) {
			Rematar_Goal goal = Create<Rematar_Goal>(_owner);
			goal.Set();
			return goal;
		}
		
		protected void Set () {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = Entity.GetComponent<Animator>();
				mMotor = gameObject.GetComponent<SoccerMotor>();
				mBalon = GameObject.FindGameObjectWithTag ("Balon");
				mBallMotor = mBalon.GetComponent<BallMotor>();
				mPorteria = GameObject.FindGameObjectWithTag ("Porteria");
				
				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			}
		}
		
		private void UpdateInfoAnimator() {
			mAnimator.SetFloat( AnimatorID.Altura, mBallMotor.TargetPosition.y );
			
			float distancia = Helper.DistanceInPlaneXZ(gameObject, mBalon);
			mAnimator.SetFloat( AnimatorID.Distancia, distancia );
			
			mAnimator.SetFloat( AnimatorID.Tiempo, mBallMotor.TimeToTarget );
		}
		
		private IEnumerator UpdateState() {
			
			yield return StartCoroutine( Helper.WaitCondition( () => { 
				return (mBallMotor.NewPropietary == Entity); 
			} ) );

			mWaitingBall = true;
			mBallMotor.OnTargetPosition += OnTargetPosition;
			
			mAnimator.SetBool( AnimatorID.Rematar, true );
			mAnimator.SetInteger( AnimatorID.Tecnica, (int) mData.Tecnica );
			UpdateInfoAnimator ();
			
			/*
			Vector3 targetPosition = mBallMotor.TargetPosition;
			targetPosition.y = 0f;
			
			mAnimator.MatchTarget( targetPosition, Quaternion.identity, AvatarTarget.Root, 
					new MatchTargetWeightMask(Vector3.one, 0), 0.3f, 0.7f);
			*/
			
			Transform pieTransform = Helper.FindTransform( Entity.transform, /*"H_Balon_AA"*/ "Pie" );
			Vector3 desp = pieTransform.position - Entity.transform.position;
			mPuntoImpacto = mBallMotor.TargetPosition + desp;
			mPuntoImpacto.y = mBallMotor.TargetPosition.y;
			mBallMotor.InterpolateToPosition( mPuntoImpacto );
			
			// Asegurarnos de que se ha activado la animación y se indica que tenemos el balón
			yield return StartCoroutine( Helper.WaitCondition( () => {
				UpdateInfoAnimator ();
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Rematar ) );
			} ) );
			
			mAnimator.SetBool( AnimatorID.Rematar, false );
			
			mData.Aim = mPorteria;
			
			// Helper.Log( Entity, Helper.AnimationClipNames(mAnimator) );
			
			// Esperar a NO tener el balón
			yield return StartCoroutine( Helper.WaitCondition( () => {
				if ( mWaitingBall ) {
					Vector3 animDesp = transform.forward * mAnimator.GetFloat ( AnimatorID.AxisZ );
					// mPuntoImpacto = mBallMotor.TargetPosition + animDesp;
					// mPuntoImpacto = transform.position + animDesp;
					// mPuntoImpacto.y = mBallMotor.TargetPosition.y;
					Transform nodeTransform = Helper.FindTransform( Entity.transform, /*"H_Balon_AA"*/ "Pie" );
					mPuntoImpacto = nodeTransform.position + animDesp;
					mPuntoImpacto.y = mBallMotor.TargetPosition.y;
					mBallMotor.InterpolateToPosition( mPuntoImpacto );
					UpdateInfoAnimator ();
				}
				return !mWaitingBall;
			} ) );

			// Esperar a terminar la animacion
			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( !Helper.IsAnimationPlaying( mAnimator, AnimatorID.Rematar ) );
			} ) );
			
			if ( !mData.ActionSuccess ) {
				mData.EvaluateAction = SoccerData.ActionState.Fail;
				mData.Perdedor = true;
			}

			status = Status.Completed;			
		}
			
		void ApplyAction() {
			mBallMotor.ShowTrail( true );
				
			if ( mData.ActionSuccess ) {
				mBallMotor.NextImpulseToPosition( mPorteria.transform.position, mData.TiempoTrayectoria );				
				mBallMotor.Estado = BallMotor.EEstado.Chut;				
				mMatchManager.InteractiveActions.MsgBallKick( ActionType.REMATE );							
				// mBalon.transform.parent = null;
				// mBallMotor.ApplyImpulseToPosition ( mPorteria.transform.position, mData.TiempoTrayectoria );
				Entrenador entrenador = mData.dummyMirror.GetComponent<Entrenador>();
				entrenador.StartCoroutine( entrenador.SendMsgGol( mData.TiempoTrayectoria ) );
			}
			else {
				mBallMotor.NextImpulseToPosition( mPorteria.transform.position + Vector3.up * 2f, mData.TiempoTrayectoria );
				mMatchManager.FinDeJugada( false, true );
			}
		}
		
		void OnTargetPosition (object sender, EventArgs e) {
			if ( (mData.EvaluateAction == SoccerData.ActionState.None) || mMatchManager.AutomaticTap.Enabled ) {
				ApplyAction();
			}
			else {
				if ( mData.EvaluateAction == SoccerData.ActionState.Wait ) {
					// No hemos tenido tiempo suficiente para que termine el Input!					
					// Notificamos al QTEStandard que se ha producido un Fail
					mMatchManager.InteractiveActions.NotifyActionIncomplete();
					mMatchManager.FinDeJugada( false, true );
				}
			}
				
			mBallMotor.NewPropietary = null;
			mData.Chutar = false;
			mData.Rematar = false;
			mData.ActionContext = false;
			mData.BallPropietary = false;
				
			mWaitingBall = false;
			mBallMotor.OnTargetPosition -= OnTargetPosition;
		}
		
		public override void Activate () {
			status = Status.Active;
			mMotor.OrientationToAim = 0f;
			RemoveAllSubgoals();
			StartCoroutine ( UpdateState () );
			// Debug.Log ( "Activate: " + ToString () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			/*
			if ( isActive () ) {
				Status statusSubgoals = ProcessSubgoals();
			}
			*/
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			mData.Aim = null;
			if ( mWaitingBall ) {
				mBallMotor.OnTargetPosition -= OnTargetPosition;
			}
			
			mAnimator.SetBool( AnimatorID.Rematar, false );
		}
		
		void OnDrawGizmos () {
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere( mPuntoImpacto, 0.1f );
		}				
	}
	
}
