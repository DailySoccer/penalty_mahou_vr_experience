using System;
using System.Collections;
using UnityEngine;

namespace FootballStar.Match3D {
	
	public class FeedbackMessage : MonoBehaviour {
		
		public float InMsgCoordY = 25;
		public float OutMsgCoordY = -70;
		
		public enum MessageKind {
			TOO_EARLY,
			EARLY,
			PERFECT,
			LATE,
			TOO_LATE,
			INCOMPLETE_DRIBBLING,
			IMPRECISE_KICK

		}

		//TODO: Utilizar los mensajes GOAL y OUTOFBOUNDS para el tiro a puerta en primera persona
		/* public enum MessageKind {
			FAIL,
			CORRECT,
			GOAL,
			OUTOFBOUNDS,
			INCOMPLETE_DRIBBLING,
			IMPRECISE_KICK			
		}*/
	
		void Awake () {
			mMatchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
			
			mAnimatedAlpha = GetComponent<AnimatedAlpha>();
			mMsgLabel = GetComponentInChildren<UILabel>();
		}
		
		void Start () {
			mMatchManager.InteractiveActions.OnEvent += OnInteractiveAction;
		}

        void OnInteractiveAction(object sender, EventInteractiveActionArgs e)
        {
            if (e.State == InteractionStates.END)
            {
                if (e.ResultKind == InteractionResponseKind.TOO_LATE && e.QuickTimeResult.Error)
                {
                    if (e.ActionType == ActionType.REGATE)
                        ShowMessage(MessageKind.INCOMPLETE_DRIBBLING);
                    else
                        ShowMessage(MessageKind.IMPRECISE_KICK);
                }
                else
                {
                    if (e.ResultKind == InteractionResponseKind.TOO_LATE)
                        ShowMessage(MessageKind.TOO_LATE);
                    else if (e.ResultKind == InteractionResponseKind.LATE)
                        ShowMessage(MessageKind.LATE);
                    else if (e.ResultKind == InteractionResponseKind.PERFECT)
                        ShowMessage(MessageKind.PERFECT);
                    else if (e.ResultKind == InteractionResponseKind.EARLY)
                        ShowMessage(MessageKind.EARLY);
                    else
                        ShowMessage(MessageKind.TOO_EARLY);
                }
            }
        }
        /*
		void OnInteractiveAction (object sender, EventInteractiveActionArgs e)
		{
			if ( e.State == InteractionStates.END ) {
				if ( e.ResultKind == InteractionResponseKind.FAIL && e.QuickTimeResult.Error ) {
					if ( e.ActionType == ActionType.REGATE )
						ShowMessage(MessageKind.INCOMPLETE_DRIBBLING);
					else
						ShowMessage(MessageKind.IMPRECISE_KICK);
				}
				else {
					if (e.ResultKind == InteractionResponseKind.CORRECT) {
						ShowMessage(MessageKind.CORRECT);
					}
					else {
						ShowMessage(MessageKind.FAIL);
					}
				}
			}
		}
		*/
        void ShowMessage(MessageKind kind)
        {

            StopAllCoroutines();

            if (kind == MessageKind.TOO_LATE)
            {
                mMsgLabel.text = "TARDE";
                mMsgLabel.color = Color.red;
                StartCoroutine(MessageIn());
            }
            else if (kind == MessageKind.TOO_EARLY)
            {
                mMsgLabel.text = "PRONTO";
                mMsgLabel.color = Color.red;
                StartCoroutine(MessageIn());
            }
            else if (kind == MessageKind.EARLY)
            {
                mMsgLabel.text = "BIEN";
                mMsgLabel.color = Color.cyan;
                StartCoroutine(MessageInScale());
            }
            else if (kind == MessageKind.LATE)
            {
                mMsgLabel.text = "BIEN";
                mMsgLabel.color = Color.cyan;
                StartCoroutine(MessageInScale());
            }
            else if (kind == MessageKind.PERFECT)
            {
                mMsgLabel.text = "PERFECTO";
                mMsgLabel.color = Color.green;
                StartCoroutine(MessageInScale());
            }
            else if (kind == MessageKind.INCOMPLETE_DRIBBLING)
            {
                mMsgLabel.text = "TARDE";
                mMsgLabel.color = Color.red;
                StartCoroutine(MessageInScale());
            }
            else if (kind == MessageKind.IMPRECISE_KICK)
            {
                mMsgLabel.text = "INCORRECTO";
                mMsgLabel.color = Color.red;
                StartCoroutine(MessageInScale());
            }
        }
        /*
        void ShowMessage(MessageKind kind) {
			
			StopAllCoroutines();			

			if (kind == MessageKind.FAIL) {
				mMsgLabel.text = "FALLO";
				mMsgLabel.color = Color.red;				
				StartCoroutine (MessageIn ());
			} else if (kind == MessageKind.CORRECT) {
				mMsgLabel.text = "CORRECTO";
				mMsgLabel.color = Color.green;				
				StartCoroutine (MessageInScale ());
			} else if (kind == MessageKind.GOAL) {
				mMsgLabel.text = "GOL";
				mMsgLabel.color = Color.green;				
				StartCoroutine (MessageInScale ());
			}
			else if (kind == MessageKind.OUTOFBOUNDS) {
				mMsgLabel.text = "FUERA";
				mMsgLabel.color = Color.red;				
				StartCoroutine(MessageIn());
			}
			else if (kind == MessageKind.INCOMPLETE_DRIBBLING) {
				mMsgLabel.text = "TARDE";
				mMsgLabel.color = Color.red;				
				StartCoroutine(MessageInScale());
			}
			else if (kind == MessageKind.IMPRECISE_KICK) {
				mMsgLabel.text = "INCORRECTO";
				mMsgLabel.color = Color.red;				
				StartCoroutine(MessageInScale());
			}

    }*/
		
		IEnumerator MessageIn() {
			while(Mathf.Abs(mMsgLabel.transform.localPosition.y - InMsgCoordY) > 0.1f) {
				var newY = Mathf.SmoothDamp(mMsgLabel.transform.localPosition.y, InMsgCoordY, ref mMsgVel, 0.150f, Mathf.Infinity, Time.deltaTime);
				mMsgLabel.transform.localPosition = new Vector3(mMsgLabel.transform.localPosition.x, newY, mMsgLabel.transform.localPosition.z);
				yield return null;
			}
			
			yield return new WaitForSeconds(0.5f);
			
			while(Mathf.Abs(mMsgLabel.transform.localPosition.y - OutMsgCoordY) > 0.1f) {
				var newY = Mathf.SmoothDamp(mMsgLabel.transform.localPosition.y, OutMsgCoordY, ref mMsgVel, 0.150f, Mathf.Infinity, Time.deltaTime);
				mMsgLabel.transform.localPosition = new Vector3(mMsgLabel.transform.localPosition.x, newY, mMsgLabel.transform.localPosition.z);
				yield return null;
			}
		}
		
		IEnumerator MessageInScale() {
			mMsgLabel.transform.localPosition = new Vector3(mMsgLabel.transform.localPosition.x, InMsgCoordY, mMsgLabel.transform.localPosition.z);
			var scaleInit = mMsgLabel.transform.localScale.x;
			mMsgLabel.transform.localScale = new Vector3(0.1f*scaleInit, 0.1f*scaleInit, 1.0f);
						
			while (Mathf.Abs(mMsgLabel.transform.localScale.x - scaleInit) > 0.1f) {
				var newScale = Mathf.SmoothDamp(mMsgLabel.transform.localScale.x, scaleInit, ref mMsgVel, 0.070f, Mathf.Infinity, Time.deltaTime);
				mMsgLabel.transform.localScale = new Vector3(newScale, newScale, 1);				
				yield return null;
			}
			mMsgLabel.transform.localScale = new Vector3(scaleInit, scaleInit, 1);
			
			yield return new WaitForSeconds(0.5f);
			
			while (Mathf.Abs(mAnimatedAlpha.alpha - 0.0f) > 0.1f) {
				mAnimatedAlpha.alpha = Mathf.SmoothDamp(mAnimatedAlpha.alpha, 0.0f, ref mMsgVelAlpha, 0.100f, Mathf.Infinity, Time.deltaTime);
				yield return null;
			}
			mMsgLabel.transform.localScale = new Vector3(scaleInit, scaleInit, 1.0f);
			mMsgLabel.transform.localPosition = new Vector3(mMsgLabel.transform.localPosition.x, OutMsgCoordY, mMsgLabel.transform.localPosition.z);
			mAnimatedAlpha.alpha = 1.0f;
		}
		
		MatchManager mMatchManager;
		AnimatedAlpha mAnimatedAlpha;
		
		float mMsgVel;
		float mMsgVelAlpha;
		UILabel mMsgLabel;
	}
	
}
