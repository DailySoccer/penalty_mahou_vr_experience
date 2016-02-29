using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	/// <summary>
	/// Lanza el balón hacia la portería
	/// Condición: Posesión del balón.
	/// </summary>
	public class Chutar_Goal : GoalComposite {
		
		private MatchManager mMatchManager;
		private SoccerData mData;
		private Animator mAnimator;
		private AnimationContext mAnimationContext;
		private GameObject mBalon;
		private BallMotor mBalonMotor;
		private Vector3 mPorteria = new Vector3(52.5f, 0, 0);
		
		public static new Chutar_Goal New( GameObject _owner ) {
			Chutar_Goal goal = Create<Chutar_Goal>(_owner);
			goal.Set();
			return goal;
		}
		
		protected void Set () {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = gameObject.transform.parent.gameObject.GetComponent<Animator>();
				mAnimationContext = Entity.GetComponent<AnimationContext>();
				mBalon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = mBalon.GetComponent<BallMotor>();
				mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			}
		}
		
		private void UpdateInfoAnimator() {


			Vector3 dirToTarget = Helper.ZeroY ( mPorteria - transform.position );
			dirToTarget.Normalize();
			Quaternion rot = Quaternion.FromToRotation( transform.forward, dirToTarget );
			mAnimator.SetFloat( AnimatorID.DireccionBalon, rot.eulerAngles.y );
			
			float distancia = Helper.DistanceInPlaneXZ( gameObject, mPorteria );
			mAnimator.SetFloat ( AnimatorID.Distancia, distancia );		
			
			float parabola = (mData.TiempoTrayectoria / distancia) * 100;
			mAnimator.SetFloat ( AnimatorID.Parabola, parabola );	
		}
		
		private void SetupAnimationContext() {
			Vector3 dirToTarget = Helper.ZeroY ( mPorteria - transform.position );
			dirToTarget.Normalize();
			Quaternion rot = Quaternion.FromToRotation( transform.forward, dirToTarget );
			mAnimationContext.Direccion = rot.eulerAngles.y;

			float distancia = Helper.DistanceInPlaneXZ( gameObject, mPorteria );
			mAnimationContext.Distancia = distancia;
			mAnimationContext.Tiempo = mData.TiempoTrayectoria;
			mAnimationContext.SetContext( ActionType.CHUT );
		}
		
		private IEnumerator UpdateState() {
			mAnimator.SetBool( AnimatorID.Chutar, true );
			mAnimator.SetFloat( AnimatorID.Altura, mBalon.transform.position.y );
			SetupAnimationContext();
			
			mData.Aim = null;
			
			// Asegurarnos de que se ha activado la animación y se indica que tenemos el balón
			yield return StartCoroutine( Helper.WaitCondition( () => { 
				UpdateInfoAnimator ();
				return ( Helper.IsAnimationPlaying( mAnimator, AnimatorID.Chut ) );
			} ) );				
			
			mBalonMotor.Estado = BallMotor.EEstado.Chut;

            mMatchManager.InteractiveActions.EvaluateAction();
            // Esperar a NO tener el balón
            yield return StartCoroutine( Helper.WaitCondition( () => { 
				UpdateInfoAnimator ();
				return mAnimator.GetFloat(AnimatorID.Balon)>0.001f;
			} ) );

            var res = mMatchManager.InteractiveActions.CurrentAction.Result;

            mBalon.transform.parent = null;			
			mBalonMotor.ShowTrail( true );
            
            mData.TiempoTrayectoria = 1.0f;
            Vector3[] targets = new Vector3[] {
                new Vector3(-0.909f, 0.108f,  2.736f), new Vector3(-0.410f, 0.093f, -0.262f), new Vector3(-0.830f, 0.101f, -3.002f),
                new Vector3(-0.368f, 1.036f,  2.297f), new Vector3(-0.346f, 1.224f,  0f)    , new Vector3(-0.279f, 1.036f, -2.302f),
                new Vector3(-0.146f, 1.567f,  2.856f), new Vector3(-0.235f, 2.272f,  0.140f), new Vector3(-0.163f, 1.587f, -2.831f),
            };

            Vector3 posPortero = GameObject.Find("Visitante1").transform.position;
            Vector3 balonDistanciaPortero = DirectionGesture.mLastTarget - posPortero;
            PorteroDespejar_Goal.balonDistanciaPortero = balonDistanciaPortero;
            int idx = 0;
            if (balonDistanciaPortero.y > 1.1f) idx = 6;
            else if (balonDistanciaPortero.y > 0.5f) idx = 3;

            if (balonDistanciaPortero.z < 2) idx++;
            if (balonDistanciaPortero.z <-2) idx++;

            // Evalua si el resultado de la interaccion es valido, o no...
            if (res) {
                Vector3 target = posPortero + targets[idx];
                mBalonMotor.Clearing( new Vector3(-(0.40f+ Random.value * 0.20f), -targets[idx].y, targets[idx].z/2.5f) );
                mMatchManager.InteractiveActions.MsgBallKick( ActionType.CHUT );

                mBalonMotor.ApplyImpulseToPosition(target, mData.TiempoTrayectoria);
                Entrenador entrenador = mData.dummyMirror.GetComponent<Entrenador>();
				entrenador.StartCoroutine( entrenador.SendMsgGol( mData.TiempoTrayectoria ) );
			}
			else {
                // Parece que si falla manda el balon a las quito pino.
                mBalonMotor.ApplyImpulseToPosition( mPorteria + new Vector3(0,1, Random.Range(-0.5f, 0.5f)) * 3f, mData.TiempoTrayectoria);
				mMatchManager.FinDeJugada( false, true );
				mData.EvaluateAction = SoccerData.ActionState.Fail;
				mData.Perdedor = true;
			}

            mBalonMotor.NewPropietary = null;
			mData.Chutar = false;
			mData.Rematar = false;
			mData.ActionContext = false;
			mData.BallPropietary = false;
			status = Status.Completed;			
		}


        public override void Activate () {
			status = Status.Active;
			RemoveAllSubgoals();
			StartCoroutine ( UpdateState () );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();
			
			mAnimator.SetBool( AnimatorID.Chutar, false );
		}
	}
	
}
