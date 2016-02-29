using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class CircleOnGroundRenderer : GizmoRenderer {
		
		public GameObject GizmoPrototype;
		public bool IsActivated {
			get {
				return GizmoParent.gameObject.activeSelf;
			}
		}

		private GameObject GizmoParent;
		private GameObject PlaneA;
		private float mRadio = 1f;
		
		void Awake () {
			if ( GizmoPrototype != null ) {
				GizmoParent = Instantiate( GizmoPrototype ) as GameObject;
				GizmoParent.transform.parent = transform;
				
				Transform planeTransform = Helper.FindTransform( GizmoParent.transform, "Plane" );
				PlaneA = planeTransform.gameObject;
				
				Deactivate();
			}
		}
		
		void Start () {
		}
		
		void Update () {
		}
		
		public void Activate( Vector3 position, float radio ) {
			GizmoParent.transform.position = position;
			mRadio = radio;
			
			Vector3 scale = PlaneA.transform.localScale;
			scale.x = GizmoParent.transform.localScale.x * mRadio;
			scale.z = GizmoParent.transform.localScale.z * mRadio;
			PlaneA.transform.localScale = scale;
			
			GizmoParent.gameObject.SetActive( true );
		}
		
		public void Deactivate() {
			GizmoParent.gameObject.SetActive(false);
		}
	}
	
}
