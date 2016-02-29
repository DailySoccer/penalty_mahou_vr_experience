using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	public class GoalNode : Object {
		public Goal Goal = null;
		public GoalEvaluator Evaluator = null;
		public float Desirability = 0f;
	};
	
	public class GoalComposite : Goal {
		public Stack<GoalNode> SubGoals = new Stack<GoalNode>();
		
		public static GoalComposite New (GameObject _owner) {
			GoalComposite goal = _owner.AddComponent<GoalComposite>();
			return goal;
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			Status subgoalStatus = ProcessSubgoals();
			
			if ( (subgoalStatus == Status.Completed) || (subgoalStatus == Status.Failed) ) {
				status = Status.Inactive;
			}
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate ();
			RemoveAllSubgoals();
		}
		
		public override void AddSubgoal (Goal g) {
			GoalNode node = new GoalNode();
			node.Goal = g;
			node.Evaluator = null;
			node.Desirability = 0f;
			SubGoals.Push( node );
		}
		
		public override void AddSubgoal (Goal g, GoalEvaluator evaluator, float desirability) {
			GoalNode node = new GoalNode();
			node.Goal = g;
			node.Evaluator = evaluator;
			node.Desirability = desirability;
			SubGoals.Push( node );
		}		
		
		public void RemoveAllSubgoals () {
			foreach (GoalNode node in SubGoals) {
				node.Goal.Terminate();
				node.Goal.enabled = false; //Destroy ( node.Goal );
				Destroy ( node );
			}
			SubGoals.Clear();
		}

		public bool IsEvaluatorRunning( GoalEvaluator evaluator ) {
			foreach (GoalNode node in SubGoals) {
				if ( node.Evaluator == evaluator )
					return true;
			}
			return false;
		}
		
		protected Status ProcessSubgoals () {
			while ( (SubGoals.Count > 0) && 
					(SubGoals.Peek ().Goal.isComplete() || SubGoals.Peek ().Goal.hasFailed()) ) { 
				GoalNode node = SubGoals.Peek ();
				node.Goal.Terminate();
				node.Goal.enabled = false; //Destroy ( node.Goal );
				Destroy ( node );
				SubGoals.Pop ();
			}
			
			if ( SubGoals.Count > 0 ) {
				Status statusOfSubGoals = SubGoals.Peek ().Goal.Process();
				
				if ( (statusOfSubGoals == Status.Completed) && (SubGoals.Count > 1) ) {
					return Status.Active;
				}
				
				return statusOfSubGoals;
			}
			
			return Status.Completed;
		}
	}
	
}
