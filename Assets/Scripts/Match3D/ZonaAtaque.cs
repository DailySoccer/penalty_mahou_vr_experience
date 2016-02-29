using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class PropietaryPosition {
		public bool active = false;
		public int angle;
		public Vector3 position;
		public GameObject propietary = null;
		public GameObject target = null;
		public float distance;
		public float priority;
		public float radio;
	}
	
	public class ZonaAtaque : MonoBehaviour {
		GameObject porteria;
		// SoccerData data;
		// BallMotor mBalonMotor;
		
		const int MaxAtacantes = 3;
		PropietaryPosition[] propietaryPositions;
	
		void Start () {
			porteria = GameObject.FindGameObjectWithTag ("Porteria");
			// porteria = GameObject.Find ("Visitante1");
			
			// GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			// mBalonMotor = balon.GetComponentInChildren<BallMotor>();
			
			// data = gameObject.GetComponentInChildren<SoccerData>();
			
			int[] angles = {-45,+45,-20,+20};
			float [] priority = {0,1,1,0};
			float [] radio = {4,5,7,6};
			
			propietaryPositions = new PropietaryPosition[MaxAtacantes];
			for (int i=0; i<propietaryPositions.Length; i++) {
				propietaryPositions[i] = new PropietaryPosition();
				propietaryPositions[i].active = false;
				propietaryPositions[i].angle = angles[i];
				propietaryPositions[i].priority = priority[i];
				propietaryPositions[i].radio = radio[i];
			}
		}
		
		void Update () {
		}
	
		void freePositionUnused() {
			/*
			for (int i=0; i<propietaryPositions.Length; i++) {
				if ( propietaryPositions[i].propietary == null ) {
					propietaryPositions.RemoveAt( i );
					break;
				}
			}
			*/
		}
		
		Vector3 calculatePosicionAtaque() {
			Vector3 dir = porteria.transform.position - transform.position;
			dir.z = 0f;
			Vector3 dirToPorteria = dir.normalized;
			return (transform.position + dirToPorteria * 2f);
		}
		
		void calculatePosicionAtaque( PropietaryPosition ataque ) {
			Vector3 dir = porteria.transform.position - transform.position;
			dir.z = 0f;
			Vector3 dirToPorteria = dir.normalized;
			Quaternion angleAxis = Quaternion.AngleAxis( ataque.angle, Vector3.up );
			
			Vector3 position = transform.position + (angleAxis * dirToPorteria) * ataque.radio;
			ataque.position = position;
		}
		
		public int NumAttackers() {
			int numAttackers = 0;
			foreach( PropietaryPosition propPosition in propietaryPositions ) {
				if ( /*!propPosition.active*/ propPosition.propietary != null )
					numAttackers++;
			}
			return numAttackers;
		}
		
		public bool HasPositionFree() {
			foreach( PropietaryPosition propPosition in propietaryPositions ) {
				if ( /*!propPosition.active*/ propPosition.propietary == null )
					return true;
			}
			return false;
			/*
			freePositionUnused();
			return propietaryPositions.Length < MaxAtacantes;
			*/
		}
		
		void AssignPosition( PropietaryPosition pointNew ) {
			PropietaryPosition modify = null;
			
			for (int i=0; i<propietaryPositions.Length; i++) {
				PropietaryPosition point = propietaryPositions[i];
				if ( point.target == pointNew.target ) {
					modify = propietaryPositions[i];
					break;				
				}
			}
			
			if ( modify == null ) {
				for (int i=0; i<propietaryPositions.Length; i++) {
					PropietaryPosition point = propietaryPositions[i];
					if ( !point.active ) {
						modify = propietaryPositions[i];
						break;
					}
				}
			}
			
			if ( modify != null ) {
				modify.active = true;
				modify.target 	= pointNew.target;
				modify.position = pointNew.position;
				modify.priority = pointNew.priority;
			}
		}
		
		void CalculatePositions() {
			PropietaryPosition point = new PropietaryPosition();
			
			// GameObject target = mBalonMotor.NewPropietary;
			
			for (int i=0; i<propietaryPositions.Length; i++) {
				propietaryPositions[i].priority = -100;
			}

			Vector3 dirToPorteria = porteria.transform.position - transform.position;
			point.target	= porteria;
			point.priority 	= 0;
			point.position	= transform.position + dirToPorteria.normalized * 5f;
			AssignPosition( point );

			/*
			if ( target == gameObject ) {
				Vector3 dirToPorteria = porteria.transform.position - transform.position;
				point.target	= porteria;
				point.priority 	= 0;
				point.position	= transform.position + dirToPorteria.normalized * 5f;
				AssignPosition( point );
			}
			else if (target != null) {
				Vector3 dir = porteria.transform.position - transform.position;
				point.target	= target;
				point.priority 	= 10;
				point.position	= transform.position + dir.normalized * 5f; //(dir.magnitude * 0.5f);
				AssignPosition( point );
			}
			*/

			/*
			foreach( SoccerMotor amigo in data.AliadoNear ) {
				Vector3 dir = amigo.transform.position - transform.position;
				point.target	= amigo.gameObject;
				point.priority 	= 10;
				point.position	= transform.position + dir.normalized * (dir.magnitude * 0.5f);
				AssignPosition( point );
			}
			*/
			
			for (int i=0; i<propietaryPositions.Length; i++) {
				if ( propietaryPositions[i].priority <= -1 ) {
					propietaryPositions[i].active = false;
					propietaryPositions[i].propietary = null;
				}
			}
		}
		
		PropietaryPosition FindPropietaryPositionNearest( GameObject solicitante ) {
			float distanceNearest = 100f;
			PropietaryPosition propPosNearest = null;
			
			foreach( PropietaryPosition propPosition in propietaryPositions ) {
				if ( /*!propPosition.active &&*/ propPosition.propietary == null ) {
					calculatePosicionAtaque( propPosition );
					
					float distance = Helper.DistanceInPlaneXZ( solicitante, propPosition.position ) + propPosition.priority;
					if ( distance < distanceNearest ) {
						distanceNearest = distance;
						propPosNearest = propPosition;
					}
				}
			}
			
			return propPosNearest;
		}

		public PropietaryPosition GetAttackPosition( GameObject solicitante ) {
			
			PropietaryPosition posicionAtaque = null;
			
			// CalculatePositions();
			
			// El solicitante es propitario de un hueco?
			foreach( PropietaryPosition ataque in propietaryPositions ) {
				if ( ataque.propietary == solicitante ) {
					// Devolver al solicitante su hueco
					posicionAtaque = ataque;
					break;
				}
			}
			
			// Si no tengo un hueco reservado
			if ( posicionAtaque == null ) {
				// Si queda alguno libre, crearle uno
				if ( HasPositionFree() ) {
					posicionAtaque = FindPropietaryPositionNearest( solicitante );
					/*
					foreach( PropietaryPosition propPosition in propietaryPositions ) {
						if ( propPosition.propietary == null ) {
							posicionAtaque = propPosition;
							break;
						}
					}
					*/
					/*
					posicionAtaque = new PropietaryPosition();
					propietaryPositions.Add ( posicionAtaque );
					*/
				}
				else {
					// Estamos más cerca que alguno de los otros atacantes?
					foreach( PropietaryPosition ataque in propietaryPositions ) {
						if ( ataque.propietary != null ) {
							float propietaryDistance = (ataque.position - ataque.propietary.transform.position).magnitude;
							float distance = (ataque.position - solicitante.transform.position).magnitude;
							
							if ( propietaryDistance > distance ) {
								// Cambiar de propietario
								posicionAtaque = ataque;
								/*
								Debug.Log ( System.String.Format( "Cambio Ataque: {0} {1} => {2} {3}", 
									ataque.propietary.transform.parent.name, propietaryTotal, solicitante.transform.parent.name, distanceTotal ) );
								*/
								break;
							}
						}
					}
				}
			}
			
			// Si devolvemos una posicion válida, garantizamos que la información esté actualizada
			if ( posicionAtaque != null ) {
				posicionAtaque.active = true;
				posicionAtaque.propietary = solicitante;
				calculatePosicionAtaque( posicionAtaque );
			}
			
			return posicionAtaque;
		}

		/*
		void OnDrawGizmos () {
			if ( propietaryPositions != null ) {
				foreach( PropietaryPosition ataque in propietaryPositions ) {
					if ( ataque.propietary ) {
						Vector3 up = new Vector3(0,0.1f,0);
						Gizmos.DrawLine( ataque.propietary.transform.position + up, ataque.position + up );
						Gizmos.DrawWireSphere ( ataque.position, 0.2f );
					}
				}
			}
		
			if (data != null)
			{
				foreach( SoccerData amigo in data.AliadoNear ) {
					Vector3 up = new Vector3(0,0.1f,0);
					Gizmos.color = Color.green;
					Gizmos.DrawLine( amigo.transform.position + up, transform.position + up );
				}
			}
		}
		*/
	}
}
