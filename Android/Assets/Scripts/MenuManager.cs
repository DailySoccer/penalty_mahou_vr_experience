using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// MenuElement to start application when hovered.
   /// </summary>
   public MenuElement StartButton;
   /// <summary>
   /// MenuElement references to select from when hovered.
   /// </summary>
   public List<MenuElement> SelectElements = new List<MenuElement>();
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
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
      _initiated = StartButton != null && SelectElements != null && SelectElements.Count > 0;
      if (_initiated)
      {
         StartButton.SetTriggerFunction(StartButtonAction);
         foreach (MenuElement me in SelectElements)
         {
            me.SetTriggerFunction(SelectButtonAction);
         }
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (StartButton == null ? " Start button \'MenuElement\' reference missing." : string.Empty) +
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
      //TODO function to activate start of game
      GameObject.FindObjectOfType<RestartAnimators>().Restart();
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
      Debug.Log("<color=\"blue\">Selected index: " + _selectedOption + "</color");
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
