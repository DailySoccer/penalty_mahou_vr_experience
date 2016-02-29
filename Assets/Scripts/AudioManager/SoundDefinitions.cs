using UnityEngine;
using System.Collections;

namespace FootballStar.Audio 
{	
	public enum SoundDefinitions
	{
		// Musics
		THEME_MAIN			= 0,
		MATCH_ENDMUSIC_GOOD	= 17,
		MATCH_ENDMUSIC_BAD	= 18,
		CROWD_BOO			= 12,				
		CROWD_GLAD			= 13,
		CROWD_LOOP			= 14,
		CROWD_GOL			= 15,				
						
		// fx
		BUTTON_MENU			= 1,
		BUTTON_SELECTOR		= 2,
		BUTTON_BACK			= 3,
		BUTTON_CONTINUE		= 4,
		BUTTON_STARTPLAYING	= 5,
		BUTTON_REPLAY		= 6,
		MATCH_INTRO			= 30,
		CITY_AMBIENT		= 31,
		SCORE_SOUND			= 32,
		COMPETITION_VICTORY	= 33,
		AWARD_DING			= 34,		
		SWOOSH   			= 35,	
		TYPE_WRITER			= 36,	

		// Match result
		COUNTER_SLIDES		= 7,		
		MATCH_RESULT		= 8,
		RESULT_BALL_1		= 9,
		RESULT_BALL_2		= 10,
		RESULT_BALL_3		= 11,
		CHUT_PASS			= 16,			

		// InGame		
		BUTTON_EXITMATCH	= 19,
		COINS				= 20,
		LOCKED_ITEM			= 21,
		CASHREGISTER		= 22,
		BUTTON_PLAY			= 23,
		UNDEFINED      		= 99,	
		
		// Voices
		VOICE_GLAD 			= 24,
		VOICE_PERFECT		= 25,
		VOICE_BOO 			= 26,
		WISHTLE				= 27,
		CROWD_GOINGUP   	= 28,
		THEME_MATCH			= 29,

	}
}