using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Audio
{
	public class AudioMaster : MonoBehaviour {
		class ClipInfo
		{
		   	public AudioSource Source { get; set; }
		   	public float Volume { get; set; }
		   	public SoundDefinitions Definition { get; set; }
			public bool StopFading { get; set; }
		}
		
		[SerializeField]
		public List<GameSound> GameSounds;		// Listado de GameSounds

		public float masterVolume = 1f;         // El master volumen

		private List<ClipInfo> mActiveAudio;   	// Lista de sonidos en reproducción
		private List<ClipInfo> mToRemove;		// Lista de sonidos que se van a borrar
		
		private Transform mOriginOfTheSounds;   // Padre de los sonidos
		
		private float mVolumeMod = 1.0f;		// Volumen de la musica por defecto
		private float mVolumeMin;				// Volumen minimo de la musica durante el VoiceOver
		
		private bool mVoiceOverFade; 			// Controla si hay un VoiceOver
		
		private AudioSource mActiveVoiceOver; 	// Sonido VoiceOver activo
		private AudioSource mActiveMusic;		// Musica en reproducción


		
		void Awake()
		{
			mOriginOfTheSounds = transform;
	        mOriginOfTheSounds.localPosition = new Vector3(0, 0, 0);				

		    mActiveAudio 	= new List<ClipInfo>();
			mToRemove 		= new List<ClipInfo>();
		    mVolumeMod 		= 1;
		    mVolumeMin 		= 0.1f;
		    mVoiceOverFade 	= false;
		    mActiveVoiceOver = null;
		    mActiveMusic 	= null;
		}
		
		void Update() 
		{
			UpdateVoiceOverSounds();
		    UpdateActiveAudio();
			CheckFaddingSoundsToStopIt();
		}
		
		public void PlayUniqueSoundDefinitionType(SoundDefinitions soundDef)
		{
			bool alreadyPlaying = false;
			for (int i = 0; i < mActiveAudio.Count && alreadyPlaying != true; i++)
			{
				if(mActiveAudio[i].Definition == soundDef)
					alreadyPlaying = true;
			}
			
			if(!alreadyPlaying)
				Play (soundDef);
		}		
		
		// Reproduce un sonido
		public AudioSource Play(SoundDefinitions soundDef)
		{
			//Create an empty game object
			GameObject soundLoc = CreateSoundLocation("Sound_" + soundDef);
			//Create the Audio source
			AudioSource source = soundLoc.AddComponent<AudioSource>();
			//Configure the GameSound
			GameSound gs = GetTheSoundClip(soundDef);
			//Sets the main clip
			gs.SetMainClip();
			//Si no hay asignado un audioclip, hay que evitar que pete y avisamos por la consola
			if(gs.TheSound == null)
			{
				Debug.Log(string.Format("No hay un Clip de audio asignado al GameSound definido como: {0}.\n" +
										"Revisa el listado de definiciones en el prefab '", soundDef));
			}
			else
			{
				//Configure the AudioSource
				SetSource(ref source, soundDef, gs.TheSound, gs.Volume);
				//Play it
				source.Play();
				//Drstroy it when stop
				Destroy(soundLoc, gs.TheSound.length);			
				//Set the source as active
				mActiveAudio.Add(new ClipInfo{Source = source, Volume = GameSounds[(int)soundDef].Volume * masterVolume, Definition = soundDef});
			}
			return source;
		}

		public AudioSource CustomPlay( SoundDefinitions soundDef, float volume, float pitch)
		{
			//Create an empty game object
			GameObject soundLoc = CreateSoundLocation("Sound_" + soundDef);
			//Create the Audio source
			AudioSource source = soundLoc.AddComponent<AudioSource>();
			//Configure the GameSound
			GameSound gs = GetTheSoundClip(soundDef);
			//Sets the main clip
			gs.SetMainClip();
			//Si no hay asignado un audioclip, hay que evitar que pete y avisamos por la consola
			if(gs.TheSound == null)
			{
				Debug.Log(string.Format("No hay un Clip de audio asignado al GameSound definido como: {0}.\n" +
				                        "Revisa el listado de definiciones en el prefab '", soundDef));
			}
			else
			{
				//Configure the AudioSource
				SetSource(ref source, soundDef, gs.TheSound, gs.Volume);
				source.pitch = (pitch < 0) ? 1 : pitch;
				source.volume = (volume < 0) ? gs.Volume : volume;
				//Play it
				source.Play();
				//Drstroy it when stop
				Destroy(soundLoc, gs.TheSound.length);			
				//Set the source as active
				mActiveAudio.Add(new ClipInfo{Source = source, Volume = GameSounds[(int)soundDef].Volume * masterVolume, Definition = soundDef});
			}
			return source;
		}
		
		/// <summary>
		/// Play the specified soundDef and ignoreIfExist.
		/// </summary>
		/// <param name='soundDef'>
		/// Sound def.
		/// </param>
		/// <param name='ignoreIfExist'>
		/// Si ignoreIfExist es 'True', añade un nuevo sonido con esta definicion a la lista de sonido activo.
		/// Si ignoreIfExist es 'False', primero detiene el que ya esta en reproduccion y añade el nuevo.
		/// Esta funcion sirve para evitar/permitir que se solapen varios sonidos iguales durante el partido
		/// </param>
		public AudioSource Play(SoundDefinitions soundDef, bool ignoreIfExist)
		{
			if(!ignoreIfExist)
				StopSound(soundDef);		
			
			return Play(soundDef);
		}
		
		// Reproduce un sonido, poniendo los demás en Fade, para que este suene por encima
		public AudioSource PlayVoiceOver(SoundDefinitions voiceOverDef)
		{
		    AudioSource source = Play(voiceOverDef);
		    mActiveVoiceOver = source;
		    mVoiceOverFade = true;
		    return source;
		}
		
		// Reproduce un sonido en forma Loop
		public AudioSource PlayLoop(SoundDefinitions soundDef) 
		{
			if( IsPlayingSoundDefinition(soundDef))
				StopSound( soundDef );
			
			GameObject soundLoc = CreateSoundLocation("Loop_" + soundDef.ToString());
			//Create the source
		    AudioSource source = soundLoc.AddComponent<AudioSource>();
			
			GameSound gs = GetTheSoundClip(soundDef);
			gs.SetMainClip();
		    SetSource(ref source, soundDef, gs.TheSound , gs.Volume);
		    source.loop = true;
		    source.Play();
		    
			//Set the source as active
			mActiveAudio.Add(new ClipInfo{Source = source, Volume = gs.Volume * masterVolume, Definition = soundDef});
		    return source;
		}
		
		// Reproduce una musica en Loop
		public AudioSource PlayMusic(SoundDefinitions soundDef) 
		{
		    mActiveMusic = PlayLoop(soundDef);
		    return mActiveMusic;
		}
		
		// Para y elimina un sonido activo
		public void StopSound(SoundDefinitions defToStop) 
		{
			GameObject sound = null;
			foreach ( ClipInfo ci in mActiveAudio)
			{
				if(ci.Definition == defToStop)
					sound = ci.Source.gameObject;
			}

			if( sound != null)	
		        Destroy(sound);
		}

		// Para y elimina los Sonidos que no estén en bucle
		private void StopAllFx() 
		{
			foreach (ClipInfo audioClip in mActiveAudio) 
			{
				if(!audioClip.Source.loop)
					Destroy(audioClip.Source.gameObject);
			}
		}
		
		// Para y elimina los Sonidos que no estén en bucle
		public void StopAllFx(bool stopFading) 
		{
			if(!stopFading)
				StopAllFx();
			else
			{
				foreach (var audioClip in mActiveAudio) 
				{
					if(audioClip.Volume > 0)
						if(!audioClip.Source.loop)
							audioClip.StopFading = true;
				}
			}
		}
		
		
		// Para y elimina los Sonidos (incluidos los loops) menos el que especifiquemos
		public void StopFaddingAllButKeepThisPlaying(SoundDefinitions soundDef) 
		{
			foreach (var audioClip in mActiveAudio) 
			{
				if( audioClip.Volume > 0 )
					if( audioClip.Definition != soundDef )
						audioClip.StopFading = true;
			}
		}
		
		// Para y elimina los Sonidos que no estén en bucle
		public void StopFaddingAllFxButThis(SoundDefinitions soundDef) 
		{
			foreach (var audioClip in mActiveAudio) 
			{
				if(audioClip.Volume > 0)
					if(!audioClip.Source.loop && audioClip.Definition != soundDef)
						audioClip.StopFading = true;
			}
		}
		
		// Para y elimina todos los FX y Musicas que haya en la lista de sonidos activos
		public void StopAll() 
		{
			foreach (ClipInfo ci in mActiveAudio) 
			{
				Destroy(ci.Source.gameObject);
			}
		}
		
		// Para y elimina un sonido activo
		public void StopAll(bool stopFading) 
		{
			if(!stopFading)
				StopAll();
			else
			{
				foreach (var audioClip in mActiveAudio) 
				{
					if(audioClip.Volume > 0)
						audioClip.StopFading = true;
				}
			}
		}
		
		// Pausa los sonidos activos
		public void PauseFX() 
		{ 
		    foreach (var audioClip in mActiveAudio) 
			{
		        try 
				{
		            if (audioClip.Source != mActiveMusic) 
					{
		                audioClip.Source.Pause();
		            }
		        } catch {
		            continue;
		        }
		    }
		}
	 	
		// Des-pausa los sonidos activos
		public void UnpauseFX() 
		{
		    foreach (var audioClip in mActiveAudio)
			{
		        try 
				{
		            if (!audioClip.Source.isPlaying) 
					{
		                audioClip.Source.Play();
		            }
		        } catch {
		            continue;
		        }
		    }
		}
		
		/***** Métodos/Functions *****/
		
		// Crea un Objeto vacío y lo posiciona en la escena y establece su padre en la Jerarquía
		private GameObject CreateSoundLocation(string name)
		{
			//Create an empty game object
			GameObject soundLoc = new GameObject(name);
			if(mOriginOfTheSounds.position != Vector3.zero)
				soundLoc.transform.position = mOriginOfTheSounds.position;
			else
				soundLoc.transform.position = transform.position;
			
			soundLoc.transform.parent = mOriginOfTheSounds;
			return soundLoc;
		}		
		
		// Elimina los sonidos que estén marcados para borrar con Fade
		void CheckFaddingSoundsToStopIt()
		{
			mToRemove.Clear();
			
		 	foreach (var audioClip in mActiveAudio) 
			{
				if(audioClip.StopFading)
				{
	            	if (audioClip.Volume > 0.001f) //Si aún tienen volumen, se lo bajamos
					{
						audioClip.Volume -= audioClip.Volume * Time.deltaTime;
					}
					else
						mToRemove.Add(audioClip);
				}
			}
			//Eliminamos los que se puedan eliminar
		    foreach (var audioClip in mToRemove) {
		       	Destroy(audioClip.Source.gameObject);
		    }
		}
		
		//Busca y retorna un GameSound y establece cual es el clip a reproducir
		//(Atencion: En principio no puede usar una misma definición para varios sonidos. El algoritmo devuelve el primero que encuentre)
		GameSound GetTheSoundClip(SoundDefinitions soundDef)
		{
			// Seleccionamos el clip definido como el parametro soundDef 
			GameSound gs = (from g in GameSounds
							where g.SoundDef == soundDef
					select g).FirstOrDefault();
			return gs;		
		}
		
		// Establece los parametros del AudioSource
		private void SetSource(ref AudioSource source, SoundDefinitions soundDef, AudioClip clip, float volume) 
		{
			source.rolloffMode = AudioRolloffMode.Logarithmic;
			source.dopplerLevel = 0.2f;
			source.minDistance = 150;
			source.maxDistance = 1500;
			source.clip = clip;
			source.volume = volume;
			source.pitch = 1;
		}
		
		private bool IsPlayingSoundDefinition(SoundDefinitions soundDef)
		{
			bool isPlaying = false;
			foreach(ClipInfo clip in mActiveAudio)
			{
				if(clip.Definition == soundDef)
				{
					isPlaying = true;
				}
			}
			return isPlaying;
		}
		
		// Actualiza los sonidos que suenan por encima del resto, haciendo un Fade
		void UpdateVoiceOverSounds()
		{
			if (mVoiceOverFade && mVolumeMod >= mVolumeMin) {
		        mVolumeMod -= 0.1f;
		    } else if (!mVoiceOverFade && mVolumeMod < 1.0f) {
		        mVolumeMod += 0.1f;
		    }
		}
			
		// Actualiza los AudioSources activos, y los que ya no se reproduzcan los elimina de la lista
		private void UpdateActiveAudio() 
		{ 
			mToRemove.Clear();
	        if (!mActiveVoiceOver) 
			{
	            mVoiceOverFade = false;
	        }
	        foreach (var audioClip in mActiveAudio) 
			{
	            if (!audioClip.Source) 
				{
	                mToRemove.Add(audioClip);
	            } 
				else if (audioClip.Source != mActiveVoiceOver) 
				{
					audioClip.Source.volume = audioClip.Volume * mVolumeMod *  masterVolume;
	            }
	        }

		    //cleanup
		    foreach (var audioClip in mToRemove) {
		        mActiveAudio.Remove(audioClip); 
		    }
		}	
	}
}

