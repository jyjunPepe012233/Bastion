using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStartArea : MonoBehaviour {
    
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {
            
            GameManager.Instance.StartGame();
            GetComponent<Collider>().enabled = false;
            
        }
    }
    
}