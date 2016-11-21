using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

    public GameObject quad;
    public int weight = 50;
    public int height = 50;
    // Use this for initialization
    void Start() {


    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < weight; i++) {
            for (int j = 0; j < height; j++) {
                Instantiate(quad, new Vector3(i, 0, j), quad.transform.rotation).hideFlags =HideFlags.HideAndDontSave;
            }
        }

    }
}
