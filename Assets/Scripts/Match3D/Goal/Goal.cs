using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Goal : MonoBehaviour {
		public enum Status { Active, Inactive, Completed, Failed };
		
		private bool mInitializeGlobal = true;
		protected bool InitializeGlobal {
			get {
				bool initialize = mInitializeGlobal;
				
				// REVIEW: Ya lo consideramos inicializado
				mInitializeGlobal = false;
				
				return initialize;
			}
		}
		
		// protected GameObject 	gameObject;
		protected Status		status = Status.Inactive;
		
		public GameObject 		Entity {
			get { return transform.parent.gameObject; }
		}
		
		public string			GameObjectName {
			get { return transform.parent.name;	}
		}

		public static T Create<T> ( GameObject _owner ) where T : Goal {
			T goal = _owner.GetComponent<T>();
			if ( !goal ) {
				goal = _owner.AddComponent<T>();
			}
			goal.enabled = true;
			return goal;
		}
		
		public virtual void		Activate	() { status = Status.Active; }
		public virtual Status 	Process		() { return status; }
		public virtual void		Terminate	() { StopAllCoroutines(); status = Status.Inactive; enabled = false; }
		
		public virtual void AddSubgoal  (Goal g) {}
		public virtual void AddSubgoal	(Goal g, GoalEvaluator evaluator, float desirability) {}
		
		public bool isComplete() { 
			return status == Status.Completed;
		}
		
		public bool isActive() {
			return status == Status.Active;
		}
		
		public bool isInactive() {
			return status == Status.Inactive;
		}
		
		public bool hasFailed() {
			return status == Status.Failed;
		}
		
		protected void ActivateIfInactive	() {
			if ( isInactive() ) {
				Activate ();
			}
		}
		
		protected void ReactivateIfFailed	() {
			if ( hasFailed() ) {
				status = Status.Inactive;
			}
		}
	}
	
}
