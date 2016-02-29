using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FootballStar.Match3D {
	
	public class PasarBalon_Test : MonoBehaviour {
	
		private MatchManager mMatchManager;
		private GameObject[] mAliados;
		private List<SoccerMotor> mAliadosMotor = new List<SoccerMotor>();
		private GameObject mBalon;
		private BallMotor mBallMotor;
			
		void Awake () {
			mBalon = GameObject.FindGameObjectWithTag ("Balon");
			mBallMotor = mBalon.GetComponent<BallMotor>();
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnNewPlay += OnNewPlay;
		}
		
		void OnNewPlay (object sender, EventArgs e) {
			if ( enabled ) {
				mAliados = GameObject.FindGameObjectsWithTag( "Local" );
				
				if ( mAliados.Length > 0 ) {
					foreach( GameObject dummy in mAliados ) {
						Entrenador entrenador = dummy.GetComponentInChildren<Entrenador>();
						mAliadosMotor.Add ( entrenador.Target.GetComponentInChildren<SoccerMotor>() );
					}
					
					StartCoroutine( Automatico() );
				}
			}
		}
		
		IEnumerator Automatico() {
			while ( true ) {
				GameObject propietary = mBallMotor.NewPropietary;
				if ( propietary != null ) {
					SoccerData propietaryData = propietary.GetComponentInChildren<SoccerData>();
					
					if ( propietaryData.BallPropietary && !propietaryData.PasarBalon ) {
						yield return new WaitForSeconds(1f);
						
						GameObject dummyTarget = null;

						// Damos prioridad a aquellos jugadores que se estén moviendo
						List<GameObject> movingSoccers = new List<GameObject>();
						foreach( SoccerMotor motor in mAliadosMotor ) {
							if ( motor.IsMoving() ) {
								SoccerData data = motor.GetComponentInChildren<SoccerData>();
								movingSoccers.Add ( data.dummyMirror );
							}
						}
						
						if ( movingSoccers.Count > 0 ) {
							GameObject dummySoccer = movingSoccers[ UnityEngine.Random.Range( 0, movingSoccers.Count-1 ) ];
							if ( dummySoccer && dummySoccer != propietaryData.dummyMirror ) {
								dummyTarget = dummySoccer;
							}
						}
						
						if ( dummyTarget == null ) {
							GameObject dummySoccer = mAliados[ UnityEngine.Random.Range( 0, mAliados.Length-1 ) ];
							if ( dummySoccer && dummySoccer != propietaryData.dummyMirror ) {
								dummyTarget = dummySoccer;
							}
						}
						
						if ( dummyTarget ) {
							propietaryData.MsgPasar( dummyTarget, 1.5f, 1.5f );
						}
					}
				}
				yield return new WaitForSeconds(1f);
			}
		}
		
		void Update () {
		}
		
	}
	
}
