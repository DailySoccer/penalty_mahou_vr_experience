using UnityEngine;
using System.Collections;
using FootballStar.Manager.Model;

namespace FootballStar.Manager
{
	
	public class DebugMenu : MonoBehaviour {
	
		void Start () {
			mMainModel = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MainModel>();
			
		}
		
		void Update () {
		
		}
		
		public void OnSaveClick()
		{
			mMainModel.SaveDefaultGame();
		}
		
		public void OnLoadClick()
		{
			mMainModel.LoadDefaultGame();
		}
		
		public void OnResetClick()
		{
			mMainModel.ResetDefaultGame();
		}
		
		public void OnCloseClick()
		{
			Destroy(this.gameObject);
		}
		
		public void OnButton01Click()
		{
			mMainModel.PlayPerformanceMatch();
		}
		
		public void OnAddEnergyClick()
		{
			mMainModel.AddEnergyUnit(1);
		}
		
		public void OnRemoveEnergyClick()
		{
			mMainModel.AddEnergyUnit(-1);
		}

		public void OnAddMoneyClick()
		{
			mMainModel.AddMoney(1000);
		}
		
		public void OnAddFansClick()
		{
			mMainModel.AddFans(1000);
		}
		
		MainModel mMainModel;
	}
}
