using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
public class GoalEvaluator : MonoBehaviour {
	
	//protected GoalThink brain;
	public float characterBias = 1;

	public virtual float CalculateDesirability() { return 0; }
	public virtual void   SetGoal() {}
	
	void Start () {
		//brain = gameObject.GetComponent<GoalThink>();
	}
}
	
}
