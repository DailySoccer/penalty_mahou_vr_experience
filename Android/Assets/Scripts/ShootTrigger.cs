using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootTrigger : MonoBehaviour
{
   //-----------------------------------------------------------//
   //                      PUBLIC MEMBERS                       //
   //-----------------------------------------------------------//
   #region Public members
   public Shooter ShooterPlayer;
   public Animator GoalKeeper;
   #endregion  //End public members

   //-----------------------------------------------------------//
   //                      PUBLIC METHODS                       //
   //-----------------------------------------------------------//
   #region Public methods
   public void SetTargetJump(Vector3 targetJump)
   {
      _jumpDir = "";
      Vector3 diff = targetJump - transform.position;
      //TODO calculate direction to jump to
   }
   public void TriggerShoot()
   {
      ShooterPlayer.Shoot();
   }

   public void GoalKeeperJump()
   {
      GoalKeeper.SetBool(_jumpDir, true);
   }

   public void GoalKeeperStand()
   {
      GoalKeeper.SetBool("Hold", true);
   }
   #endregion  //End public methods

   //-----------------------------------------------------------//
   //                  MONOBEHAVIOUR METHODS                    //
   //-----------------------------------------------------------//
   #region Monobehaviour methods
   #endregion  //End monobehaviour methods

   //-----------------------------------------------------------//
   //                      PRIVATE METHODS                      //
   //-----------------------------------------------------------//
   #region Private methods
   private string _jumpDir = "LL";
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   #endregion  //End private members
}
