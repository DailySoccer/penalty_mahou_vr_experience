using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shooter : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Ball to be hit.
   /// </summary>
   public Rigidbody Ball;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   public void Shoot(Vector3 shootVector)
   {
      if (_initiated)
      {
         Ball.AddForce(shootVector, ForceMode.Impulse);
      }
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   // Use this for initialization
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   void Start()
   {
      _initiated = Ball != null;
      if (!_initiated)
      {
         Debug.Log("<color=#FFA500FF>CrosshairHUD.cs - Warning: Initial parameters undefined." + (Ball == null ? " Rigidbody reference missing." : string.Empty) +
                   " </color>");
      }
   }

   //TODO erase this
   void Update()
   {
      if (_initiated && Input.GetKeyDown(KeyCode.Space))
      {
         Shoot(new Vector3(-9, 3, 0.1f));
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
