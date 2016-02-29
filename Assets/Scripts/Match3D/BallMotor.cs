using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;

namespace FootballStar.Match3D {

	public class BallMotor : MonoBehaviour {
		
		public enum EEstado {
			Ninguno,
			Propietario,
			Pase,
			Chut
		};
		public EEstado Estado = EEstado.Ninguno;

        public Vector3 Impulse { get; set; }
        public Vector3 LastImpulse { get; set; }

        public GameObject NewPropietary;
		public float TimeToTarget = 0f;
		public Vector3 TargetPosition;
        public Vector3 ReactionVector = Vector3.one;

        Vector3 lastPosition;
        Vector3 lastImpulse;

        public bool MovingToPosition = false;
		
		float interpolateTime = 1f;
		float mRadio;
		TrailRenderer mTrailRenderer;
		// Rigidbody mRigidBody;

		public event EventHandler OnTargetPosition;

		void Start () {
			Impulse = Vector3.zero;
            ReactionVector = Vector3.one;

            mTrailRenderer = GetComponentInChildren<TrailRenderer>();
			// mRigidBody = GetComponent<Rigidbody>();
			
			Trigger trigger = GetComponent<Trigger>();
			trigger.OnEnter += OnMatchTriggerEnter;
	
			// Si es posible, obtenemos el radio del propio Mesh
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if ( meshFilter ) {
				mRadio = meshFilter.mesh.bounds.size.y * 0.5f;
			}
			else {
				mRadio = transform.position.y;
			}
			
			MovingToPosition = false;
			
#if UNITY_EDITOR			
			StartCoroutine( RecordState() );
#endif
		}
		
		void OnDestroy() {
			Trigger trigger = GetComponent<Trigger>();
			trigger.OnEnter -= OnMatchTriggerEnter;
		}
		
#if UNITY_EDITOR
		private Vector3 oldPosition;
		IEnumerator RecordState () {
			/*
			while ( true ) {
				float dist = (transform.position - oldPosition).magnitude;
				if ( dist > 0.1f ) {
					GameObject partido = GameObject.FindGameObjectWithTag("Partido");
					Match3DGame soccerGame = partido.GetComponentInChildren<Match3DGame>();
					soccerGame.RecordEvent( gameObject, "" );
					oldPosition = transform.position;
				}
				yield return new WaitForSeconds(0.1f);
			}
			*/
			yield return null;
		}
#endif
		
		public void Setup() {
			Estado = EEstado.Ninguno;
			
			// Garantizar que el balon no está linkado a alguien
			UnAttach ();
			
			NewPropietary = null;
			Impulse = Vector3.zero;
			MovingToPosition = false;
			TimeToTarget = 0f;
			
			ShowTrail( false );
		}
		
		void Update () {
            if ( transform.parent != null && interpolateTime < 1f) {
				interpolateTime += Time.deltaTime;
				transform.localPosition = Vector3.Lerp( transform.localPosition, Vector3.zero, interpolateTime );
				// Helper.Log ( ()=>{ return transform.localPosition.magnitude>0; }, "Distancia: " + transform.localPosition.magnitude );
			}

            
            Vector3 position = transform.position;
			
			if ( TimeToTarget >= 0f ) {
				TimeToTarget -= Time.deltaTime;
			}
			
			if ( transform.parent == null ) {
				Impulse += Physics.gravity * Time.deltaTime;
                position += Impulse * Time.deltaTime;
				
				// Estamos desplazandonos a una posicion? Y Ha pasado el tiempo previsto para alcanzarlo?
				if ( MovingToPosition && TimeToTarget <= 0f ) {
					// Colocarnos exactamente donde nos indicaron
					position = TargetPosition;
					// Marcar que hemos dejado de movernos
					MovingToPosition = false;

                    var rb = gameObject.GetComponent<Rigidbody>();
                    if( rb==null) rb = gameObject.AddComponent<Rigidbody>();

                    rb.velocity = new Vector3( Impulse.x * ReactionVector.x, Impulse.y * ReactionVector.y, Impulse.z * ReactionVector.z);
                    ReactionVector = Vector3.one;

                    if ( OnTargetPosition != null ) {
						OnTargetPosition(this, EventArgs.Empty);
					}
					
					// Si seguimos estando quietos... (ningun suscriptor nos ha vuelto a impulsar)
					if ( !MovingToPosition ) {
						ShowTrail ( false );
						
						// Si la posicion destino no es elevada? (va a un punto del path en el suelo)
						if ( TargetPosition.y < (mRadio + 0.125f) ) {
							// Colocarnos exactamente donde nos indicaron
							position = TargetPosition;
							// Quitar el impulso para quedarnos quietecitos en ese lugar (ajustando únicamente la altura)
							Impulse = Vector3.zero;
						}
					}
				}
				
				if ( !MovingToPosition ) {
					// Garantizar que no nos metemos bajo el suelo...
					if ( (TimeToTarget <= 0f) && (position.y <= (mRadio + 0.01f)) ) {
						position.y = mRadio;
						Impulse = Vector3.zero;
					}
				}
			}
			else {
				Impulse = Vector3.zero;
			}
            if (transform.parent != null)
            {
                lastImpulse = transform.position - lastPosition;
                lastImpulse.y = 0;
                float v = lastImpulse.magnitude * 600000;
                transform.Rotate(transform.parent.forward, v * Time.deltaTime, Space.World);
            }
            lastPosition = transform.position;

            if (MovingToPosition)
			    transform.position = position;			
			
		}

        void LateUpdate() {
			if ( transform.parent != null ) {
				Vector3 position = transform.position;
				position.y = mRadio;
				transform.position = position;
			}

        }

        void ApplyImpulseToPosition ( Vector3 origen, Vector3 targetPosition, float timeToTarget ) {
			Impulse = Helper.CalculateBestThrowSpeed( origen, targetPosition, timeToTarget );
			/*
			Vector3 impulse = Helper.CalculateBestThrowSpeed( origen, targetPosition, timeToTarget );
			mRigidBody.AddForce ( impulse, ForceMode.Impulse );
			*/
			TargetPosition = targetPosition;
			TimeToTarget = timeToTarget;
			MovingToPosition = true;
		}
		
		public void ApplyImpulseToPosition ( Vector3 targetPosition, float timeToTarget ) {
			// Aplica el nuevo impulso desde la posicion actual
			ApplyImpulseToPosition( transform.position, targetPosition, timeToTarget );
		}
		
		public void NextImpulseToPosition ( Vector3 targetPosition, float timeToTarget ) {
			// Aplica el nuevo impulso desde la posicion Target
			ApplyImpulseToPosition( TargetPosition, targetPosition, timeToTarget );
		}
		
		public void AttachTo( GameObject propietary, string nodeName ) {
			Transform nodeTransform = Helper.FindTransform( propietary.transform, nodeName );
			transform.parent = nodeTransform;
			// transform.localPosition = Vector3.zero;
			NewPropietary = propietary;
			Estado = BallMotor.EEstado.Propietario;
			
			interpolateTime = 0f;
		}
		
		public void UnAttach() {
			transform.parent = null;
            var rb = gameObject.GetComponent<Rigidbody>();
            // transform.localPosition = Vector3.zero;
        }

        public void InterpolateToPosition( Vector3 position ) {
			
			if ( TimeToTarget > 0.01f ) {
				Vector3 newImpulse = Helper.CalculateBestThrowSpeed( transform.position, position, TimeToTarget );
				
				newImpulse = Vector3.Lerp ( Impulse, newImpulse, 1f );
				newImpulse.y = Impulse.y;
				
				// Helper.Log ( gameObject, "Impulse: " + Impulse.ToString() + " NewImpulse: " + newImpulse.ToString() );
				Impulse = newImpulse;
				
				TargetPosition = position;
			}
		}
		
		public void ShowTrail( bool activate ) {
			if ( mTrailRenderer ) {
				mTrailRenderer.enabled = activate;
			}
		}
		
		void OnMatchTriggerEnter( object sender, EventTriggerArgs args ) {
			Trigger other = args.Trigger;
			if ( other.tag == "Porteria" ) {
				Impulse = Vector3.zero;
				// data.BallNear = true;
			}
		}	
        
        public void Clearing(Vector3 vector)
        {
            ReactionVector = vector;

        }

        void OnDrawGizmos () {
			if ( MovingToPosition ) {
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere( TargetPosition, 0.3f );
			}
		}
	}
	
}
