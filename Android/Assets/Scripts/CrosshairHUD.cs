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
   /// Reference to the camera transform where the crosshair will be placed.
   /// </summary>
   public Transform CameraRef;
   /// <summary>
   /// Graphic used as crosshair.
   /// </summary>
   public SightPointer CrosshairGraphicPref;
   /// <summary>
   /// Distance where the crosshair will be placed in front of the camera.
   /// </summary>
   [Range(0.5f, 20f)]
   public float CROSSHAIR_DISTANCE = 1.75f;
   public Transform CrosshairGraphic
   {
      get { return _initiated ? _crosshairRef.transform : null; }
   }
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
         SightPointer auxRef = GameObject.Instantiate<SightPointer>(CrosshairGraphicPref);
         auxRef.Init();
         _crosshairRef = auxRef.gameObject;
         _crosshairRef.transform.SetParent(CameraRef);
         _crosshairRef.transform.localPosition = new Vector3(0, 0, CROSSHAIR_DISTANCE);
         _crosshairRef.transform.localRotation = Quaternion.identity;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (CameraRef == null ? " Camera reference missing." : string.Empty) +
                   (_crosshairRef == null ? " Crosshair \'SightPointer\' object reference missing." : string.Empty) + " </color>");
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
   private GameObject _crosshairRef;
   #endregion  //End private members
}
