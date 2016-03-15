﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestartGame : MonoBehaviour
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
   public void Restart(MenuElement invoker)
   {
      ResetElement[] resetElements = GameObject.FindObjectsOfType<ResetElement>();
      foreach (ResetElement re in resetElements)
      {
         re.Restart();
      }
      Debug.Log("Restarting demo");
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
   private bool _initiated;
   #endregion  //End private members
}
