using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairHUD : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   /// <summary>
   /// Reference to the camera where the crosshair will be placed.
   /// </summary>
   public Camera CameraRef;
   /// <summary>
   /// Graphic used as crosshair.
   /// </summary>
   public Image CrosshairGraphicPref;
   /// <summary>
   /// Distance where the crosshair will be placed in front of the camera.
   /// </summary>
   public float CROSSHAIR_DISTANCE = 1.75f;
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

   // Use this for initialization
   /// <summary>
   /// Unity Start() method.
   /// </summary>
   void Start()
   {
      _initiated = CameraRef != null && CrosshairGraphicPref != null;
      if (_initiated)
      {
         CrosshairGraphicPref.transform.SetParent(CameraRef.transform);
         CrosshairGraphicPref.transform.localPosition = new Vector3(0, 0, CROSSHAIR_DISTANCE);
         CrosshairGraphicPref.transform.localRotation = Quaternion.identity;
      }
      else
      {
         Debug.Log("<color=s#FFA500ff>CrosshairHUD.cs - Warning: Initial parameters undefined." + (CameraRef == null ? " Camera reference missing." : string.Empty) +
                   (CrosshairGraphicPref == null ? " Crosshair image reference missing." : string.Empty) + " </color>");
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
