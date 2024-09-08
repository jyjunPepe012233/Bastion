using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPool : MonoBehaviour {
    
    [SerializeField]
    private Queue<GameObject> soundPool = new Queue<GameObject>();
    
    

    public GameObject CallObject(GameObject sound) {
        
        foreach (GameObject curSound in soundPool) {

            if (curSound == null) continue;
            
            if (curSound.activeSelf == false) {
                curSound.SetActive(true);
                return curSound;
            }
        }
        
            
        GameObject obj = Instantiate(sound, transform);
        soundPool.Enqueue(obj);
        return obj;
    }
}
