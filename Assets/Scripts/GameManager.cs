using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera camera1;  // Perspective Camera
    public Camera camera2;  // Orthographic Camera
    public GameObject player;

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private bool usingCamera1 = true;

    void Start()
    {
        playerMovement3D = player.GetComponent<PlayerMovement3D>();
        playerMovement2D = player.GetComponent<PlayerMovement2D>();

        camera1.enabled = true;
        camera2.enabled = false;

        playerMovement3D.enabled = true;
        playerMovement2D.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
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
    }
}
