using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public GameObject player;

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private bool usingCamera1 = true;
    private int defaultLayer;
    private int layer2D;

    void Start()
    {
        Debug.Log("CameraSwitch script started");

        playerMovement3D = player.GetComponent<PlayerMovement3D>();
        playerMovement2D = player.GetComponent<PlayerMovement2D>();

        camera1.enabled = true;
        camera2.enabled = false;

        playerMovement3D.enabled = true;
        playerMovement2D.enabled = false;

        defaultLayer = LayerMask.NameToLayer("Default");
        layer2D = LayerMask.NameToLayer("2D");

        if (defaultLayer == -1 || layer2D == -1)
        {
            Debug.LogError("Layer 'Default' or '2D' not defined. Please define these layers in the Tags and Layers settings.");
        }
    }

    void Update()
    {
        Debug.Log("Update method called");

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C key pressed");
            SwitchCamera();
        }
    }

    public void SwitchCamera()
    {
        usingCamera1 = !usingCamera1;

        camera1.enabled = usingCamera1;
        camera2.enabled = !usingCamera1;

        playerMovement3D.enabled = usingCamera1;
        playerMovement2D.enabled = !usingCamera1;

        Debug.Log("Switched camera to: " + (usingCamera1 ? "Camera 1" : "Camera 2"));

        UpdateJumpableLayer();
    }

    void UpdateJumpableLayer()
    {
        GameObject[] jumpables = GameObject.FindGameObjectsWithTag("Jumpable");
        foreach (GameObject jumpable in jumpables)
        {
            jumpable.layer = usingCamera1 ? defaultLayer : layer2D;
        }
    }
}
