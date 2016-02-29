using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Circle3DRenderer : GizmoRenderer {
		
		public GameObject GizmoPrototype;
		public float ScaleMin = 1f;
		float ScaleMax = 2f;
		public float Velocity = 2f;
		public float MaxTimeScaling = 1f;
		
		public bool IsActivated {
			get {
				return GizmoParent.activeSelf;
			}
		}
		
		protected GameObject GizmoParent;
		protected GameObject CircleScaling;
		protected GameObject CircleInsideRed;
		protected GameObject CircleInsideOn;
		protected GameObject CircleInsideOff;
		
		protected GameObject mSource;
		protected float mTimeAviso = 0.5f;
		protected float mTimeScaling = 0.5f;
		protected float mTimeDelay = 0.5f;
		protected float mTime = 0f;
		
		protected float mCurrentTimeAviso;
		protected float mCurrentTime0;
		protected float mCurrentTime1;
		
		// private BallMotor mBallMotor;
		private int mState = -1;
		
		protected virtual void Initialize () {
			if ( GizmoPrototype != null ) {
				GizmoParent = Instantiate( GizmoPrototype ) as GameObject;
				GizmoParent.transform.parent = transform;
			}
			else
				GizmoParent = gameObject;
			
			CircleScaling = Helper.FindTransform( GizmoParent.transform, "Scaling" ).gameObject;
			CircleInsideOn = Helper.FindTransform( GizmoParent.transform, "InsideOn" ).gameObject;
			CircleInsideOff = Helper.FindTransform( GizmoParent.transform, "InsideOff" ).gameObject;
			CircleInsideRed = Helper.FindTransform( GizmoParent.transform, "InsideRed" ).gameObject;
			
			CircleScaling.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			CircleInsideOn.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			CircleInsideOff.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			CircleInsideRed.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			
			//GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
			//mBallMotor = balon.GetComponent<BallMotor>();
			
			Deactivate();
		}
		
		void Awake () {
			Initialize ();
		}
		
		void Start () {
		}
		
		protected virtual void UpdateState() {
			float timeAviso = 0.3f;
			
			mTime += Time.deltaTime;
			
			// Queremos mostrar el aviso de la interaccion? (circulo central activo y circulo externo con accion escalado)
			// Si somos el propietario del balon y...
			bool avisoInteraccion = true; //(mBallMotor.NewPropietary == mSource);
			if ( avisoInteraccion && (mTime < mCurrentTimeAviso) ) {
				// ...y el tiempo es menor a un maximo determinado (para no avisar con demasiada antelacion)
				avisoInteraccion = ((mCurrentTimeAviso - mTime - timeAviso) < MaxTimeScaling);
			}
			
			if ( avisoInteraccion ) {
				if ( mTime < mCurrentTimeAviso ) {
					if ( mState != 0 ) {
						CircleScaling.SetActive( true );
						CircleInsideOff.SetActive( false );
						CircleInsideOn.SetActive( false );
						CircleInsideRed.SetActive ( true );
						mState = 0;
					}
					
					float scale = Mathf.Lerp ( ScaleMin, ScaleMax, (mCurrentTime0 - mTime)/mCurrentTime0 );
					CircleScaling.transform.localScale = new Vector3( scale, scale, scale );
				}
				else if ( mTime < mCurrentTime0 ) {
					if ( mState != 1 ) {
						CircleInsideOn.transform.localScale = new Vector3( ScaleMin-0.1f, ScaleMin-0.1f, ScaleMin-0.1f );
						AMTween.ScaleTo( CircleInsideOn, AMTween.Hash( 
							"scale",Vector3.one*ScaleMin, 
							"time",timeAviso + 0.2f, 
							"easetype","easeOutElastic" ) );
						
						CircleScaling.SetActive( true );
						CircleInsideOff.SetActive( false );
						CircleInsideOn.SetActive( true );
						CircleInsideRed.SetActive ( false );
						mState = 1;
					}
					
					float scale = Mathf.Lerp ( ScaleMin, ScaleMax, (mCurrentTime0 - mTime)/mCurrentTime0 );
					CircleScaling.transform.localScale = new Vector3( scale, scale, scale );
				}
				else if ( mTime < mCurrentTime1 ) {
					if ( mState != 2 ) {
						CircleInsideOn.transform.localScale = new Vector3( ScaleMin-0.1f, ScaleMin-0.1f, ScaleMin-0.1f );
						AMTween.ScaleTo( CircleInsideOn, AMTween.Hash(
							"scale",Vector3.one*ScaleMin, 
							"time",timeAviso + 0.2f, 
							"easetype","easeOutElastic" ) );
						CircleScaling.SetActive( false );
						CircleInsideOff.SetActive( false );
						CircleInsideOn.SetActive( true );
						mState = 2;
					}
				}
			}
			else {
				if ( mState != 3 ) {
					CircleScaling.SetActive( false );
					CircleInsideOff.SetActive( true );
					CircleInsideOn.SetActive( false );
					mState = 3;
				}
			}
		}
		
		void Update () {
			UpdateState ();
		}	
		
		void LateUpdate () {
			if ( mSource != null ) {
				Vector3 position = mSource.transform.position;
				position.y = 0.1f;
				GizmoParent.transform.position = position;
			}
		}
		
		void StopTweens () {
			AMTween tweenActivo = CircleInsideOn.GetComponent<AMTween>();
			if ( tweenActivo ) {
				Destroy( tweenActivo );
			}
		}
		
		public virtual void Activate( GameObject source, GameObject target, float timeTotal, float timeScaling, float timeDelay ) {
			Activate ( source, null, timeTotal, timeScaling, timeDelay );
		}
		
		public virtual void Activate( GameObject source, float timeTotal, float timeScaling, float timeDelay ) {
			StopTweens();
			
			timeScaling = Mathf.Clamp( timeScaling, 0f, 100f );
			timeDelay 	= Mathf.Clamp( timeDelay, 0f, 100f );
			
			mSource = source;
			mTimeAviso 		= timeTotal - timeScaling - timeDelay;
			mTimeScaling 	= timeScaling;
			mTimeDelay 		= timeDelay;
			mTime 			= 0f;
			mState 			= -1;
			
			mCurrentTimeAviso = mTimeAviso;
			mCurrentTime0 = mCurrentTimeAviso + mTimeScaling;
			mCurrentTime1 = mCurrentTime0 + mTimeDelay;
			
			if ( Velocity <= 0f ) Velocity = 0.001f;
			ScaleMax = ScaleMin + (mTimeAviso + mTimeScaling) / Velocity;
			
			CircleScaling.SetActive( false );
			CircleInsideOff.SetActive( false );
			CircleInsideOn.SetActive( false );
			CircleInsideRed.SetActive ( false );
			
			CircleScaling.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			CircleInsideOff.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			CircleInsideOn.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
			
			GizmoParent.SetActive( true );
		}
		
		public virtual void Deactivate() {
			StopAllCoroutines();
			
			mSource = null;
			GizmoParent.SetActive(false);
			
			StopTweens();
		}
		
		public bool IsPointInRadio( Vector2 point, float radio ) {
			Vector3 onScreen = Camera.main.WorldToScreenPoint(transform.position);
			Vector2 position2D = new Vector2( onScreen.x, onScreen.y );
			
			float distance2 = (point - position2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
	}
	
}

