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
   public void TriggerShoot()
   {
      ShooterPlayer.Shoot();
   }

   public void GoalKeeperJump()
   {
      int rand = Random.Range(0, 6);
      switch (rand)
      {
         case 0:
            GoalKeeper.SetBool("LL", true);
            break;
         case 1:
            GoalKeeper.SetBool("LM", true);
            break;
         case 2:
            GoalKeeper.SetBool("LH", true);
            break;
         case 3:
            GoalKeeper.SetBool("RL", true);
            break;
         case 4:
            GoalKeeper.SetBool("RM", true);
            break;
         case 5:
            GoalKeeper.SetBool("RH", true);
            break;
      }
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
   #endregion  //End private methods

   //-----------------------------------------------------------//
   //                      PRIVATE MEMBERS                      //
   //-----------------------------------------------------------//
   #region Private members
   #endregion  //End private members
}
