using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public float padding = 1.2f;
    public float centerOffset = 0.5f;

    private const float CamAngle = 60.0f;

    public void AdjustCamera(Bounds mapBounds)
    {
        // Center the Camera
        var mainCam = Camera.main;
        var yPos = mainCam.transform.position.y;
        var zOffset = yPos / Mathf.Tan(CamAngle * Mathf.Deg2Rad);
        mainCam.transform.position = new Vector3(
            mapBounds.center.x,
            yPos,
            mapBounds.center.z - zOffset + centerOffset
        );

        // Zoom the Camera
        var visibleMapHeight = mapBounds.size.z * Mathf.Sin(CamAngle * Mathf.Deg2Rad);
        var visibleMapWidth = mapBounds.size.x;

        var screenRatio = (float)Screen.width / Screen.height;
        var mapRatio = visibleMapWidth / visibleMapHeight;

        if (mapRatio > screenRatio)
        {
            mainCam.orthographicSize = (visibleMapWidth / screenRatio) / 2.0f * padding;
        }
        else
        {
            mainCam.orthographicSize = visibleMapHeight / 2.0f * padding;
        }
    }
}