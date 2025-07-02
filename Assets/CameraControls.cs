using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    Vector3 oldPosition;
    HexComponent[] hexes;

    // Update is called once per frame
    void Update()
    {

        // TODO: Code to click and drag to move the camera
        // WASD
        // ZOOM

        CheckIfCameraMoved();
    }

    public void PanToHex(Hex hex)
    {
        // TODO: Implement camera panning to the specified hex position
    }

    void CheckIfCameraMoved()
    {
        if (oldPosition != transform.position)
        {
            oldPosition = transform.position;

            if (hexes == null)
            {
                hexes = FindObjectsByType<HexComponent>(FindObjectsSortMode.None);
            }

            // Maybe theres a better way to cull what hexes need to be updated
            foreach (HexComponent hex in hexes)
            {
                hex.UpdatePosition();
            }
        }
    }
}
