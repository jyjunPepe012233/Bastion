using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {
	
	[SerializeField] private int lazerSummonSize;
	[SerializeField] private LazerPool lazerPool;
	[Space(10)]
	[SerializeField] private int impactSummonSize;
	[SerializeField] private ImpactPool impactPool;
	[Space(10)]
	[SerializeField] private List<EnemyPool> enemyPools;
	[Space(10)]
	[SerializeField] private List<ProjectilePool> projectilePools;
	[Space(10)]
	[SerializeField] private List<SoundPool> soundPools;
	
	public int LazerSummonSize { get => lazerSummonSize; }
	public int ImpactSummonSize { get => impactSummonSize; }



	

	// 아니 로직이 이게 좀 고쳐야하는데 그

	public GameObject InstantiateLazer(GameObject lazer) {

		if (lazerPool == null) {
			lazerPool = new GameObject("Lazer Pool", typeof(LazerPool)) .GetComponent<LazerPool>();
		}

		return lazerPool.CallObject(lazer);
	}
	
	
	
	public GameObject InstantiateImpact(GameObject impact) {

		if (impactPool == null) {
			impactPool = new GameObject("Impact Pool", typeof(ImpactPool)) .GetComponent<ImpactPool>();
		}

		return impactPool.CallObject(impact);
	}



	public GameObject InstantiateEnemy(GameObject enemy) {
		string poolName = enemy.name + " Pool";

		foreach (EnemyPool enemyPool in enemyPools) {
			
			if (enemyPool.name == poolName) // 이미 존재하는 종류의 Pool일 경우
				return enemyPool.CallObject(enemy);
		}
		
		
		var newPool = new GameObject(poolName, typeof(EnemyPool)).GetComponent<EnemyPool>();
		
		enemyPools.Add(newPool);
		return newPool.CallObject(enemy);
	}


	public int GetEnableEnemyCount() {

		int count = 0;
		foreach (EnemyPool enemyPool in enemyPools) {
			count += enemyPool.GetActiveCount();
		}

		return count;
	}

	public void ClearEnemyPool() {
		enemyPools.Clear();
	}
	
	
	
	public GameObject InstantiateProjectile(GameObject projectile) {
		string poolName = projectile.name + " Pool";

		foreach (ProjectilePool projectilePool in projectilePools) {
			
			if (projectilePool.name == poolName) // 이미 존재하는 종류의 Pool일 경우
				return projectilePool.CallObject(projectile);
		}
		
		
		var newPool = new GameObject(poolName, typeof(ProjectilePool)).GetComponent<ProjectilePool>();
		
		projectilePools.Add(newPool);
		return newPool.CallObject(projectile);
	}
	
	
	
	public GameObject InstantiateSound(GameObject sound) {
		string poolName = sound.name + " Pool";

		foreach (SoundPool soundPool in soundPools) {
			
			if (soundPool.name == poolName) // 이미 존재하는 종류의 Pool일 경우
				return soundPool.CallObject(sound);
		}
		
		
		var newPool = new GameObject(poolName, typeof(SoundPool)).GetComponent<SoundPool>();
		newPool.transform.SetParent(SoundManager.Instance.transform);
		
		soundPools.Add(newPool);
		return newPool.CallObject(sound);
	}
}
