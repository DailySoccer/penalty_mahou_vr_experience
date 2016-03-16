﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StartGame : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public SoundLocalManager[] SoundManagers;
   public Animator StartAnimator;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   public void StartFromTeamSelection()
   {
      StartMethod();
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
      _initiated = SoundManagers != null && SoundManagers.Length != 0 && StartAnimator != null;
      if (_initiated)
      {
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. </color>");
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void StartMethod()
   {
      if (_initiated)
      {
         foreach (SoundLocalManager slm in SoundManagers)
         {
            slm.PlaySound();
         }
         StartAnimator.SetBool("Start", true);
      }
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   #endregion  //End private members
}
