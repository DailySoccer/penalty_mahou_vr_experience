using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class CircleArrow3DRenderer : Circle3DRenderer {
		
		private GameObject PlaneArrow;
		
		private GameObject mTarget;
		
		protected override void Initialize () {
			base.Initialize();
			PlaneArrow = Helper.FindTransform( GizmoParent.transform, "Arrow" ).gameObject;
			PlaneArrow.transform.localScale = new Vector3( ScaleMin, ScaleMin, ScaleMin );
		}
		
		void Awake () {
			Initialize ();
		}
		
		void Start () {
		}
		
		protected override void UpdateState() {
			base.UpdateState();
			
			if ( mTime < mCurrentTime1 ) {
				if ( mTarget )
					PlaneArrow.SetActive ( true );
			}
			
			if ( PlaneArrow.activeSelf && mSource != null && mTarget != null ) {
				Vector3 dirToTarget = Helper.ZeroY( mTarget.transform.position - mSource.transform.position );
				Quaternion rotArrow = PlaneArrow.transform.localRotation;
				rotArrow = Quaternion.LookRotation( -dirToTarget );
				PlaneArrow.transform.localRotation = rotArrow;
			}
		}
		
		void Update () {
			UpdateState ();
		}	
		
		public override void Activate( GameObject source, GameObject target, float timeTotal, float timeScaling, float timeDelay ) {
			base.Activate ( source, timeTotal, timeScaling, timeDelay );
			mTarget = (target != source) ? target : null;
			PlaneArrow.SetActive ( false );
		}
	}
	
}
