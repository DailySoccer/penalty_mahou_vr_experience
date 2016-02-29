using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class GoalThink : GoalComposite {
		
		public SoccerData Data { get; set; }
		public float ThinkTime = 1f;
		
		private GoalEvaluator[] goalsEvaluators;
		private GoalEvaluator currentEvaluator;
		private float currentDesirability = 0;
		private float timing = 0;
		
		private MatchManager mMatchManager;
		
		void Awake () {
			Data = gameObject.GetComponent<SoccerData>();
		}
		
		void Start () {
			goalsEvaluators = gameObject.GetComponents<GoalEvaluator>();
			
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			Assert.Test ( () => mMatchManager, "No existe MatchManager" );
			if ( !mMatchManager )
				enabled = false;
			
			/*
			foreach ( GoalEvaluator evaluator in goalsEvaluators ) {
				Debug.Log ( evaluator.ToString() );
			}
			*/
		}
		
		void Update () {
			Process();
		}
		
		public override void Activate () {
			// status = Status.Active;
			Arbitrate ();
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			timing += Time.deltaTime;
			if ( (timing > ThinkTime) || Data.NewOrder ) {
				Arbitrate();
			}
				
			if ( isActive () ) {
				status = ProcessSubgoals();
				
				if ( (status == Status.Failed) || (status == Status.Completed) ) {
					status = Status.Inactive;
					currentEvaluator = null;	
				}
			}			
	
			return status;
		}
			
		protected void Arbitrate () {
			float best = 0;
			
			if ( isActive() && (currentEvaluator != null) ) {
				best = currentDesirability;
			}
			
			GoalEvaluator mostDesirable = null;
			
			foreach ( GoalEvaluator evaluator in goalsEvaluators ) {
				if (!evaluator.enabled) continue;
				
				float desirability = evaluator.CalculateDesirability();
				
				if ( desirability > best ) {
					best = desirability;
					mostDesirable = evaluator;
				}
			}
			
			if ( mostDesirable ) {
				// Helper.Log ( () => { return ( GameObjectName == "Soccer-Local7" ) && ( mostDesirable != currentEvaluator ); }, mostDesirable.ToString () );
				// Helper.Log ( () => { return ( GameObjectName == "Soccer-Local7" ) && ( mostDesirable == currentEvaluator ); }, mostDesirable.ToString () + " continue" );
				
				bool isDifferent = !IsEvaluatorRunning( mostDesirable );
				
				if ( !isActive () || (mostDesirable != currentEvaluator) || isDifferent ) {
					status = Status.Active;
					
					Profiler.BeginSample( "SetGoal" );
					mostDesirable.SetGoal();
					Profiler.EndSample();
					
					currentEvaluator = mostDesirable;
					
					// TODO: Cómo gestionar los evaluators que añaden su goal a una lista anterior (ej. PasarBalon_Evaluator) 
					if ( SubGoals.Count == 1 ) {
						currentDesirability = best;
					}
				}
			}
			else {
				// Debug.LogError ( "No evaluator selected" );
			}
			
			timing = 0;
			
			Data.NewOrder = false;
		}
	}
	
}
