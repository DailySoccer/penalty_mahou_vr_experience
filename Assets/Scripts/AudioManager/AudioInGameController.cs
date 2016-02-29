using System;
using UnityEngine;
using System.Collections;
using FootballStar.Match3D;
using FootballStar.Common;
using FootballStar.Manager.Model;

namespace FootballStar.Audio {
	
	public class AudioInGameController : MonoBehaviour 
	{
		void Awake () 
		{
		}
		
		void Start()
		{
			if (mAudioManager == null) // Esta comprobacion es necesaria cuando rulamos la escena "jugadas01"
				mAudioManager = GetComponent<AudioMaster>();
		}
		
		public void SubscribeToMatchEvents()
		{
			if(mAudioManager == null)// Esta comprobacion es necesaria cuando rulamos la escena "jugadas01"
				mAudioManager = GetComponent<AudioMaster>();
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchBridge = GetComponent<MatchBridge>();
							
			mMatchManager.OnNewPlay 			+= OnMatchStart;
			mMatchManager.OnGol 				+= OnGol;
			mMatchManager.InteractiveActions.OnEvent += OnInteractiveAction;
		}

		public void DeSubscribeFromMatchEvents()
		{
			mMatchManager.OnNewPlay 			-= OnMatchStart;
			mMatchManager.OnGol 				-= OnGol;
			mMatchManager.InteractiveActions.OnEvent -= OnInteractiveAction;
		}
		
		
		//  Metodos //
		public void PlayDefinition(SoundDefinitions soundDef)
		{
			PlayDefinition(soundDef, false);
		}
		public void PlayDefinition(SoundDefinitions soundDef, bool loop)
		{
			if(loop)
				mAudioManager.PlayMusic(soundDef);
			else
				mAudioManager.Play(soundDef);
		}
		
		// --> Menu SFX		
		public void CustomPlay(SoundDefinitions soundDef, float volume, float pitch)
		{
			mAudioManager.CustomPlay(soundDef, volume, pitch);
		}

		public void PlayMenuButtonSound()
		{
			PlayDefinition(SoundDefinitions.BUTTON_MENU);
		}
		
		public void PlayGoBackButtonSound()
		{
			PlayDefinition(SoundDefinitions.BUTTON_BACK);
		}
		
		public void PlayStartMatchSound()
		{
			PlayDefinition(SoundDefinitions.BUTTON_STARTPLAYING);
		}
		
		public void PlayReplayButtonSound()
		{
			//StopAllActiveAudios(true);
			//mAudioManager.StopFaddingAllButKeepThisPlaying(SoundDefinitions.THEME_MAIN);
			PlayDefinition(SoundDefinitions.BUTTON_REPLAY);
		}
		
		public void PlayContinueButtonSound()
		{
			//StopAllActiveAudios(true);			
			PlayDefinition(SoundDefinitions.BUTTON_CONTINUE);
		}
		
		public void PlaySelectorSound()
		{
			PlayDefinition(SoundDefinitions.BUTTON_SELECTOR);
		}		
		
		public void PlayItemLockedSound()
		{
			PlayDefinition(SoundDefinitions.LOCKED_ITEM);
		}

		public void PlayBoughtSound()
		{
			PlayDefinition(SoundDefinitions.CASHREGISTER);
		}
		
		public void PlayMatchStartSound()
		{
			PlayDefinition(SoundDefinitions.BUTTON_PLAY);
		}
		
		// <-- Menu SFX	

		// --> CANTICOS
		public void PlayVoiceGlad()
		{
			PlayDefinition(SoundDefinitions.VOICE_GLAD);
		}
		
		public void PlayVoicePerfect()
		{
			PlayDefinition(SoundDefinitions.VOICE_PERFECT);
		}
		
		public void PlayVoiceBoo()
		{
			PlayDefinition(SoundDefinitions.VOICE_BOO);
		}
		
		public void PlayMissAction()
		{
			PlayVoiceBoo();
			mAudioManager.StopSound(SoundDefinitions.CROWD_GLAD);
			mAudioManager.StopSound(SoundDefinitions.CROWD_GOINGUP);
			PlayDefinition( SoundDefinitions.CROWD_BOO);
		}
		// <-- CANTICOS
		
		// --> OTHERS
		public void PlayWishtleSound()
		{
			PlayDefinition(SoundDefinitions.WISHTLE);
		} 
		
		
		public void StopAllActiveAudios(bool fadingSound)
		{
			mAudioManager.StopAll(fadingSound);
		}
		
		// Event Subscriptions //
		
		void OnMatchStart (object sender, EventArgs e) {
			if(mIsFirstPlay)
			{
				StopAllActiveAudios(true);
				mAudioManager.PlayMusic(SoundDefinitions.CROWD_LOOP);
				//mAudioManager.PlayMusic(SoundDefinitions.THEME_MATCH);
				mIsFirstPlay = false;
			}
			else
				mAudioManager.StopAllFx(false); //Paramos todos los FXs al inicio de la jugada

			mMatchScorePercentage = 0f;
			PlayDefinition(SoundDefinitions.WISHTLE, false);
		}
		
		void OnMatchEnd() {
			mAudioManager.StopFaddingAllButKeepThisPlaying(SoundDefinitions.CROWD_LOOP);
			//La musica del final del partido, depende del resultado
			if (!mMatchBridge.ReturnMatchResult.PlayerWon)
				PlayDefinition(SoundDefinitions.MATCH_ENDMUSIC_BAD);
			else
				if( mMatchBridge.MatchRelevance == Match.eMatchRelevance.IRRELEVANT )
					PlayDefinition(SoundDefinitions.MATCH_ENDMUSIC_GOOD);
			
			mIsFirstPlay = true;
		}
		
		void OnGol (object sender, EventArgs e) {
			PlayDefinition(SoundDefinitions.CROWD_GOL);
		}
		
		void OnInteractiveAction(object sender, EventInteractiveActionArgs e)
		{	
			if ( e.State == InteractionStates.END ) {
				if (e.ResultKind == InteractionResponseKind.TOO_LATE)
				{
					PlayMissAction();
				}
				else if (e.ResultKind == InteractionResponseKind.LATE)
				{
					PlayVoiceGlad();
				}
				else if (e.ResultKind == InteractionResponseKind.PERFECT)
				{
					PlayVoicePerfect();
				}
				else if (e.ResultKind == InteractionResponseKind.EARLY)
				{
					PlayVoiceGlad();
				}
				else
				{
					PlayMissAction();
				}
			}
			else if ( e.State == InteractionStates.BALL_KICK ) {
				// Puntuacion por interaccion
				mScorePerInteraction = 100 / e.Total;
				if(e.Success)
				{
					mMatchScorePercentage += mScorePerInteraction;  // Sumamos puntos
					PlayDefinition(SoundDefinitions.CHUT_PASS); // Sonido de pases
	
					// A partir 25% de puntuacion
					if ( mMatchScorePercentage >= 25)
						mAudioManager.PlayUniqueSoundDefinitionType(SoundDefinitions.CROWD_GLAD);
					
					// A partir 60% de puntuacion, empieza a subir el tono de la aficcion
					// Los 3 ultimas jugadas, si lo hemos hecho bien con anterioridad, el publico se pone contento
					if ( e.Current >= e.Total - 2 )//mMatchScorePercentage >= 60)
					{
						//PlayCrowdGoingUp();
					}
				}
			}
		}
				
		// Declarations //
		MatchManager mMatchManager;
		private MatchBridge mMatchBridge;

		private AudioMaster mAudioManager;
		private bool mIsFirstPlay = true;
		
		private float mScorePerInteraction;      	// puntuacion que recibimos por cada interaccion satisfactoria
		private float mMatchScorePercentage = 0f;	// porcentaje de puntuacion que tenemos acumulado durante el partido
	}
}