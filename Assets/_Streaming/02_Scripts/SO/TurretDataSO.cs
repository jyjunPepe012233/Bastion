using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bastion/Turret Data", fileName = "Turret Data", order = int.MaxValue)]
public class TurretDataSO : ScriptableObject {

	public string name;
	public int creditCost;
	[Space(15)]
	public GameObject turret;
	public GameObject previewObject;

}
