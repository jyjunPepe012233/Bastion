using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageCollider : MonoBehaviour {

	[Serializable]
	private enum Type { PlayerHit, EnemyHit }

	[SerializeField] private Type type;
	[Space(5)]
	[SerializeField] private int damage;
	
	private bool isPlayerDamaged;
	private bool isSupplyDamaged;
	private List<Collider> damagedEnemys = new List<Collider>();
	
	public int Damage { set => damage = value; }
	

	private void OnTriggerEnter(Collider other) {
		
		switch (type) {
			
			case Type.PlayerHit:
				switch (other.tag) {

					case "Player": // 플레이어 공격
						if (isPlayerDamaged) return;

						other.GetComponent<Player>().GetDamage(damage);

						isPlayerDamaged = true;
						break;

					case "SupplyBox": // 보급 공격
						if (isSupplyDamaged) return;

						GameManager.Instance.GetSupplyDamage(damage);

						isSupplyDamaged = true;
						break;
				}
				break;

			
			case Type.EnemyHit:
				if (other.CompareTag("Enemy") && !damagedEnemys.Contains(other)) {
					other.GetComponent<Enemy>().GetHit(damage);
					damagedEnemys.Add(other);
				}

				break;
		}
		
	}


	
	private void OnTriggerExit(Collider other) {
		isPlayerDamaged = false;
		isSupplyDamaged = false;
		damagedEnemys.Clear();
	}
	
	private void OnDisable() {
		isPlayerDamaged = false;
		isSupplyDamaged = false;
		damagedEnemys.Clear();
	}
}
