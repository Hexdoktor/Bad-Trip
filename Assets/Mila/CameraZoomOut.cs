using Cinemachine;
using UnityEngine;

public class CameraZoomOut : MonoBehaviour
{
    [SerializeField] float fallThreshold;
    [SerializeField] float minCameraSize;
    [SerializeField] float maxCameraSize;
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomInSpeed;
    float previousYPosition;

    [SerializeField] GameObject player;
    [SerializeField] CinemachineVirtualCamera mainCamera;

    void Start()
    {
        
        previousYPosition = player.transform.position.y;
    }

    void Update()
    {
       //If player is falling the camera zooms out and then zooms back in after player is no longer falling
        if (IsFalling())
        {   
            float newSize = Mathf.Lerp(mainCamera.m_Lens.OrthographicSize, maxCameraSize, Time.deltaTime * zoomSpeed);
            mainCamera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        }
        else
        {
            
            mainCamera.m_Lens.OrthographicSize = Mathf.Lerp(mainCamera.m_Lens.OrthographicSize, minCameraSize, Time.deltaTime * zoomInSpeed);
        }
      
        previousYPosition = player.transform.position.y;
    }

    //If players current y position is lower then the previous position it means that the player is falling
    //And if that position is also greater then the set treshold this ensures that it wont check small fluctuations
    bool IsFalling()
    {  
        float yDifference = previousYPosition - player.transform.position.y;

        return yDifference > fallThreshold;
    }
}

