using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldTracking : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Transform reference for tracking.
   /// </summary>
   public Transform TrackingTransform;
   /// <summary>
   /// Local position respect new parent.
   /// </summary>
   public Vector3 WorldRelativePosition = Vector3.zero;
   /// <summary>
   /// If true, the transform's localRotation will be set to Quaternion.identity.
   /// </summary>
   public bool TrackRotation = false;
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
      _initiated = TrackingTransform != null;
      if (_initiated)
      {
         transform.SetParent(TrackingTransform);
         transform.localPosition = WorldRelativePosition;
         if (TrackRotation)
         {
            transform.localRotation = Quaternion.identity;
         }
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (TrackingTransform == null ? " Transfrom reference missing." : string.Empty) +
                   " </color>");
      }
   }

   /// <summary>
   /// Unity Update method.
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         transform.position = TrackingTransform.position + WorldRelativePosition;
         if (TrackRotation)
         {
            transform.rotation = TrackingTransform.rotation;
         }
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
