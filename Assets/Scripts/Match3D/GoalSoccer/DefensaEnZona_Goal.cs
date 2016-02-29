using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class DefensaEnZona_Goal : GoalPipe {
		
		public float Radio = 10f;
		private PropietaryPosition mPointAttack;
		private ZonaAtaque mZonaAtaque;
		private GameObject mPorteria;
		
		private GameObject mTarget;
		private GameObject Target {
			get {
				return mTarget;
			}
			set {
				if ( mTarget != value ) {
					mTarget = value;
					if ( mTarget != null ) {
						mZonaAtaque = mTarget.GetComponentInChildren<ZonaAtaque>();
					}
				}
			}
		}
		
		private float mDistanceToTarget;
		private SoccerData mData;
		private SoccerMotor mMotor;
		private BallMotor mBalonMotor;
		// private InfluenceMap influenceMap;
		
		// Steers
		ArriveSteer mArriveSteer;

		enum SubState {
			ProtectZone,
			OrientToBall,
			TargetBlock,
			
			MarcarJugador,
			ApproachToTarget,
			AccionesDistanciaLejana,
			AccionesDistanciaMedia,
			AccionesDistanciaCercana,
		};
		SubState substate;
		
		public new static DefensaEnZona_Goal New( GameObject _owner ) {
			DefensaEnZona_Goal goal = Create<DefensaEnZona_Goal>(_owner);
			goal.Set();
			return goal;
		}
				
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mMotor = gameObject.GetComponent<SoccerMotor>();
				
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = balon.GetComponent<BallMotor>();
				
				mArriveSteer = gameObject.GetComponent<ArriveSteer>();

				mPorteria = GameObject.FindGameObjectWithTag ("Porteria");
				
				/*
				influenceMap = FindObjectOfType ( typeof(InfluenceMap) ) as InfluenceMap;
				Assert.Test ( ()=> { return influenceMap; }, "No existe un InfluenceMap" );
				*/
			}
		}
	
		private IEnumerator FSM() {
			while (true) {
				yield return StartCoroutine( substate.ToString() );
				yield return null;
			}
		}
		
		private IEnumerator ProtectZone() {
			mZonaAtaque.ToString();	// <---- Remove WARNING!!
			
			/*
			while ( !data.BallNear ) {
				
				// influenceMap.UpdateInfluence();
				Vector3 bestPosition = influenceMap.BestPosition( data );
				if ( bestPosition.y < Vector3.up.y ) {
					Vector3 oldBestPosition = bestPosition;
					data.BestCellPosition = bestPosition;
					
					moveSteer.enabled = true;
					moveSteer.Target = null;
					moveSteer.targetPosition = bestPosition;
					
					rotationSteer.enabled = true;
					
					while ( !data.BallNear ) {
						// influenceMap.UpdateInfluence();
						
						bestPosition = influenceMap.BestPosition( data );
						if ( bestPosition.y < Vector3.up.y && (bestPosition - oldBestPosition).magnitude > 1f) {
							data.BestCellPosition = bestPosition;
							oldBestPosition = bestPosition;
							
							moveSteer.targetPosition = Entity.transform.position + Entity.transform.forward * 1f;
							yield return new WaitForSeconds(1f);
							
							Vector3 dir = Helper.ZeroY(bestPosition - Entity.transform.position);
							bestPosition = bestPosition + dir.normalized * 2;
							moveSteer.targetPosition = bestPosition;
						}
						yield return new WaitForSeconds(0.5f);
					}
					
					moveSteer.enabled = false;
					rotationSteer.enabled = false;		
				}
				
				yield return new WaitForSeconds(1f);
			}
			
			substate = SubState.OrientToBall;
			*/
			yield return null;
		}
		
		private bool InArea( GameObject go, float _radio ) {
			return Helper.InRadio( go, mData.dummyMirror, _radio );
		}
		
		private SoccerData ContrarioInArea() {
			float nearestDist = Radio;
			SoccerData contrario = null;
			
			foreach( SoccerData soccer in mData.EnemigoNear ) {
				bool near = Helper.InRadio( soccer.gameObject, mData.dummyMirror, nearestDist );
				if ( near ) {
					contrario = soccer;
					break;
				}
			}
			
			return contrario;
		}
		
		private Vector3 PositionNearToTarget() {
			Vector3 dummyPosition = mData.dummyMirror.transform.position;
			Vector3 dir = (Target.transform.position - dummyPosition);
			float len = dir.magnitude;
			
			return ( dummyPosition + dir.normalized * len * 0.5f );
		}
		
		private IEnumerator TargetBlock () {
			
			// data.Aim = Target;
			mData.Aim = mBalonMotor.gameObject;
			
			mArriveSteer.enabled = true;
			mArriveSteer.Target = null;
			mArriveSteer.targetPosition = PositionNearToTarget();
			
			// Mientras permanezca el objetivo cerca del area
			while ( InArea(Target, Radio+1) ) {
				if ( mMotor.CurrentSpeed < 1f ) {
					
					Vector3 positionNear = PositionNearToTarget();
					
					bool near = Helper.InRadio ( gameObject, positionNear, 1 );
					if ( near ) {
						mData.Aim = Target;
					}
					else {
						mData.Aim = mBalonMotor.gameObject;
						mArriveSteer.targetPosition = positionNear;
					}
				}
				yield return new WaitForSeconds(1);
			}
			
			mArriveSteer.enabled = false;

			Target = null;
			substate = SubState.OrientToBall;
		}
		
		private IEnumerator OrientToBall() {
			mData.Aim = mBalonMotor.gameObject;
			
			while ( true ) {
				SoccerData contrario = ContrarioInArea();
				if ( contrario ) {
					Target	= contrario.gameObject;
					substate = SubState.TargetBlock;
					break;
				}
				
				yield return null;
			}
		}
		
		bool IsZonaValid ( Vector3 direction ) {
			bool valid = false;
			
			Vector3 position = transform.position;
			
			// El movimiento está fuera de la zona de defensa?
			float nextDistance = Helper.DistanceInPlaneXZ( mData.dummyMirror, position + direction );
			valid = ( nextDistance < Radio );
			
			if ( !valid ) {
				// Nos acerca más al centro?
				float currentDistance = Helper.DistanceInPlaneXZ( mData.dummyMirror, position );
				valid = ( nextDistance < currentDistance );
				
				/*
				if ( !valid ) {
					Helper.Log ( Entity, "Distance: " + currentDistance );
				}
				*/
			}
			
			return valid;
		}
		
		bool IsZonaValid () {
			return IsZonaValid ( mCurrentDirection );
		}
		
		bool IsProtectionValid( Vector3 direction, GameObject enemy ) {
			bool valid = true;
			
			if ( enemy != null ) {
				Vector3 nextPosition = transform.position + direction;
				
				float distanceToBall = Helper.DistanceInPlaneXZ( mBalonMotor.gameObject, transform.position );
				float distanceToEnemy = Helper.DistanceInPlaneXZ( enemy, transform.position );
				float distanceNextToBall = Helper.DistanceInPlaneXZ( mBalonMotor.gameObject, nextPosition );
				float distanceNextToEnemy = Helper.DistanceInPlaneXZ( enemy, nextPosition );
				float distanceEnemyToBall = Helper.DistanceInPlaneXZ( mBalonMotor.gameObject, enemy );
				
				if ( distanceToEnemy > 5f ) {
					valid = (distanceNextToEnemy < distanceToEnemy);
				}
				else {
					if ( distanceToBall < distanceEnemyToBall ) {
						valid = (distanceNextToBall < distanceEnemyToBall);
					}
					else {
						valid = (distanceNextToBall < distanceToBall);
					}
				}
			}
			
			return valid;
		}
		
		bool IsNear ( GameObject go, Vector3 direction ) {
			bool valid = true;
			
			float distanceToDummy = Helper.DistanceInPlaneXZ( go, gameObject );
			
			// Si estamos algo lejos del dummy...
			if ( true /*|| distanceToDummy > (Radio * 0.5f)*/ ) {
				
				// Aceptaremos movimientos que nos acerquen
				float distanceNextToDummy = Helper.DistanceInPlaneXZ( go, transform.position + direction );
				valid = ( distanceNextToDummy <= distanceToDummy );
			}
			
			return valid;
		}
		
		private SoccerData CalculateEnemyNearest() {
			return mData.SoccerNearest( mData.EnemigoNear, soccer => true );
		}
		
		private IEnumerator AccionesDistanciaLejana() {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			
			GameObject target = mBalonMotor.gameObject;
			GameObject reference = mData.dummyMirror;
			mData.Aim = target;
			
			float tiempoEspera = 1f + 1f * Random.value;
			
			SoccerData enemyData = CalculateEnemyNearest();
			if ( enemyData != null ) {
				reference = enemyData.gameObject;
				
				SoccerMotor motor = enemyData.Motor;
				if ( motor && motor.Velocity.magnitude > 1f ) {
					tiempoEspera = 0f;
				}
				// Helper.Log ( Entity, "Enemy: " + enemy.name + " Distance: " + Helper.DistanceInPlaneXZ(gameObject, enemy) );
			}
			
			/*
			if ( tiempoEspera >= 0.5f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			*/
			
			int r = Random.Range (0, 4);
			
			bool valid = false;
			
			switch( r ) {
			case 0: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.Approach, transform.position );
				valid = IsZonaValid (nextDirection) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.Approach, target, 40f, 80f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 1: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 30f, 80f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 2: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveLeft, target, 30, 100f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 3: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 30f, 100f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			}
			
			if ( valid && tiempoEspera > 0.3f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			
			substate = SelectStateByDistance();
		}
		
		private IEnumerator AccionesDistanciaMedia() {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			
			GameObject target = mBalonMotor.gameObject;
			GameObject reference = mData.dummyMirror;
			mData.Aim = target;
			
			float tiempoEspera = 1f + 1f * Random.value;
			
			SoccerData enemyData = CalculateEnemyNearest();
			if ( enemyData != null ) {
				reference = enemyData.gameObject;
				
				SoccerMotor motor = enemyData.Motor;
				if ( motor && motor.Velocity.magnitude > 1f ) {
					tiempoEspera = 0f;
				}
				// Helper.Log ( Entity, "Enemy: " + enemy.name + " Distance: " + Helper.DistanceInPlaneXZ(gameObject, enemy) );
			}
			
			/*
			if ( tiempoEspera > 0.3f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			*/
			
			int r = Random.Range (0, 4);
			
			bool valid = false;
			
			switch( r ) {
			case 0: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.Approach, transform.position );
				valid = IsZonaValid (nextDirection) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.Approach, target, 15f, 30f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 1: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 10f, 20f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 2: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveLeft, target, 10, 30f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 3: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 10f, 30f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			}
			
			if ( valid && tiempoEspera > 0.3f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			
			substate = SelectStateByDistance();
		}
		
		private IEnumerator AccionesDistanciaCercana() {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			
			GameObject target = mBalonMotor.gameObject;
			GameObject reference = mData.dummyMirror;
			mData.Aim = target;
			
			float tiempoEspera = 1f + 1f * Random.value;
			
			SoccerData enemyData = CalculateEnemyNearest();
			if ( enemyData != null ) {
				reference = enemyData.gameObject;
				
				SoccerMotor motor = enemyData.Motor;
				if ( motor && motor.Velocity.magnitude > 1f ) {
					tiempoEspera = 0f;
				}
				// Helper.Log ( Entity, "Enemy: " + enemy.name + " Distance: " + Helper.DistanceInPlaneXZ(gameObject, enemy) );
			}
			
			/*
			if ( tiempoEspera > 0.3f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			*/
			
			int r = Random.Range (0, 4);
			
			bool valid = false;
			
			switch( r ) {
			case 0: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.Approach, transform.position );
				valid = IsZonaValid (nextDirection) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.Approach, target, 5f, 10f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 1: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 1f, 10f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 2: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveLeft, target, 3f, 10f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			case 3: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position );
				valid = IsZonaValid ( nextDirection ) && IsNear(reference, nextDirection);
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 3f, 10f );
					yield return StartCoroutine( RunningOrders(1f + 1f * Random.value, IsZonaValid) );
				}
			}
			break;
				
			}
			
			if ( valid && tiempoEspera > 0.3f ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(tiempoEspera, null ) );
			}
			
			substate = SelectStateByDistance();
		}
		
		SubState SelectStateByDistance() {
			SubState subState = SubState.AccionesDistanciaLejana;
			
			/*
			float distance = Helper.DistanceInPlaneXZ( mBalonMotor.gameObject, gameObject );
			
			if ( distance < 10f ) {
				subState = SubState.AccionesDistanciaCercana;
			}
			else if ( distance < 30f ) {
				subState = SubState.AccionesDistanciaMedia;
			}
			*/
			
			return subState;
		}
		
		private IEnumerator ApproachToTarget () {
			
			mData.MulSpeed = 0.2f + 0.8f * Random.value;
			
			GameObject target = mBalonMotor.NewPropietary; //mBalonMotor.gameObject; // mBalonMotor.NewPropietary;
			SoccerData enemyData = CalculateEnemyNearest();
			GameObject enemy = enemyData ? enemyData.gameObject : null;
			
			int r = Random.Range (0, 4);
			
			bool valid = false;
			
			switch( r ) {
			case 0: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.Approach, transform.position );
				valid = IsZonaValid (nextDirection) && IsProtectionValid (nextDirection, enemy);
				if ( valid ) {
					PushOrder( EOrder.Approach, target, 5f, 100f );
					yield return StartCoroutine( RunningOrders(2f, IsZonaValid) );
				}
			}
			break;
				
			case 1: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position );
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position ) ) && IsProtectionValid (nextDirection, enemy);
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 0f, 100f );
					yield return StartCoroutine( RunningOrders(2.2f, IsZonaValid) );
				}
			}
			break;
				
			case 2: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position );
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position ) ) && IsProtectionValid (nextDirection, enemy);
				if ( valid ) {
					PushOrder( EOrder.MoveLeft, target, 3, 100f );
					yield return StartCoroutine( RunningOrders(2.4f, IsZonaValid) );
				}
			}
			break;
				
			case 3: {
				Vector3 nextDirection = NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position );
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position ) ) && IsProtectionValid (nextDirection, enemy);
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 3f, 100f );
					yield return StartCoroutine( RunningOrders(2.6f, IsZonaValid) );
				}
			}
			break;
				
			}
			
			/*
			if ( !valid ) {
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.Approach, transform.position ) );
				if ( valid ) {
					PushOrder( EOrder.Approach, target, 5f, 30f );
					yield return StartCoroutine( RunningOrders(2f, IsZonaValid) );
				}
			}
			
			if ( !valid ) {
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveAway, transform.position ) );
				if ( valid ) {
					PushOrder( EOrder.MoveAway, target, 0f, 20f );
					yield return StartCoroutine( RunningOrders(2.2f, IsZonaValid) );
				}
			}
			
			if ( !valid ) {
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveLeft, transform.position ) );
				if ( valid ) {
					PushOrder( EOrder.MoveLeft, target, 3, 40f );
					yield return StartCoroutine( RunningOrders(2.4f, IsZonaValid) );
				}
			}
			
			if ( !valid ) {
				valid = IsZonaValid ( NextDirection(target, GoalPipe.EOrder.MoveRight, transform.position ) );
				if ( valid ) {
					PushOrder( EOrder.MoveRight, target, 3f, 40f );
					yield return StartCoroutine( RunningOrders(2.6f, IsZonaValid) );
				}
			}
			*/
			
			if ( valid ) {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(1f + 2f * Random.value, null ) );
			}
			
		}
		
		private IEnumerator MarcarJugador () {
			mData.MulSpeed = 0.2f + 0.4f * Random.value;
			
			SoccerData enemyData = CalculateEnemyNearest();
			Target = enemyData ? enemyData.gameObject : null;
			if ( Target == null ) {
				status = Status.Completed;
				yield return null;
			}
			
			GameObject target = Target;
			mData.Aim = target;
			
			if ( mZonaAtaque != null ) {
				mPointAttack = mZonaAtaque.GetAttackPosition( gameObject );
				if ( mPointAttack != null ) {
					float distanceToPorteria = Helper.DistanceInPlaneXZ( mPorteria, gameObject );
					float distanceToTarget = (target.transform.position - transform.position).magnitude;
					
					Vector3 dirPoint = (mPointAttack.position - mZonaAtaque.transform.position);
					Quaternion quatPoint = Quaternion.LookRotation(dirPoint);
					float distancePoint = dirPoint.magnitude;
					
					Vector3 dir = (transform.position - mZonaAtaque.transform.position);
					Quaternion quatDir = Quaternion.LookRotation(dir);
					
					float distance = dir.magnitude;
					if ( distance > 10f || distanceToPorteria < 40f )
						mData.MulSpeed = 0.2f + 0.6f * Random.value;
					else
						mData.MulSpeed = 0.2f + 0.4f * Random.value; 
					
					float angleToPoint = quatPoint.eulerAngles.y - quatDir.eulerAngles.y;
					if ( Mathf.Abs(angleToPoint) > 30f && (distanceToTarget < distancePoint) ) { 
						
						if ( angleToPoint < 0 )
							angleToPoint += 360;
						if ( angleToPoint > 180f && angleToPoint < 350f ) {
							// Helper.Log( Entity, "Right: " + angleToPoint );
							PushOrder( EOrder.MoveRight, target, 2f, 20f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else if ( angleToPoint > 10f && angleToPoint < 180f ) {
							// Helper.Log( Entity, "Left: " + angleToPoint );
							PushOrder( EOrder.MoveLeft, target, 2f, 20f );
							yield return StartCoroutine( RunningOrders(1f, null) );
						}
						else {
							/*
							PushOrder( EOrder.Approach, target, 6f, 20f );
							yield return StartCoroutine( RunningOrders(2f, null) );
							*/
							yield return null;
						}
						// Helper.Log( Entity, "Angle: " + angleToPoint );
					}
					else {
						PushOrder( EOrder.Approach, target, dirPoint.magnitude, 20f );
						yield return StartCoroutine( RunningOrders(1f, null) );
					}
					
				}
				else {
					status = Status.Completed;
					yield return null;
				}
			}
			else {
				PushOrder( EOrder.None );
				yield return StartCoroutine( RunningOrders(1f, null ) );
			}
		}
	
		public override void Activate () {
			base.Activate ();
			
			status = Status.Active;
			
			RemoveAllSubgoals();
			
			mMotor.ForcedZeroY = true;
			
			// REVIEW: Limitar la velocidad?
			mData.MulSpeed = 1f - 0.7f * Random.value;
			
			// Debug.Log ( GameObjectName + ": Defensa En Zona: " + Time.time );
			
			/*
			balon.transform.parent = null;
			
			BallMotor ballMotor = balon.GetComponentInChildren<BallMotor>();
			ballMotor.ApplyImpulseToPosition ( owner.transform.position + (owner.transform.forward * 10f), 1 );
			
			data.BallPropietary = false;
			*/
			
			substate = SubState.OrientToBall;
			// substate = SelectStateByDistance(); //SubState.ApproachToTarget;
			// substate = SubState.MarcarJugador;
			StartCoroutine ( FSM() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			mMotor.ForcedZeroY = true;
			
			// Debug.Log ( GameObjectName + ": updated" );
			
			ReactivateIfFailed();
			
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			Target = null;
			// Debug.Log ( GameObjectName + ": end" );

			mArriveSteer.enabled = false;

			StopAllCoroutines();
		}	
		
		void OnDrawGizmos () {
			/*
			if ( influenceMap != null && substate == SubState.ProtectZone ) {
				Vector3 bestPosition = influenceMap.BestPosition( data );
				if ( bestPosition.y < Vector3.up.y ) {
					
					Gizmos.color = Color.blue;
					Gizmos.DrawLine( Entity.transform.position + Vector3.up, bestPosition + Vector3.up );
				}
			}
			*/
			
			/*
			if ( Target != null ) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine( Entity.transform.position + Vector3.up, Target.transform.position + Vector3.up );
			}
			
			if ( mData.Aim != null ) {
				Gizmos.color = Color.green;
				
				Vector3 ori = Entity.transform.position + Vector3.up;
				Vector3 dir = (mData.Aim.transform.position - ori).normalized;
				Gizmos.DrawLine( ori, ori + dir );
			}
			*/
		}		
	}
	
}
