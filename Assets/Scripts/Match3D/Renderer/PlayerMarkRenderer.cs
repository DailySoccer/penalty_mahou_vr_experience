using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PlayerMarkRenderer : GizmoRenderer {
		
		public UITexture TextureMark;
		public float DisplayHeight = 1.8f;
		
		private GameObject mTarget = null;
		[HideInInspector] public GameObject Target {
			get { return mTarget; }
			set {
				if ( mTarget != value ) {
					mTarget = value;
					TextureMark.gameObject.SetActive( mTarget != null );
				}
			}
		}
	
		private MatchManager mMatchManager;
	
		private float mPixelSizeAdjustment;
		
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			
			mPixelSizeAdjustment = (mMatchManager.GetComponentInChildren<UIRoot>() as UIRoot).pixelSizeAdjustment;
			TextureMark.gameObject.SetActive( false );
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		void LateUpdate() {
			if ( Target ) {
				Vector3 targetPos = Target.transform.position;
				targetPos.y = DisplayHeight;
				
				Vector3 onScreen = Camera.main.WorldToScreenPoint(targetPos);
				
				Vector3 dim = TextureMark.transform.localScale;
				onScreen.y = onScreen.y + dim.y * 0.5f * mPixelSizeAdjustment;
				
				float positionX = onScreen.x;
				float positionY = onScreen.y;
				
				// Pasamos al viewport ortografico
				var adjustedX = (positionX - Screen.width * 0.5f) * mPixelSizeAdjustment;
				var adjustedY = (positionY - Screen.height * 0.5f) * mPixelSizeAdjustment;
				
				TextureMark.gameObject.SetActive( true );
				TextureMark.transform.localPosition = new Vector3( adjustedX, adjustedY, 0f );
			}
			else {
				TextureMark.gameObject.SetActive( false);
			}
		}
	}
	
}
