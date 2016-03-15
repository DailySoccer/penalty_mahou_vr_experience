using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shooter : ResetElement
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public bool AbleToShoot = false;
   /// <summary>
   /// AudioSource ball kick origin.
   /// </summary>
   public AudioSource BallKick;
   /// <summary>
   /// Ball to be hit.
   /// </summary>
   public Rigidbody Ball;
   /// <summary>
   /// Force to be applied to shoot.
   /// </summary>
   public float Force = 5;
   /// <summary>
   /// Percentage error to vary the final force applied.
   /// </summary>
   [Range(0.01f,0.2f)]
   public float ErrorForcePer = 0.1f;
   /// <summary>
   /// Angle to deviate the shoot trajectory horizontally.
   /// </summary>
   [Range(0,10)]
   public float ErrorDeviation = 0.1f;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   /// <summary>
   /// Sets target location for shooting.
   /// </summary>
   /// <param name="newTarget"></param>
   public void SetTarget(Vector3 newTarget)
   {
      _target = newTarget;
   }
   /// <summary>
   /// Shoots rigidbody ball towards inner target.
   /// </summary>
   public void Shoot()
   {
      Shoot(_target);
   }
   /// <summary>
   /// Respawns rigidbody ball in penalty location.
   /// </summary>
   public void Respawn()
   {
      if (_initiated)
      {
         Ball.velocity = Ball.angularVelocity = Vector3.zero;
         Ball.transform.position = _respawnPoint;
         AbleToShoot = false;
      }
   }
   /// <summary>
   /// Method to trigger when reset.
   /// </summary>
   public override void Restart()
   {
      Respawn();
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
      _initiated = Ball != null && BallKick != null;
      if (_initiated)
      {
         _respawnPoint = Ball.transform.position;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." + (Ball == null ? " Rigidbody reference missing." : string.Empty) +
                   (BallKick == null ? " Sound effect reference missing." : string.Empty) +
                   " </color>");
      }
   }

   //TODO remove this
   void Update()
   {
      if (_initiated)
      {
         /*if (Input.GetKeyDown(KeyCode.RightShift))
         {
            Respawn();
         }
         if (Input.GetKeyDown(KeyCode.Space))
         {
            Shoot();
         }*/
      }
   }
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private void Shoot(Vector3 targetPosition)
   {
      if (_initiated)
      {
         if (AbleToShoot)
         {
            BallKick.Play();
            Vector3 maxEnergyDir = (targetPosition - Ball.transform.position).normalized;
            maxEnergyDir += Vector3.up * 0.3f * maxEnergyDir.y;
            //TODO improve ecuation later, by now just trying with different values.
            /*
            float angleMax = Mathf.Asin(maxEnergyDir.y);
            float shootAngle = Random.Range(angleMax, Mathf.PI * 0.25f);
            */
            float errorAngle = Random.Range(-ErrorDeviation, ErrorDeviation);
            maxEnergyDir = Quaternion.AngleAxis(errorAngle, Vector3.up) * maxEnergyDir;
            float errorApplied = 1 + Random.Range(-ErrorForcePer, ErrorForcePer);
            Ball.AddForce(maxEnergyDir * Force * errorApplied, ForceMode.Impulse);
         }
      }
   }
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   private bool _initiated;
   private Vector3 _respawnPoint = Vector3.zero;
   private Vector3 _target = Vector3.zero;
   #endregion  //End private members
}
