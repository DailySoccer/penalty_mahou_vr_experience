using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuManager : ResetElement
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public delegate void VoidNoParams();
   public event VoidNoParams StartCall;
   /// <summary>
   /// MenuElement to start application when hovered.
   /// </summary>
   public MenuElement StartButton;
   /// <summary>
   /// Button to restart application when hovered.
   /// </summary>
   public MenuElement RestartButton;
   /// <summary>
   /// MenuElement references to select from when hovered.
   /// </summary>
   public List<MenuElement> SelectElements = new List<MenuElement>();
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Assign method to be called when start button
   /// </summary>
   /// <param name="method"></param>
   public void SetStartCall(VoidNoParams method)
   {
      if (StartCall == null)
      {
         StartCall = method;
      }
      else
      {
         Debug.Log("<color=#8B2252FF>" + this.GetType().ToString() + ".cs - Warning: Parameter conflict." + " There is already a trigger event defined."
            + " </color>");
      }
   }
   /// <summary>
   /// Clear method to be called when start button triggerd.
   /// </summary>
   public void ClearStartCall()
   {
      StartCall = null;
   }
   /// <summary>
   /// 
   /// </summary>
   public override void Restart()
   {
      _lastSelected = null;
      _selectedOption = -1;
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
      _initiated = RestartButton != null && StartButton != null && SelectElements != null && SelectElements.Count > 0;
      if (_initiated)
      {
         RestartGame reference = GameObject.FindObjectOfType<RestartGame>();
         if (reference != null)
         {
            RestartButton.SetTriggerFunction(reference.Restart);
         }
         StartButton.SetTriggerFunction(StartButtonAction);
         foreach (MenuElement me in SelectElements)
         {
            me.SetTriggerFunction(SelectButtonAction);
         }
         _selectedOption = -1;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (StartButton == null ? " Start button \'MenuElement\' reference missing." : string.Empty) +
                   (RestartButton == null ? " Restart button \'MenuElement\' reference missing." : string.Empty) +
                   (SelectElements == null || SelectElements.Count == 0 ? " Selection elements \'MenuElement\' references missing." : string.Empty) +
                   " </color>");
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
   private void StartButtonAction(MenuElement invoker)
   {
      if (_initiated)
      {
         if (StartCall != null && _selectedOption != -1)
         {
            StartCall();
         }
      }
   }
   private void SelectButtonAction(MenuElement invoker)
   {
      invoker.Select();
      if (_lastSelected != null)
      {
         _lastSelected.Deselect();
      }
      _lastSelected = invoker;
      _selectedOption = SelectElements.IndexOf(invoker);
      //TODO add texture change, maybe take texture from invoker?
      //Debug.Log("<color=blue>Selected index: " + _selectedOption + "</color>");
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private MenuElement _lastSelected = null;
   private int _selectedOption;
   #endregion  //End private members
}
