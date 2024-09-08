using System;
using UnityEngine;

[Serializable]
public struct EnemyDataForWave {
	
	public GameObject prefab;
	
	[Space(5)]
	public int spawnCount;
	[Range(0, 1)] public float spawnTiming;
}

[Serializable]
public struct WaveData {
	public float waveTime;
	[Space(10)]
	public EnemyDataForWave[] enemyData;
}

[CreateAssetMenu(menuName = "Bastion/Wave Game Data", fileName = "Wave Data", order = int.MaxValue)]
public class WaveGameDataSO : ScriptableObject {

	public string sceneName;
	public string mapName;
	public Sprite image;

	[Space(10)]
	[TextArea] public string enemyInfo;
	[TextArea] public string storyInfo;
	
	public float maxGameTime;
	[Space(10)]
	public WaveData[] waveDatas;

	private void OnValidate() {

		maxGameTime = 0;
		enemyInfo = "";
		
		foreach (WaveData data in waveDatas) {
			
			maxGameTime += data.waveTime;
				// 총 게임시간 게산

			foreach (EnemyDataForWave _enemyData in data.enemyData) {

				if (!enemyInfo.Contains(_enemyData.prefab.name))
					enemyInfo += _enemyData.prefab.name + "\r\n";
					// 적 정보 계산
			}
			
		}	
		
	}
}
