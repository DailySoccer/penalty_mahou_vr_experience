//#define RECOGE_PASE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class Entrenador : MonoBehaviour {
		
		public enum ETactica {
			Ninguna,
			Portero
		};
		public ETactica Tactica = ETactica.Ninguna;
		
		[HideInInspector] public GameObject Target;
		[HideInInspector] public GameObject ActionTarget;
		public int Dorsal;
		public float MaxSpeed = 10f;
		SoccerMotor mMotor = null;
		SoccerData mData;
		MatchManager mMatchManager;
		
		// Position y Rotation del dummy al inicio del juego
		Vector3 mPositionStart;
		Quaternion mRotationStart;
		
		AMTween[] mTweens = null;
		AMTween[] Tweens {
			get {
				if ( mTweens == null ) 
					mTweens = gameObject.GetComponentsInChildren<AMTween>();
				return mTweens;
			}
		}
		// float mActionTime = -1f;
		
		// float mMotorMaxSpeed = 0;
		
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			Assert.Test ( () => mMatchManager, "No existe MatchManager" );
			
			mPositionStart = gameObject.transform.position;
			mRotationStart = gameObject.transform.rotation;
			
			Dorsal = Helper.ParseNumber( gameObject.name );
			if ( Dorsal == 1 )
				Tactica = ETactica.Portero;
		}
		
		public void AttachTarget( GameObject target ) {
			string soccerName = "Soccer-" + gameObject.name;
			
			Target = target;
			
			Target.SetActive( true );
			Target.name = soccerName;

			mData = Target.GetComponentInChildren<SoccerData>();
			mData.dummyMirror = gameObject;
			mData.Bando = (gameObject.tag == "Local") ? SoccerData.EBando.Local : SoccerData.EBando.Visitante;
			
			mMotor = Target.GetComponentInChildren<SoccerMotor>();
			// mMotorMaxSpeed = mMotor.MaxSpeed;
			
			Target.transform.position = new Vector3(mPositionStart.x, 0.01f, mPositionStart.z);
			Target.transform.rotation = mRotationStart;
			
			mMatchManager.OnNewPlay += OnNewPlay;
		}

		public void StopOrders(bool inmediate) {
			if ( mTweens != null ) {
				foreach( AMTween tween in mTweens ) {
					if ( tween ) {
						if ( inmediate )
							DestroyImmediate( tween );
						else
							Destroy( tween );
					}
				}
				mTweens = null;
			}
		}

		public void DestroySoccer () {
			mMatchManager.OnNewPlay -= OnNewPlay;
			
			AnimationContext animationContext = Target.GetComponent<AnimationContext>();
			animationContext.Reset();
			
			SoccerData targetData = Target.GetComponentInChildren<SoccerData>();
			targetData.gameObject.SetActive( false );
			targetData.gameObject.transform.parent = null;
			Destroy ( targetData.gameObject );
			
			// Destroy( Target );
			Target = null;

			StopOrders(true);
		}
		
		public bool IsTargetNear( float distance ) {
			Vector3 dirToTarget = Helper.ZeroY( Target.transform.position - transform.position );
			return ( dirToTarget.sqrMagnitude < (distance*distance) );
		}

        public bool CanReach(Vector3 target, float time, out float diff) {
            Vector3 dirToTarget = Helper.ZeroY(target - transform.position);
            float distance = 5 * time; //(MaxSpeed * time);
            diff = dirToTarget.sqrMagnitude - (distance * distance);
            return diff < 0;
        }
		
		public bool IsMoving() {
			foreach( AMTween tween in Tweens ) {
				if ( tween && tween.isRunning && tween.type == "move" ) {
					return true;
				}
			}
			return mMotor.IsMoving();
		}
		
		bool IsTweenRunning() {
			bool isRunning = false;
			foreach( AMTween tween in Tweens ) {
				if ( tween.isRunning ) {
					isRunning = true;
					break;
				}
			}
			return isRunning;
		}
		
		public void SetIA( bool enabled ) {
			mData.IAOn = enabled;
            /*
			Tactica_Evaluator tactica = Target.GetComponentInChildren<Tactica_Evaluator>();
			tactica.enabled = enabled;
			*/

			PresionSobreContrario_Evaluator presion = Target.GetComponentInChildren<PresionSobreContrario_Evaluator>();
            if (presion)
                presion.enabled = enabled;

            DefensaEnZona_Evaluator defensa = Target.GetComponentInChildren<DefensaEnZona_Evaluator>();
			if ( defensa )
				defensa.enabled = enabled;
		}

		IEnumerator UpdateStateIA() {
			// Helper.Log ( gameObject, "IA Off" );
			SetIA( false );

			// Mientras haya algun Tween reproduciendose
			do {
				yield return new WaitForSeconds( 0.1f );
			} while ( IsTweenRunning() );

			// Ya no hay ningun Tween activo, Activamos la IA
			SetIA( true );
			// Helper.Log ( gameObject, "IA On" );
		}

		IEnumerator ResetAnimator() {
			Animator targetAnimator = Target.GetComponentInChildren<Animator>();
			targetAnimator.SetBool( "Reset", true );
			AnimatorID.AnimationReset( targetAnimator );
			
			yield return null;
			
			if ( !Helper.IsAnimationPlaying( targetAnimator, AnimatorID.Idle ) ) {
				Debug.LogWarning( "Reset failed" );
			}
			
			targetAnimator.SetBool( "Reset", false );
		}
	
		void OnNewPlay (object sender, EventArgs e) {
			// StartCoroutine( ResetAnimator() );
			
			// Apagamos la IA de los contrarios que tengan alguna accion en el AnimatorTimeLine
			if ( mData.Bando == SoccerData.EBando.Visitante ) {
				if ( Tweens.Length > 0 ) {
					StartCoroutine( UpdateStateIA () );
				}
				else {
					SetIA( true );
				}
			}
		}
		
		/*
		void FixedUpdate() {
			// Acelerar al futbolista para que se ajuste al dummy
			if ( mMotor && (MaxSpeed > mMotorMaxSpeed) ) {
				if ( mData.FollowingPath ) {
					float distance = Helper.DistanceInPlaneXZ( gameObject,  Target );
					if ( distance > (mMotor.MaxSpeed * Time.deltaTime * 10f) ) {
						if ( mMotor.MaxSpeed < MaxSpeed )
							mMotor.MaxSpeed += 1f;
						//Debug.Log( Target.name + ": SpeedUp: " + motor.MaxSpeed );
					}
					else if ( distance < (mMotor.MaxSpeed * Time.deltaTime * 9f) ) {
						if ( mMotor.MaxSpeed > mMotorMaxSpeed ) {
							mMotor.MaxSpeed -= 1f;
							//Debug.Log( Target.name + ": SpeedDown: " + motor.MaxSpeed );
						}
						else
							mMotor.MaxSpeed = mMotorMaxSpeed;
					}
				}
				else {
					if ( mMotor.MaxSpeed != mMotorMaxSpeed )
						mMotor.MaxSpeed = mMotorMaxSpeed;
				}
			}
			
			// EvaluarInteraccionJugador();
		}
		*/
		
		public Vector3 GetPositionAfter( float time ) {
			Vector3 positionAfter = gameObject.transform.position;
			
			float timeAfter = time;
			AMTween tweenRunning = null;
			
			foreach( AMTween tween in Tweens ) {
				if ( tween && tween.type == "move" ) {
					if ( tween.isRunning ) {
						tweenRunning = tween;
						break;
					}
				}
			}
			
			if ( tweenRunning != null ) {
				positionAfter = tweenRunning.getPositionAfter( timeAfter );
			}
			
			return positionAfter;
		}

		public Vector3 GetPositionAfterOnTrack( float time ) {
			Vector3 positionAfter = gameObject.transform.position;
			
			float timeAfter = time;
			AMTween tweenRunning = null;
			
			foreach( AMTween tween in Tweens ) {
				if ( tween && tween.type == "move" ) {
					if ( tween.isRunning ) {
						tweenRunning = tween;
					}
					else {
						// Si estamos reproduciendo un tween, nos interesamos también por el siguiente que vaya a activarse (para dar continuidad; sin "cortes" de movimiento)
						float tweenStart = tween.StartTime;
						if ( (Time.time < tweenStart) && ((Time.time + time) > tweenStart) ) {
							tweenRunning = tween;
							timeAfter = (Time.time + timeAfter) - tweenStart;
							break;
						}
					}
				}
			}
			
			if ( tweenRunning != null ) {
				positionAfter = tweenRunning.getPositionAfter( timeAfter );
			}
			
			return positionAfter;
		}
		
		public Vector3 GetPositionAfterAtDistance( float time, float distance ) {
			Vector3 positionAfter = gameObject.transform.position;
			
			float timeAfter = time;
			AMTween tweenRunning = null;
			
			foreach( AMTween tween in Tweens ) {
				if ( tween && tween.type == "move" ) {
					if ( tween.isRunning ) {
						tweenRunning = tween;
						break;
					}
				}
			}
			
			if ( tweenRunning != null ) {
				positionAfter = tweenRunning.getPositionAfterAtDistance( timeAfter, distance );
			}
			
			return positionAfter;
		}

		public Vector3 GetPositionAfterAtDistanceOnTrack( float time, float distance ) {
			Vector3 positionAfter = gameObject.transform.position;
			
			float timeAfter = time;
			AMTween tweenRunning = null;
			
			foreach( AMTween tween in Tweens ) {
				if ( tween && tween.type == "move" ) {
					if ( tween.isRunning ) {
						tweenRunning = tween;
					}
					// Si estamos reproduciendo un tween, nos interesamos también por el siguiente que vaya a activarse (para dar continuidad; sin "cortes" de movimiento)
					else {
						float tweenStart = tween.StartTime;
						if ( (Time.time < tweenStart) && ((Time.time + time) > tweenStart) ) {
							tweenRunning = tween;
							timeAfter = (Time.time + timeAfter) - tweenStart;
							break;
						}
					}
				}
			}
			
			if ( tweenRunning != null ) {
				positionAfter = tweenRunning.getPositionAfterAtDistance( timeAfter, distance );
			}
			
			return positionAfter;
		}
		
		public IEnumerator SendMsgBalon(float time) {
			yield return new WaitForSeconds( time );
#if RECOGE_PASE
            Balon ();
#endif
		}
		
		public IEnumerator SendMsgAccion(float time) {
			yield return new WaitForSeconds( time );
			// Balon ();
			if ( mData ) 
				mData.ActionContext = true;
		}
		
		public void NotifyToGoalKeeper() {
			Profiler.BeginSample( "MsgGol" );

			Entrenador entrenadorPortero = mMatchManager.Customization.DefenderKeeper;
			if ( entrenadorPortero != null ) {
				SoccerData porteroData = entrenadorPortero.Target.GetComponentInChildren<SoccerData>();
	 			if ( porteroData != null ) {
					porteroData.NewOrder = true;
				}
			}

			Profiler.EndSample();
		}
		
		public IEnumerator SendMsgGol(float time) {
			NotifyToGoalKeeper();
			yield return new WaitForSeconds( time );
		}
		
		/*
		 * INSTRUCCIONES DE JUEGO 
		 */
		public void _NOP_ () {
		}
		
		public void Balon() {
            mData.MsgControlBalon();
		}
		
		public void Espera( float tiempo ) {
			mData.MsgEsperar( tiempo );
		}

//        public void Pase( GameObject aQuien=null, float tiempo=1, float hastaDestino=1 ) {
        public void Pase() {

            mData.MsgPasar( null, 1, 1 );
		}
		
		public void Centro( GameObject aQuien, float tiempo, float hastaDestino, float altura ) {
			mData.MsgCentrar( aQuien, tiempo, hastaDestino, altura );
		}	
		
		public void Chut( float hastaDestino ) {
			mData.MsgChutar( hastaDestino );
		}

		public void Remate( float hastaDestino ) {
			mData.MsgRematar( hastaDestino, ETecnica.Pie );
		}

		public void RematePie( float hastaDestino ) {
			mData.MsgRematar( hastaDestino, ETecnica.Pie );
		}
		
		public void RemateCabeza( float hastaDestino ) {
			mData.MsgRematar( hastaDestino, ETecnica.Cabeza );
		}
		
		public void Regatear() {
			mData.MsgRegatear();
		}

		public void RegatearA( GameObject aQuien ) {
			mData.MsgRegatearA( aQuien );
		}
		
		public void Entrar() {
			mData.MsgEntrar();
		}
		
		public void Orientar( bool activar ) {
			mData.MsgOrientar( activar );
		}

		public void Velocidad( float multiplicador, float tiempo ) {
			mData.MsgVelocidad( multiplicador, tiempo );
		}
		
		public void SetMaxSpeed(float speed) {
			MaxSpeed = speed;
		}

        public void Chaseball()
        {
            mData.Chaseball = true;
            mData.NewOrder = true;
        }

    }
	
}
