using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {

	public class TimelineRenderer : MonoBehaviour {
	
		public Transform GizmoParent;
		
		public bool IsActivated {
			get {
				return GizmoParent.gameObject.activeSelf;
			}
		}
		
		void Awake () {
			if ( GizmoParent == null )
				GizmoParent = transform;
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			// mPixelSizeAdjustment = mMatchManager.GetComponentInChildren<UIRoot>().pixelSizeAdjustment;
			
			mTextureMetaOn = Helper.GetComponent<UITexture>( GizmoParent, "MetaOn" );
			mTextureMetaOff = Helper.GetComponent<UITexture>( GizmoParent, "MetaOff" );
			// mTextureBalon = Helper.GetComponent<UITexture>( GizmoParent, "IconBalon" );
			mTextureArrow = Helper.GetComponent<UITexture>( GizmoParent, "IconArrow" );
			mTexturePase = Helper.GetComponent<UITexture>( GizmoParent, "IconPase" );
			mTextureRegate = Helper.GetComponent<UITexture>( GizmoParent, "IconRegate" );
			mTextureRemate = Helper.GetComponent<UITexture>( GizmoParent, "IconRemate" );
			
			mPaseCached = new TextureCached( mTexturePase );
			mRegateCached = new TextureCached( mTextureRegate );
			mRemateCached = new TextureCached( mTextureRemate );
			
			mDirectionGesture = mMatchManager.GetComponent<DirectionGesture>();
			
			Deactivate();
		}
		
		void Start () {
		}
		
		/*
		void Update () {
		}
		*/
		
		void UpdateActionIcon(UITexture icon, TweenAction tweenAction) {
			float widthMeta = mTextureMetaOn.transform.localScale.x + icon.transform.localScale.x;
			float xMeta = mTextureMetaOn.transform.localPosition.x + widthMeta * 0.5f;

			float timeInteraction = mInteractiveActions.TimeInteraction( tweenAction );
			float factorTimeToWidth = widthMeta / timeInteraction;
			
			float diffTime = mInteractiveActions.TimeForAction( tweenAction ) - Time.time;
			
			Vector3 posIcon = icon.transform.localPosition;
			posIcon.x = xMeta - (diffTime * factorTimeToWidth);
			icon.transform.localPosition = posIcon;
		}
			
		void Update() {
			bool interactionOn = false;
			float xPositionPrev = 1000f;
			for (int i=0; i<mInteractiveActions.Actions.Count; i++) {
				TweenAction tweenAction = mInteractiveActions.Actions[i];
				if ( tweenAction.State != TweenAction.EState.Evaluated ) {
					interactionOn = interactionOn || tweenAction.InteractionOn;
					
					UITexture icon = mTextureList[i];
					
					// Las acciones de Chut/Remate se transforman en Flecha...
					if ( tweenAction.IsType(ActionType.CHUT) || tweenAction.IsType(ActionType.REMATE) ) {
						if ( mDirectionGesture.IsActivated ) {
							if ( !mTextureArrow.gameObject.activeSelf ) {
								icon.gameObject.SetActive( false );
								mTextureArrow.gameObject.SetActive( true );
							
								Quaternion rotArrow = mTextureArrow.transform.localRotation;
								float angle = mDirectionGesture.AngleDirectionSuccess;
								rotArrow = Quaternion.AngleAxis( angle, new Vector3(0,0,-1) );
								mTextureArrow.transform.localRotation = rotArrow;
							}
							icon = mTextureArrow;
						}
					}
					
					UpdateActionIcon( icon, tweenAction );
					
					if ( xPositionPrev < icon.transform.localPosition.x ) {
						icon.gameObject.SetActive( false );
					}
					else {
						icon.gameObject.SetActive( true );
						xPositionPrev = icon.transform.localPosition.x;
					}
				}
				else {
					xPositionPrev = 1000f;
					mTextureList[i].gameObject.SetActive( false );
				}
			}
			mTextureMetaOff.gameObject.SetActive( !interactionOn );
			mTextureMetaOn.gameObject.SetActive( interactionOn );
		}
		
		UITexture TextureForAction( TweenAction tweenAction ) {
			return mRemateCached.New(); 
		}
		
		public void Activate( InteractiveActions interactiveActions ) {
			StopAllCoroutines();
			
			mInteractiveActions = interactiveActions;
			
			mPaseCached.Reset ();
			mRegateCached.Reset ();
			mRemateCached.Reset ();
			
			mTextureList.Clear ();
			foreach ( TweenAction tweenAction in mInteractiveActions.Actions ) {
				UITexture textureForAction = TextureForAction( tweenAction );
				mTextureList.Add ( textureForAction );
				textureForAction.gameObject.SetActive( true );
			}
						
			mTextureMetaOff.gameObject.SetActive( true );
			mTextureMetaOn.gameObject.SetActive( false );
			
			GizmoParent.gameObject.SetActive( true );
		}
		
		void StopTweens ( GameObject go ) {
			AMTween[] tweenActivos = go.GetComponents<AMTween>();
			foreach( AMTween tween in tweenActivos ) {
				if ( tween ) {
					Destroy( tween );
				}
			}
		}		
		
		public void Deactivate() {
			StopAllCoroutines();
			
			StopTweens( gameObject );
			
			
			GizmoParent.gameObject.SetActive( false );
			mTextureArrow.gameObject.SetActive( false );
		}	
		
		private MatchManager mMatchManager;
		private InteractiveActions mInteractiveActions;
		
		class TextureCached {
			public TextureCached( UITexture texturePrefab ) {
				mTexturePrefab = texturePrefab;
			}
			
			public void Reset() {
				foreach( UITexture texture in mList ) {
					texture.gameObject.SetActive( false );
				}
				mIndex = 0;
			}
			
			public UITexture New() {
				UITexture texture = null;
				if ( mIndex < mList.Count ) {
					texture = mList[mIndex];
				}
				else  {
					GameObject gameObjectTexture = Instantiate(mTexturePrefab.gameObject) as GameObject;
					texture = gameObjectTexture.GetComponent<UITexture>();
					mList.Add ( texture );
					
					texture.gameObject.transform.parent = mTexturePrefab.gameObject.transform.parent;
					texture.transform.localPosition = mTexturePrefab.transform.localPosition;
					texture.transform.localScale = mTexturePrefab.transform.localScale;
					texture.gameObject.SetActive( true );
				}
				mIndex++;
				return texture;
			}
			
			UITexture mTexturePrefab;
			List<UITexture> mList = new List<UITexture>();
			int mIndex = 0;
		}
		
		private UITexture mTextureMetaOn;
		private UITexture mTextureMetaOff;
		// private UITexture mTextureBalon;
		private UITexture mTextureArrow;
		private UITexture mTexturePase;
		private UITexture mTextureRegate;
		private UITexture mTextureRemate;
		private List<UITexture> mTextureList = new List<UITexture>();
		private TextureCached mPaseCached;
		private TextureCached mRegateCached;
		private TextureCached mRemateCached;
		private DirectionGesture mDirectionGesture;
		
		private Vector3 mDimension2D;
		
		//private float mPixelSizeAdjustment;
	}
	
}