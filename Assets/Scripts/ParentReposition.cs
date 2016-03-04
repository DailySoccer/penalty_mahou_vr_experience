using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParentReposition : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// New parent for the oculus camera to be repositioned.
   /// </summary>
   public Transform NewParent;
   /// <summary>
   /// Local position respect new parent.
   /// </summary>
   public Vector3 NewLocalPosition = Vector3.zero;
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
      _initiated = NewParent != null;
      if (_initiated)
      {
         transform.SetParent(NewParent);
         transform.localPosition = NewLocalPosition;
         transform.localRotation = Quaternion.identity;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (NewParent == null ? " Transfrom reference missing." : string.Empty) +
                   " </color>");
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
