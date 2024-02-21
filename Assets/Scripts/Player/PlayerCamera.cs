using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sens = 400;

    public Transform orientation;

    float xrotation;
    float yrotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yrotation += mouseX;

        xrotation -= mouseY;
        xrotation = Mathf.Clamp(xrotation, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(xrotation, yrotation, 0);
        orientation.rotation = Quaternion.Euler(0, yrotation, 0);
    }
}
