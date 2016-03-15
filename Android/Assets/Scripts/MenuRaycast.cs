using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MenuRaycast : ResetElement
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Layer targeted for raycast collisions.
   /// </summary>
   public LayerMask TargetLayer;
   /// <summary>
   /// Distance where to cast the ray.
   /// </summary>
   public float HitDistance = 10;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Method to be called when reset.
   /// </summary>
   public override void Restart()
   {
      Init();
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
      Init();
   }
   /// <summary>
   /// Unity FixedUpdate() method
   /// </summary>
   void FixedUpdate()
   {
      RaycastHit hit;
      if (Physics.Raycast(transform.position, transform.forward, out hit, HitDistance, TargetLayer))
      {
         MenuElement newHit = hit.collider.gameObject.GetComponent<MenuElement>();
         if (newHit != _lastHit)
         {
            if (_lastHit != null)
            {
               _lastHit.Unhover();
            }
            newHit.Hover();
            _lastHit = newHit;
         }
      }
      else {
         if (_lastHit != null)
         {
            _lastHit.Unhover();
            _lastHit = null;
         }
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void Init()
   {
      _lastHit = null;
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private MenuElement _lastHit;
   #endregion  //End private members
}
