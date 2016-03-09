using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Minigame : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public float TOTAL_TIME = 60;
   public Animator EndGame;
   public ShootTrigger GoalkeeperJumper;
   public Shooter PlayerShoot;
   public UseReferee RefereeReference;
   public Transform GoalKeeper;
   public Raytracer Raycaster;
   public CrosshairHUD Crosshair;
   public Image Clock;
   public Transform ClockTrans;
   public float HUDAnimTime = 2;
   public float AdaptFactor = 1;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
      /// <summary>
      /// Method to start the minigame
      /// </summary>
   public void StartGame()
   {
      _startTime = Time.time;
      _active = true;
      _finished = false;
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   void Start()
   {
      _initiated = GoalKeeper != null && Raycaster != null && Crosshair != null && Clock != null;
      if (_initiated)
      {
         ClockTrans.localScale = Crosshair.CrosshairGraphic.localScale = Vector3.zero;
         _INITGoalKeeper = GoalKeeper.rotation;
         _goalkeeperJump = GoalKeeper.position;
         HALF_PERIOD = TOTAL_TIME * 0.5f + HUDAnimTime;
      }
      else
      {
         Debug.Log("<color=#FFA500FF>" + this.GetType().ToString() + ".cs - Warning: Initial parameters undefined." +
                     " </color>");
      }
   }
   /// <summary>
   /// Unity Update() method
   /// </summary>
   void Update()
   {
      if (_initiated)
      {
         if (_active)
         {
            Vector3 target = Raycaster.RaycastPoint();
            float now = Time.time;
            float percTime = now - (_startTime + HUDAnimTime) / TOTAL_TIME;
            Clock.fillAmount = 1 - Mathf.Clamp01(percTime);
            if (Clock.fillAmount < 1)
            {
               _goalkeeperJump += (target - _goalkeeperJump).normalized * Time.deltaTime * AdaptFactor;
               Vector3 seeTarget = _goalkeeperJump + Vector3.right * 5;
               seeTarget.y = 0;
               GoalKeeper.rotation = Quaternion.Lerp(GoalKeeper.rotation, Quaternion.LookRotation(_goalkeeperJump), AdaptFactor * Time.deltaTime);
            }
            else if (Clock.fillAmount == 1 && !_finished)
            {
               _finished = false;
               PlayerShoot.SetTarget(Raycaster.RaycastPoint());
               GoalkeeperJumper.SetTargetJump(_goalkeeperJump);
               RefereeReference.AbleToShoot();
            }
            float perc = (HALF_PERIOD - Mathf.Abs(now - HALF_PERIOD)) / HUDAnimTime;
            float clampPerc = Mathf.Clamp01(perc);
            GoalKeeper.rotation = Quaternion.Lerp(_INITGoalKeeper, GoalKeeper.rotation, clampPerc);
            Crosshair.CrosshairGraphic.localScale = ClockTrans.localScale = Vector3.one * clampPerc;
            _active = (now - _startTime) < (TOTAL_TIME + 2 * HUDAnimTime);
            if (!_active)
            {
               EndGame.SetBool("Shoot", true);
            }
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
   private bool _active;
   private bool _finished;
   private float _startTime;
   private Vector3 _goalkeeperJump;
   private Quaternion _INITGoalKeeper;
   private float HALF_PERIOD;
   #endregion  //End private members
}
