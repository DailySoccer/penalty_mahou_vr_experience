using System;
using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PlayFilter : MonoBehaviour {
	
		public ActionType ActionType;
		private bool mApplyFilter = false;
		
		MatchManager mMatchManager;
		
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnNewPlay += OnNewPlay;
			StartCoroutine( FilterUpdate() );
		}

		void OnNewPlay (object sender, EventArgs e) {
			mApplyFilter = true;
		}
		
		IEnumerator FilterUpdate () {
			while ( true ) {
				yield return StartCoroutine( Helper.WaitCondition( () => {
					return mApplyFilter;
				} ) );		

				if ( ActionType != ActionType.NINGUNO ) {
					if ( !mMatchManager.InteractiveActions.HasAction ( ActionType ) ) {
						yield return new WaitForSeconds( 0.5f );
						mMatchManager.FinDeJugada( true, false );
					}
				}
				mApplyFilter = false;
			}
		}
		
		void Update () {
		}
	}
	
}
