using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Not tested
        // Check for mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has a Hex component
                Hex hex = hit.collider.GetComponent<Hex>();
                if (hex != null)
                {
                    Debug.Log($"Hex clicked at Q: {hex.Q}, R: {hex.R}, S: {hex.S}");
                    // You can add more logic here to handle the hex click

                }
            }
        }
    }
    

}
