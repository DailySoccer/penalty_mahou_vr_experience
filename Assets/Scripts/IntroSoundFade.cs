using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroSoundFade : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// List of audio sources to control its volume.
   /// </summary>
   public SoundSwapper[] GeneralAudios;
   public AudioSource Heart;
   /// <summary>
   /// Initial volume for the audios.
   /// </summary>
   [Range(0, 1)]
   public float InitVolume = 0.2f;
   /// <summary>
   /// Ending volume for the audios.
   /// </summary>
   [Range(0, 1)]
   public float FinalVolume = 0.6f;
   /// <summary>
   /// Fade in time for the audios going from initial volume to final volume.
   /// </summary>
   [Range(0.01f, 3)]
   public float FadeTime = 2;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Method to trigger audio volume animation.
   /// </summary>
   public void TriggerAnimation()
   {
      _animateVolume = true;
      _startTime = Time.time;
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
      _initiated = GeneralAudios != null && GeneralAudios.Length != 0 && Heart != null;
      if (!_initiated)
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. Sounds references missing. </color>");
      }
      else
      {
         _initHeartVolume = Heart.volume;
         foreach (SoundSwapper ss in GeneralAudios)
         {
            ss.MaxVolume = InitVolume;
         }
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         if (_animateVolume)
         {
            float diff = Time.time - _startTime;
            float perc = diff / FadeTime;
            float volume = InitVolume + (FinalVolume - InitVolume) * Mathf.Clamp01(perc);
            foreach (SoundSwapper ss in GeneralAudios)
            {
               ss.MaxVolume = volume;
            }
            Heart.volume = (1 - Mathf.Clamp01(perc)) * _initHeartVolume;
            _animateVolume = perc < 1;
         }
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private bool _animateVolume;
   private float _startTime;
   private float _initHeartVolume;
   #endregion  //End private members
}
