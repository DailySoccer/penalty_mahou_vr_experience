using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FootballStar.Common;

namespace FootballStar.Match3D {
	
	public class FisicaBalon_Test : MonoBehaviour {

		public Transform Target;
		public float TimeToPosition = 1f;
		
		private MatchManager mMatchManager;
		private GameObject mBalon;
		private Rigidbody mBallRigidBody;
		private BallMotor mBallMotor;
		private Vector3 mStartPosition;
			
		void Awake () {
			mBalon = GameObject.FindGameObjectWithTag ("Balon");
			mBallMotor = mBalon.GetComponent<BallMotor>();
			mBallRigidBody = mBalon.GetComponent<Rigidbody>();
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnNewPlay += OnNewPlay;
		}
		
		void OnNewPlay (object sender, EventArgs e) {
			if ( enabled ) {
				mBallRigidBody.useGravity = true;
				mBallRigidBody.isKinematic = false;
			
				mStartPosition = mBalon.transform.position;
				StartCoroutine( Automatico() );
			}
		}
		
		IEnumerator Automatico() {
			bool tap = false;
			while ( true ) {
				if ( Target != null ) {
					// bool tapBackup = tap;
					tap = (tap) ? !GameInput.IsTouchUp() : GameInput.IsTouchDown();
					
					 //if ( tap && tap != tapBackup ) {
						mBallMotor.ApplyImpulseToPosition( Target.position, TimeToPosition );
						yield return new WaitForSeconds( TimeToPosition );
						
						// mBallRigidBody.AddForce( Vector3.up * 5f, ForceMode.Impulse );
					
						yield return new WaitForSeconds( 3f );
					
						mBallRigidBody.useGravity = false;
						mBallRigidBody.isKinematic = true;
						
						yield return new WaitForSeconds( 1f );
						
						mBalon.transform.position = mStartPosition;
						
						yield return new WaitForSeconds( 1f );
					
						mBallRigidBody.isKinematic = false;
						mBallRigidBody.useGravity = true;
					//}
				}
				yield return null;
			}
		}
		
		void Update () {
		}
		
	}
	
}
