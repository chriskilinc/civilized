using UnityEngine;

public class CameraKeyboardController : MonoBehaviour
{

    public float moveSpeed = 20f; // Speed of camera movement

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 translate = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(moveSpeed * Time.deltaTime * translate, Space.World);
    }
}
