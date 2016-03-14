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
   /// <summary>
   /// If true, the transform's localRotation will be set to Quaternion.identity.
   /// </summary>
   public bool ResetLocalRotation = true;
   /// <summary>
   /// Time to reposition to parent trnasform.
   /// </summary>
   [Range(0,20)]
   public float RepositionFactor = 0;
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
         if (RepositionFactor == 0)
         {
            transform.SetParent(NewParent);
            transform.localPosition = NewLocalPosition;
            if (ResetLocalRotation)
            {
               transform.localRotation = Quaternion.identity;
            }
         }
         else
         {
            _restarted = false;
         }
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (NewParent == null ? " Transfrom reference missing." : string.Empty) +
                   " </color>");
      }
   }

   /// <summary>
   /// Unity Update method.
   /// </summary>
   void Update()
   {
      if (_initiated && RepositionFactor != 0)
      {
         Vector3 targetPosition = NewParent.position + NewParent.right * NewLocalPosition.x + NewParent.up * NewLocalPosition.y + NewParent.forward * NewLocalPosition.z;
         if ((transform.position - targetPosition).magnitude > 0.5f || Quaternion.Angle(transform.rotation, NewParent.rotation) > 5)
         {
            /*float now = Time.time;
            if (!_restarted)
            {
               _restarted = true;
               _startTime = now;
            }*/
            //float perc = (now - _startTime) / RepositionTime;
            float formatPerc = Time.deltaTime * RepositionFactor;/*Mathf.Clamp01(perc);*/
            transform.position = Vector3.Lerp(transform.position, targetPosition, formatPerc);
            transform.rotation = Quaternion.Lerp(transform.rotation, NewParent.rotation, formatPerc);
            /*if (perc >= 1)
            {
               _restarted = false;
            }*/
         }
         else
         {
            _restarted = false;
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
   private bool _restarted;
   private float _startTime;
   private Vector3 _oldPosition;
   #endregion  //End private members
}
