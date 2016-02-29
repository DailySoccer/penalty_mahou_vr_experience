using UnityEngine;
using System;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class EventTriggerArgs : EventArgs {
		public Trigger Trigger;
		
		public EventTriggerArgs( Trigger trigger ) {
			Trigger = trigger;
		}
	};
	
	public class Trigger : MonoBehaviour {
		public float Radio = 15f;
		public float SqrRadio {
			get {
				return Radio * Radio;
			}
		}
		public event EventHandler<EventTriggerArgs> OnEnter;
		public event EventHandler<EventTriggerArgs> OnExit;
		
		public bool Contains( Vector3 point ) {
			float sqrLen = (transform.position - point).sqrMagnitude;
			return sqrLen < SqrRadio;
		}
		
		public virtual void Initialize() {}
		public virtual void Enter( Trigger other ) {
			if ( OnEnter != null )
				OnEnter( this, new EventTriggerArgs( other ) );
		}
		
		public virtual void Exit( Trigger other ) {
			if ( OnExit != null )
				OnExit( this, new EventTriggerArgs( other ) );
		}
	}
	
}
