using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class Receptor_Goal : GoalComposite {

        private Vector3 position;
        private Vector3 direction;
        private SoccerData mData;
        private BallMotor mBallMotor;
        private MatchManager mMatchManager;

        SeekSteer steer;
        LookWhereYoureGoingSteer rotationSteer;
        SoccerData data;
        private float mTimeWaitingBall;

        public static new Receptor_Goal New( GameObject _owner) {
            Receptor_Goal goal = Create<Receptor_Goal>(_owner);
            goal.Set();
			return goal;
		}

		protected void Set()
        {
            mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
            mBallMotor = GameObject.FindGameObjectWithTag("Balon").GetComponent<BallMotor>();
            position = mBallMotor.TargetPosition;
            mData = gameObject.GetComponent<SoccerData>();
        }
		
		public override void Activate () {
			status = Status.Active;
            mTimeWaitingBall = 0f;
            RemoveAllSubgoals();
            steer = gameObject.GetComponent<SeekSteer>();
            steer.enabled = true;
            steer.TargetPosition = position;
            rotationSteer = gameObject.GetComponent<LookWhereYoureGoingSteer>();
            rotationSteer.enabled = true;
            mData.Aim = mBallMotor.gameObject;
        }

        public override Status Process () {
			ActivateIfInactive();
            if (isActive()) {
                float distance = Helper.DistanceInPlaneXZ(gameObject, mBallMotor.gameObject);
                if (distance <= 0.5f && mBallMotor.Estado != BallMotor.EEstado.Propietario) {
                    mBallMotor.AttachTo(Entity, "Pie");
                    mData.BallPropietary = true;
                    steer.Target = null;
                    steer.TargetPosition = new Vector3(52.5f, 0, 0);
                    mMatchManager.FinDeJugada(true,false);
                }

                ReactivateIfFailed();
                if(mBallMotor.Estado != BallMotor.EEstado.Propietario && !mBallMotor.MovingToPosition && steer.Target == null) {
                    steer.Target = mBallMotor.gameObject;
                }
            }
            return status;
		}
		
		public override void Terminate () {
            mData.Chaseball = false;
			base.Terminate();
            steer.enabled = false;
            rotationSteer.enabled = false;
        }
    }
	
}

