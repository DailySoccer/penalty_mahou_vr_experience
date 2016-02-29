using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class Tactica : MonoBehaviour {
		
		MatchManager mMatchManager;
		List<Entrenador> mEntrenadores = new List<Entrenador>();
		
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnNewPlay += OnNewPlay;
		}
		
		void OnNewPlay (object sender, EventArgs e) {
			StopAllCoroutines();
			
			StartCoroutine( Run () );
		}
		
		IEnumerator Run () {
			GameObject[] visitantes = GameObject.FindGameObjectsWithTag( "Visitante" );
			foreach( GameObject visitante in visitantes ) {
				Entrenador entrenador = visitante.GetComponent<Entrenador>();
				mEntrenadores.Add ( entrenador );
			}
			
			yield return null;
		}
		
		void Update () {
		}
	}
	
}
