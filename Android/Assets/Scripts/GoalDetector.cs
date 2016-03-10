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
   public delegate void void1Bool(bool result);
   /// <summary>
   /// Event to be triggered when the rigidbody enters a trigger with 'Goal' tag.
   /// </summary>
   public event void1Bool BallEvent = null;
   public AudioSource[] Goal;
   public ParticleSystem[] GoalParticles;
   public AudioSource Out;
   public Animator Close;
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
      _initiated = Goal != null && Out != null && Close != null;
      if (!_initiated)
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." +
                     " </color>");
      }
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
      if (_initiated)
      {
         if (other.tag.Equals("Goal"))
         {
            if (_state == MotionState.Moving)
            {
               if (BallEvent != null)
               {
                  BallEvent(true);
               }
            }
            _state = MotionState.Quiet;
            foreach (AudioSource asc in Goal)
            {
               asc.Play();
            }
            foreach (ParticleSystem ps in GoalParticles)
            {
               ps.Play();
            }
            Close.SetBool("End", true);
            //Debug.Log("<color=green>Collision with " + other.transform.name + "</color>");
         }
         else if (other.tag.Equals("Out"))
         {
            if (BallEvent != null)
            {
               BallEvent(false);
            }
            Out.Play();
            Close.SetBool("End", true);
         }
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
   
   private MotionState _state;
   private float ZERO_PRECISSION = 0.05f;
   private Rigidbody _myRigidbody;
   private bool _initiated;
   #endregion  //End private members
}
