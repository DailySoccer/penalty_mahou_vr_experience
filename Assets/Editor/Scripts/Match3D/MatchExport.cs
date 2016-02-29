using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using JsonFx.Json;
using System.IO;
using System.Linq;

public class MatchExport {
	static string mFolderName = "Assets/Scenes/Resources";
	
	static List<GameObject> mJugadas = new List<GameObject>();
	
	static void RegisterJugadas() {
		mJugadas.Clear ();
		
		GameObject listaJugadas = GameObject.Find ( "Lista Jugadas" );
		
		for ( int i=0; i<listaJugadas.transform.childCount; i++ ) {
			Transform child = listaJugadas.transform.GetChild(i);
			if ( child.tag == "Jugada" ) {
				mJugadas.Add ( child.gameObject );
			}
		}
	}
	
	public static void ExportAll() {
		RegisterJugadas();
		
		AnimatorData animatorData = AnimatorTimeline.aData as AnimatorData;
		foreach ( AMTake take in animatorData.takes ) {
			Export ( take );
		}
		
		// refresh project directory
		AssetDatabase.Refresh();
	}
	
	public static void ExportCurrentTake() {
		RegisterJugadas();
		
		AnimatorData animatorData = AnimatorTimeline.aData as AnimatorData;
		AMTake take = animatorData.getCurrentTake();
		Export( take );
		
		// refresh project directory
		AssetDatabase.Refresh();
	}
	
	static void Export( AMTake take ) {
		GameObject jugada = mJugadas.Find ( play => { return play.name == take.name; } );
		if ( jugada != null ) {
			
			// Crear el directorio con el nombre de la Escena
			string sceneName = Path.GetFileNameWithoutExtension( EditorApplication.currentScene );
			string path = mFolderName + "/" + sceneName + "/";
			
			string fullPath = Application.dataPath + path.TrimStart("Assets".ToCharArray());
			if ( !System.IO.Directory.Exists(fullPath) ) {
				string guid = AssetDatabase.CreateFolder( mFolderName, sceneName );
				path = AssetDatabase.GUIDToAssetPath(guid) + "/";
			}

			int numberOfInteractiveActions = 0;
			int numPases 	= 0, 
				numRegates 	= 0, 
				numChuts 	= 0;

			// Convertir el take en JSON
			string json = ExportTake( path, take, out numberOfInteractiveActions, out numPases, out numRegates, out numChuts );
			if ( json != null ) {
				// Asociar el take, en formato JSON, a un componente
				TakeJSON takeJSON = jugada.GetComponent<TakeJSON>();
				if ( takeJSON == null ) {
					takeJSON = jugada.AddComponent( typeof(TakeJSON) ) as TakeJSON;
				}
				else {
					Assert.Test ( ()=> { return jugada.GetComponents<TakeJSON>().Length == 1; }, jugada.name + " tiene varios componentes TakeJSON" );
				}
				takeJSON.Text = json;
				takeJSON.NumInteractiveActions = numberOfInteractiveActions;
				takeJSON.NumPases 	= numPases;
				takeJSON.NumRegates = numRegates;
				takeJSON.NumChuts 	= numChuts;
				
				// Crear un prefab con la jugada (que incluye el componente con JSON)
				CreatePrefab( path, take.name, jugada );
			}
		}
		else {
			Debug.LogWarning ( take.name + " no existe" );
		}
		
	}

	static void CreatePrefab( string path, string prefabName, GameObject gameObject ) {
		string namePrefab = prefabName + ".prefab";
		Object prefab = AssetDatabase.LoadAssetAtPath( path + namePrefab, typeof(GameObject) ) as GameObject;
		
		if ( prefab == null ) {
			prefab = PrefabUtility.CreateEmptyPrefab(path + namePrefab);
			// Debug.Log ( "Create Prefab: Jugada: " + prefabName );
		}
		else {
			// Debug.Log ( "Replace Prefab: Jugada: " + prefabName );
		}
		
	    PrefabUtility.ReplacePrefab( gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab );
	}

	static string GetMethodName( AnimatorTimeline.JSONAction action ) {
		string methodName = "";
		switch( action.method ) {
		case "sendmessage": 	methodName = action.strings[0]; break;
		case "invokemethod":	methodName = action.strings[1]; break;
		}
		return methodName;
	}

	static string ExportTake(string path, AMTake take, out int numberOfInteractiveActions, out int numPases, out int numRegates, out int numChuts) {
		numberOfInteractiveActions = numPases = numRegates = numChuts = 0;

		string takeName = take.name;
		string saveJSONPath =  path + takeName + ".txt";
		if(saveJSONPath == "") return null;
		
		// start serialization
		AnimatorTimeline.JSONTake j = new AnimatorTimeline.JSONTake();
		j.takeName = takeName;
		List<AnimatorTimeline.JSONInit> lsInits = new List<AnimatorTimeline.JSONInit>();
		List<AnimatorTimeline.JSONAction> lsActions = new List<AnimatorTimeline.JSONAction>();
		foreach(AMTrack track in take.trackValues) {
			// set initial values
			AnimatorTimeline.JSONInit init = track.getJSONInit();
			if(init != null) lsInits.Add(init);
			// set actions
			foreach(AMAction action in track.cache) {
				AnimatorTimeline.JSONAction a = action.getJSONAction(take.frameRate);
				if(a != null) lsActions.Add(a);
			}
		}
		j.inits = lsInits.ToArray();
		j.actions = lsActions.ToArray();

		foreach( AnimatorTimeline.JSONAction action in j.actions ) {
			string methodName = GetMethodName( action );
			if ( methodName != "" ) {
				if ( FootballStar.Match3D.TweenAction.IsInteractiveAction( methodName ) ) {
					if ( FootballStar.Match3D.TweenAction.IsPase( methodName ) ) {
						numPases++;
					}
					else if ( FootballStar.Match3D.TweenAction.IsRegate( methodName ) ) {
						numRegates++;
					}
					else if ( FootballStar.Match3D.TweenAction.IsChut( methodName ) ) {
						numChuts++;
					}
					numberOfInteractiveActions++;
				}
			}
		}
		// Debug.Log( "Number Of Interactive Actions: " + numberOfInteractiveActions );

		// serialize json
		string json = JsonWriter.Serialize(j);
		// write json to file
		// File.WriteAllText(saveJSONPath, json);
		// refresh project directory
		// AssetDatabase.Refresh();
		
		Debug.Log ( "Exporting Take: " + takeName );
		
		return json;
	}	

}
