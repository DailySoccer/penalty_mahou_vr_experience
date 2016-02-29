using UnityEngine;
using System.Collections;
using FootballStar.Common;
using System.Collections.Generic;

/*

namespace FootballStar.Match3D {

	public class StarsFeedback : MonoBehaviour {

		public UISprite[] Stars;
		public FlyingStar FlyingStar;
	
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}

		void Start() {
			mMatchManager.OnMatchStart += OnMatchStart;
			mMatchManager.OnMatchEnd += OnMatchEnd;
						
			mMatchManager.InteractiveActions.OnEvent += HandleInteractiveAction;

			// Creamos el pool de flying stars
			mFlyingStars = new FlyingStar[Stars.Length];

			for (int c = 0; c < mFlyingStars.Length; ++c)
				mFlyingStars[c] = Instantiate(FlyingStar) as FlyingStar;
		}

		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e) {
		}

		void OnMatchStart (object sender, System.EventArgs e) {
			mCurrentScoreStars = 0;
			mTargetScoreStars = 0;
			mCurrentScoreVel = 0;

			foreach (var star in Stars)
				star.fillAmount = 0.0f;
		}

		void HandleInteractiveAction (object sender, EventInteractiveActionArgs e) {
			if (e.State == InteractionStates.END) {
				StartCoroutine(OnNewInteractiveAction(e));
			}
		}

		IEnumerator OnNewInteractiveAction(EventInteractiveActionArgs e) {
			yield return new WaitForSeconds(0.1f);

			int numFlyingStars = (int)Mathf.Ceil(MatchResult.CalcNumPrecisionBalls(e.Result));

			for (int c = 0; c < numFlyingStars; ++c) {
				if (c < numFlyingStars)
					mFlyingStars[c].Fly(e.QuickTimeResult.TouchPosition, Stars[(int)(mCurrentScoreStars)].transform, c);
			}

			yield return new WaitForSeconds(0.5f);

			var currentScore = mMatchManager.CurrentScoreSequence();
			mTargetScoreStars = MatchResult.CalcNumPrecisionBallsSoFar(currentScore, mMatchManager.TotalInteractiveActions);
		}
	
		void Update() {
			if (mTargetScoreStars != mCurrentScoreStars)
				InterpolateScorePanelStars ();
		}

		void InterpolateScorePanelStars () {
			mCurrentScoreStars = Mathf.SmoothDamp (mCurrentScoreStars, mTargetScoreStars, ref mCurrentScoreVel, 0.5f, Mathf.Infinity, Time.deltaTime);

			var numOfCompletedStarts = (int)(mCurrentScoreStars);
			for (int c = 0; c < Stars.Length; ++c) {
				if (c < numOfCompletedStarts) {
					Stars [c].fillAmount = 1.0f;
				}
				else {
					Stars [c].fillAmount = mCurrentScoreStars - numOfCompletedStarts;
					break;
				}
			}
		}

		MatchManager mMatchManager;

		float mTargetScoreStars;
		float mCurrentScoreStars;
		float mCurrentScoreVel;

		FlyingStar[] mFlyingStars;
	}

}

    */