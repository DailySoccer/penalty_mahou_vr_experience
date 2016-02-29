using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class TriggerManager : MonoBehaviour {
			
		List<Trigger> mColliders = new List<Trigger>();
		Dictionary< int, HashSet<int> > mCollisions = new Dictionary< int, HashSet<int> >();
		Dictionary< int, GameObject > mGameObjects = new Dictionary< int, GameObject >();
		
		// REVIEW: Triggers que necesitamos comprobar en cada frame
		Trigger mPorteriaTrigger;
		Trigger mBalonTrigger;
	
		public void Setup () {
			StopAllCoroutines();
			
			mColliders.Clear ();
			mCollisions.Clear ();
			
			StartCoroutine ( UpdateCollisions() );
		}
		
		void Start () {
			Setup ();
		}
		
		void Update () {
		
		}
		
		void OnDestroy () {
			StopAllCoroutines();
			mColliders.Clear ();
			mCollisions.Clear ();
			mGameObjects.Clear ();
		}
		
		void SetupTriggers() {
			Profiler.BeginSample( "FindColliders" );

			// REVIEW: Garantizar que la Porteria tiene el componente Trigger
			GameObject porteria = GameObject.FindGameObjectWithTag ("Porteria");
			mPorteriaTrigger = porteria.GetComponent<Trigger>();
			if ( mPorteriaTrigger == null ) {
				mPorteriaTrigger = porteria.AddComponent<Trigger>();
				mPorteriaTrigger.Radio = 1f;
			}
			
			// REVIEW: Garantizar que el Balon tiene el componente Trigger
			GameObject balon = GameObject.FindGameObjectWithTag( "Balon" );
			mBalonTrigger = balon.GetComponent<Trigger>();
			if ( mBalonTrigger == null ) {
				mBalonTrigger = porteria.AddComponent<Trigger>();
				mBalonTrigger.Radio = 1f;
			}
			
			// Incluir la Poteria
			mColliders.Add ( mPorteriaTrigger );
			mGameObjects[ mPorteriaTrigger.GetHashCode() ] = mPorteriaTrigger.gameObject;
			
			// Incluir el Balon
			mColliders.Add ( mBalonTrigger );
			mGameObjects[ mBalonTrigger.GetHashCode() ] = mBalonTrigger.gameObject;
			
			// Incluir los futbolistas Locales
			GameObject[] locales = GameObject.FindGameObjectsWithTag( "Local" );
			foreach( GameObject local in locales ) {
				Entrenador entrenador = local.GetComponent<Entrenador>();
				Trigger trigger = entrenador.Target.GetComponent<Trigger>();
				mColliders.Add ( trigger );
				mGameObjects[ trigger.GetHashCode() ] = trigger.gameObject;
			}
			
			// Incluir los futbolistas Visitantes
			GameObject[] visitantes = GameObject.FindGameObjectsWithTag( "Visitante" );
			foreach( GameObject visitante in visitantes ) {
				Entrenador entrenador = visitante.GetComponent<Entrenador>();
				Trigger trigger = entrenador.Target.GetComponent<Trigger>();
				mColliders.Add ( trigger );
				mGameObjects[ trigger.GetHashCode() ] = trigger.gameObject;
			}
			
			Profiler.EndSample();
		}
		
		bool UpdateCollision( Trigger collider1, Trigger collider2 ) {
			bool changed = false;
			
			if ( collider2.enabled && (collider1 != collider2) ) {
				
				// first = HashCode menor
				int firstHashCode = collider1.GetHashCode();
				int secondHashCode = collider2.GetHashCode();
				if ( firstHashCode > secondHashCode ) {
					int swap = firstHashCode;
					firstHashCode = secondHashCode;
					secondHashCode = swap;
				}
				
				// Intersectan?
				bool intersectan = false;
				if ( collider1.Radio > collider2.Radio )
					intersectan = collider1.Contains( collider2.transform.position );
				else
					intersectan = collider2.Contains( collider1.transform.position );
				
				if ( intersectan ) { //.Intersects( collider2.bounds ) ) {
					if ( !mCollisions.ContainsKey ( firstHashCode ) )
						mCollisions[firstHashCode] = new HashSet<int>();
					
					HashSet<int> collisions = mCollisions[firstHashCode];
					if ( !collisions.Contains(secondHashCode) ) {
						// Incluirlo
						collisions.Add( secondHashCode );
							
						// Informar del comienzo de la colisión
						collider1.Enter( collider2 );
						collider2.Enter( collider1 );
						
						changed = true;
					}
				}
				// No Intersectan !
				else {
					bool registered = mCollisions.ContainsKey ( firstHashCode ) && mCollisions[firstHashCode].Contains( secondHashCode );
					if ( registered ) {
						// Quitarlo
						mCollisions[firstHashCode].Remove( secondHashCode );
							
						// Informar del final de la colisión
						collider1.Exit( collider2 );
						collider2.Exit( collider1 );
						
						changed = true;
					}
				}
			}
			
			return changed;
		}
		
		IEnumerator UpdateCollisions () {
			// REVIEW: Esperamos un frame para que se eliminen los gameObjects de una jugada anterior
			yield return null;
			
			SetupTriggers();
			
			bool firstTime = true;
			
			while ( true ) {
				// float timeBegin = Time.realtimeSinceStartup;
					
				foreach( Trigger collider in mColliders ) {
					if ( !collider.enabled ) continue;
					
					bool changed = false;
					
					foreach ( Trigger collider2 in mColliders ) {
						bool changeCollision = UpdateCollision( collider, collider2 );
						changed = changed || changeCollision;
					}
							
					if ( changed ) {
						/*
						if ( mCollisions.ContainsKey( collider.GetHashCode() ) ) {
							HashSet<int> collisions = collisions = mCollisions[ collider.GetHashCode() ];
							string texto = collisions.Count + " = ";
							foreach ( int hash in collisions ) {
								texto += mGameObjects[ hash ].name + ", ";
							}
							Helper.Log ( collider.gameObject, texto );
						}
						*/
					}
					
					if ( !firstTime ) {
						// REVIEW: Garantizar que siempre comprobamos la colision entre balon y porteria
						UpdateCollision( mBalonTrigger, mPorteriaTrigger );
						
						yield return null;
					}
				}
				
				firstTime = false;
				
				yield return null;
				// Helper.Log ( gameObject, "Collisions: " + (Time.realtimeSinceStartup - timeBegin) );
					
			}
		}
	}
	
}
