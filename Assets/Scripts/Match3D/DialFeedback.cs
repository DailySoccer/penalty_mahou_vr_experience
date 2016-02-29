using UnityEngine;
using System.Collections;
using FootballStar.Common;
using System.Collections.Generic;

namespace FootballStar.Match3D {

	public class DialFeedback : MonoBehaviour {

		public UISprite Dial;
					
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}

		void Start() {
			mMatchManager.OnMatchStart += OnMatchStart;
			mMatchManager.OnMatchEnd += OnMatchEnd;
						
			mMatchManager.InteractiveActions.OnEvent += HandleInteractiveAction;

			mDialInitX = Dial.transform.localPosition.x;
		}

		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e) {
		}

		void OnMatchStart (object sender, System.EventArgs e) {
			mCurrentLife = 0;
			mTargetLife = 0;
			mCurrentLife = 0;

			SetDialPos(0.0f);
		}

		void HandleInteractiveAction (object sender, EventInteractiveActionArgs e) {
			if (e.State == InteractionStates.END) {
				StartCoroutine(OnNewInteractiveAction(e));
			}
		}

		IEnumerator OnNewInteractiveAction(EventInteractiveActionArgs e) {
			yield return new WaitForSeconds(0.1f);

			mTargetLife = mMatchManager.Life;
		}
	
		void Update() {
			if (mTargetLife != mCurrentLife)
				InterpolateScorePanel ();
		}

		void InterpolateScorePanel () {
			mCurrentLife = Mathf.SmoothDamp (mCurrentLife, mTargetLife, ref mCurrentLifeVel, 0.5f, Mathf.Infinity, Time.deltaTime);

			// Está entre MaxLife y MinLife. Lo pasamos a entre -1 y 1, y por el rango maximo en pixeles
			SetDialPos(180 * mCurrentLife / mMatchManager.MaxLife);
		}

		void SetDialPos(float dialPosX) {
			var currentPos = Dial.transform.localPosition;
			currentPos.x = mDialInitX + dialPosX;
			Dial.transform.localPosition = currentPos;    
		}

		MatchManager mMatchManager;

		float mTargetLife;
		float mCurrentLife;
		float mCurrentLifeVel;

		float mDialInitX;
	}

}
