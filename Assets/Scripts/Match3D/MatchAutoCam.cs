using System;
using System.Collections;
using System.Collections.Generic;
using FootballStar.Common;
using FootballStar.Audio;
using UnityEngine;

namespace FootballStar.Match3D {
	
	public class MatchAutoCam : MonoBehaviour {
	
		[Serializable]
		public class SideCameraParameters {
			public string CameraName;
			
			public float RightSideCoordZ = -36.0f;
		
			public float MaxSideCoordX = 40.0f;
			public float MinSideCoordX = -40.0f;
			
			public float MaxPositionSpeed = 15.0f;
			
			public float SideMinHeight = 2.0f;
			public float SideMaxHeight = 7.0f;
			public float SideNearDistance = 5.0f;
			public float SideFarDistance = 36.0f;
			public float SideNearFOV = 55.0f;
			public float SideFarFOV = 15.0f;
		};
		
		public SideCameraParameters[] SideCameras;
		public AnimationCurve NoiseCurve;
		public float NoiseAmplitude = 0.02f;
		public Vector3 OrbitPoint = new Vector3(0.0f, 1.1f, 1.0f);
		
		public static Quaternion EulerSmoothDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float time)
		{
			// Soportamos el caso en el que congelamos el tiempo => velocidad NaN
			if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
				velocity = Vector3.zero;
			
			Vector3 result = Vector3.zero;
			result.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, time);
			result.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, time);
			result.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, time);
						
			return Quaternion.Euler(result);
		}
		

		void Start() {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			
			mMatchManager.OnNewPlay += OnNewPlay;
			mMatchManager.OnMatchEnd += OnMatchEnd;
			mMatchManager.OnMatchStart += OnMatchStart;
            // Eliminada la corrutina que controla la camara cuando hay gol.
//			mMatchManager.OnGol += OnGoal;
			mMatchManager.OnMatchFailed += OnMatchFailed;
			
			// Por ejemplo, la primera camara
			mCurrentCamera = SideCameras[0];
			
			// Cargamos el controlador de audio
			mAudioController = GameObject.FindGameObjectWithTag("GameModel").GetComponent<AudioInGameController>();
		}


		// MatchStart: Antes de la cutscene
		void OnMatchStart (object sender, EventArgs e)
		{
			mMode = eMode.WAITING_FOR_LINK;
			mLastPlayerWithBall = null;
		}
		
		// NewPlay: La cutscene ha acabado y/o hay una nueva jugada
		void OnNewPlay (object sender, EventArgs e)
		{
			mMode = eMode.WAITING_FOR_LINK;
			mLastPlayerWithBall = null;
		}

		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e)
		{
//			if (mMode != eMode.MATCH_FAILED) StartCoroutine(LookUpToScoreboardCoroutine());
		}

/*			
		void OnGoal (object sender, EventArgs e)
		{			
			StartCoroutine(OnGoalCoroutine());
		}
*/
		void OnMatchFailed (object sender, EventArgs e)
		{
			StartCoroutine(OnMatchFailedCoroutine());
		}

		void Update()
		{
			if (mMode == eMode.WAITING_FOR_LINK)
			{	
				var daPoint = mMatchManager.PointOfInterest;
				
				if (daPoint != null && daPoint.Source != null)
				{		
					SetInitialPosition(daPoint.Balon.transform, daPoint.Source.transform);

                    if( mMatchManager.InteractiveActions.CurrentAction.Action == ActionType.CHUT ||
                        mMatchManager.InteractiveActions.CurrentAction.Action == ActionType.REMATE )
                        ChangeMode(eMode.GOAL_SHOT);
                    else if (mMatchManager.InteractiveActions.CurrentAction.Action == ActionType.PASE)
                        ChangeMode(eMode.PASS_MODE);
                    else 
                        ChangeMode(eMode.DRIBBLE_MODE);

                }
            }
		}
		
		public IEnumerator IntroCutscene(float fadeInTime, bool isTutorial)
		{
			mMode = eMode.CUTSCENE;

			var gameModel = GameObject.FindGameObjectWithTag("GameModel");

			var performanceTuner = gameModel.GetComponent<PerformanceTuner>();
			performanceTuner.SetCutsceneQuality(true);

			var currentStadium = gameModel.GetComponent<MatchLoader>().CurrentStadium;
			var castellana = GeneralUtils.GetDisabledGameObject(currentStadium, "Castellana");
			castellana.SetActive(true);
			
			List<string> animNames = new List<string>()
										{
											"CastellanaCamAnim", 
											"CastellanaCamAnim 1", 
											"CastellanaCamAnim 2",
											"CastellanaCamAnim 3"
										};

			// Para el tutorial siempre queremos la misma camera, nuestra preferida
			int animIdx = 0;

			if (!isTutorial)
				animIdx = UnityEngine.Random.Range(0, animNames.Count);
		
			GetComponent<Animation>().Play(animNames[animIdx]);
						
			yield return StartCoroutine(GeneralUtils.WaitForAnimation(GetComponent<Animation>()));
			castellana.SetActive(false);
			performanceTuner.SetCutsceneQuality(false);
			yield return StartCoroutine(ScoreboardCutscene());
		}

		public IEnumerator ScoreboardCutscene()
		{
			mMode = eMode.CUTSCENE;
			
			GetComponent<Animation>().Stop();
			GetComponent<Camera>().fieldOfView = 50.0f;
			
			var duration = 1.5f;
			var lookAtTarget = new Vector3(-75, 50.5f, 0);
			transform.position = new Vector3(-57.5f, 48, -1.4f);
			
			AMTween.MoveTo(gameObject, AMTween.Hash("position", new Vector3(-58.5f, 48, -1.4f), "easeType", AMTween.EaseType.easeOutSine, "time", duration));

			float totalTime = 0;
			while (totalTime < duration)
			{
				transform.LookAt(lookAtTarget);
				totalTime += Time.deltaTime;
				yield return null;
			}
			
			mMode = eMode.STOPPED;
		}
		
		void LateUpdate()
		{
            switch (mMode)
            {
                case eMode.FOLLOWING_PLAY:
                    UpdateFollowingPlay();
                    break;
                case eMode.DRIBBLE_MODE:
                    UpdateDribbleMode();
                    break;
                case eMode.PASS_MODE:
                    UpdatePassMode();
                    break;
                case eMode.GOAL_SHOT:
                    UpdateGoalMode();
                    break;
            }
        }

        public void ChangeMode(eMode newMode = eMode.GOAL_SHOT)
        {
            mEulerVel = Vector3.zero;
            mVel = Vector3.zero;
            mFovVel = 0;

            switch (newMode)
            {
                case eMode.FOLLOWING_PLAY:
                    if (mMode != eMode.PASS_MODE)
                        UpdateFollowingPlay(true);
                    break;
                case eMode.DRIBBLE_MODE:
                    UpdateDribbleMode(true);
                    break;
                case eMode.PASS_MODE:
                    UpdatePassMode(true);
                    break;
                case eMode.GOAL_SHOT:
                    UpdateGoalMode(true);
                    break;

            }
            mMode = newMode;
        }


        void SetInitialPosition(Transform balon, Transform targetPlayer)
		{
			var tempPos = new Vector3(Mathf.Clamp(balon.position.x, mCurrentCamera.MinSideCoordX, mCurrentCamera.MaxSideCoordX), 
									  0.0f, 
									  mCurrentCamera.RightSideCoordZ);
			float currentFactor;
			GetComponent<Camera>().fieldOfView = CalculateFromTheSideFOV(tempPos, balon.position, out currentFactor);
			transform.position = new Vector3(tempPos.x, 
											 Mathf.Lerp(mCurrentCamera.SideMinHeight, mCurrentCamera.SideMaxHeight, Mathf.Sin(currentFactor * Mathf.PI * 0.5f)), 
											 mCurrentCamera.RightSideCoordZ);
			
			// Inicialmente miramos al balon
			transform.rotation = Quaternion.LookRotation(balon.position - transform.position);
			
			// Una velocidad cualquiera (modulo 1) para no salir desde parados
			mVel = targetPlayer.forward;
		}


        void UpdateBulletTime(bool forced = false)
        {
        }

        void UpdatePassMode(bool forced = false) {
            var daPoint = mMatchManager.PointOfInterest;
            Vector3 finalLookAtPos;
            if (daPoint.Target!=null)
                finalLookAtPos = Vector3.Lerp( daPoint.Target.transform.position, new Vector3(55, 1, 0), 0.35f ) ;
            else
                finalLookAtPos = new Vector3(55, 1, 0);
            Vector3 dif = finalLookAtPos - daPoint.Balon.transform.position;
            dif.y = 0;
            Vector3 dir = dif.normalized;
            Vector3 end = daPoint.Balon.transform.position - dir * 15.0f;
            end.y = 2.5f;

            if (forced) {
                transform.position = end;
                transform.LookAt(finalLookAtPos);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, end, ref mVel, 0.1f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);
                
                var lookAtEulerRot = Quaternion.LookRotation(finalLookAtPos - transform.position).eulerAngles;
                transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 0.1f);
                GetComponent<Camera>().fieldOfView = 24.0f;
            }
            if (daPoint.Source != null)
                mLastPlayerWithBall = daPoint.Source;
        }

        void UpdateDribbleMode(bool forced = false)
        {
            var daPoint = mMatchManager.PointOfInterest;

            Vector3 p;
            if (daPoint.Target != null)
                p = daPoint.Target.transform.position;
            else
                p = daPoint.Source.transform.position;

            Vector3 finalLookAtPos = Vector3.Lerp(p, new Vector3(55, 1, 0), 0.45f);
            Vector3 dif = finalLookAtPos - daPoint.Balon.transform.position;
            dif.y = 0;
            Vector3 dir = dif.normalized;
            Vector3 end = daPoint.Balon.transform.position - dir * 15.0f;
            end.y = 2.5f;

            if (forced)
            {
                transform.position = end;
                transform.LookAt(finalLookAtPos);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, end, ref mVel, 0.1f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);
                var lookAtEulerRot = Quaternion.LookRotation(finalLookAtPos - transform.position).eulerAngles;
                transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 0.1f);
                GetComponent<Camera>().fieldOfView = 24.0f;
            }
            if (daPoint.Source != null)
                mLastPlayerWithBall = daPoint.Source;
        }

        void UpdateGoalMode(bool forced=false)
        {
            var daPoint = mMatchManager.PointOfInterest;

           
            Vector3 targetPos = new Vector3(55, 1, 0); // Hack de posicion de la porteria
            // targetPos = daPoint.Target.transform.position;
            // 3rd person mode.
            Vector3 dif = targetPos - daPoint.Balon.transform.position;
            dif.y = 0;
            Vector3 dir = dif.normalized;
            Vector3 end;
            if(daPoint.Source!=null)
                end = daPoint.Source.transform.position - dir * 10.0f;
            else
                end = mLastPlayerWithBall.transform.position - dir * 10.0f;
            end.y = 2.5f;

            if (forced) {
                transform.position = end;
                transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, end, ref mVel, 0.1f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);

                transform.LookAt(targetPos);
//                var lookAtEulerRot = Quaternion.LookRotation(targetPos - transform.position).eulerAngles;
//                transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 1.0f);
                GetComponent<Camera>().fieldOfView = 24.0f;
            }
            if (daPoint.Source != null)
                mLastPlayerWithBall = daPoint.Source;
        }


        void UpdateFollowingPlay(bool forced = false)
        {
            var daPoint = mMatchManager.PointOfInterest;
            Vector3 finalLookAtPos = daPoint.Balon.transform.position;
            /*
            if (daPoint.Target != null)
            {
                // Miramos entre el balon y el target...
                var middlePoint = daPoint.Balon.transform.position;

                // ... o, si es la porteria, nos quedamos mirando al Source del chut + la porteria
                if (daPoint.Target.CompareTag("Porteria"))
                    middlePoint = mLastPlayerWithBall.transform.position;

                finalLookAtPos = middlePoint + ((daPoint.Target.transform.position - middlePoint) * 0.5f);

                // Clipeamos la distancia
                var diff = finalLookAtPos - daPoint.Balon.transform.position;

                if (diff.magnitude > 9.0f)
                    finalLookAtPos = middlePoint + (finalLookAtPos - middlePoint).normalized * 9.0f;
            }
            */
            Vector3 fromTheSidePos = new Vector3(Mathf.Clamp(finalLookAtPos.x, mCurrentCamera.MinSideCoordX, mCurrentCamera.MaxSideCoordX),
                                                 0.0f,
                                                 mCurrentCamera.RightSideCoordZ);
            float currentFactor;
            float finalFOV = CalculateFromTheSideFOV(fromTheSidePos, finalLookAtPos, out currentFactor) *1.25f;
            fromTheSidePos.y = Mathf.Lerp(mCurrentCamera.SideMinHeight, mCurrentCamera.SideMaxHeight, Mathf.Sin(currentFactor * Mathf.PI * 0.5f));

            Vector3 rpos;
            Quaternion rrot;
            float rfov;

            if (!forced)
            {
                // Posicion
                rpos = Vector3.SmoothDamp(transform.position, fromTheSidePos, ref mVel, 1.0f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);
                // Rotacion (Look at)
                var lookAtEulerRot = Quaternion.LookRotation(finalLookAtPos - rpos).eulerAngles;
                rrot = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 1.0f);
                // FOV
                rfov = Mathf.SmoothDamp(GetComponent<Camera>().fieldOfView, finalFOV, ref mFovVel, 0.3f, 100.0f, Time.deltaTime);
            }
            else
            {
                // Posicion
                rpos = fromTheSidePos;
                // Rotacion (Look at)
                rrot = Quaternion.LookRotation(finalLookAtPos - rpos);
                // FOV
                rfov = finalFOV;
            }
            transform.position = rpos;
            transform.rotation = rrot;
            GetComponent<Camera>().fieldOfView = rfov;

            if (daPoint.Source != null)
                mLastPlayerWithBall = daPoint.Source;
        }

        IEnumerator OnGoalCoroutine() {
            mMode = eMode.CELEBRATING_GOAL;
			
			float currentTime = 0.0f;
			
			mAudioController.PlayDefinition(SoundDefinitions.CROWD_GOINGUP, false);
			
			// 1 segundo de no dejar la camara quieta cambiando del punto medio al q solemos enfocar al player que chuto
			while (currentTime < 2.0f && mMode == eMode.CELEBRATING_GOAL) 
			{
                transform.position = Vector3.SmoothDamp(transform.position, mLastPlayerWithBall.transform.position, ref mVel, 3.5f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);
				var lookAtEulerRot = Quaternion.LookRotation(mLastPlayerWithBall.transform.position - transform.position).eulerAngles;
				transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 2.5f);
				currentTime += Time.deltaTime;
				yield return null;
			}
						
			if (mMode == eMode.CELEBRATING_GOAL)
			{
				// Cambio de camara a detras de la porteria, animacion de FOV!
				var goalCams = new Vector3[] { new Vector3(55, 1.5f, -18),
											   new Vector3(55, 1.5f, 18)
											 };
				transform.position = goalCams[UnityEngine.Random.Range(0, goalCams.Length)];
				GetComponent<Animation>().Play("CamGoalAnim01");
				
				// Al cuello o incluso spine mejor
				// "Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head"
				Transform head = mLastPlayerWithBall.transform.Find("Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck");
				transform.LookAt(head.transform.position);
				
				currentTime = 0.0f;

				while (mMode == eMode.CELEBRATING_GOAL)
				{
					var lookAtEulerRot = Quaternion.LookRotation(head.transform.position - transform.position).eulerAngles;
					transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 0.3f);
									
					transform.position = new Vector3(transform.position.x + NoiseCurve.Evaluate(currentTime) * NoiseAmplitude,
													 transform.position.y,
													 transform.position.z);
					currentTime += Time.deltaTime;
					
					yield return null;
				}
			}
		}

		IEnumerator OnMatchFailedCoroutine()
		{
            Debug.Log(">>> OnMatchFailedCoroutine");
			mMode = eMode.MATCH_FAILED;
			
			float currentTime = 0.0f;
			
			mAudioController.PlayDefinition(SoundDefinitions.CROWD_BOO, false);
			
			// 1 segundo de no dejar la camara quieta cambiando del punto medio al q solemos enfocar al player que fallo
			while (currentTime < 2.0f && mMode == eMode.MATCH_FAILED) 
			{
				transform.position = Vector3.SmoothDamp(transform.position, mLastPlayerWithBall.transform.position, ref mVel, 3.5f, mCurrentCamera.MaxPositionSpeed, Time.deltaTime);
				var lookAtEulerRot = Quaternion.LookRotation(mLastPlayerWithBall.transform.position - transform.position).eulerAngles;
				transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 2.5f);
				currentTime += Time.deltaTime;
				yield return null;
			}
			
			if (mMode == eMode.MATCH_FAILED)
			{
				// Cambio de camara a la banda, animacion de FOV!
				//transform.position = new Vector3(55, 1.5f, 18);
				transform.position = new Vector3(mLastPlayerWithBall.transform.position.x, 1.5f, -36);
				GetComponent<Animation>().Play("CamMatchFailedAnim01");

				// Al cuello o incluso spine mejor
				// "Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head"
				Transform head = mLastPlayerWithBall.transform.Find("Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck");
				transform.LookAt(head.transform.position);
				
				currentTime = 0.0f;
				
				while (mMode == eMode.MATCH_FAILED && currentTime < 3.0f)
				{
					var lookAtEulerRot = Quaternion.LookRotation(head.transform.position - transform.position).eulerAngles;
					transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref mEulerVel, 0.3f);
					
					transform.position = new Vector3(transform.position.x + NoiseCurve.Evaluate(currentTime) * NoiseAmplitude,
					                                 transform.position.y,
					                                 transform.position.z);
					currentTime += Time.deltaTime;
					
					yield return null;
				}

				StartCoroutine(LookUpToTheSkyCoroutine());
			}
		}
	
		float CalculateFromTheSideFOV(Vector3 currentPos, Vector3 targetPos, out float currentFactor)
		{
			var distToBalon = (targetPos - currentPos).magnitude;
			currentFactor =  Mathf.Clamp01((distToBalon - mCurrentCamera.SideNearDistance) / (mCurrentCamera.SideFarDistance - mCurrentCamera.SideNearDistance));
			
			return Mathf.Lerp(mCurrentCamera.SideNearFOV, mCurrentCamera.SideFarFOV, Mathf.Sin(currentFactor * Mathf.PI * 0.5f));
		}
				
		IEnumerator LookUpToScoreboardCoroutine()
		{
			mMode = eMode.LOOK_UP_TO_THE_SCOREBOARD;
			
			GetComponent<Animation>().Stop();
			
			float fovVel = 0.0f;
			Vector3 eulerVel = Vector3.zero;
			
			// Da scoreboard
			var lookAtTarget = new Vector3(-75, 50.5f, 0);
			
			while (mMode == eMode.LOOK_UP_TO_THE_SCOREBOARD)
			{
				var lookAtEulerRot = Quaternion.LookRotation(lookAtTarget - transform.position).eulerAngles;
				transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref eulerVel, 2.0f);
				
				GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(GetComponent<Camera>().fieldOfView, 25.0f, ref fovVel, 5.0f, Mathf.Infinity, Time.deltaTime);
				
				yield return null;
			}
		}

		IEnumerator LookUpToTheSkyCoroutine()
		{
			mMode = eMode.LOOK_UP_TO_THE_SKY;
			
			GetComponent<Animation>().Stop();
			
			float fovVel = 0.0f;
			Vector3 eulerVel = Vector3.zero;
			
			// Da sky
			var lookAtTarget = new Vector3(mLastPlayerWithBall.transform.position.x, 100.0f, mLastPlayerWithBall.transform.position.z);
			
			while (mMode == eMode.LOOK_UP_TO_THE_SKY)
			{
				var lookAtEulerRot = Quaternion.LookRotation(lookAtTarget - transform.position).eulerAngles;
				transform.rotation = EulerSmoothDamp(transform.rotation.eulerAngles, lookAtEulerRot, ref eulerVel, 2.0f);
				
				GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(GetComponent<Camera>().fieldOfView, 25.0f, ref fovVel, 5.0f, Mathf.Infinity, Time.deltaTime);
				
				yield return null;
			}
		}
				
		IEnumerator OrbitPlayerCoroutine()
		{
            Debug.Log(">>> OrbitPlayerCoroutine");
            mMode = eMode.ORBIT_PLAYER;
		
			// Tentativo: Por convenio tu futbolista es el N. TODO: InteractiveActions.MainCharacter
			Transform targetPlayer = mLastPlayerWithBall.transform;
			
			if (targetPlayer == null)
				targetPlayer = GameObject.Find ("Soccer-Local11").transform;
			
			Transform targetHead = targetPlayer.Find("Player_Local/Root/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
			var angle = 0.0f;
			
			GetComponent<Animation>().Stop();
			GetComponent<Camera>().fieldOfView = 45.0f;
			
			while (mMode == eMode.ORBIT_PLAYER)
			{
				var lookAtPoint = targetHead.position; 

				angle += 20.0f * Time.deltaTime;
				transform.position = lookAtPoint + (Quaternion.Euler(0.0f, angle, 0.0f) * OrbitPoint);
				
				lookAtPoint.y = 1.7f;
				transform.rotation = Quaternion.LookRotation(lookAtPoint - transform.position);
								
				yield return null;				
			}
		}

		private Vector3 mVel;
		private float mFovVel;
				
		// Velocidad del LookAt
		private Vector3 mEulerVel;
		
		// Ultimo player que ha tocado el balon en el chut
		private GameObject mLastPlayerWithBall;
		
		public enum eMode {
			WAITING_FOR_LINK,
			FOLLOWING_PLAY,
			ORBIT_PLAYER,
			LOOK_UP_TO_THE_SCOREBOARD,
			LOOK_UP_TO_THE_SKY,
			CELEBRATING_GOAL,
			MATCH_FAILED,
			CUTSCENE,
			STOPPED,
            GOAL_SHOT,
            PASS_MODE,
            DRIBBLE_MODE
        }
        private eMode mMode = eMode.WAITING_FOR_LINK;
        public eMode Mode { get { return mMode; } }
		
		private SideCameraParameters mCurrentCamera;
		private MatchManager mMatchManager;
		private AudioInGameController mAudioController;
	}
	
}
