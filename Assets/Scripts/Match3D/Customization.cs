using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;
using System;

namespace FootballStar.Match3D {
	
	[System.Serializable]
	public class SoccerCustomMaterials {
		public Material Equipacion;
		public Material Botas;
		public Material Piel;
	}

	[System.Serializable]
	public class MainCharacterCustom {
		public GameObject Hair;
		public Material Piel;
	}

	public class Customization : MonoBehaviour {
	
		public GameObject SoccerPrototype;
		public GameObject SoccerPrototypeLow;

		public GameObject IAGoalkeeper;
		public GameObject IAAttacker;
		public GameObject IADefender;
		
		public SoccerCustomMaterials MaterialsGoalkeeper;
		public SoccerCustomMaterials MaterialsDefender;
		public SoccerCustomMaterials MaterialsLocal;
		
		public GameObject[] HairPrefabs;

		public MainCharacterCustom MainCharacterCustom;
		
		public Texture2D NumbersTexture;
		public Texture2D DefaultBodyTexture;

		
		void Start () {
			mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();
			mEquipacionesCache = mMatchBridge.GetComponent<EquipacionesCache>();
			mPerformanceTuner = GameObject.FindGameObjectWithTag("GameModel").GetComponent<PerformanceTuner>();

			StartCoroutine(LoadDefenderTexture());
            StartCoroutine(LoadAttackerTexture());
        }

        // Cargamos la textura de equipacion del oponente
        private IEnumerator LoadDefenderTexture()
        {
            if (mDefenderTextureAssigned || mDefenderTextureLoader != null)
                throw new Exception("WTF 92921");

            string root = Application.streamingAssetsPath;
            if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
                root = Application.dataPath + "/StreamingAssets/";
            else if (Application.platform != RuntimePlatform.Android)
                root = "file:///" + root;

            // A ver si necesitamos segunda equipacion...
            if (FootballStar.Manager.Model.TierDefinition.UseSecondEquipation(mMatchBridge.CurrentMatchDefinition.MyID, mMatchBridge.CurrentMatchDefinition.OpponentID) )
                root = root + "SE/";

            // Cargamos la textura de equipacion del contrario. Para los iPhones4, usamos una que lo incluye todo (piel, equipacion, botas)
            mDefenderTextureLoader = new WWW(System.IO.Path.Combine(root, mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + ".jpg"));

            yield return mDefenderTextureLoader;
            AssignDefenderTexture(mDefenderTextureLoader.texture);
        }

        // Cargamos la textura de equipacion del oponente
        private IEnumerator LoadAttackerTexture()
        {
            if (mAttackerTextureAssigned || mAttackerTextureLoader != null)
                throw new Exception("WTF 92921");

            string root = Application.streamingAssetsPath;

            if (Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
                root = Application.dataPath + "/StreamingAssets/";
            else if (Application.platform != RuntimePlatform.Android)
                root = "file:///" + root;
            // Cargamos la textura de equipacion del contrario. Para los iPhones4, usamos una que lo incluye todo (piel, equipacion, botas)

            mAttackerTextureLoader = new WWW(System.IO.Path.Combine(root, mMatchBridge.CurrentMatchDefinition.MyBadgeName + ".jpg"));

            yield return mAttackerTextureLoader;
            AssignAttackerTexture(mAttackerTextureLoader.texture);
        }

        void AssignDefenderTexture(Texture2D defenderTexture) {
			// Si nos llaman dos veces => No dio tiempo a cargar. Nos quedamos con la primera, en rosa.
			if (mDefenderTextureAssigned)
				return;
			
			if ( defenderTexture == null && DefaultBodyTexture != null ) {
				Debug.LogWarning(" Asigning Default Body Texture! " + DefaultBodyTexture.name );
				defenderTexture = DefaultBodyTexture;
			}
			
			// Creamos un nuevo material para no afectar al del disco
			MaterialsDefender.Equipacion = new Material(MaterialsDefender.Equipacion);
			
			// Y ahora ya si en este material podemos quedarnos con la textura equipacion
			MaterialsDefender.Equipacion.mainTexture = defenderTexture;
			
			// Ya estamos asignados...
			mDefenderTextureAssigned = true;
		}

        void AssignAttackerTexture(Texture2D attackerTexture)
        {
            // Si nos llaman dos veces => No dio tiempo a cargar. Nos quedamos con la primera, en rosa.
            if (mAttackerTextureAssigned)
                return;

            if (attackerTexture == null && DefaultBodyTexture != null)
            {
                Debug.LogWarning(" Asigning Default Body Texture! " + DefaultBodyTexture.name);
                attackerTexture = DefaultBodyTexture;
            }

            // Creamos un nuevo material para no afectar al del disco
            MaterialsLocal.Equipacion = new Material(MaterialsLocal.Equipacion);

            // Y ahora ya si en este material podemos quedarnos con la textura equipacion
            MaterialsLocal.Equipacion.mainTexture = attackerTexture;

            // Ya estamos asignados...
            mAttackerTextureAssigned = true;
        }

        // Antes de llamar a Setup se tienen que asegurar de que la textura esta cargada y asignada en el material
        public bool IsTextureAssigned
        {
            get { return mDefenderTextureAssigned && mAttackerTextureAssigned; }
        }

        public void Setup() {
			if (!IsTextureAssigned) {
				Debug.LogWarning(" Textura equipacion no cargada");
				AssignDefenderTexture(null);
			}
			Initialize();

			if (mPerformanceTuner.OwnQualityLevel >= 3)
				InstantiateFutbolistas();
			else
				InstantiateFutbolistasLow();

			AttachIAFutbolistas();
		}

		public void SetupMainCharacter( GameObject mainCharacter ) {
			if ( mainCharacter.name != "Soccer-Local11" ) {
				Debug.LogWarning( "Invalid Main Character : " + mainCharacter.name );
			}
		}

		void Initialize() {
			mAttackers = new List<Entrenador>(11);
			mDefenders = new List<Entrenador>(11);
			
			GameObject[] locales = GameObject.FindGameObjectsWithTag( "Local" );
			foreach( GameObject local in locales ) {
				mAttackers.Add ( local.GetComponent<Entrenador>() );
			}

			GameObject[] visitantes = GameObject.FindGameObjectsWithTag( "Visitante" );
			foreach( GameObject visitante in visitantes ) {
                var ent = visitante.GetComponent<Entrenador>();
                if (visitante.name == "Visitante1") DefenderKeeper = ent;
                mDefenders.Add ( ent );
			}
		}
		
		public void InstantiateFutbolistas() {
			// Si hay cualquier futbolista registrado, asumimos que estan todos
			if ( mFutbolistas.ContainsKey( "Local1".GetHashCode() ) )
				return;
			
			// Descompresion de la textura a memoria. Aseguramos que la hacemos solo 1 vez
			Profiler.BeginSample("InstantiateFutbolistas Descompresion");
			
			// El mismo portero para ambos equipos. 
			Color32[] originalGoalkeeperPixels = null;
			Color32[] originalLocalPixels = null;
			
			// El equipo contrario se regenera cada vez
			Color32[] originalDefenderPixels = (MaterialsDefender.Equipacion.mainTexture as Texture2D).GetPixels32(0);
			
			// Si hay cualquier elemento en el cache, asumimos que estan todos (los dorsales no pueden cambiar)
			if (mEquipacionesCache.IsEmpty) {
				originalLocalPixels = (MaterialsLocal.Equipacion.mainTexture as Texture2D).GetPixels32(0);
				originalGoalkeeperPixels = (MaterialsGoalkeeper.Equipacion.mainTexture as Texture2D).GetPixels32(0);
			}
						
			Profiler.EndSample();
			
			Profiler.BeginSample("InstantiateFutbolistas Atacantes");
			foreach( Entrenador local in mAttackers ) {
				GameObject soccer = Instantiate( SoccerPrototype ) as GameObject;

				if ( local.Tactica == Entrenador.ETactica.Portero )
					// Ignoramos el dorsal que nos da el Entrenador y forzamos a 1. Queremos estar seguros de que es la misma textura en el cache
					ChangeMaterial( soccer, MaterialsGoalkeeper, originalGoalkeeperPixels, 1, true );
				else
					// Pone el numero pero no cambia los materiales. Es decir, se queda con los de por defecto, los del R. Madrid
					ChangeMaterial( soccer, null, originalLocalPixels, local.Dorsal, true );

				// Por defecto, el MainCharacter es Dorsal = 11
				if ( local.Dorsal == 11 ) {
					CreatePelo( soccer.transform, MainCharacterCustom.Hair );
					ChangePiel ( soccer, MainCharacterCustom.Piel );
				}
				else {
					CreatePelo(soccer.transform, local.Dorsal);
				}
							
				RegisterInstanceFutbolista( local, soccer );
			}
			Profiler.EndSample();
			
			Profiler.BeginSample("InstantiateFutbolistas Defensores");
			foreach( Entrenador visitante in mDefenders ) {
				GameObject soccer = Instantiate( SoccerPrototype ) as GameObject;
				soccer.tag = "Defender";
				// Material dependiendo de si es portero o no
				if ( visitante.Tactica == Entrenador.ETactica.Portero )
					ChangeMaterial( soccer, MaterialsGoalkeeper, originalGoalkeeperPixels, 1, true );
				else
					ChangeMaterial( soccer, MaterialsDefender, originalDefenderPixels, visitante.Dorsal, false );
				
				CreatePelo(soccer.transform, visitante.Dorsal);
								
				RegisterInstanceFutbolista( visitante, soccer );
			}
			Profiler.EndSample();
		}

		// En iPhone4 usamos un modelo especial que solo tiene 1 textura
		void InstantiateFutbolistasLow() {
			if ( mFutbolistas.ContainsKey( "Local1".GetHashCode() ) )
				return;
									
			foreach( Entrenador local in mAttackers ) {
				GameObject soccer = Instantiate( SoccerPrototypeLow ) as GameObject;

				RegisterInstanceFutbolista( local, soccer );
			}
									
			foreach( Entrenador visitante in mDefenders ) {
				GameObject soccer = Instantiate( SoccerPrototypeLow ) as GameObject;

				SkinnedMeshRenderer[] meshRenderers = soccer.GetComponentsInChildren<SkinnedMeshRenderer>();
				Material[] theMaterials = meshRenderers[0].materials;
				theMaterials[0].mainTexture = MaterialsDefender.Equipacion.mainTexture;
				meshRenderers[0].materials = theMaterials;

				RegisterInstanceFutbolista( visitante, soccer );
			}
		}
		
		void AttachIAFutbolistas() {
					
			Profiler.BeginSample("AttachIAFutbolistas Atacantes");
			foreach( Entrenador local in mAttackers ) {
				GameObject soccer = GetInstanceFutbolista( local );

				// La misma IA para todos los atacantes (incluido el portero)
				AttachIA( soccer, IAAttacker );
				
				local.AttachTarget ( soccer );
			}
			Profiler.EndSample();
			
			Profiler.BeginSample("AttachIAFutbolistas Defensores");
			foreach( Entrenador visitante in mDefenders ) {
				GameObject soccer = GetInstanceFutbolista( visitante );
				
				// IA dependiendo de si es portero o no
				if ( visitante.Tactica == Entrenador.ETactica.Portero )
					AttachIA( soccer, IAGoalkeeper );
				else
					AttachIA( soccer, IADefender );
				
				visitante.AttachTarget ( soccer );
			}
			Profiler.EndSample();
		}
		
		void RegisterInstanceFutbolista( Entrenador entrenador, GameObject soccer ) {
			int key = entrenador.name.GetHashCode();
			Assert.Test ( !mFutbolistas.ContainsKey(key), "RegisterInstanceFutbolista failed" );
			
			if ( !mFutbolistas.ContainsKey(key) ) {
				mFutbolistas[ key ] = soccer;
				
				// Helper.Log ( entrenador.gameObject, "Instantiate" );
			}
		}
		
		GameObject GetInstanceFutbolista( Entrenador entrenador ) {
			GameObject soccer = null;
			
			// Tenemos al futbolista en caché?
			int key = entrenador.name.GetHashCode();
			Assert.Test ( mFutbolistas.ContainsKey(key), "GetInstanceFutbolista failed" );
			
			if ( mFutbolistas.ContainsKey(key) ) {
				// Obtener el gameObject ya creado
				soccer = mFutbolistas[ key ];
			}
			
			return soccer;
		}
		
		void AttachIA ( GameObject soccer, GameObject IAPrototype ) {
			GameObject IA = Instantiate( IAPrototype ) as GameObject;
			IA.transform.parent 		= soccer.transform;
			IA.transform.localPosition 	= Vector3.zero;
			IA.transform.localRotation 	= Quaternion.identity;
		}
				
		void ChangeMaterial( GameObject soccer, SoccerCustomMaterials soccerCustomMaterials, Color32[] equipacionOriginal, int dorsal, bool cacheable ) {
			
			Profiler.BeginSample("Customization ChangeMaterial");

			SkinnedMeshRenderer[] meshRenderers = soccer.GetComponentsInChildren<SkinnedMeshRenderer>();

			// http://docs.unity3d.com/Documentation/ScriptReference/Renderer-materials.html
			Material[] theMaterials = meshRenderers[0].materials;
			
			if (soccerCustomMaterials != null) {
				theMaterials[0] = soccerCustomMaterials.Equipacion;
				theMaterials[1] = soccerCustomMaterials.Piel;
				theMaterials[2] = soccerCustomMaterials.Botas;
			}
			
			var clonedMat = new Material(theMaterials[0]);
			
			// Es cacheable y deberiamos buscarlo en el cache?
			if (cacheable && equipacionOriginal == null) {
				clonedMat.mainTexture = mEquipacionesCache.GetFromCache(dorsal);
			}
			else {
				clonedMat.mainTexture = ComposeTextureNumber(theMaterials[0].mainTexture as Texture2D, equipacionOriginal, dorsal);
				
				if (cacheable)
					mEquipacionesCache.AddToCache(clonedMat.mainTexture as Texture2D, dorsal);
			}

			theMaterials[0] = clonedMat;

			// Aplicamos el material a todos los MeshRenders
			foreach ( SkinnedMeshRenderer meshRenderer in meshRenderers ) {
				meshRenderer.materials = theMaterials;
			}
			
			Profiler.EndSample();
		}

		void ChangePiel( GameObject soccer, Material piel ) {
			
			Profiler.BeginSample("Customization ChangePiel");
			
			SkinnedMeshRenderer[] meshRenderers = soccer.GetComponentsInChildren<SkinnedMeshRenderer>();
			
			// http://docs.unity3d.com/Documentation/ScriptReference/Renderer-materials.html
			Material[] theMaterials = meshRenderers[0].materials;

			// Cambiamos la piel
			theMaterials[1] = piel;

			// Aplicamos el material a todos los MeshRenders
			foreach ( SkinnedMeshRenderer meshRenderer in meshRenderers ) {
				meshRenderer.materials = theMaterials;
			}
			
			Profiler.EndSample();
		}

		// sourceTexture y sourcePixels32 es lo mismo. Queremos sourceTexture solo para acceder a width & height
		Texture2D ComposeTextureNumber(Texture2D sourceTexture, Color32[] sourcePixels32, int dorsal) {
						
			const int numberPixelsWidth = 32;
			
			// Copia de la textura fuente
			Profiler.BeginSample("ComposeTextureNumber Begin");
			Color32[] targetPixels32 = new Color32[sourcePixels32.Length];
			Array.Copy(sourcePixels32, targetPixels32, sourcePixels32.Length);
			Profiler.EndSample();
						
			// Escritura de los numeros
			Profiler.BeginSample("ComposeTextureNumber Writes");
			
			int targetCenterX = Mathf.FloorToInt((493.0f / 1024.0f) * sourceTexture.width);
						
			int secondDigit = dorsal % 10;
			int firstDigit = (dorsal - secondDigit) / 10;
			
			if (firstDigit != 0) {
				WriteNumber(targetPixels32, sourceTexture, firstDigit, numberPixelsWidth, targetCenterX);
				WriteNumber(targetPixels32, sourceTexture, secondDigit, numberPixelsWidth, targetCenterX + numberPixelsWidth);
			}
			else {
				WriteNumber(targetPixels32, sourceTexture, secondDigit, numberPixelsWidth, targetCenterX + numberPixelsWidth/2);
			}
			Profiler.EndSample();
			
			// Creacion y blit a la textura final
			Profiler.BeginSample("ComposeTextureNumber End");	
			var targetTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, true);
			targetTexture.SetPixels32(targetPixels32);
			targetTexture.Apply(true, true);			// no longer readable FTW!
			Profiler.EndSample();
			
			return targetTexture;
		}
		
		void WriteNumber(Color32[] targetPixels32, Texture2D sourceTexture, int number, int numberPixelsWidth, int startX) {
			
			Color[] numberPixels = NumbersTexture.GetPixels(number * numberPixelsWidth, 0, numberPixelsWidth, NumbersTexture.height);
			
			int startY = sourceTexture.height - Mathf.FloorToInt((60.0f / 512.0f) * sourceTexture.height);
			int addressTargetTexture = (startY * sourceTexture.width) + startX;

			int totalPixelsToBlit = numberPixelsWidth * NumbersTexture.height;
			int lineCounter = 0;
			
			// Las texturas van bottom-top, left-right (row by row). Pintamos cada linea hacia atras (right-left)
			for (int c = totalPixelsToBlit-1; c >= 0; --c)
			{
				float alpha = numberPixels[c].a;
				Color targetColor = targetPixels32[addressTargetTexture];
				
				targetPixels32[addressTargetTexture] = numberPixels[c] * alpha + ((1-alpha) * targetColor); // TODO: Performance (conversiones...)
				addressTargetTexture--;
				lineCounter++;
				
				if (lineCounter == numberPixelsWidth) {
					lineCounter = 0;
					addressTargetTexture -= sourceTexture.width-numberPixelsWidth;
				}
			}
		}

		void CreatePelo(Transform soccerTransform, GameObject prefabHair) {
			var hair = Instantiate(prefabHair) as GameObject;
			hair.transform.parent = soccerTransform.Find("Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
			hair.transform.localPosition = Vector3.zero;
			hair.transform.localRotation = Quaternion.identity;
			hair.transform.localScale = Vector3.one;
		}

		void CreatePelo(Transform soccerTransform, int dorsal) {
			CreatePelo( soccerTransform, HairPrefabs[dorsal-1] );
		}

        Dictionary< int, GameObject > mFutbolistas = new Dictionary< int, GameObject >();
        public List<Entrenador> Attackers { get { return mAttackers; } }
        List<Entrenador> mAttackers;
        public List<Entrenador> Defenders { get { return mDefenders; } }
        List<Entrenador> mDefenders;

        public Entrenador DefenderKeeper { get; private set; }

        MatchBridge mMatchBridge;
		EquipacionesCache mEquipacionesCache;
		PerformanceTuner mPerformanceTuner;

        WWW mDefenderTextureLoader;
        bool mDefenderTextureAssigned = false;
        WWW mAttackerTextureLoader;
        bool mAttackerTextureAssigned = false;
    }

}
