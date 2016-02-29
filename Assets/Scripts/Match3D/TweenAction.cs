using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {

	public enum ActionType {
		NINGUNO,
		MOVIMIENTO,
		BALON,
		PASE,
		CHUT,
		CENTRO,
		REMATE,
		REGATE
	};
	
	class CachedField<T> {
		public bool Cached = false;
		
		public T mField = default(T);
		public T Field {
			get {
				return mField;
			}
			set {
				mField = value;
				Cached = true;
			}
		}
	}
	
	public class TweenAction {

		const string MoveID 	= "move";
		const string BalonID 	= "Balon";
		const string PaseID 	= "Pase";
		const string ChutID 	= "Chut";
		const string CentroID 	= "Centro";
		const string RemateID 	= "Remate";
		const string RematePieID 	= "RematePie";
		const string RemateCabezaID = "RemateCabeza";
		const string RegateID 	= "Regatear";
		const string RegateAID 	= "RegatearA";
		
		public float 		StartTime;
		public Entrenador	Entrenador;
		public ActionType 	Action;
		public AMTween 		Tween;
		
		public bool InteractionOn = false;
		
		public enum EState {
			None,
			Waiting,
			Evaluated,
			Running
		};
		public EState State = EState.None;
		
		private bool mResult = false;
		public bool	Result {
			get { return mResult; }
			set {
				State = EState.Evaluated;
				mResult = value;
			}
		}

		public GameObject 	Actor {
			get { return Entrenador.Target; }
		}
		
		CachedField<SoccerData> mActorData = new CachedField<SoccerData>();
		public SoccerData ActorData {
			get {
				if ( !mActorData.Cached ) {
					mActorData.Field  = Actor.GetComponentInChildren<SoccerData>();
				}
				return mActorData.Field;	
			}
		}
		
		CachedField<GameObject> mTarget = new CachedField<GameObject>();
		public GameObject Target {
			get {
				if ( !mTarget.Cached ) {
					mTarget.Field  = CalculateTarget();
				}
				return mTarget.Field;	
			}
		}
		
		CachedField<float> mHastaDestino = new CachedField<float>();
		public float HastaDestino {
			get {
				if ( !mHastaDestino.Cached ) {
					mHastaDestino.Field  = TiempoHasteDestino();
				}
				return mHastaDestino.Field;	
			}
		}

		public static bool IsPase ( string methodName ) {
			return  ( 	methodName == PaseID
			         || methodName == CentroID );
		}

        public static bool IsRegate ( string methodName ) {
			return  ( 	methodName == RegateID
			         || methodName == RegateAID );
		}

		public static bool IsChut ( string methodName ) {
			return  ( 	methodName == ChutID
			         || methodName == RemateID
			         || methodName == RematePieID
			         || methodName == RemateCabezaID );
		}

		public static bool IsInteractiveAction ( string methodName ) {
			return  ( 	 methodName == PaseID
				 	  || methodName == ChutID
				      || methodName == CentroID
			          || methodName == RemateID
			          || methodName == RematePieID
			          || methodName == RemateCabezaID
			          || methodName == RegateID
			          || methodName == RegateAID );
		}

		public static ActionType GetActionType ( AMTween tween ) {
			ActionType actionType = ActionType.NINGUNO;
			
			if 		( tween.type == MoveID ) 		actionType = ActionType.MOVIMIENTO;
			else if ( tween.isMethod(BalonID) )		actionType = ActionType.BALON;
			else if ( tween.isMethod(PaseID) ) 		actionType = ActionType.PASE;
			else if ( tween.isMethod(ChutID) ) 		actionType = ActionType.CHUT;
			else if ( tween.isMethod(CentroID) ) 	actionType = ActionType.CENTRO;
			else if ( tween.isMethod(RemateID) ) 	actionType = ActionType.REMATE;
			else if ( tween.isMethod(RematePieID) ) 	actionType = ActionType.REMATE;
			else if ( tween.isMethod(RemateCabezaID) ) 	actionType = ActionType.REMATE;
			else if ( tween.isMethod(RegateID) ) 	actionType = ActionType.REGATE;
			else if ( tween.isMethod(RegateAID) ) 	actionType = ActionType.REGATE;
			
			return actionType;
		}
		
		public static TweenAction Create ( float startTime, AMTween tween ) {
			TweenAction tweenAction = null;
			
			ActionType actionType = GetActionType( tween );
			if ( actionType != ActionType.NINGUNO ) {
				tweenAction = new TweenAction( startTime, actionType, tween );
			}
			
			return tweenAction;
		}
		
		public static int CompareStartTime (TweenAction first, TweenAction second)
    	{
			if ( first.StartTime > second.StartTime ) return 1;
			else if ( first.StartTime < second.StartTime ) return -1;
			return 0;
		}
		
		public TweenAction( float startTime, ActionType action, AMTween tween ) {
			StartTime 	= startTime;
			Action 		= action;
			Entrenador = tween.gameObject.GetComponentInChildren<Entrenador>();
			Tween 		= tween;
		}
		
		public T GetParameter<T> (int parameterIndex) {
			return Tween.GetParameter<T>(parameterIndex);
		}
		
		public bool IsType( ActionType actionType ) {
			return Action == actionType;
		}

		public bool IsPase() {
			return Action == ActionType.PASE || Action == ActionType.CENTRO;
		}

		public bool IsRegate() {
			return Action == ActionType.REGATE;
		}

		public bool IsChut() {
			return Action == ActionType.CHUT || Action == ActionType.REMATE;
		}

		public bool IsCoordinated() {
			return Action == ActionType.REGATE;
		}
		
		public float Distance {
			get {
				return (Actor.transform.position - Target.transform.position).magnitude;
			}
		}
		
		public override string ToString () {
			return StartTime.ToString("0.00") + ": " + Tween.name + ": " + Action.ToString();
		}
		
		float TiempoHasteDestino () {
			float hastaDestino = 0f;

            // Lo quito, las acciones no calculan el tiempo.
//			if ( Action == ActionType.PASE || Action == ActionType.CENTRO ) {
//				hastaDestino = GetParameter<float>( 2 );
//			}
			
			return hastaDestino;
		}
		
		GameObject CalculateTarget () {
			// Por defecto, nosotros somos el objeto de interes
			GameObject target = Actor;
			
			// Queremos pasar el valor a alguien?
			if ( Action == ActionType.PASE || Action == ActionType.CENTRO || Action == ActionType.REGATE ) {
				// Target es el jugador al que queremos realizar el pase
				GameObject aQuien = GetParameter<GameObject>( 0 );
				if ( aQuien != null ) {
					Entrenador entrenadorTarget = aQuien.GetComponentInChildren<Entrenador>();
					if ( entrenadorTarget ) {
						target = entrenadorTarget.Target;
					}
				}
			}
			// Queremos chutar a portería?
			else if ( Action == ActionType.CHUT || Action == ActionType.REMATE ) {
				// Target es la portería
				target = GameObject.FindGameObjectWithTag ("Porteria");
			}
			
			return target;
		}
	}
	
}
