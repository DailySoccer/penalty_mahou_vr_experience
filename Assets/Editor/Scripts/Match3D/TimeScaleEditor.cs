using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FootballStar.Match3D {

	[CustomEditor(typeof(TimeScale))]
	public class TimeScaleEditor : Editor
	{
		public override void OnInspectorGUI () {
			TimeScale timeScale = target as TimeScale;
			
			timeScale.factor = EditorGUILayout.Slider ("Scale", timeScale.factor, 0, 1);
				
			if ( GUI.changed ) {
				EditorUtility.SetDirty ( timeScale );
			}
		}
	}
	
}

