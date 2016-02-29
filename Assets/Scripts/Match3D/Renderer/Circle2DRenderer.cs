using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Circle2DRenderer : GizmoRenderer {
		
		public Transform GizmoParent;
		
		public float MinSize = 1.0f;
		public float MaxSize = 2.0f;
		
		public float DisplayHeight = 1.8f;
		public float RotSpeedTextureA = 100.0f;
		
		public bool IsActivated = false;
		/*
		{
			get {
				return GizmoParent.gameObject.activeSelf;
			}
		}
		*/
		
		enum States {
			OFF,
			START,
			INVALID,
			SCALING,
			REACTION,
			ERROR,
			FADEOUT
		};
		States mState = States.OFF;
		public bool Success {
			get { return (mState == States.SCALING) || (mState == States.REACTION); }
		}
		
		public bool IsDeactivate {
			get { return (mState == States.OFF || mState == States.FADEOUT); }
		}
		
		void Awake () {
			if ( GizmoParent == null )
				GizmoParent = transform;
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mPixelSizeAdjustment = mMatchManager.GetComponentInChildren<UIRoot>().pixelSizeAdjustment;
			
			// mUIPanel = GetComponent<UIPanel>();
			
			// TextureIcon = Helper.GetComponent<UITexture>( GizmoParent.transform, "Icon" );
			TextureScalingOn = Helper.GetComponent<UITexture>( GizmoParent.transform, "ScalingOn" );
			TextureScalingOff = Helper.GetComponent<UITexture>( GizmoParent.transform, "ScalingOff" );
			TextureInsideOff = Helper.GetComponent<UITexture>( GizmoParent.transform, "InsideOff" );
			TextureInsideOn = Helper.GetComponent<UITexture>( GizmoParent.transform, "InsideOn" );
			
			mDimensionMin = TextureInsideOn.transform.localScale * MinSize;
			mDimensionMax = TextureInsideOn.transform.localScale * MaxSize;
			
			Stop();
		}
		
		void Start () {
		}
		
		void ChangeVisibility( States toState ) {
			//mForcePanelRefresh = true;
			//Debug.Log( "" + Time.time + ": Circle2D: " + mCurrentTimer + " " + toState );
			
			if (toState == States.INVALID) {
				/**** BEG: nueva version *****/
				//TextureScalingOff.gameObject.SetActive( true );
				TextureScalingOff.gameObject.SetActive( false );
				TextureInsideOff.gameObject.SetActive( true );
				/**** END: nueva version *****/
			}
			else if (toState == States.SCALING) {
				/**** BEG: nueva version *****/
				//TextureScalingOn.gameObject.SetActive( true );	
				TextureScalingOn.gameObject.SetActive( false );
				TextureInsideOn.gameObject.SetActive( true );
				TextureScalingOff.gameObject.SetActive( false );
				TextureInsideOff.gameObject.SetActive( false );
				/**** END: nueva version *****/
			}
			else if (toState == States.REACTION) {
				/**** BEG: nueva version *****/
				TextureScalingOff.gameObject.SetActive( false );
				//TextureScalingOn.gameObject.SetActive( true );
				TextureScalingOn.gameObject.SetActive( false );
				TextureInsideOff.gameObject.SetActive( false );
				TextureInsideOn.gameObject.SetActive( true );
				/**** END: nueva version *****/
				
			}
			else if (toState == States.ERROR) {
				/**** BEG: nueva version *****/
				//TextureScalingOff.gameObject.SetActive( true );
				TextureScalingOff.gameObject.SetActive( false );
				TextureScalingOn.gameObject.SetActive( false );
				TextureInsideOff.gameObject.SetActive( true );
				TextureInsideOn.gameObject.SetActive( false );
				/**** END: nueva version *****/
				
			}
		}
		
		void RefreshState() {
			if ( mTarget != null ) {
				
				// mCurrentTimer += Time.deltaTime;
				UpdateTimer ();
				
				if ( mCurrentTimer < mCurrentTimer0 ) {
					if ( mState != States.INVALID ) {
						ChangeVisibility(States.INVALID);
					}
					mState = States.INVALID;
				}
				else if ( mCurrentTimer < mCurrentTimer1 ) {
					if ( mState != States.SCALING ) {
						ChangeVisibility(States.SCALING);
					}
					mState = States.SCALING;
				}
				else if ( mCurrentTimer < mCurrentTimerEnd ) {
					if ( mState != States.REACTION ) {
						ChangeVisibility(States.REACTION);
					}
					mState = States.REACTION;
				}
				else {
					if ( mState != States.ERROR ) {
						ChangeVisibility(States.ERROR);
					}
					mState = States.ERROR;
				}
			}
		}
		
		void UpdateTimer () {
			float timerSync = TimerSync(mCurrentTimer);
			mDeltaTimer 	= timerSync - mCurrentTimer;
			mCurrentTimer 	= timerSync;
		}
		
		//bool isPerfect = false;
		void Update () {
			if ( mState == States.OFF )
				return;
			
			if ( mState == States.FADEOUT )
				return;
			
			if ( mTarget != null ) {
				// mForcePanelRefresh = false;
				
				RefreshState();
				
				/*
				if ( IsPerfect( mCurrentTimer ) ) {
					TextureInsideOn.gameObject.SetActive( false );
					TextureInsideOff.gameObject.SetActive( true );
					if ( !isPerfect )
						UnityEngine.Debug.Break();
					isPerfect = true;
				}
				else {
					TextureInsideOn.gameObject.SetActive( true );
					TextureInsideOff.gameObject.SetActive( false );
					if ( isPerfect ) 
						UnityEngine.Debug.Break();
					isPerfect = false;
				}
				*/
			}
		}
		
		void LateUpdate() {
			if ( mState == States.OFF )
				return;
			
			if ( mTarget != null ) {
				Vector3 soccerPos = mTarget.transform.position;
				soccerPos.y = DisplayHeight;
				
				Vector3 onScreen = Camera.main.WorldToScreenPoint(soccerPos);
				
				// Clipeado
				float width = (float) TextureInsideOn.width;
				mPosition2D.x = Mathf.Clamp(onScreen.x, width * 0.5f, Screen.width - width * 0.5f);
				mPosition2D.y = Mathf.Clamp(onScreen.y, width * 0.5f, Screen.height - width * 0.5f);
				
				if ( mState != States.FADEOUT ) {
					mTextureASize -= mDeltaScale * mDeltaTimer;
					mRotationTextureA += RotSpeedTextureA * mDeltaTimer;
				}
				
				UpdateGizmos( mPosition2D );
				
				/*
				if ( mForcePanelRefresh )
					mUIPanel.Refresh();
				*/
			}
		}
		
		private void UpdateGizmos( Vector2 position ) {
			// Pasamos al viewport ortografico
			var adjustedX = (position.x - Screen.width * 0.5f) * mPixelSizeAdjustment;
			var adjustedY = (position.y - Screen.height * 0.5f) * mPixelSizeAdjustment;
			
			GizmoParent.localPosition = new Vector3(adjustedX, adjustedY, 0.0f);
			
			// if (mState == States.INVALID || mState == States.SCALING) {
			if ( true ) {
				// Scale
				Vector3 scale = new Vector3(mTextureASize, mTextureASize, 1.0f);
				TextureScalingOff.transform.localScale = scale;	
				TextureScalingOn.transform.localScale = scale;				
				
				// Rotate
				Vector3 eulerAngles = new Vector3(0, 0, mRotationTextureA);
				TextureScalingOff.transform.localEulerAngles = eulerAngles;
				TextureScalingOn.transform.localEulerAngles = eulerAngles;
			}
		}
		
		public void Activate( GameObject target, float timeTotal, float timeScaling, float timeDelay ) {
			mTimeInvalid = timeTotal - (timeScaling + timeDelay);
			mTimeScaling = timeScaling;
			mTimeDelay = timeDelay;
			
			// Si estamos realizando un Fade Out (de una acción anterior)
			if ( mState == States.FADEOUT ) {
				// Damos por terminado el Fade Out
				TweenAlpha tweenAlpha = GetComponent<TweenAlpha>();
				if ( tweenAlpha )
					tweenAlpha.enabled = false;
			}
			
			mState = States.START;
			mTarget = target;
			
			TimerReset();
			mCurrentTimer = 0f;
			mCurrentTimer0 = mTimeInvalid;
			mCurrentTimer1 = mCurrentTimer0 + mTimeScaling;
			mCurrentTimerEnd = mCurrentTimer1 + mTimeDelay;
			mDeltaTimer = 0;
			
			// TextureIcon.gameObject.SetActive( true );
			TextureScalingOn.gameObject.SetActive( false );
			TextureScalingOff.gameObject.SetActive( false );
			TextureInsideOn.gameObject.SetActive( false );
			TextureInsideOff.gameObject.SetActive( false );
			
			mDeltaScale = (mDimensionMax.x - mDimensionMin.x) / (mTimeInvalid + mTimeScaling);
			mTextureASize = mDimensionMax.x;
			
			TextureInsideOff.transform.localScale = mDimensionMin;
			TextureInsideOn.transform.localScale = mDimensionMin;
			
			//GizmoParent.gameObject.SetActive( true );
			IsActivated = true;
			enabled = true;
			
			UIPanel panel = GetComponent<UIPanel>();
			panel.alpha = 1f;
			
			StopAllCoroutines();
		}
		
		public void Stop() {
			//GizmoParent.gameObject.SetActive( false );
			IsActivated = false;
			enabled = false;
			
			mState = States.OFF;
		}
		
		public void Deactivate() {
			if ( IsDeactivate )
				return;
			
			StopAllCoroutines();
			
			RefreshState();
			
			mState = States.FADEOUT;
			TweenAlpha.Begin ( gameObject, 0.3f, 0f );
		}	
		
		public bool IsPointInRadio( Vector2 point, float radio ) {
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
		public bool IsPointInside( Vector2 point ) {
			float radio = mTextureASize * 0.5f * mPixelSizeAdjustment;
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
		private MatchManager mMatchManager;
		private GameObject mTarget;
		
		//private UIPanel mUIPanel;
		//private bool mForcePanelRefresh;
		
		// private UITexture TextureIcon;
		private UITexture TextureScalingOn;
		private UITexture TextureScalingOff;
		private UITexture TextureInsideOn;
		private UITexture TextureInsideOff;
		
		private Vector3 mDimensionMin;
		private Vector3 mDimensionMax;
		
		private float mTimeInvalid;
		private float mTimeScaling;
		private float mTimeDelay;
		
		private float mDeltaTimer;
		private float mCurrentTimer;
		private float mCurrentTimer0;
		private float mCurrentTimer1;
		private float mCurrentTimerEnd;
		
		private bool mTap;	// Ha habido un tap en el ultimo frame?
		
		private Vector2 mPosition2D;
		private float mRotationTextureA;
		
		private bool  mShowTextureA;
		private float mTextureASize;
		private float mDeltaScale;
		
		private float mPixelSizeAdjustment;
	}
	
}
