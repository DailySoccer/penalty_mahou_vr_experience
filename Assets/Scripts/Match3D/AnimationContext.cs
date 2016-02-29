using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class AnimationContext : MonoBehaviour {
	
		static public int WaitingReset = 0;
		static public bool IsResetFinished {
			get { return (WaitingReset == 0); }
		}

		public float Direccion = 0;
		public float Distancia = 0;
		public float Tiempo = 0;
		
		private MatchManager mMatchManager;
		private Animator mAnimator;

		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mAnimator = GetComponent<Animator>();
			
			mMatchManager.OnNewPlay += OnNewPlay;
		}

		void OnNewPlay (object sender, MatchManager.EventNewPlayArgs e)	{
		}
		
		void OnDestroy () {
			mMatchManager.OnNewPlay -= OnNewPlay;
		}
		
		IEnumerator ResetController() {
			WaitingReset++;

			AnimatorID.AnimationReset( mAnimator );
			mAnimator.SetBool( "Reset", true );
			// mAnimator.speed = 10f;

			// string animationClipNames = Helper.AnimationClipNames( mAnimator );

			yield return StartCoroutine( Helper.WaitCondition( () => {
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Idle ) );
			} ) );			
			
			mAnimator.SetBool( "Reset", false );
			// mAnimator.speed = 1f;
			
			WaitingReset--;
			Assert.Test ( () => (WaitingReset>=0), "AnimationContext: ResetController failed" );
		}
		
		public void Reset () {
			StartCoroutine( ResetController() );
		}
		
		/*		
		 * Distancia Context:
		 * 0. Cerca
		 * 1. Medio
		 * 2. Lejos
		 * 
		 * */
		void SetDistancia () {
			if ( Distancia < 40f ) {
				mAnimator.SetInteger( AnimatorID.DistanciaContext, 1 );	// Medio
			}
			else {
				mAnimator.SetInteger( AnimatorID.DistanciaContext, 2 );	// Lejos
			}
		}
		
		/*
		 * Direccion Context:
		 * 0. Front
		 * 1. Left
		 * 2. Right
		 * 3. Back
		 */
		void SetDireccion () {
			// Direccion CONTEXTO
			if ( Direccion <= 45f || Direccion >= 315f ) {
				mAnimator.SetInteger( AnimatorID.DireccionContext, 0 );		// Front
			}
			else if ( Direccion >= (180f+65f) && Direccion <= (360f-45f)  ) {
				mAnimator.SetInteger( AnimatorID.DireccionContext, 1 );		// Left
			}
			else if ( Direccion >= 45f && Direccion <= (180f-65f)  ) {
				mAnimator.SetInteger( AnimatorID.DireccionContext, 2 );		// Right
			}
			else {
				mAnimator.SetInteger( AnimatorID.DireccionContext, 3 );		// Back
			}
			/*
			if ( mAnimator.gameObject.name == "Soccer-Local9" )
				Helper.Log ( mAnimator.gameObject, "Direccion: " + Direccion );
			*/
			
			mAnimator.SetBool( AnimatorID.FrontContext, ( Direccion <= 90f || Direccion >= 270f ) );
			mAnimator.SetBool( AnimatorID.RightContext, ( Direccion <= 180f ) );
			mAnimator.SetBool( AnimatorID.LeftContext, 	( Direccion >= 180f ) );
			mAnimator.SetBool( AnimatorID.BackContext,  ( Direccion >= 90f && Direccion <= 270f ) );
		}
		
		/*
		 * Potencia Context:
		 * 0. Baja
		 * 1. Media
		 * 2. Alta
		 */
		void SetPotencia () {
			if ( Tiempo > 0f ) {
				float velocidad = Distancia / Tiempo;
				if ( velocidad < 12f )
					mAnimator.SetInteger( AnimatorID.PotenciaContext, 0 );	// Baja
				else if ( velocidad < 20f )
					mAnimator.SetInteger( AnimatorID.PotenciaContext, 1 );	// Media
				else
					mAnimator.SetInteger( AnimatorID.PotenciaContext, 2 );	// Alta
			}
		}
		
		void SetContextPase () {
			SetDistancia ();
			SetDireccion ();
			// SetPotencia ();
		}

		void SetContextChut () {
			SetDistancia ();
			SetDireccion ();
			SetPotencia ();
		}
		
		public void SetContext ( ActionType actionType ) {
			switch ( actionType ) {
			case ActionType.PASE: SetContextPase(); break;
			case ActionType.CHUT: SetContextChut(); break;
			}
		}
	}
	
}
