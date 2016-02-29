using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class CircleArrow2DRenderer : MonoBehaviour {
	
		public Transform GizmoParent;
		
		public float DisplayHeight = 1.8f;
		
		public bool IsActivated {
			get {
				return GizmoParent.gameObject.activeSelf;
			}
		}

		public Vector2 Position2D {
			get { return mPosition2D; }
		}
		
		void Awake () {
			if ( GizmoParent == null )
				GizmoParent = transform;
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mPixelSizeAdjustment = mMatchManager.GetComponentInChildren<UIRoot>().pixelSizeAdjustment;
			
			mTextureArrow = Helper.GetComponent<UITexture>( GizmoParent, "CircleArrow" );

			Deactivate();
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		void LateUpdate() {
			if ( mTarget != null ) {
				Vector3 soccerPos = mSource.transform.position;
				soccerPos.y = DisplayHeight;
	
				Vector3 onScreen = Camera.main.WorldToScreenPoint(soccerPos);
				
				mPosition2D.x = onScreen.x;
				mPosition2D.y = onScreen.y;
	
				UpdateGizmos( mPosition2D );
			}
		}
		
		private void UpdateGizmos( Vector2 position ) {
			// Pasamos al viewport ortografico
			var adjustedX = (position.x - Screen.width * 0.5f) * mPixelSizeAdjustment;
			var adjustedY = (position.y - Screen.height * 0.5f) * mPixelSizeAdjustment;
			
			GizmoParent.localPosition = new Vector3(adjustedX, adjustedY, 0.0f);
		}

		public void Activate( GameObject source, GameObject target ) {
			mSource = source;
			mTarget = target;
			
			mTextureArrow.gameObject.SetActive( true );
			
			GizmoParent.gameObject.SetActive( true );
			
			// Orientar la flecha hacia el objetivo (en coords. de pantalla)
			/*
			Vector3 sourceOnScreen = Camera.main.WorldToScreenPoint(mSource.transform.position);
			Vector3 targetOnScreen = Camera.main.WorldToScreenPoint(mTarget.transform.position);
			Vector3 dirToTarget = targetOnScreen - sourceOnScreen;
			dirToTarget.z = 0f;
			
			float angle = Vector3.Angle( new Vector3(0,1,0), dirToTarget );
			rotArrow = Quaternion.AngleAxis( angle, new Vector3(0,0,-1) );
			*/
			Vector3 dirToTarget = Helper.ZeroY ( mTarget.transform.position - mSource.transform.position );

			float angle = Vector3.Angle( new Vector3(0,0,1), dirToTarget );

			mTextureArrow.transform.localRotation = Quaternion.AngleAxis( angle, new Vector3(0,0,-1) );
			// Debug.Log ( "Angle: " + angle );
		}
		
		public void Deactivate() {
			GizmoParent.gameObject.SetActive( false );
		}	
		
		public bool IsPointInRadio( Vector2 point, float radio ) {
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		
		/*
		public bool IsPointInside( Vector2 point ) {
			float radio = mTextureASize * 0.5f * mPixelSizeAdjustment;
			float distance2 = (point - mPosition2D).sqrMagnitude;
			return distance2 < (radio * radio);
		}
		*/
		
		private MatchManager mMatchManager;
		private GameObject mSource;
		private GameObject mTarget;
		
		private UITexture mTextureArrow;

		private Vector2 mPosition2D;
		
		private float mPixelSizeAdjustment;
	}
	
}