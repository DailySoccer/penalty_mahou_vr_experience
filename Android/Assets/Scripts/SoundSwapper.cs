using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundSwapper : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Max volume the clips will be reproduced at
   /// </summary>
   [Range(0,1)]
   public float MaxVolume = 1;
   /// <summary>
   /// New parent for the oculus camera to be repositioned.
   /// </summary>
   public AudioClip[] Sounds;
   /// <summary>
   /// Time delayed for audio player start.
   /// </summary>
   [Range(0.01f, 10)]
   public float DelayPlay = 1;
   /// <summary>
   /// Time the fade in and fade out effects will last.
   /// </summary>
   [Range(0.01f,3)]
   public float FadeTime = 1;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Start playing sounds.
   /// </summary>
   public void PlaySound()
   {
      if (_initiated)
      {
         if (_paused)
         {
            NextCicle();
            _paused = false;
         }
      }
   }
   /// <summary>
   /// Pause playing sounds.
   /// </summary>
   public void PauseSound()
   {
      if (_initiated)
      {
         if (!_paused)
         {
            _audioPlayer.Stop();
            _paused = true;
         }
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   /// <summary>
   /// Unity Start() method
   /// </summary>
   void Start()
   {
      _initiated = Sounds != null && Sounds.Length != 0;
      if (_initiated)
      {
         _paused = true;
         _audioPlayer = gameObject.GetComponent<AudioSource>();
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. Sounds references missing. </color>");
      }
   }

   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         if (!_paused && Time.time - _startDelay > _nextDelay)
         {
            if (_letsPlaySound)
            {
               _letsPlaySound = false;
               StartReproduction();
            }
            else
            {
               float volume = Mathf.Clamp01((_clipLengthHalf - Mathf.Abs( _audioPlayer.time - _clipLengthHalf))/FadeTime);
               _audioPlayer.volume = volume * MaxVolume;
               if (_audioPlayer.time > _clipLengthHalf && _audioPlayer.volume < 0.05f)
               {
                  NextCicle();
               }
            }
         }
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void NextCicle()
   {
      _audioPlayer.Stop();
      _startDelay = Time.time;
      _nextDelay = Random.Range(DelayPlay * 0.5f, DelayPlay);
      _audioPlayer.clip = Sounds[Random.Range(0, Sounds.Length)];
      _clipLength = _audioPlayer.clip.length;
      _clipLengthHalf = _clipLength * 0.5f;
      _letsPlaySound = true;
   }

   private void StartReproduction()
   {
      _audioPlayer.volume = 0;
      _audioPlayer.Play();
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private AudioSource _audioPlayer;
   private float _startDelay;
   private float _nextDelay;
   private float _clipLength;
   private float _clipLengthHalf;
   private bool _letsPlaySound;
   private bool _paused;
   #endregion  //End private members
}
