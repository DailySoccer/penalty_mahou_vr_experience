using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FootballStar.Match3D {
	
	public class SoccerMotor : MonoBehaviour {
		
		public Vector3 		Velocity;
		public float 		Rotation = 0;
		public SteeringInfo Steering = new SteeringInfo();
		public float		MaxSpeed = 5f;
		public float		MaxRotation = 90f;
		public bool			ForcedZeroY = false;
		public float		OrientationToAim = 0f;
						
		[HideInInspector] public Vector3 CurrentVelocity;
		[HideInInspector] public float	CurrentSpeed = 0f;
		
		private GameObject	mEntity;
		private SoccerData  mData;
		private	ISteer[]	mSteers;
	
		private Vector3		mOldDummyPosition;
		private Vector3		mOldPosition;
		private float		mOldTime;
		private float 		mDummySpeed;
		
		protected Animator mAnimator;
		protected HeadLookController mHeadLook;
		protected Locomotion mLocomotion;
		protected Entrenador mEntrenador;
		protected BallMotor mBalonMotor;
		
		private Renderer mRendererMesh;
		
		void Start () {
			mEntity	= transform.parent.gameObject;
			mAnimator = transform.parent.GetComponent<Animator>();
			mHeadLook = transform.parent.GetComponent<HeadLookController>();
			mLocomotion = new Locomotion(mAnimator);
			
			GameObject balon = GameObject.FindWithTag( "Balon" );
			mBalonMotor = balon.GetComponentInChildren<BallMotor>();
			
			mRendererMesh = mEntity.GetComponentInChildren<Renderer>();
			
			/*
			Transform[] allChildren = GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) {
			    Debug.Log ( child.name );
			}
			*/
			
			mData = gameObject.GetComponentInChildren<SoccerData>();
			mEntrenador = mData.dummyMirror.GetComponent<Entrenador>();
			
			mSteers = gameObject.GetComponentsInChildren<ISteer>();
			foreach( ISteer steer in mSteers ) {
				steer.enabled = false;
			}
			
			mOldPosition = mData.dummyMirror.transform.position;
			mOldTime = Time.time;
			
			mDummySpeed = 0f;
			mDummySpeed.ToString();	// <---- Remove WARNING!!
			
			Trigger trigger = mEntity.GetComponent<Trigger>();
			trigger.OnEnter += OnMatchTriggerEnter;
			trigger.OnExit += OnMatchTriggerExit;
			
			if ( mEntity.name != "Soccer-Visitante1" && mEntity.name.Contains("Visitante") ) {
				OrientationToAim = 10f;
			}
			
			if ( (mEntrenador.Tactica == Entrenador.ETactica.Portero) && (mData.Bando == SoccerData.EBando.Visitante) ) {
				mAnimator.SetBool( AnimatorID.Alerta, true );
			}
			
			// StartCoroutine( "UpdateSpeed" );
			// StartCoroutine( UpdateActionTarget() );
			StartCoroutine ( UpdateAim () );
			StartCoroutine ( UpdateMovement() );
			
			/*
			if ( mEntity.name == "Soccer-Local2" )
				StartCoroutine( Helper.LogPlayingAnimations( mEntity ) );
			*/
		}
		
		void OnDestroy () {
			Trigger trigger = mEntity.GetComponent<Trigger>();
			trigger.OnEnter -= OnMatchTriggerEnter;
			trigger.OnExit -= OnMatchTriggerExit;
		}
		
		
		/*
	    void OnAnimatorMove()
	    {
	        CurrentVelocity = animator.deltaPosition / Time.deltaTime;
			transform.rotation = animator.rootRotation;
	    }
	    */
		
		IEnumerator UpdateActionTarget () {
			/*
			GameObject actionTarget = null;

			if ( mData.Bando == SoccerData.EBando.Local ) {
				Entrenador entrenador = mData.dummyMirror.GetComponentInChildren<Entrenador>();
				if ( entrenador ) {
					GameObject balon = GameObject.FindWithTag( "Balon" );
					BallMotor balonMotor = balon.GetComponentInChildren<BallMotor>();
						
					while ( true ) {
						actionTarget = null;
						if ( mData.BallPropietary || balonMotor.NewPropietary == gameObject ) {
							GameObject aQuien = entrenador.NextTweenEventParameter<GameObject>( "Pase", 0, 1f );
							if ( aQuien != null ) {
								Entrenador entrenadorTarget = aQuien.GetComponentInChildren<Entrenador>();
								if ( entrenadorTarget ) {
									actionTarget = entrenadorTarget.Target;
								}
							}
						}
						yield return new WaitForSeconds(0.5f);
					}
				}
			}
			*/
			yield return null;
		}
		
		IEnumerator UpdateSpeed () {
			while ( true ) {
				float delta = Time.time - mOldTime;
				if ( delta > 0.001f ) {
					Vector3 dummyPosition = mData.dummyMirror.transform.position;
					mDummySpeed = (dummyPosition - mOldDummyPosition).magnitude / delta;
						
					Vector3 position = mEntity.transform.position;
					CurrentVelocity = Velocity.normalized * (position - mOldPosition).magnitude / delta;
					
					mOldDummyPosition = dummyPosition;
					mOldPosition = position;
					mOldTime = Time.time;
				}
				
				yield return new WaitForSeconds(0.1f);
			}
		}
		
		void UpdateSteering () {
			Steering.linear = Vector3.zero;
			
			foreach( ISteer steer in mSteers ) {
				if ( steer.enabled ) {
					SteeringInfo newSteering = steer.getSteering();
					Steering.linear += newSteering.linear;
					Steering.angular += newSteering.angular;
				}
			}
		}
		
		int RelevanceInThePlay() {
			/*
			float sqrDist = Helper.ZeroY (mBalonMotor.transform.position - transform.position).sqrMagnitude;
			int relevance = (int) (sqrDist / (10f*10f));
			return relevance;
			*/
			return 0;
		}
		
		IEnumerator UpdateAim () {
			while ( true ) {
				GameObject currentAim = mData.Aim;
				
				if ( mEntrenador.ActionTarget != null )
					currentAim = mEntrenador.ActionTarget;
				
				if ( currentAim ) {
					Vector3 dir = Helper.ZeroY(currentAim.transform.position - mEntity.transform.position).normalized;
					Quaternion aimAngleRad = Quaternion.FromToRotation( Velocity.normalized, dir );
					float aimAngle = aimAngleRad.eulerAngles.y;
					if ( aimAngle < 0 ) aimAngle += 360f;
					
					mAnimator.SetFloat( AnimatorID.AimAngle, aimAngle );
					
					yield return null;
				}
				
				if ( mHeadLook ) {
					if ( currentAim && mRendererMesh.isVisible ) {
						mHeadLook.enabled = true;
						Vector3 aimPosition = currentAim.transform.position;
						if ( aimPosition.y < 1.4f ) {
							aimPosition.y = 1.4f;
						}
						// Si el target esta demasiado cerca, lo alejamos un poco
						float sqrDist = Helper.ZeroY( aimPosition - mEntity.transform.position ).sqrMagnitude;
						if ( sqrDist < (1f*1f) )
							aimPosition += mEntity.transform.forward;
						
						mHeadLook.target = aimPosition;
					}
					else {
					 	mHeadLook.enabled = false;
					}
				}
				
				// Damos prioridad a actualizar a los futbolistas mas importantes en la jugada (esperan menos)
				int relevance = RelevanceInThePlay();
				do {
					--relevance;
					yield return null;
				} while ( relevance > 0 );
			}
		}
		
		public bool IsMoving () {
			return Velocity.sqrMagnitude > 0.1f;
		}
		
		float SignedAngle( Vector3 a, Vector3 b ){
		  var angle = Vector3.Angle(a, b); // calculate angle
		  // assume the sign of the cross product's Y component:
		  return angle * Mathf.Sign(Vector3.Cross(a, b).y);
		}
		
		void UpdateOrientation() {
			if ( mData.Orientar && (Velocity.sqrMagnitude > 0.01f) ) {
				Vector3 positionTarget = mBalonMotor.transform.position;
				Vector3 dirToTarget = Helper.ZeroY(positionTarget - mEntity.transform.position);
				float angleToDir = SignedAngle( Velocity, dirToTarget );
				
				/*
				 * Direcciones de Movimiento:
				 * 1: Front / 2: Back / 3: Left / 4: Right
				 * */
				int direction = 0;
				
				if ( angleToDir > 0f ) {
					if 		( angleToDir < 45f ) 	direction = 1;
					else if ( angleToDir < 135f ) 	direction = 3;
					else direction = 2;
				}
				else {
					if 		( angleToDir > -45f ) 	direction = 1;
					else if ( angleToDir > -135f ) 	direction = 4;
					else direction = 2;
				}
				
				mAnimator.SetInteger ( AnimatorID.MovingDirection, direction );
			}
		}
		
		IEnumerator UpdateMovement() {
			while ( true ) {
				
				UpdateSteering();
				
				// Actualizar la velocidad
				Velocity = Steering.linear;
				
				UpdateOrientation();
				
				if ( Velocity.sqrMagnitude > (MaxSpeed * MaxSpeed * mData.MulSpeed * mData.MulSpeed) ) {
					Velocity.Normalize();
					Velocity *= MaxSpeed * mData.MulSpeed;
				}
				
				float speed = Velocity.magnitude;
				if ( speed < 0.5f ) {
					float angle = 0f;
					
					if ( (OrientationToAim > 0f) && mData.Aim ) {
						Vector3 aimDir = mData.Aim.transform.position - mEntity.transform.position;
						Vector3 velocity = Quaternion.Inverse(mEntity.transform.rotation) * aimDir.normalized;
						angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
						
						if ( Mathf.Abs(angle) < OrientationToAim ) {
							angle = 0f;
						}
					}
					
					mLocomotion.Do(0, angle);
				}
				else {
					Vector3 velocity = Quaternion.Inverse(mEntity.transform.rotation) * Velocity;
		
					float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
					
					mLocomotion.Do(speed, angle);
				}
				
				if ( speed > 0.1f ) {
					bool orientByAnimation = false;
					float maxDegrees = 360f * Time.deltaTime;
					
					/*
					if ( animator.IsInTransition(0) ) {
						AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(0);
						orientByAnimation = transition.IsUserName( "TurnOnSpot" );
					}
					else {
						AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
						orientByAnimation = state.IsTag( "TurnOnSpot" );
					}
					*/
					
					AnimatorStateInfo state = mAnimator.GetCurrentAnimatorStateInfo(0);
					bool inTransition = mAnimator.IsInTransition(0);
					int stateTagHash = state.tagHash;
					
					bool moving = false;
					bool moveFront = false;
					bool moveBack = false;
					bool moveLeft = false;
					bool moveRight = false;
					bool entrada = false;
					
					int transitionUserNameHash = 0;
					if ( inTransition ) {
						AnimatorTransitionInfo transitionInfo = mAnimator.GetAnimatorTransitionInfo(0);
						transitionUserNameHash = transitionInfo.userNameHash;
					}
					
					moveFront = !inTransition && (stateTagHash == AnimatorID.Front);
					if ( !moveFront && inTransition ) {
						moveFront = (transitionUserNameHash == AnimatorID.Front);
					}
					moving = moveFront;
					
					if ( !moving ) {
						moveBack = !inTransition && (stateTagHash == AnimatorID.Back);
						if ( !moveBack && inTransition ) {
							moveBack = (transitionUserNameHash == AnimatorID.Back);
						}
						
						maxDegrees = 360f * Time.deltaTime;
						moving = moveBack;
					}
	
					if ( !moving ) {
						moveLeft = !inTransition && (stateTagHash == AnimatorID.MoveLeft);
						if ( !moveLeft && inTransition ) {
							moveLeft = (transitionUserNameHash == AnimatorID.MoveLeft);
						}
						
						maxDegrees = 360f * Time.deltaTime;
						moving = moveLeft;
					}
					
					if ( !moving ) {
						moveRight = !inTransition && (stateTagHash == AnimatorID.MoveRight);
						if ( !moveRight && inTransition ) {
							moveRight = (transitionUserNameHash == AnimatorID.MoveRight);
						}
						
						maxDegrees = 360f * Time.deltaTime;
						moving = moveRight;
					}
					
					if ( !moving ) {
						entrada = Helper.IsAnimationPlaying( mAnimator, AnimatorID.Entrada );
						moving = entrada;
						maxDegrees = 360f * Time.deltaTime;
					}
					
					orientByAnimation = !moving;
					
					if ( !orientByAnimation ) {
						if ( entrada ) {
							Vector3 dirToIncercept = mBalonMotor.transform.position - mEntity.transform.position;
							mEntity.transform.rotation = Quaternion.RotateTowards( mEntity.transform.rotation, Quaternion.LookRotation(dirToIncercept), maxDegrees );
						}
						else
							mEntity.transform.rotation = Quaternion.RotateTowards( mEntity.transform.rotation, Quaternion.LookRotation(Velocity), maxDegrees );
					}
				}
				
				// Los futbolistas controlados por la IA (no siguen un path),
				//  se actualizan con menos frecuencia
				if ( mData.IAOn ) {
					// Damos prioridad a actualizar a los futbolistas mas importantes en la jugada (esperan menos)
					int relevance = RelevanceInThePlay();
					do {
						relevance -= 5;
						yield return null;
					} while ( relevance > 0 );
						
					yield return null;
				}
				
				yield return null;
			}
		}
		
		void Update () {
			if ( Time.deltaTime == 0f )
				return;
			
			// Que el gameObject no se hunda a causa de la animación
			//  Aprovechamos las transiciones de la animación para "resetear" la altura
			if ( ForcedZeroY && (mEntity.transform.position.y > 0.01f) && mAnimator.IsInTransition(0) ) {
				Vector3 position = transform.parent.position;
				position.y = Mathf.Lerp( position.y, 0.01f, 0.20f );
				mEntity.transform.position = position;
			}
	
			
			// UpdateAim ();
		}
		
		void OnMatchTriggerEnter( object sender, EventTriggerArgs args ) {
			Trigger other = args.Trigger;
			
			if ( other.tag == "Balon" ) {
				// Debug.Log ( gameObject.name + ": enter" );
				mData.BallNear = true;
			}
			
			SoccerData otherData = other.GetComponentInChildren<SoccerData>();
			if ( otherData != null ) {
				mData.AddSoccerNear ( otherData );
			}
			/*
			if ( gameObject.name == "Local9" ) {
				string texto = "Add Near: Aliados: ";
				foreach( SoccerData m in mData.AliadoNear ) {
					texto += m.transform.parent.gameObject.name + ", ";
				}
				texto += "Enemigos: ";
				foreach( SoccerData m in mData.EnemigoNear ) {
					texto += m.transform.parent.gameObject.name + ", ";
				}
				Helper.Log ( mEntity, texto );
			}
			*/
		}
		
		void OnMatchTriggerExit( object sender, EventTriggerArgs args ) {
			Trigger other = args.Trigger;
			
			if ( other.tag == "Balon" ) {
				// Debug.Log ( gameObject.name + ": exit" );
				mData.BallNear = false;
			}
			
			SoccerData otherData = other.GetComponentInChildren<SoccerData>();
			if ( otherData != null ) {
				mData.RemoveSoccerNear( otherData );
			}
			/*
			if ( gameObject.name == "Local9" ) {
				string texto = "Remove Near: ";
				foreach( SoccerMotor m in data.SoccerNear ) {
					texto += m.gameObject.name + ", ";
				}
				Debug.Log ( texto );
			}
			*/
		}		
		
		void OnDrawGizmos () {
			Gizmos.DrawLine( transform.position + Vector3.up, transform.position + Velocity + Vector3.up );
			/*
			if ( Steers != null ) {
				foreach( ISteer steer in Steers ) {
					if ( steer.enabled ) {
						SteeringInfo newSteering = steer.getSteering();
						Steering.linear += newSteering.linear;
						Gizmos.DrawLine( transform.position + Vector3.up, transform.position + newSteering.linear + Vector3.up );
					}
				}
			}
			*/
		}			
	}

}
