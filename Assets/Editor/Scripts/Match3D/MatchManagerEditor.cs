using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using JsonFx.Json;

namespace FootballStar.Match3D {

	[CustomEditor(typeof(MatchManager))]
	public class MatchManagerEditor : Editor {
		
		public void Awake() {
			if ( EditorApplication.isPlayingOrWillChangePlaymode == false ) {
				readUltimoPartido();
			}
	    }	
	 
		public void OnDestroy() {
		}
		
		public void OnSceneGUI() {
#if REGISTER_GAME_EVENTS
			MatchManager matchManager = target as MatchManager;
			
			foreach ( MatchManager.GameObjectData data in matchManager.Data ) {
				Vector3 position = data.position.V3;
				if ( data.mensaje != "" ) {
					Handles.color = Color.blue;
					Handles.Label ( position + new Vector3(0,1f,0), data.mensaje );
					Handles.Label ( position + new Vector3(0,0.5f,0), data.time.ToString("####0.00") );
					Handles.ConeCap( 0, position, Quaternion.LookRotation (Vector3.up), 0.3f );
				}
				else {
					Handles.color = Color.red;
					Handles.Label ( position + new Vector3(0,0.5f,0), data.time.ToString("####0.00") );
					Handles.ConeCap( 0, position, Quaternion.LookRotation (Vector3.up), 0.2f );
				}
			}
#endif
			
			//Debug.Log("onSceneGUI");
		}
		
		private void readUltimoPartido() {
			
#if REGISTER_GAME_EVENTS			
			MatchManager matchManager = target as   MatchManager;
				
			string assetPath = Application.dataPath + "/Scripts/Match3D/";
			FileStream fs = new FileStream(assetPath + "SoccerGame.json", FileMode.Open, FileAccess.Read);
	        StreamReader sr = new StreamReader(fs);
			
			string texto = sr.ReadToEnd();
			JsonReader reader = new JsonReader(texto);  
			
			MatchManager.GameObjectData[] info = reader.Deserialize<MatchManager.GameObjectData[]>();
			foreach ( MatchManager.GameObjectData data in info ) {
				matchManager.Data.Add ( data );
			}
					
	        sr.Close();
	        fs.Close();
#endif
		}
	}
	
}
