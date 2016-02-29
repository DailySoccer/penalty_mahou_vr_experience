using UnityEngine;
using System.Collections;

public class MoneyRewardsCounter : MonoBehaviour {


	public UILabel RewardsLeftLabel;
	void Awake () {
		if ( PlayerPrefs.HasKey (KEY_COINS_REDEEMED) )
			count = 5 - PlayerPrefs.GetInt ( KEY_COINS_REDEEMED );
		else
			count = 5;
		RewardsLeftLabel.text = RewardsLeftMessage + " [FFCC20]" + (500 * count).ToString();
	}

	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private const string KEY_COINS_REDEEMED = "COINS_REDEEMED";
	private string RewardsLeftMessage ="SALDO DISPONIBLE PARA CANJEAR \nPOR PINCODES:";
	private int count;
}
