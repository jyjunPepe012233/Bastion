using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	private static T instance;
	public static T Instance {

		get {
			if (instance == null) {
				
				instance = FindObjectOfType<T>();

				if (instance == null) {
					
					var obj = new GameObject(typeof(T).Name, typeof(T));
					instance = obj.GetComponent<T>();
				}
			}
			
			return instance;
		}
	}

	public void Awake() {

		if (transform.root == null) {
			DontDestroyOnLoad(this.gameObject);
			
		} else DontDestroyOnLoad(transform.root);
	}
}
