using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FootballStar.Match3D {

	[CustomEditor(typeof(Jugada))]
	public class JugadaEditor : Editor
	{
		public override void OnInspectorGUI () {
			Jugada jugada = target as Jugada;
			
			GameObject go = GameObject.Find ("AnimatorData");
			AnimatorData aData = (AnimatorData) go.GetComponent ("AnimatorData");
			
			jugada.animatorTake = EditorGUILayout.Popup ( jugada.animatorTake, aData.getTakeNames() );
			
			if ( GUI.changed ) {
				EditorUtility.SetDirty ( jugada );
			}
		}
	}
	
}

