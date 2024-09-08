using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIManager : Singleton<UIManager> {

	[Serializable]
	public struct MapSection {
		public GameObject parent;
		[Space(5)]
		public Image image;
		public TextMeshProUGUI mapName;
		[Space(3)]
		public TextMeshProUGUI enemyInfo;
		public TextMeshProUGUI storyInfo;
		[Space(3)]
		public Button playButton;
	}
	
	[Header("Main Menu")]
	[Space(3)]
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject title;
	[SerializeField] private GameObject[] menus;
	[Space(3)]
	[SerializeField] private ScrollRect mapBarScroll;
	[SerializeField] private GameObject mapBarElement;
	[SerializeField] private MapSection mapSection;
	

	[Header("PlayerHUD")]
	[Space(5)]
	[SerializeField] private GameObject playerHUD;
	[Space(5)]
	[SerializeField] private WorldSpaceUI boxMark;
	[SerializeField] private WorldSpaceUI boxFallWarning;
	[SerializeField] private GameObject waveBar;
	[SerializeField] private Image waveBarFill;
	[SerializeField] private TextMeshProUGUI waveBarText;
	[Space(5)]
	[SerializeField] private GameObject enemyBar;
	[Space(5)]
	[SerializeField] private Image hpBarFill;
	[SerializeField] private TextMeshProUGUI healPackText;
	[Space(5)]
	[SerializeField] private TextMeshProUGUI creditText;
	[Space(5)]
	[SerializeField] private GameObject supplyHpBar;
	[SerializeField] private Image supplyHpBarFill;
	[Space(5)]
	[SerializeField] private GameObject buildHUD;
	[SerializeField] private TextMeshProUGUI buildHUDText;
	[SerializeField] private TextMeshProUGUI buildHUDCredit;
	[Space(5)]
	[SerializeField] private GameObject fail;
	[SerializeField] private GameObject success;

	private WaveGameDataSO[] gameDataTemp;
	private GameObject[] canvases = new GameObject[2];
	private GameObject previousMenu;
	private CanvasType currentCanvas;
	private float supplyRedTime;

	
	public WorldSpaceUI BoxMark { get => boxMark; }
	public WorldSpaceUI BoxFallWarning { get => boxFallWarning; }
	public float WaveBarFillAmount { set => waveBarFill.fillAmount = value; }
	public string WaveBarText { set => waveBarText.text = value; }
	public float HpBarFillAmount { set => hpBarFill.fillAmount = value; }
	public float SupplyHpBarFillAmount { set => supplyHpBarFill.fillAmount = value; }
	public bool WaveBarEnabled { set => waveBar.gameObject.SetActive(value); }
	public bool EnemyBarEnabled { set => enemyBar.SetActive(value); }
	public string CreditText { set => creditText.text = value; }
	public bool SupplyBarEnabled { set => supplyHpBar.gameObject.SetActive(value); }
	public bool BuildHUDEnabled { set => buildHUD.SetActive(value); }
	public string BuildHUDText { set => buildHUDText.text = value; }
	public string BuildHUDCredit { set => buildHUDCredit.text = value; }
	public bool FailEnabled { set => fail.SetActive(value); }
	public bool SuccessEnabled { set => success.SetActive(value); }
	public string HealPackText { set => healPackText.text = value; }





	void Awake() {
		gameDataTemp = GameManager.Instance.GameDatas;
		
		canvases[(int)CanvasType.MainMenu] = mainMenu;
		canvases[(int)CanvasType.PlayerHUD] = playerHUD;
		
		SetUpMapBar();
	}
	


	void Update() {

		if (supplyRedTime > 0)
			supplyRedTime -= Time.deltaTime;
		supplyHpBarFill.color = supplyRedTime > 0 ? Color.red : Color.white;
			// 보급상자 데미지 표시(SupplyRed)


		if (currentCanvas == CanvasType.MainMenu && Input.GetKeyDown(KeyCode.Escape)) {
			ChangeMenu(0);
		}
	}

	
	
	public void SupplyRed() {
		supplyRedTime = 3;
	}

	
	
	public void SetCanvas(CanvasType type) {
		
		canvases[(int)currentCanvas].SetActive(false);
		canvases[(int)type].SetActive(true);
		currentCanvas = type;

	}
	
	

	public void ChangeMenu(int index) {
		
		title.SetActive(index == 0);
		
		if (previousMenu != null)
			previousMenu.SetActive(false);
		
		menus[index].SetActive(true);
		previousMenu = menus[index];
	}

	
	
	void SetUpMapBar() {
		
		for (int i = 0; i < gameDataTemp.Length; i++) {
			
			GameObject newElement = Instantiate(mapBarElement, mapBarScroll.content);
			
			// 배치
			var rectTransform = newElement.GetComponent<RectTransform>();
			rectTransform.anchoredPosition += new Vector2(0, i * -150);

			
			int index = i; // 아래 코드를 정상적으로 작동시키기 위한 Temp
			newElement.GetComponent<Button>().onClick.AddListener(() => MapBarElementShowData(index));
				// 버튼이 호출하는 함수의 매개변수에 게임데이터 인덱스를 넣음

				
			newElement.GetComponentInChildren<TextMeshProUGUI>().text = gameDataTemp[i].mapName;
				// 버튼 위 텍스트를 맵의 이름으로 바꿈
		}
		
	}


	
	void MapBarElementShowData(int gameDataIndex) {
		
		var gameData = gameDataTemp[gameDataIndex];
		
		mapSection.parent.SetActive(true);
		mapSection.mapName.text = gameData.mapName;
		mapSection.image.sprite = gameData.image;
		mapSection.enemyInfo.text = gameData.enemyInfo;
		mapSection.storyInfo.text = gameData.storyInfo;
		mapSection.playButton.onClick.AddListener(() => GameManager.Instance.PlayGame(gameDataIndex));

	}



	public void EnterPlayMenu() {
		ChangeMenu(1);
	}
	
	public void EnterOptionMenu() {
		ChangeMenu(2);
	}
	
	public void EnterExitMenu() {
		Application.Quit();
	}
	
}








public enum CanvasType { MainMenu, PlayerHUD }





[Serializable]
public struct WorldSpaceUI {
		
	[SerializeField] private GameObject ui;
		
	private bool enabled;
	private RectTransform rect;
		
	public GameObject Ui { get => ui; }
	public RectTransform Rect {
		get {
			if (rect == null) rect = ui.GetComponent<RectTransform>();
			return rect;
		}
	}
	
	
		
	public void SetEnabled(bool enabled) {
			
		this.enabled = enabled;
		ui.SetActive(enabled);
			
	}
		
	public void SetPosition(Vector3 worldPosition) {
		// 조건 검토 후 UI의 위치를 정함
			
		if (enabled) {
			ui.SetActive(CalculateEnable(worldPosition));
			Rect.position = Camera.main.WorldToScreenPoint(worldPosition);
		}
	}
		
	private bool CalculateEnable(Vector3 position) {
		
		Camera main = Camera.main;
		
		float angle = Vector3.Angle(main.transform.forward, (position - main.transform.position).normalized);
		// 카메라의 방향과 position의 방향 각도 차 

		return Mathf.Abs(angle) < main.fieldOfView;
		// UI가 카메라 시야각 내에 위치할 때 활성화
	}
	
}