using UnityEngine;
using System.Collections;

/*
 * Class that represents camera and its behaviour in play mode
 */

public class Camera : MonoBehaviour {

    public float horizontalSpeed = 40;
    public float verticalSpeed = 40;
    public float cameraRotateSpeed = 80;
    public float cameraDistance = 30;

    float curDistance;

    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxis("Horizontal") * horizontalSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * verticalSpeed * Time.deltaTime;
        float rotation = Input.GetAxis("Rotation");

        transform.Translate(Vector3.forward * vertical);
        transform.Translate(Vector3.right * horizontal);

        if (rotation != 0) {
            transform.Rotate(Vector3.up, rotation * cameraRotateSpeed * Time.deltaTime);
        }
    }
}
