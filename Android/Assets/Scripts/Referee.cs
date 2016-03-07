using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Referee : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public Shooter ShooterReference;
   public AudioClip Whistle;
   public float WAIT_TIME = 30;
   public float FIRST_WHISTLE = 10;
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
      _initiated = ShooterReference != null && Whistle != null;
      if (_initiated)
      {
         _startTime = Time.time;
         _pending = true;
         _firstWhistle = true;
         _whistlePlayer = gameObject.GetComponent<AudioSource>();
         _whistlePlayer.clip = Whistle;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined. </color>");
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         if (_firstWhistle && Time.time - _startTime > FIRST_WHISTLE)
         {
            _firstWhistle = false;
            _whistlePlayer.Play();
         }
         if (_pending && Time.time - _startTime > WAIT_TIME)
         {
            _pending = false;
            _whistlePlayer.Play();
            ShooterReference.AbleToShoot = true;
            _startTime = Time.time;
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
   private float _startTime;
   private bool _pending;
   private bool _firstWhistle;
   private bool _initiated;
   private AudioSource _whistlePlayer;
   #endregion  //End private members
}
