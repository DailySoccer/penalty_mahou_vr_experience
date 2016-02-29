using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Locomotion
	{
	    private Animator mAnimator = null;
	    
	    private float mSpeedDampTime = 0.3f;
	    private float mAngularSpeedDampTime = 0.25f;
	    private float mDirectionResponseTime = 0.4f;
	    
	    public Locomotion(Animator animator) {
	        mAnimator = animator;
	    }
	
	    public void Do(float speed, float direction) {
	        AnimatorStateInfo state = mAnimator.GetCurrentAnimatorStateInfo(0);
			
	        bool inTransition = mAnimator.IsInTransition(0);
	        bool inIdle = (state.tagHash == AnimatorID.Idle);
	        //bool inTurn = state.IsName("Locomotion.TurnOnSpot");
			bool inTurn = Helper.IsAnimationPlaying( mAnimator, AnimatorID.Girar );
	        bool inWalkRun = (state.tagHash == AnimatorID.Front);
			bool inRegate = (state.tagHash == AnimatorID.Regatear);
			
	        float speedDampTime = inIdle || inTurn ? 1f : mSpeedDampTime;
	        float angularSpeedDampTime = inWalkRun || inTransition ? mAngularSpeedDampTime : 0;
	        // float directionDampTime = inTurn || inTransition ? 1000000 : 0;
			
			angularSpeedDampTime = mAngularSpeedDampTime;
	
	        float angularSpeed = direction / mDirectionResponseTime;
			
			if ( !inTransition && !inRegate ) {
		        mAnimator.SetFloat( AnimatorID.Speed, speed, speedDampTime, Time.deltaTime );
				mAnimator.SetFloat( AnimatorID.SpeedFinal, speed );
			}
	        mAnimator.SetFloat( AnimatorID.AngularSpeed, angularSpeed, angularSpeedDampTime, Time.deltaTime );

			if ( inTurn ) {
				// Comprobar si nos hemos pasado de la dirección deseada
				float animatorDirection = mAnimator.GetFloat( AnimatorID.Direction );
				if ( (direction > 0f) && (animatorDirection < 0f) ||
				     (direction < 0f) && (animatorDirection > 0f) ) {
					// Fijar la dirección
					mAnimator.SetFloat( AnimatorID.Direction, direction /*, 0.2f, Time.deltaTime*/ );
					// Helper.Log( mAnimator.gameObject, "Stop Direction: " + direction );
				}
			}
			if ( !inTurn && !inTransition ) {
	        	mAnimator.SetFloat( AnimatorID.Direction, direction /*, directionDampTime, Time.deltaTime*/ );
			}
			
			/*
			if ( mAnimator.name == "Soccer-Local8" )
				Helper.Log( mAnimator.gameObject, "Speed: " + speed + " Time: " + speedDampTime + " Anim: " + mAnimator.GetFloat (AnimatorID.Speed) );
			*/
	    }	
	}
}	
