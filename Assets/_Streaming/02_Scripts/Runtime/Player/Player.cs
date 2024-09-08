using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField] private int maxHp;
	[SerializeField] private int currentHp;
	[Space(5)]
	[SerializeField] private int healPack;
	[Space(20)]
	[SerializeField] private VolumeController volumeCtrl;
	
	private bool isDied;
	private bool isUsingHealPack;
		

	void Awake() {

		currentHp = maxHp;
		UIManager.Instance.HealPackText = "x" + healPack;
	}

	void Update() {

		UIManager.Instance.HpBarFillAmount = (float)currentHp / maxHp;

		if (Input.GetKeyDown(KeyCode.R) && healPack > 0) {
			StartCoroutine(UseHealPack());
		}

	}



	IEnumerator UseHealPack() {
		isUsingHealPack = true;
		healPack -= 1;
		UIManager.Instance.HealPackText = "x" + healPack;

		currentHp += 225;
		if (currentHp > maxHp) currentHp = maxHp;

		yield return new WaitForSeconds(1f);

		isUsingHealPack = false;
	}
	
	
	

	public void GetDamage(int damage) {
		
		currentHp -= damage;

		if (currentHp <= 0) { // 플레이어 죽음
			currentHp = 0; // Clamp
			
			StartCoroutine(Die());
			return;
		}
		
		volumeCtrl.HitEffect();
	}
	


	IEnumerator Die() {
		if (isDied) yield break;

		isDied = true;

		GetComponent<PlayerMove>().enabled = false;
		GetComponent<PlayerWeapon>().enabled = false;
		GetComponent<PlayerAnimation>().Died();

		yield return new WaitForSeconds(2);
		
		GameManager.Instance.GameOver();
	}
}
