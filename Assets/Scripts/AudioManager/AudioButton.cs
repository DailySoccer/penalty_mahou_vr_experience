using UnityEngine;
using System.Collections;
using FootballStar.Audio;

public class AudioButton : MonoBehaviour {
	
	public SoundDefinitions SoundDefinition = SoundDefinitions.BUTTON_MENU;
 	
	void Start()
	{
		mAudioGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
	}

	void OnClick()
	{
		switch (SoundDefinition) 
		{
			case SoundDefinitions.BUTTON_MENU:
					mAudioGameController.PlayMenuButtonSound();
				break;
				case SoundDefinitions.BUTTON_BACK:
					mAudioGameController.PlayGoBackButtonSound();
				break;
				case SoundDefinitions.BUTTON_STARTPLAYING:
					mAudioGameController.PlayStartMatchSound();
				break;
				case SoundDefinitions.BUTTON_SELECTOR :
					mAudioGameController.PlaySelectorSound();
				break;
				case SoundDefinitions.BUTTON_REPLAY:
					mAudioGameController.PlayReplayButtonSound();
				break;
				case SoundDefinitions.BUTTON_EXITMATCH:
					mAudioGameController.StopAllActiveAudios(true);
					mAudioGameController.PlayContinueButtonSound();
				break;
				case SoundDefinitions.BUTTON_CONTINUE:
					mAudioGameController.PlayContinueButtonSound();
				break;
				case SoundDefinitions.BUTTON_PLAY:
					mAudioGameController.PlayMatchStartSound();
				break;
		}
	}
	private AudioInGameController mAudioGameController;
}
