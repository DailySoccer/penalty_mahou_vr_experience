using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {

	public Camera TheaterCamera;
	public GameObject  EnergyParticles;

	public GameObject  FansParticles;
	public GameObject  MoneyAddParticles;
	public GameObject  MoneySpentParticles;
	private Vector3 mMoneyParticlesScreenPosition;
	private Vector3 mFansParticlesScreenPosition;

	public void SetMoneyParticlesPosition( Vector3 worldPosition )
	{
		mMoneyParticlesScreenPosition = TheaterCamera.ScreenToWorldPoint(worldPosition);
		MoneySpentParticles.transform.position = mMoneyParticlesScreenPosition;
		MoneyAddParticles.transform.position = mMoneyParticlesScreenPosition;
	}

	public void SetFansParticlesPosition( Vector3 worldPosition )
	{
		mFansParticlesScreenPosition = TheaterCamera.ScreenToWorldPoint(worldPosition);
		FansParticles.transform.position = mFansParticlesScreenPosition;
	}

	// Use this for initialization
	void Start () {
	
	}

	public void ShowRemoveMoneyParticles()
	{
		MoneySpentParticles.SetActive(false);
		MoneySpentParticles.SetActive(true);
	}
	
	public void ShowAddMoneyParticles()
	{
		MoneyAddParticles.SetActive(false);
		MoneyAddParticles.SetActive(true);			
	}
	
	public void ShowFansParticle()
	{
		FansParticles.SetActive(false);
		FansParticles.SetActive(true);
	}
	
	public void ShowEnergyParticle()
	{
		EnergyParticles.SetActive(false);
		EnergyParticles.SetActive(true);
	}

	// Update is called once per frame
	void Update () {
	
	}


}
