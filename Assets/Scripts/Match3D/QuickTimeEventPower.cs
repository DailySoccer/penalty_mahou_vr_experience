using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;
	
namespace FootballStar.Match3D
{	
	public class QuickTimeEventPower : MonoBehaviour {	
		
		const int NumDifficultyLevels = 4;
				
		public int Difficulty = 0;
		
		public float[] TimeReactions = new float[NumDifficultyLevels];
		
		public bool PointOfInterest = true;
		
		public Transform GizmoParent;
		
		public float DisplayHeight = 1.8f;
			
		public float TimeReaction {	get { return TimeReactions[Difficulty]; } }
		public float TimeTotal { get { return TimeReaction; } }
		
		public bool InputEnabled { get; set; }
		public bool IsEvaluating { get { return (mActions != null); } }
		public bool IsTap { get { return InputEnabled? mTap : false; } }		// Ha habido un tap en en ultimo frame?
		public bool IsLoadingPower { get { return mPowerStartTime>0; } }
		
		public event EventHandler OnShowInteraction;
		public event EventHandler OnPerfectInteraction;
		public event EventHandler<EventQuickTimeResultArgs> OnEndInteraction;	// Tanto en caso de exito como de fracaso
		
		public float RangeMaxDistance = 40f;
		public float RangeMaxTime = 0.2f;
		public float RangeTimeTolerance = 0.1f;
		public bool ShowRangeTolerance = true;
		
		void Awake () {
			InputEnabled = true;
			mMatchManager = gameObject.GetComponent<MatchManager>();
			mBallMotor = GameObject.FindGameObjectWithTag("Balon").GetComponentInChildren<BallMotor>();
			
			mPixelSizeAdjustment = GetComponentInChildren<UIRoot>().pixelSizeAdjustment;
			
			// Componentes necesarios para realizar la accion interactiva
			mPowerActionTransform = Helper.FindTransform( GizmoParent, "Action" );
			mPowerGreenBar = Helper.GetComponent<FilledBar>( mPowerActionTransform, "Green Bar" );
			mPowerRedBar = Helper.GetComponent<FilledBar>( mPowerActionTransform, "Red Bar" );
			mPowerMark = Helper.GetComponent<UITexture>( mPowerActionTransform, "Mark" );
			mTolerancePowerBar = Helper.GetComponent<TolerancePowerBar>( mPowerActionTransform, "Tolerance Bar" );
			
			// Componente a reproducir cuando tenga exito la accion
			mSuccessAnimation = Helper.GetComponent<AnimationScript>( transform, "Success" );
			// Componente a reproducir cuando falle la accion
			mFailAnimation = Helper.GetComponent<AnimationScript>( transform, "Fail" );
			
			mPowerActionTransform.gameObject.SetActive( false );
		}
		
		public void Activate( InteractiveActions interactiveActions ) {
		}
		
		public void Deactivate() {
		}
		
		public void EvaluateAction(InteractiveActions _actions, float distance)	{
			StopAllCoroutines();
			
			// mTap = false;
			mPowerActive = true;
			mPowerStartTime = -1f;
			
			mPowerDistance = distance;
			if ( mPowerDistance > RangeMaxDistance )
				mPowerDistance = RangeMaxDistance;
			
			mPowerTime = TimeFromDistance ( mPowerDistance );
			
			mPowerActionTransform.gameObject.SetActive( true );
			
			mPowerGreenBar.Activate ();
			mPowerRedBar.Activate ();
			
			Vector3 pos = mPowerMark.transform.localPosition;
			pos.x = mPowerGreenBar.TextureCenter.transform.localPosition.x + (mPowerTime / RangeMaxTime) * mPowerGreenBar.Width;
			mPowerMark.transform.localPosition = pos;
			
			// pos = PowerToleranceBar.transform.localPosition;
			// PowerToleranceBar.transform.localPosition = pos;
			mTolerancePowerBar.SetRange( (RangeTimeTolerance / RangeMaxTime) * mPowerGreenBar.Width, pos.x, (RangeTimeTolerance / RangeMaxTime) * mPowerGreenBar.Width );
			
			// Debug.Log( "--------------> Distance: " + distance );
			
			mActions = _actions;
			mActions.CurrentAction.ActorData.EvaluateAction = SoccerData.ActionState.Wait;
			
			mCurrentTimer = 0;
			mCurrentTimer0 = 0;
			mCurrentTimerEnd = TimeReaction;

			StartCoroutine( UpdateTimers() );
		}
		
		IEnumerator WaitTimer( float maxTimer ) {
			while ( mCurrentTimer <= maxTimer ) {
				yield return null;
			}
		}
		
		IEnumerator UpdateTimers() {
			if ( OnShowInteraction != null )
				OnShowInteraction( this, EventArgs.Empty );
			
			yield return StartCoroutine( WaitTimer (mCurrentTimer0) );

			if ( OnPerfectInteraction != null )
				OnPerfectInteraction( this, EventArgs.Empty );
						
			StartCoroutine( WaitTimer (mCurrentTimerEnd) );
		}
		
		// Dados RangeMaxTime y RangeMaxDistance, calculamos el tiempo que hay que pulsar para alcanzar una distancia determinada
		float TimeFromDistance( float distance ) {
			float t = distance * RangeMaxTime / RangeMaxDistance;
			t = (t > RangeMaxTime) ? RangeMaxTime : t;
			return t;
		}
		
		float InterpolatePowerDistance( float time ) {
			return time * RangeMaxDistance / RangeMaxTime;
		}
				
		void Update() {
			mTap = (mTap) ? !GameInput.IsTouchUp() : GameInput.IsTouchDown();
			
			if (!IsEvaluating)
				return;

			mCurrentTimer += Time.deltaTime;
			
			if ( mCurrentTimer > mCurrentTimerEnd ) {
				if ( mPowerActive ) {
					EndPower();
				}
			}
			
			if ( mPowerActive ) {
				if ( IsTap ) {
					StartPower();
				}
				else if ( mPowerStartTime > 0f ) {
					EndPower();
				}
			}
		}
		
		// float powerFake = 0f;
		
		void LateUpdate() {
			if (!IsEvaluating)
				return;
			
			// Potencia actual
			if ( mPowerStartTime > 0f ) {
				float time = Time.time - mPowerStartTime;
				if (time > RangeMaxTime)
					time = RangeMaxTime;
				
				// PowerGreenBar.Percent = time / RangeMaxTime;
				if ( EvaluateTime(time) ) {
					mPowerRedBar.Percent = 0;
					mPowerGreenBar.Percent = time / RangeMaxTime;
				}
				else {
					mPowerGreenBar.Percent = 0;
					mPowerRedBar.Percent = time / RangeMaxTime;
				}
			}
			/*
			else {
				powerFake += Time.deltaTime;
				if (powerFake > RangeMaxTime) {
					powerFake = 0f;
				}
				
				if ( EvaluateTime(powerFake) ) {
					PowerRedBar.Percent = 0;
					PowerGreenBar.Percent = powerFake / RangeMaxTime;
				}
				else {
					PowerGreenBar.Percent = 0;
					PowerRedBar.Percent = powerFake / RangeMaxTime;
				}
			}
			*/
			
			GameObject target = mBallMotor.NewPropietary;
			
			if ( target != null ) {
				Vector3 soccerPos = target.transform.position;
				
				soccerPos.y = DisplayHeight;
	
				Vector3 onScreen = Camera.main.WorldToScreenPoint(soccerPos);
				
				// Clipeado
				mPositionX = onScreen.x; //Mathf.Clamp(onScreen.x, MinSize * 0.5f, Screen.width - MinSize * 0.5f);
				mPositionY = onScreen.y; //Mathf.Clamp(onScreen.y, MinSize * 0.5f, Screen.height - MinSize * 0.5f);
	
				UpdateGizmos( mPositionX, mPositionY );
			}
		}
		
		private void UpdateGizmos( float positionX, float positionY ) {
			// Pasamos al viewport ortografico
			var adjustedX = (positionX - Screen.width * 0.5f) * mPixelSizeAdjustment;
			var adjustedY = (positionY - Screen.height * 0.5f) * mPixelSizeAdjustment;
			
			mPowerActionTransform.localPosition = new Vector3(adjustedX, adjustedY, 0.0f);
		}
		
		float EvaluatePowerScore (float time) {
			float valoracion = 0f;
			
			if (time > RangeMaxTime)
				time = RangeMaxTime;
		
			if ( time < mPowerTime - RangeTimeTolerance )
				valoracion = -1f;
			else if ( time > mPowerTime + RangeTimeTolerance )
				valoracion = 1f;
			
			return valoracion;
		}
/*		
		float EvaluateScore ()	{
			float valoracion = 0f;
			return valoracion;
		}
*/		
		void StartPower() {
			if ( mPowerStartTime < 0f ) {
				mPowerStartTime = Time.time;
			}
		}
		
		void EndPower() {
			if ( mPowerStartTime > 0f ) {
				float diffTime = Time.time - mPowerStartTime;
				NotifyPower( diffTime );
			}
			else {
				NotifyPower( -1f );
			}
		}
		
		bool EvaluateTime ( float time ) {
			if (time > RangeMaxTime)
				time = RangeMaxTime;
			
			return ( time >= mPowerTime - RangeTimeTolerance && time <= mPowerTime + RangeTimeTolerance );
		}
		
		public void NotifyPower( float powerTime ) {
			
			StopAllCoroutines();
			
			float score = EvaluatePowerScore(powerTime);
			
			bool success = (powerTime >= 0f) ? EvaluateTime( powerTime ) : false;
			
			if ( mMatchManager.AutomaticTap.Enabled ) {
				success = true;
			}

			if ( OnEndInteraction != null ) {
				EventQuickTimeResultArgs args = new EventQuickTimeResultArgs( success, score );
				args.Direction = mBallMotor.NewPropietary.transform.forward;
				args.Distance  = ( powerTime > 0f ) ? InterpolatePowerDistance( powerTime ) : mPowerDistance * 0.4f;
				OnEndInteraction( this, args );
			}
			
			if (success) {
				ProjectToScreen projectToScreen = mSuccessAnimation.gameObject.GetComponent<ProjectToScreen>();
				projectToScreen.Source = mBallMotor.NewPropietary;
				mSuccessAnimation.Play();
			}
			else {
				ProjectToScreen projectToScreen = mFailAnimation.gameObject.GetComponent<ProjectToScreen>();
				projectToScreen.Source = mBallMotor.NewPropietary;
				mFailAnimation.Play();
			}
			
			mActions = null;
			
			mPowerActive = false;
			
			mPowerActionTransform.gameObject.SetActive( false );
			
			// Debug.Log ( "------------> Time: " + powerTime + " Distance: " + powerDistance + " <----------------" + " == PowerDistance: " + mPowerDistance );
		}
		
		public void NotifyActionIncomplete() 
		{
			Debug.LogWarning( "QuickTimeEvent cancelado: Necesita mas tiempo para interaccion: +" + (mCurrentTimerEnd - mCurrentTimer) );
			mCurrentTimer = mCurrentTimerEnd;
			NotifyPower(-1f);
		}
		
		private InteractiveActions mActions;
		private MatchManager mMatchManager;
		private BallMotor mBallMotor;
		
		private Transform mPowerActionTransform;
		private FilledBar mPowerGreenBar;
		private FilledBar mPowerRedBar;
		private UITexture mPowerMark;
		private TolerancePowerBar mTolerancePowerBar;
		
		private AnimationScript mSuccessAnimation;
		private AnimationScript mFailAnimation;
		
		private float mCurrentTimer;
		private float mCurrentTimer0;
		private float mCurrentTimerEnd;
		
		private bool mTap;	// Ha habido un tap en el ultimo frame?
				
		
		private float mPositionX;
		private float mPositionY;
		
		private float mPixelSizeAdjustment;
		
		private bool mPowerActive = false;
		private float mPowerStartTime = -1f;
		private float mPowerDistance;
		private float mPowerTime;
	}
}