using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour {
	
	[SerializeField] private EnemySpawnPoint[] spawnPoints;

	

	public void EnemySpawn(GameObject enemy, int spawnCount) {
		
		EnemySpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

		
		for (int i = 0; i < spawnCount; i++) {
			Vector3? spawnPos = spawnPoint.GetSpawnPosition();

			if (spawnPos == null) {
				Debug.LogError("SpawnPoint can't found a Ground. Enemy wasn't spawn");
				
			} else {
				GameObject newEnemy = PoolManager.Instance.InstantiateEnemy(enemy);
				newEnemy.GetComponent<Enemy>().Spawn();
				newEnemy.transform.position = (Vector3)spawnPos;
			}
		}
		
	}
	
	
	
	
}
