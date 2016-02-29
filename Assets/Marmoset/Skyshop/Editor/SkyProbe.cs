// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
	
namespace mset {
	//TODO: bring glory and greatness to this class.
	public class SkyProbe {
		public Cubemap cube = null;
		
		public void capture(Transform at) { capture(ref cube, at); }
		public void capture(ref Cubemap targetCube, Transform at) {
			if( targetCube == null ) return;
			GameObject go = new GameObject("_temp_probe");
			go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideAndDontSave;
			go.SetActive(true);			
			Camera cam = go.AddComponent<Camera>();
			if( at != null ) {
				go.transform.position = at.position;
			}
			
			cam.RenderToCubemap(targetCube);
			targetCube.Apply(false);
			
			GameObject.DestroyImmediate(go);
		}
	};
}