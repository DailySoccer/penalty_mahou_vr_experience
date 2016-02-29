using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class PorteroDespejar_Goal : GoalComposite {

        private MatchManager mMatchManager;
        private SoccerData mData;
		private Animator mAnimator;
		private BallMotor mBalonMotor;

        public static Vector3 balonDistanciaPortero;

        public static new PorteroDespejar_Goal New( GameObject _owner ) {
			PorteroDespejar_Goal goal = Create<PorteroDespejar_Goal>(_owner);
			goal.Set();
			return goal;
		}
			
		protected void Set() {
			if ( InitializeGlobal ) {
				mData = gameObject.GetComponent<SoccerData>();
				mAnimator = transform.parent.GetComponent<Animator>();
				GameObject balon = GameObject.FindGameObjectWithTag ("Balon");
				mBalonMotor = balon.GetComponent<BallMotor>();
                mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
            }
		}

		private IEnumerator UpdateState() {
            float[] times = new float[] {
                0.88f, 0.36f, 0.92f,
                0.82f, 0.58f, 0.82f,
                0.66f, 0.76f, 0.66f,
            };

            int idx = 0;
            if (balonDistanciaPortero.y > 1.1f) idx = 6;
            else if (balonDistanciaPortero.y > 0.5f) idx = 3;
            if (balonDistanciaPortero.z > -2) idx++;
            if (balonDistanciaPortero.z > 2) idx++;

            bool falla = Random.value>0.5f; // 50-50 CACA
            float time = 0;
            if (falla) {
                mBalonMotor.Clearing(Vector3.one);
                time = Random.Range(0.25f, 0.75f);
            }

            while (mBalonMotor.TimeToTarget > times[idx] + time ) {
                yield return null;
			}

			mAnimator.SetBool( AnimatorID.Despejar, true );
            mAnimator.SetFloat( AnimatorID.Altura, balonDistanciaPortero.y);
            mAnimator.SetFloat( AnimatorID.DireccionBalon, balonDistanciaPortero.z );

            // Esperar a que comience la animación
            AnimatorStateInfo state = mAnimator.GetCurrentAnimatorStateInfo(0);

            // Despejar Bajo IZQ altura < 0.3 Direccion > 0     Despeje_Q2_L0_IZQ       P: -0.909f, 0.108f,  2.736f T: 0.88s
            // Despejar Bajo MED altura < 0.3                   Despeje_Q0_L0_DER       P: -0.410f, 0.093f, -0.262f T: 0.36s
            // Despejar Bajo DER altura < 0.3 Direccion < 0     Despeje_Q2_L0_DER       P: -0.830f, 0.101f, -3.002f T: 0.92s

            // Despejar Medio IZQ altura < 1.1 Direccion > 2    Despeje_Q2_L1_IZQ       P: -0.368f, 1.036f,  2.297f T: 0.82s
            // Despejar Medio MED altura < 1.1                  Despeje_Q0_L1           P: -0.346f, 1.224f,  0.000f T: 0.58s
            // Despejar Medio DER altura < 1.1 Direccion <-2    Despeje_Q2_L1_DER       P: -0.279f, 1.036f, -2.302f T: 0.82s

            // Despejar Alto IZQ altura > 1.1 Direccion > 0     Despeje_Q2_L2_IZQ       P: -0.146f, 1.567f,  2.856f T: 0.66s
            // Despejar Alto MED altura > 1.1                   Despeje_corner_Q0_L2    P: -0.235f, 2.272f,  0.140f T: 0.76s
            // Despejar Alto DER altura > 1.1 Direccion < 0     Despeje_Q2_L2_DER       P: -0.163f, 1.587f, -2.831f T: 0.66s
        
            float timerMax = 1;
            while ( !state.IsTag ( "Despejar" ) && (timerMax > 0) ) {
				yield return null;
				state = mAnimator.GetCurrentAnimatorStateInfo(0);				
				timerMax -= Time.deltaTime / Time.timeScale;
			}
			
			mAnimator.SetBool( AnimatorID.Despejar, false );
			// Esperar a terminar la animación
			while ( state.IsTag ( "Despejar" ) && mBalonMotor.MovingToPosition ) {
				yield return null;
				state = mAnimator.GetCurrentAnimatorStateInfo(0);
            }

			// No cambiamos de Goal. Permanecemos en Idle (sin Alerta)
			mData.Aim = null;
			mAnimator.SetBool( AnimatorID.Alerta, false );
            mMatchManager.FinDeJugada(falla, true);
        }

        public override void Activate () {
			status = Status.Active;
			RemoveAllSubgoals();
			mData.Aim = null;
			StartCoroutine( UpdateState() );
		}
		
		public override Status Process () {
			ActivateIfInactive();
			
			if ( isActive () ) {
				/*Status statusSubgoals =*/ ProcessSubgoals();				
				// status = Status.Completed;			
				ReactivateIfFailed();
			}
			return status;
		}
		
		public override void Terminate () {
			base.Terminate();			
			mAnimator.speed = 1;
		}
	}
	
}

