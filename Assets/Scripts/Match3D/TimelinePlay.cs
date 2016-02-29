using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	public class TimelinePlay : MonoBehaviour
	{
		public GameObject ChronoPrefab;
		public GameObject TimelineBallPrefab;


		void Awake() {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
		}

		void Start() {
			mMatchManager.OnNewPlay += OnNewPlay;
			mMatchManager.OnMatchStart += OnMatchStart;
			mMatchManager.InteractiveActions.OnEvent += OnInteractiveAction;
			
			gameObject.SetActive(false);
		}

		// El tutorial por ejemplo nos destruye...
		void OnDestroy()
		{
			mMatchManager.OnNewPlay -= OnNewPlay;
			mMatchManager.OnMatchStart -= OnMatchStart;
			mMatchManager.InteractiveActions.OnEvent -= OnInteractiveAction;
		}
		
		void OnInteractiveAction (object sender, EventInteractiveActionArgs e)
        {
			if ( gameObject.activeSelf && e.State == InteractionStates.BEGIN ) {
				StopAllCoroutines();
				StartCoroutine(FadeOutCoroutine());
			}
		}

		void OnMatchStart (object sender, System.EventArgs e)
		{
			foreach (var go in mBalls)
				Destroy(go);
			
			mBalls.Clear();
		}

		void OnNewPlay(object sender, MatchManager.EventNewPlayArgs e)
		{
			gameObject.SetActive(true);
			GetComponent<AnimatedAlpha>().alpha = 1.0f;
			
			if (e.PrevPlayResult != MatchManager.Resultado.Esperando && mBalls.Count > 0)
			{
				var prevBall = mBalls[mBalls.Count - 1];
				
				if (e.PrevPlayResult != MatchManager.Resultado.Correcto)
					prevBall.transform.FindChild("TimelineBall").GetComponent<UISprite>().spriteName = "TimelineError";
				else
					prevBall.transform.FindChild("TimelineBall").GetComponent<UISprite>().spriteName = "TimelineOk";
				
				Destroy(prevBall.transform.FindChild("Chrono").gameObject);
			}

			float barLength = 540.0f;
			var newBall = NGUITools.AddChild(gameObject, TimelineBallPrefab);
			var newChrono = NGUITools.AddChild(newBall, ChronoPrefab);
			
			newChrono.name = "Chrono";

            int minute = 1;
            newChrono.GetComponentInChildren<UILabel>().text = minute.ToString();
			
			newBall.transform.localPosition = new Vector3((minute / 90.0f)*barLength - (barLength*0.5f), 0, 0);
			newChrono.transform.localPosition = new Vector3(0, 53, 0);
			
			mBalls.Add(newBall);
			
			StartCoroutine(OnNewPlayCoroutine());
		}
		
		IEnumerator OnNewPlayCoroutine()
		{			
			yield return new WaitForSeconds(3.0f);
			yield return StartCoroutine(FadeOutCoroutine());	
		}
		
		IEnumerator FadeOutCoroutine()
		{
			var animatedAlpha = GetComponent<AnimatedAlpha>();
			while (animatedAlpha.alpha > 0) {
				var diff = Time.deltaTime * 5.0f;
				
				if (animatedAlpha.alpha - diff > 0) {
					animatedAlpha.alpha -= diff;
					yield return null;
				}
				else {
					animatedAlpha.alpha = 0;
				}
			}
			gameObject.SetActive(false);
		}
		
		void Update ()
		{
		}
		
		List<GameObject> mBalls = new List<GameObject>();		
		MatchManager mMatchManager;
	}
	
}
