// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections; 

//A collection of handy menu functions 
public class SkyshopUtil {
	[MenuItem("Edit/Skyshop/Refresh Scene Skies %#r",false,1000)]
	public static void RefreshSkies() {
		Undo.RegisterSceneUndo("Refresh Skies");
		mset.Sky[] skies = GameObject.FindObjectsOfType(typeof(mset.Sky)) as mset.Sky[];
		mset.Sky currSky = mset.Sky.activeSky;
		for(int i=0; i<skies.Length; ++i) {
			if( skies[i] ) {
				mset.Sky sky = skies[i];
				mset.SkyInspector.detectColorSpace(ref sky);
				sky.Apply();
			}
		}
		if( currSky ) currSky.Apply();
	}
	
	[MenuItem("Edit/Skyshop/Upgrade Scene Skies",false,1001)]
	public static void UpgradeSkies() {
		Undo.RegisterSceneUndo("Upgrade Skies");
		Component[] all = GameObject.FindObjectsOfType(typeof(Transform)) as Component[];
		
		//Create a dummy game object, add a namespaced Sky to it, find its serialized script type
		GameObject refObj = new GameObject("_dummy_sky");
		mset.Sky refSky = refObj.AddComponent<mset.Sky>();
		SerializedObject refSr = new SerializedObject(refSky);
		SerializedProperty scriptType = refSr.FindProperty("m_Script");
		
		int count = 0;
		//Find all old sky objects, swap out the Sky script references to mset.Sky
		for(int i=0; i<all.Length; ++i) {
			GameObject obj = all[i].gameObject;
			if(obj) {
				Sky old = obj.GetComponent<Sky>() as Sky;
				if(old != null) {
					SerializedObject sr = new SerializedObject(old);
					sr.CopyFromSerializedProperty(scriptType);
					sr.ApplyModifiedProperties();
					count++;
				}
			}
		}
		if( count == 0 ) {
			EditorUtility.DisplayDialog("Done Upgrading!", "No deprecated skies found.\n\nPro Tip: Don't forget to use the \"mset\" namespace when scripting with the Sky class.", "Ok");
		} else {
			EditorUtility.DisplayDialog("Done Upgrading!", count + " deprecated skies found and upgraded.\n\nPro Tip: Don't forget to use the \"mset\" namespace when scripting with the Sky class.", "Ok");
		}
		Component.DestroyImmediate(refObj);
	}
	
	private static bool conversionWarning( string scope, bool toMobile ) { return conversionWarning(scope,toMobile,-1); }
	private static bool conversionWarning( string scope, bool toMobile, int initialCount ) {
		string shaderType = ""; 
		if( toMobile ) shaderType = "Marmoset Mobile";
		else           shaderType = "Marmoset Standard";
		
		string text = 
			"Converting all Marmoset materials in "+scope+" to use "+shaderType+
			" shaders may take several minutes and *cannot* be undone.\n\nAre you sure you wish to continue?";
		
		if( initialCount != -1 ) text = initialCount + " total materials found in " + scope + ".\n\n" + text;
		
		return EditorUtility.DisplayDialog(
			"Convert all Marmoset materials in "+scope+" to " + shaderType + "?", text,
			"Continue","Cancel");
	}

	[MenuItem("Edit/Skyshop/Convert Scene to Mobile", false, 1100)]
	public static void SceneToMobile() {
		Undo.RegisterSceneUndo("Convert Scene to Mobile");
		toggleSceneToMobile(true);
	}
	
	[MenuItem("Edit/Skyshop/Convert Scene to Standard", false, 1101)]
	public static void SceneToStandard() {
		Undo.RegisterSceneUndo("Convert Scene to Stanard");
		toggleSceneToMobile(false);
	}
	
	[MenuItem("Edit/Skyshop/Convert Project to Mobile", false, 1200)]
	public static void ProjectToMobile() {
		Undo.RegisterSceneUndo("Convert Project to Mobile");
		toggleProjectToMobile(true);	
	}
	
	[MenuItem("Edit/Skyshop/Convert Project to Standard", false, 1201)]
	public static void ProjectToStandard() {
		Undo.RegisterSceneUndo("Convert Project to Standard");
		toggleProjectToMobile(false);	
	}
	
	//converts one material to or from using marmoset mobile shaders
	private static bool toggleMobile(ref Material mat, bool toMobile) {
		string name = mat.shader.name;
		string marmoPrefix = "Marmoset/";
		string mobilePrefix = "Marmoset/Mobile/";
		
		//shader type is derived from the name prefix
		bool marmoset = name.StartsWith(marmoPrefix);
		if( marmoset ) {
			bool isMobile = name.StartsWith(mobilePrefix);
			//change prefix to desired string
			if( toMobile && !isMobile ) {
				name = mobilePrefix + name.Substring(marmoPrefix.Length);
			} else if( !toMobile && isMobile ) {
				name = marmoPrefix + name.Substring(mobilePrefix.Length);
			} else {
				return false;
			}
			
			//swap!
			Shader newShader = Shader.Find(name);
			if(newShader) {
				mat.shader = newShader;
				return true;
			}
		}
		return false;
	}
	
	//converts all materials referenced by the current scene to or from marmoset mobile shaders
	private static void toggleSceneToMobile(bool toMobile) {
		
		//warn the user before changing a bunch of material files
		bool k = conversionWarning("this scene", toMobile);
		if(!k) return;
		
		//go through all renderers in the scene and change their materials to use mobile shaders 
		Renderer[] all = GameObject.FindObjectsOfType(typeof(Renderer)) as Renderer[];
		int count = 0;
		for(int i=0; i<all.Length; ++i) {
			Renderer r = all[i];
			if(r) {
				Material[] mats = r.sharedMaterials;
				for(int m=0; m<mats.Length; ++m) {
					if( toggleMobile(ref mats[m], toMobile) ) count++;
				}
			}
		}
		if( toMobile )  EditorUtility.DisplayDialog("Done Converting!", count + " Marmoset materials converted to Marmoset Mobile materials.", "Ok");
		else            EditorUtility.DisplayDialog("Done Converting!", count + " Marmoset Mobile materials converted to Marmoset Standard materials.", "Ok");
	}
	
	private static void toggleProjectToMobile(bool toMobile) {
		string[] all = AssetDatabase.GetAllAssetPaths();
		//count material files in the asset database
		int matCount = 0;
		for(int i=0; i<all.Length; ++i) {
			if( Path.GetExtension(all[i]).ToLowerInvariant() == ".mat" ) {
				matCount++;
			}
		}
		//warn the user before changing a whole bunch of material files
		bool k = conversionWarning("this project", toMobile, matCount); 
		if( !k ) return;
		
		// go through all material assets in the asset database and convert their shaders
		int count = 0;
		for(int i=0; i<all.Length; ++i) {
			if( Path.GetExtension(all[i]).ToLowerInvariant() == ".mat" ) {
				Material mat = AssetDatabase.LoadAssetAtPath(all[i], typeof(Material)) as Material;
				if(mat) {
					if( toggleMobile(ref mat, toMobile) ) count++;
				}
			}
		}
		if(count > 0 ) AssetDatabase.Refresh();
		if( toMobile ) {
			EditorUtility.DisplayDialog("Done Converting!", count + " materials switched to using Marmoset Mobile shaders.", "Ok");
		} else {
			EditorUtility.DisplayDialog("Done Converting!", count + " materials switched to using Marmoset Standard shaders.", "Ok");
		}
		//for good measure
		RefreshSkies();
	}
}