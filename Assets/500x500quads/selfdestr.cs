using UnityEngine;
using System.Collections;

public class selfdestr : MonoBehaviour {
    void OnApplicationQuit() {
        DestroyImmediate(gameObject);
    }
}
