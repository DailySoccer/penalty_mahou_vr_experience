using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundLocalManager : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Sound sources to be ableto pause are resume.
   /// </summary>
   public SoundSwapper[] SoundGroup;
   public AudioSource[] SoundSourceGroup;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Pause playing sounds.
   /// </summary>
   public void PauseSound()
   {
      if (!_paused)
      {
         _paused = true;
         foreach (SoundSwapper ss in SoundGroup)
         {
            ss.PauseSound();
         }
         foreach (AudioSource ads in SoundSourceGroup)
         {
            ads.Stop();
         }
      }
   }
   /// <summary>
   /// Start playing sounds.
   /// </summary>
   public void PlaySound()
   {
      if (_paused)
      {
         _paused = false;
         foreach (SoundSwapper ss in SoundGroup)
         {
            ss.PlaySound();
         }
         foreach (AudioSource ads in SoundSourceGroup)
         {
            ads.Play();
         }
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   void Start()
   {
      _initiated = SoundGroup != null && SoundGroup.Length != 0 && SoundSourceGroup != null && SoundSourceGroup.Length != 0;
      if (_initiated)
      {
         _paused = true;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. SoundSwapper references missing. </color>");
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
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
   private bool _paused;
   #endregion  //End private members
}
