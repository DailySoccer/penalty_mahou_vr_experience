using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestartAnimators : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Restart animators and audio sources in the scene.
   /// </summary>
   public void Restart()
   {
      Animator[] animators = GameObject.FindObjectsOfType<Animator>();
      SoundLocalManager[] sounds = GameObject.FindObjectsOfType<SoundLocalManager>();
      foreach (SoundLocalManager slm in sounds)
      {
         slm.PauseSound();
      }
      foreach (Animator ani in animators)
      {
         ani.SetBool("Restart", true);
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
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
   #endregion  //End private members
}
