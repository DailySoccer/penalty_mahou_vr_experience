using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public enum ETecnica {
		Pie,
		Cabeza
	};
	
	public class SoccerData : MonoBehaviour {
		
		public delegate bool ConditionSoccer ( SoccerData soccer );
		
		public enum EBando {
			Ninguno,
			Local,
			Visitante
		};
		public EBando Bando = EBando.Ninguno;
		
		public enum ActionState {
				None,
				Wait,
				Fail
		};
			
		[HideInInspector] public Vector3 BestCellPosition = Vector3.up;
		
		[HideInInspector] public GameObject dummyMirror = null;
		[HideInInspector] public bool BallPropietary = false;
		[HideInInspector] public bool IAOn = false;
		
		bool _ballNear = false;
		[HideInInspector] public bool BallNear {
			get {
				return _ballNear;
			}
			set {
 				if (value) NewOrder = true;
				_ballNear = value;
			}
		}
		
		[HideInInspector] public float MulSpeed = 1f;
		
		[HideInInspector] public bool NewOrder = false;
		
		private bool _controlBalon = false;
		public bool ControlBalon {
			get {
				return _controlBalon;
			}
			set {
 				if (value) NewOrder = true;
				_controlBalon = value;
			}
		}
		
		private bool _pasarBalon = false;
		public bool PasarBalon {
			get {
				return (EvaluateAction != ActionState.Wait) ? _pasarBalon : false;
			}
			set {
				if (value) NewOrder = true;
				_pasarBalon = value;
			}
		}
		[HideInInspector] public bool CentrarBalon = false;
		
		private bool _chutar = false;
		public bool Chutar {
			get {
				return (EvaluateAction != ActionState.Wait) ? _chutar : false;
			}
			set {
				if (value) NewOrder = true;
				_chutar = value;
			}
		}

		private bool _rematar = false;
		public bool Rematar {
			get {
				return _rematar; //(EvaluateAction != ActionState.Wait) ? _rematar : false;
			}
			set {
				if (value) NewOrder = true;
				_rematar = value;
			}
		}

		private bool _regatear = false;
		public bool Regatear {
			get {
				return (EvaluateAction != ActionState.Wait) ? _regatear : false;
			}
			set {
				if (value) NewOrder = true;
				_regatear = value;
			}
		}

		private bool _entrada = false;
		public bool Entrada {
			get { return _entrada; }
			set {
				if (value) NewOrder = true;
				_entrada = value;
			}
		}
		
		public bool Orientar = false;
		
		private bool _esperar = false;
		[HideInInspector] public bool Esperar {
			get {
				return _esperar;
			}
			set {
				if (value) NewOrder = true;
				_esperar = value;
			}
		}
		
		private bool _actionContext = false;
		[HideInInspector] public bool ActionContext {
			get {
				return _actionContext;
			}
			set {
				if (value) NewOrder = true;
				_actionContext = value;
			}
		}

        public bool Chaseball { get; set; }

        [HideInInspector] public bool FollowingPath = false;
		[HideInInspector] public GameObject Target;
		[HideInInspector] public GameObject Aim;
		[HideInInspector] public float Tiempo = 0;
		[HideInInspector] public float TiempoTrayectoria = 1;
		[HideInInspector] public float Altura = 0f;
		[HideInInspector] public ETecnica Tecnica = ETecnica.Pie;
		[HideInInspector] public GameObject Marcaje = null;
		[HideInInspector] public SoccerMotor Motor = null;
		public GameObject Entity {
			get {
				return transform.parent.gameObject;
			}
		}
		
		private float _coordinatedTime = -1f;
		public float CoordinatedTime {
			get { return _coordinatedTime; }
			set {
				if ( value > _coordinatedTime ) NewOrder = true;
				_coordinatedTime = value;
			}
		}
		
		[HideInInspector] public GameObject CoordinatedTarget = null;
		
			
		[HideInInspector] public ActionState EvaluateAction = ActionState.None;
		
		[HideInInspector] public bool ActionSuccess = true;
		[HideInInspector] public float ActionDistance = 0f;
		[HideInInspector] public Vector3 ActionDirection = Vector3.zero;
		
		[HideInInspector] public List<SoccerData> AliadoNear = new List<SoccerData>();
		[HideInInspector] public List<SoccerData> EnemigoNear = new List<SoccerData>();

		// Celebración / Lamentarse
		private MatchManager mMatchManager;

		private bool _ganador = false;
		[HideInInspector] public bool Ganador {
			get {
				return _ganador;
			}
			set {
				if (value) NewOrder = true;
				_ganador = value;
			}
		}
		private bool _perdedor = false;
		[HideInInspector] public bool Perdedor {
			get {
				return _perdedor;
			}
			set {
				if (value) NewOrder = true;
				_perdedor = value;
			}
		}

		void Start() {
			Motor = gameObject.GetComponent<SoccerMotor>();

			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			mMatchManager.OnGol += HandleOnPlayExito;
			mMatchManager.OnMatchFailed += HandleOnPlayFracaso;
		}

		void OnDestroy () {
			mMatchManager.OnGol -= HandleOnPlayExito;
			mMatchManager.OnMatchFailed -= HandleOnPlayFracaso;
		}

		void HandleOnPlayExito (object sender, System.EventArgs e) {
			Aim = null;
            Ganador = (Bando == SoccerData.EBando.Local) && (mMatchManager.CurrentPlayerGoals > mMatchManager.CurrentOpponentGoals);
			Perdedor = (Entity.name == "Soccer-Visitante1");
			NewOrder = true;
		}

		void HandleOnPlayFracaso (object sender, System.EventArgs e) {
			Aim = null;

			if ( !mMatchManager.MatchFailed && !mMatchManager.IsTutorialMatch ) {
				Ganador = (Bando == SoccerData.EBando.Local) && (mMatchManager.CurrentPlayerGoals > mMatchManager.CurrentOpponentGoals);
				//Perdedor = !Ganador;
			}
			else {
				if ( !ActionSuccess ) {
					Perdedor = (Bando == SoccerData.EBando.Local);
				}
			}
		}

		public void AddSoccerNear( SoccerData soccer ) {
			if ( soccer.Bando == Bando ) {
				Assert.Test ( ()=> { return !AliadoNear.Contains( soccer ); }, "Ya existe ese elemento en la tabla: AliadoNear" );
				AliadoNear.Add ( soccer );
			}
			else {
				Assert.Test ( ()=> { return !EnemigoNear.Contains( soccer ); }, "Ya existe ese elemento en la tabla: EnemigoNear" );
				EnemigoNear.Add ( soccer );
			}
		}
		
		public void RemoveSoccerNear( SoccerData soccer ) {
			if ( soccer.Bando == Bando ) {
				Assert.Test ( ()=> { return AliadoNear.Contains( soccer ); }, "No existe ese elemento en la tabla: AliadoNear" );
				AliadoNear.Remove ( soccer );
			}
			else {
				Assert.Test ( ()=> { return EnemigoNear.Contains( soccer ); }, "No existe ese elemento en la tabla: EnemigoNear" );
				EnemigoNear.Remove ( soccer );
			}
		}

		public SoccerData EnemyNearest( Vector3 posicion, ConditionSoccer filter ) {
			return SoccerNearest( posicion, EnemigoNear, filter );
		}		
		
		public SoccerData SoccerNearest( Vector3 posicion, List<SoccerData> list, ConditionSoccer filter ) {
			float distanceNear = 1000f;
			SoccerData near = null;
			foreach( SoccerData el in list ) {
				if ( filter(el) ) {
					float distance = Helper.DistanceInPlaneXZ( el.gameObject, posicion );
					if ( distance < distanceNear ) {
						near = el;
						distanceNear = distance;
					}
				}
				else {
					// Helper.Log ( el.transform.parent.gameObject, "SoccerNearest: Filtered" );
				}
			}
			return near;
		}		
		
		public SoccerData SoccerNearest( List<SoccerData> list, ConditionSoccer filter ) {
			return SoccerNearest ( dummyMirror.transform.position, list, filter );
		}		
		
		public bool isActionFail() {
			return (EvaluateAction == ActionState.Fail);
		}
		
		public void MsgControlBalon() {
			if ( isActionFail() ) return;
			ControlBalon = true;
		}
		
		public void MsgEsperar( float cuantoTiempo ) {
			if ( isActionFail() ) return;
			Esperar = true;
			Tiempo = cuantoTiempo;
		}
		
		public void MsgPasar( GameObject aQuien, float tiempo, float tiempoTrayectoria ) {
			if ( isActionFail() ) return;
			PasarBalon = true;
			CentrarBalon = false;
			Target = aQuien;
			Tiempo = tiempo;
			TiempoTrayectoria = tiempoTrayectoria;
			Altura = 0f;
			
			// Helper.Log ( transform.parent.gameObject, "MsgPasar" );
		}
		
		public void MsgCentrar( GameObject aQuien, float tiempo, float tiempoTrayectoria, float altura ) {
			if ( isActionFail() ) return;
			PasarBalon = true;
			CentrarBalon = true;
			Target = aQuien;
			Tiempo = tiempo;
			TiempoTrayectoria = tiempoTrayectoria;
			Altura = altura;
		}		
		
		public void MsgChutar( float tiempoTrayectoria ) {
			if ( isActionFail() ) return;
			Chutar = true;
			TiempoTrayectoria = tiempoTrayectoria;
		}
		
		public void MsgRematar( float tiempoTrayectoria, ETecnica tecnica ) {
			if ( isActionFail() ) return;
			Rematar = true;
			TiempoTrayectoria = tiempoTrayectoria;
			Tecnica = tecnica;
		}		

		public void MsgRegatear() {
			if ( isActionFail() ) return;
			Regatear = true;
			Target = null;
		}
		
		public void MsgRegatearA( GameObject aQuien ) {
			if ( isActionFail() ) return;
			Regatear = true;
			
			Entrenador entrenadorTarget = aQuien.GetComponent<Entrenador>();
			CoordinatedTarget = entrenadorTarget.Target;
		}

		public void MsgEntrar() {
			if ( isActionFail() ) return;
			Entrada = true;
		}
		
		public void MsgOrientar( bool activar ) {
			if ( isActionFail() ) return;
			Orientar = activar;
			
			// Al desactivar la orientacion, seleccionamos direccion "Front"
			if ( !Orientar ) {
				Animator animator = Entity.GetComponent<Animator>();
				animator.SetInteger ( AnimatorID.MovingDirection, 1 );
			}
		}
		
		struct DataVelocity {
			public DataVelocity( float _mul, float _tiempo ) {
				multiplicador = _mul;
				tiempo = _tiempo;
			}
			public float multiplicador;
			public float tiempo;
		};
		
		IEnumerator ChangeVelocity( DataVelocity dataVelocity ) {
			float timeIni = Time.time;
			
			if ( dataVelocity.tiempo > 0f ) {
				while ( Time.time < (timeIni+dataVelocity.tiempo) ) {
					MulSpeed = Mathf.Lerp ( MulSpeed, dataVelocity.multiplicador, (Time.time - timeIni) / dataVelocity.tiempo );
					yield return null;
				}
			}
			
			MulSpeed = dataVelocity.multiplicador;
		}
		
		public void MsgVelocidad( float multiplicador, float tiempo ) {
			StopCoroutine( "ChangeVelocity" );
			StartCoroutine( "ChangeVelocity", new DataVelocity(multiplicador, tiempo) );
			// MulSpeed = multiplicador;
		}
		
	}
	
}
