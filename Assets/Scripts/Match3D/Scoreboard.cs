using System.Collections;
using System.Linq;
using UnityEngine;
using FootballStar.Common;
using System;

namespace FootballStar.Match3D {
	
	public class Scoreboard : MonoBehaviour {
		
		public UILabel LeftScoreLabel;
		public UILabel RightScoreLabel;

        public UISprite LeftBadge;
        public UISprite RightBadge;


        void Awake () 
		{
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			
			mMatchManager.OnNewPlay += OnNewPlay;
			mMatchManager.OnMatchEnd += OnMatchEnd;
			mMatchManager.OnCutsceneBegin += OnCutsceneBegin;
			mMatchManager.OnCutsceneEnd += OnCutsceneEnd;
		}
		
		void Start()
		{
			mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();
			RightBadge.spriteName = mMatchBridge.CurrentMatchDefinition.OpponentBadgeName + "_Small";
            LeftBadge.spriteName = mMatchBridge.CurrentMatchDefinition.MyBadgeName + "_Small";
        }		

		void OnCutsceneEnd (object sender, EventArgs e)
		{
			ShowTheThing();
		}

		void OnCutsceneBegin (object sender, EventArgs e)
		{
			HideTheThing();
		}
		
		void OnMatchEnd (object sender, MatchManager.EventMatchEndArgs e)
		{
			StartCoroutine(HideTheThingCoroutine());
		}
		
		void HideTheThing()
		{
			EnableAnchors(false);
			transform.localPosition = new Vector3(transform.localPosition.x, 530.0f, transform.localPosition.z);
		}
		
		IEnumerator HideTheThingCoroutine()
		{
			EnableAnchors(false);
						
			float coordYVel = 0.0f;
			float targetY = 530.0f;
			
			while (Mathf.Abs(transform.localPosition.y - targetY) > 0.5f)
			{
				var newY = Mathf.SmoothDamp(transform.localPosition.y, targetY, ref coordYVel, 0.300f, Mathf.Infinity, Time.deltaTime);
				transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
				yield return null;
			}
		}
		
		void ShowTheThing()
		{
			EnableAnchors(true);
			transform.position = new Vector3(-621, 360, 0);
		}

		void EnableAnchors(bool enable) {
			UIAnchor[] anchors = GetComponentsInChildren<UIAnchor>();

			foreach (var anchor in anchors)
				anchor.enabled = enable;
		}
		
		void OnNewPlay(object sender, MatchManager.EventNewPlayArgs e)
		{
			RightScoreLabel.text = mMatchManager.CurrentOpponentGoals.ToString();
		}
		
		void Update () 
		{
			if (mCurrentGoals != mMatchManager.CurrentPlayerGoals) {
				mCurrentGoals = mMatchManager.CurrentPlayerGoals;
				LeftScoreLabel.text = mCurrentGoals.ToString();
			}
		}
	
		
		MatchManager mMatchManager;
		MatchBridge mMatchBridge;
		
		int mCurrentGoals;
	}
}
