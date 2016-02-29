using UnityEngine;
using System;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PlayResult : MonoBehaviour {
		
		public bool NextPlay = false;
		
		MatchManager mMatchManager;
		
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}
		
		void Start () {
			StartCoroutine( UpdateState() );
		}
		
		IEnumerator UpdateState () {
			while ( true ) {
				if ( NextPlay ) {
					mMatchManager.FinDeJugada( true, false);
					NextPlay = false;
				}
				yield return null;
			}
		}
		
		void Update () {
		}
	}
	
}
