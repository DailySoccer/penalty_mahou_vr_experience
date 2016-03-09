using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UseReferee : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Reference to the referee.
   /// </summary>
   public Referee RefereeReference;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   public void BlowWhistle()
   {
      if (_initiated)
      {
         RefereeReference.SoundWhistle();
      }
   }
   public void AbleToShoot()
   {
      if (_initiated)
      {
         RefereeReference.AbleShoot(true);
         BlowWhistle();
      }
   }
   public void DisableToShoot()
   {
      if (_initiated)
      {
         RefereeReference.AbleShoot(false);
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   /// <summary>
   /// 
   /// </summary>
   void Start()
   {
      _initiated = RefereeReference != null;
      if (_initiated)
      {
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. </color>");
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
   #endregion  //End private members
}
