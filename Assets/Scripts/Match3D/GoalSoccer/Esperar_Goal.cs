using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class Esperar_Goal : GoalComposite {
	
	private float seconds = 0;
	private SoccerData data;
	
	public static Esperar_Goal New( GameObject _owner, float _seconds ) {
		Esperar_Goal goal = Create<Esperar_Goal>(_owner);
		goal.Set(_seconds );
		return goal;
	}
		
	protected void Set(float _seconds ) {
		if ( InitializeGlobal ) {
			data = gameObject.GetComponent<SoccerData>();
		}
		seconds = _seconds;
	}
	
	public override void Activate () {
		status = Status.Active;
		
		RemoveAllSubgoals();
		
		ArriveSteer arriveSteer = gameObject.GetComponentInChildren<ArriveSteer>();
		arriveSteer.enabled = true;
		arriveSteer.Target = gameObject;
		
		AddSubgoal ( WaitForSeconds_Goal.New( gameObject, seconds ) );
	}
	
	public override Status Process () {
		ActivateIfInactive();
		
		if ( isActive () ) {
			status = ProcessSubgoals();
			
			if ( status == Goal.Status.Completed ) {
				data.Esperar = false;
			}
			
			ReactivateIfFailed();
		}
		return status;
	}
	
	public override void Terminate () {
		base.Terminate();
		
		ArriveSteer arriveSteer = gameObject.GetComponentInChildren<ArriveSteer>();
		arriveSteer.enabled = false;
	}
}
	
}
