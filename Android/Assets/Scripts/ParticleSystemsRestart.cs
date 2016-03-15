using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ParticleSystemsRestart : ResetElement
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
   /// Method to trigger when reset.
   /// </summary>
   public override void Restart()
   {
      ParticleSystem[] aux = gameObject.GetComponentsInChildren<ParticleSystem>();
      foreach (ParticleSystem ps in aux)
      {
         ps.Stop();
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
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
