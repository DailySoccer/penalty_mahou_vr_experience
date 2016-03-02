using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GoalDetector : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public delegate void voidNoparams();
   /// <summary>
   /// Event to be triggered when the rigidbody enters a trigger with 'Goal' tag.
   /// </summary>
   public event voidNoparams GoalScored = null;
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
      _state = MotionState.Moving;
      _myRigidbody = gameObject.GetComponent<Rigidbody>();
   }

   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_state == MotionState.Quiet)
      {
         if (_myRigidbody.velocity.magnitude < ZERO_PRECISSION)
         {
            _myRigidbody.angularVelocity = _myRigidbody.velocity = Vector3.zero;
            _state = MotionState.Moving;
         }
      }
   }

   /// <summary>
   /// Unity's OnTriggerEnter() method. Executed when entering a trigger collider.
   /// </summary>
   /// <param name="other">Collider the collision was detected with.</param>
   void OnTriggerEnter(Collider other)
   {
      if (other.tag.Equals("Goal")){
         if (_state == MotionState.Moving)
         {
            if (GoalScored != null)
            {
               GoalScored();
            }
         }
         _state = MotionState.Quiet;
         Debug.Log("<color=green>Collision with " + other.transform.name + "</color>");
      }
   }

   /// <summary>
   /// Unity's OnTriggerEnter() method. Executed when entering a trigger collider.
   /// </summary>
   /// <param name="other">Collider the collision was detected with.</param>
   void OnCollisionEnter(Collision collision)
   {
      if (_state == MotionState.Quiet)
      {
         _myRigidbody.angularVelocity *= 0.25f;
         _myRigidbody.velocity *= 0.25f;
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
   private enum MotionState
   {
      Quiet,
      Moving
   };

   private bool _initiated;
   private MotionState _state;
   private float ZERO_PRECISSION = 0.05f;
   private Rigidbody _myRigidbody;
   #endregion  //End private members
}
