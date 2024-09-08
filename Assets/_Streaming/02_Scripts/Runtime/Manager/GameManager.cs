using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager> {

    [SerializeField] private WaveGameDataSO[] gameDatas;
    
    [SerializeField] private WaveGameDataSO currentGameData; // 현재 플레이중인 게임 웨이브 정보
    [Space(10)]
    [SerializeField] private int currentWave;
    [SerializeField] private float currentWaveProgress;
    
    private SupplyBox supplyBox;
    private Transform player;
    private EnemyDataForWave[] currentEnemyDatas; // 현재 적의 데이터
    private WaveData currentWaveData;
    private int nextEnemyIndex;
    private int enemyCount;
    private int supplyHp = 2000;
    [SerializeField] private int credit;
    private bool isDoneSpawn;
    private bool isWhileEnemyWarning;
    private bool isGaming; // 게임중인지 확인함
    private bool isBuildMode;

    private EnemySpawner spawner;
    
    public WaveGameDataSO[] GameDatas { get => gameDatas;  }
    public SupplyBox SupplyBox{ get => supplyBox;}
    public Vector3 PlayerPosition { get => player.position; }
    public bool IsGaming { get => isGaming; }
    public bool IsBuildMode { get => isBuildMode; set => isBuildMode = value; }


    void Start() {
        
        UIManager.Instance.SupplyBarEnabled = false;
    }


    void CursorLock(bool setting) {
        
        Cursor.visible = !setting;
        Cursor.lockState = setting ? CursorLockMode.Locked : CursorLockMode.None;

    }



    public void PlayGame(int gameDataIndex) {
        
        CursorLock(true);
        credit = 30; // 기본 크레딧
        

        currentGameData = gameDatas[gameDataIndex];
        SceneManager.LoadScene(currentGameData.sceneName);
        
        UIManager.Instance.SetCanvas(CanvasType.PlayerHUD);
        UIManager.Instance.CreditText = credit.ToString();
    }



    public void StartGame() {
        if (currentGameData == null) return;
        
        spawner = FindObjectOfType<EnemySpawner>();
        supplyBox = FindObjectOfType<SupplyBox>();
        
        player = FindObjectOfType<Player>().transform;

        isGaming = true;
        UIManager.Instance.WaveBarEnabled = true;
        UIManager.Instance.SupplyBarEnabled = true;

        StartWave(0);
    }



    void StartWave(int wave) {
        
        currentWave = wave;
        
        currentWaveProgress = 0;
        nextEnemyIndex = 0;
        isDoneSpawn = false;
            // reset
            
        currentWaveData = currentGameData.waveDatas[wave];
        currentEnemyDatas = currentGameData.waveDatas[currentWave].enemyData;
            // temp
            
        
		
        // 적 배열을 스폰 타이밍에 따라 정렬(선택정렬)
        for (int i = 0; i < currentEnemyDatas.Length; i++) {
			
            float min = currentEnemyDatas[i].spawnTiming;

            for (int j = i; j < currentEnemyDatas.Length; j++) {
                
                if (currentEnemyDatas[j].spawnTiming < min)
                    (currentEnemyDatas[i], currentEnemyDatas[j]) = (currentEnemyDatas[j], currentEnemyDatas[i]);
                        // Swap via Destruction
            }
        }

        UIManager.Instance.WaveBarText = "WAVE " + (currentWave + 1);

    }



    void EndGame() {
        CursorLock(false);
        
        currentGameData = null;
        currentWave = 0;
        currentWaveProgress = 0;

        supplyBox = null;
        player = null;
        currentEnemyDatas = null;
        currentWaveData = new WaveData();
        nextEnemyIndex = 0;
        enemyCount = 0;
        supplyHp = 3000;
            // reset

        UIManager.Instance.EnemyBarEnabled = false;
        UIManager.Instance.SupplyBarEnabled = false;
        UIManager.Instance.WaveBarEnabled = false;
        UIManager.Instance.SuccessEnabled = false;
        UIManager.Instance.FailEnabled = false;

        UIManager.Instance.SetCanvas(CanvasType.MainMenu);  
        SceneManager.LoadScene("Main Menu");
        
        PoolManager.Instance.ClearEnemyPool();
    }



    public void GameOver() {
        StartCoroutine(Fail());
    }
    
    IEnumerator Fail() {
        isGaming = false;
        UIManager.Instance.FailEnabled = true;

        yield return new WaitForSeconds(5);

        EndGame();

    }
    
    
    IEnumerator Win() {
        isGaming = false;
        UIManager.Instance.SuccessEnabled = true;

        yield return new WaitForSeconds(5);

        EndGame();

    }



    public void GetCredit(int amount) {
        credit += amount;
        UIManager.Instance.CreditText = credit.ToString();
    }

    public bool UseCredit(int amount) {
        
        if (credit - amount < 0) return false;

        credit -= amount;
        UIManager.Instance.CreditText = credit.ToString();
        return true;
    }
    



    void Update() {
        if (isGaming) {
            WaveManage();
            
            UIManager.Instance.SupplyHpBarFillAmount = (float)supplyHp / 2000;
            UIManager.Instance.WaveBarFillAmount = currentWaveProgress;
        }
    }
    
    
    
    void WaveManage() {

        enemyCount = PoolManager.Instance.GetEnableEnemyCount(); // temp;
        
        
        // 다음 웨이브 계산
        if (currentWaveProgress >= 1) {
            
            if (currentWave + 1 >= currentGameData.waveDatas.Length) {
                if (enemyCount == 0) StartCoroutine(Win()); // 다음 웨이브가 존재하지 않을때, 남은 적이 없으면 게임을 끝냄
                
            } else StartWave(currentWave + 1);
            
        } else currentWaveProgress += Time.deltaTime / currentWaveData.waveTime;
            // 웨이브 진행도는 0~1

        
        
        
        if (!isDoneSpawn) {
            
            var nextEnemyData = currentEnemyDatas[nextEnemyIndex];
                // 적이 아직 남았으면 다음 적 데이터를 불러옴
                
            if (currentWaveProgress >= nextEnemyData.spawnTiming) { // 웨이브 시간 경과에 따라 적 생성(적이 남았다면)
                
                spawner.EnemySpawn(nextEnemyData.prefab, nextEnemyData.spawnCount);
                StartCoroutine(EnemyDetected());

                nextEnemyIndex += 1;
                
                if (nextEnemyIndex >= currentEnemyDatas.Length) isDoneSpawn = true;
                    // 이 웨이브에서 더이상의 적이 없다
            }
        }
    }


    IEnumerator EnemyDetected() { // 경고 표시  
        if (isWhileEnemyWarning) yield break;

        isWhileEnemyWarning = true;

        for (int i=0; i < 10; i++) {

            UIManager.Instance.EnemyBarEnabled = true;
            yield return new WaitForSeconds(1f);
            UIManager.Instance.EnemyBarEnabled = false;
            yield return new WaitForSeconds(0.5f);

        }

        yield return new WaitForSeconds(3f);
        
        isWhileEnemyWarning = false;
    }



    private bool itsOver;
    public void GetSupplyDamage(int damage) {
        supplyHp -= damage;

        if (supplyHp <= 0 && !itsOver) {
            GameOver();
            itsOver = true;
        }

        UIManager.Instance.SupplyRed();
    }
}
