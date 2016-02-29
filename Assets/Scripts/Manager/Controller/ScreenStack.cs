using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;
using FootballStar.Audio;

namespace FootballStar.Manager
{
	public class ScreenStack : MonoBehaviour
	{
		public string OnStackChangedMessage = "OnScreenStackChanged";
		public GameObject MessageTarget;
		
		public string ManagerScreenTag = "ManagerScreen";
		public GameObject MainScreen;
		public GameObject IntroScreen;

		public GameObject SelectTeamScreen;

		public GameObject GenetsisScreen;

		protected virtual void Awake()
		{
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			mAudioController = mMainModel.GetComponent<AudioInGameController>();
			
			mAllScreens.AddRange(GameObject.FindGameObjectsWithTag(ManagerScreenTag));
			
			// La primera que jugamos mostramos la pantalla Intro, las siguientes, directamente la MainScreen
			var firstScreen = mMainModel.Player.ShowIntro? IntroScreen : MainScreen;
			
			// Seguimos la politica de que todo venga activo desde el editor y lo tengamos que desactivar por codigo
			foreach (var screen in mAllScreens)
			{
				if (screen != firstScreen)
					screen.SetActive(false);
				else
					mScreensStack.Push(screen);
			}
		}
		
		void Start()
		{			
			if (!mMainModel.Player.ShowIntro)
				CameraFade.Fade(true, 2.5f, 0.0f);	
		}
		
		public void GoToMainScreen()
		{
            if (!mMainModel.Player.IsTeamSelected) {
				GoToSelectTeamScreen();
				return;
			}            

			foreach (var screen in mScreensStack)
			{
				if (screen != MainScreen)
					screen.SetActive(false);
			}
			mScreensStack.Clear();
			mScreensStack.Push(MainScreen);
			MainScreen.SetActive(true);
						
			CameraFade.Fade(true, 2.0f, 0.0f);
			mAudioController.StopAllActiveAudios(false);
		}

		public void GoToSelectTeamScreen()
		{
			foreach (var screen in mScreensStack)
			{
				if (screen != MainScreen)
					screen.SetActive(false);
			}
			mScreensStack.Clear();
			mScreensStack.Push(SelectTeamScreen);
			SelectTeamScreen.SetActive(true);
			
			CameraFade.Fade(true, 2.0f, 0.0f);
			mAudioController.StopAllActiveAudios(false);
		}
	
		public void PopScreenController()
		{
			StartCoroutine(PopScreenControllerCoRoutine());
		}
		
		private IEnumerator PopScreenControllerCoRoutine()
		{
			yield return new WaitForEndOfFrame();

			if (mScreensStack.Count > 1)
			{
				var current = mScreensStack.Pop();
				
				current.SetActive(false);
				mScreensStack.Peek().SetActive(true);
			}
			
			GameObject eventReceiver = MessageTarget == null? gameObject : MessageTarget;			
			eventReceiver.SendMessage(OnStackChangedMessage, StackDepth, SendMessageOptions.DontRequireReceiver);				
		}
		
		public void PushScreenController(string name)
		{
			StartCoroutine(PushScreenControllerCoRoutine(name));
		}
		
		public int StackDepth
		{
			get { return mScreensStack.Count; }
		}
		
		private IEnumerator PushScreenControllerCoRoutine(string name)
		{
			yield return new WaitForEndOfFrame();
			
			foreach (var screen in mAllScreens)
			{
				if (screen.name == name)
				{
					screen.SetActive(true);
					mScreensStack.Peek().SetActive(false);
					mScreensStack.Push(screen);
				}
			}
			
			GameObject eventReceiver = MessageTarget == null? gameObject : MessageTarget;
			eventReceiver.SendMessage(OnStackChangedMessage, StackDepth, SendMessageOptions.DontRequireReceiver);				
		}

        protected MainModel mMainModel;
		protected AudioInGameController mAudioController;
		
		List<GameObject> mAllScreens = new List<GameObject>();
		Stack<GameObject> mScreensStack = new Stack<GameObject>();
	}
}
	