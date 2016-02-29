using UnityEngine;
using System.Collections;
using FootballStar.Audio;

namespace FootballStar.Manager {
	
	public class TypeWritterComponent : MonoBehaviour {
		
		public float letterPause = 0.015f;
		
		void Awake() {
			//mAudioGameController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
			mTextLabel = GameObject.Find("Text").GetComponent<UILabel>();
		}
		
		// Use this for initialization
		void Start () {
			if (!mTextLabel)
				Debug.LogError ("Se necesita que el GameObject contenga un UILabel (nGUI) cuyo nombre sea 'Text'");
			else {
				message = mTextLabel.text;
				mTextLabel.text = "";
				StartCoroutine ("TypeText");
			}
		}

		IEnumerator TypeText () {
			string newText = "";
			bool isColorCode = false;
			foreach (char letter in message.ToCharArray()) 
			{
				if( char.Equals(letter,'[') )
					isColorCode = true;					
					
				newText += letter;

				if( isColorCode )
				{
					if ( newText.Substring(newText.Length - 1, 1) == "]" )
					{
						PlaySound();
						mTextLabel.text += newText;
						isColorCode = false;
						newText ="";
						//yield return 0;
						yield return new WaitForSeconds (letterPause);
					}
				}
				else
				{
					PlaySound();
					mTextLabel.text += newText;
					newText ="";
					//yield return 0;
					yield return new WaitForSeconds (letterPause);
				}
			}      
		}

		private void PlaySound()
		{

			/*
			 if (mAudioGameController)
				if(mTextLabel.text.Length % 3 == 0)
					mAudioGameController.PlayDefinition(SoundDefinitions.TYPE_WRITER);
			*/
		}
		
		SmallTheater mSmallTheater;
		
		private UILabel mTextLabel;
		private string message;
		//private AudioInGameController mAudioGameController;
	}
}
