using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Mark DiVelbiss
/// When attached to the Main Camera in Unity, the camera is able to pan around and over a square maze of width sideLength.
/// The arrow keys and WASD keys can be used to control the camera.
/// The camera must be set to orthographic view to work correctly.
/// </summary>
public class MazeCamera : MonoBehaviour {
    public int horizontalSpeed = 5;
    public int verticalSpeed = 20;
    public int smooth = 2;

    enum Side { South, West, East, North };
    Side currentSide = Side.South;

    public int sideLength = 10;
    float halfLength;

    float sliderH; // Side to side panning. Between -1 and sideLength+1
    float sliderV; // Vertical panning. Between 0 and 90
    int slideMod = 1; // Allows the horizontal slider to be reversible when the camera is turned around.
    const int minSlideH = -1;
    int maxSlideH; // determined by sideLength
    const int minSlideV = 0;
    const int maxSlideV = 90;
    int distanceToCenter; // determined by sideLength
    float halfMazeHeight; // determined by Maze constants

    int rotation = 0;
    const int rotateLeft = -90;
    const int rotateRight = 90;

    const float heightRatio = 0.57f;

    Vector3 camAngle = new Vector3(0, 0, 0);
    Vector3 camPos = new Vector3(-1, 0, -1);
	
    void Start()
    {
        Maze.CreateMaze(sideLength);
        maxSlideH = sideLength + 1;
        distanceToCenter = 2 * sideLength;
        halfMazeHeight = Maze.maxBlocks / 2.0f;
        sliderH = minSlideH;
        sliderV = minSlideV;
        halfLength = sideLength / 2.0f;
        transform.position = camPos;
        transform.rotation = Quaternion.Euler(camAngle);
        GetComponent<Camera>().orthographicSize = halfLength + 1;
    }

	void Update ()
    {
        UpdateSliders();
        UpdatePosition();
	}

    /// <summary>
    /// Retrieves user input from the horizontal and vertical input axis and updates the sliders accordingly.
    /// Adjusts sliders and rotation if the sliders are beyond their min/max values.
    /// </summary>
    void UpdateSliders()
    {
        sliderH += slideMod * horizontalSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        sliderV += verticalSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        // If horizontal slider has gone off side, rotate based on slider position and slide mod.
        if (sliderH < minSlideH)
        {
            if (slideMod > 0) RotateRight();
            else RotateLeft();
        }
        else if (sliderH > maxSlideH)
        {
            if (slideMod > 0) RotateLeft();
            else RotateRight();
        }
        // Keep vertical slider within min/max range.
        if (sliderV < minSlideV) sliderV = minSlideV;
        else if (sliderV > maxSlideV) sliderV = maxSlideV;
    }

    /// <summary>
    /// This method first determines what the new expected position and angle should be based on the sliders, rotation, and currentSide.
    /// Then, the camera is interpolated towards the desired camPos and camAngle.
    /// </summary>
    void UpdatePosition()
    {
        // Set x and z positions based on sliderH and what side of the maze area the camera is on.
        switch (currentSide)
        {
            case Side.South:
                camPos.x = sliderH;
                camPos.z = halfLength - distanceToCenter * Mathf.Cos(sliderV * Mathf.Deg2Rad);
                break;
            case Side.West:
                camPos.x = halfLength - distanceToCenter * Mathf.Cos(sliderV * Mathf.Deg2Rad);
                camPos.z = sliderH;
                break;
            case Side.East:
                camPos.x = halfLength + distanceToCenter * Mathf.Cos(sliderV * Mathf.Deg2Rad);
                camPos.z = sliderH;
                break;
            case Side.North:
                camPos.x = sliderH;
                camPos.z = halfLength + distanceToCenter * Mathf.Cos(sliderV * Mathf.Deg2Rad);
                break;
        }

        // Set camera height based on sliderV (rises as sliderV increases)
        camPos.y = halfMazeHeight + distanceToCenter * Mathf.Sin(sliderV * Mathf.Deg2Rad);

        // Set camera's x, y rotation (tilts downward as sliderV increases)
        camAngle.y = rotation;
        camAngle.x = sliderV;

        // Use modified form of camera's rotation to ensure that the rotation takes the shortest path to the desired rotation.
        Vector3 currentAngle = transform.rotation.eulerAngles;
        if (currentAngle.y - camAngle.y > 180) currentAngle.y -= 360;
        else if (currentAngle.y - camAngle.y < -180) currentAngle.y += 360;

        // Interpolate the camera from its current position and rotation to the desired one.
        transform.position = Vector3.Lerp(transform.position, camPos, smooth * Time.deltaTime);
        transform.rotation = Quaternion.Euler(Vector3.Lerp(currentAngle, camAngle, smooth * Time.deltaTime));
    }

    /// <summary>
    /// Sets the desired rotation to be 90 degrees to the right and updates the currentSide and slideMod accordingly.
    /// </summary>
    void RotateRight()
    {
        rotation += rotateRight;
        switch (currentSide)
        {
            case Side.South: currentSide = Side.West; sliderH = minSlideH; slideMod = -1; break;
            case Side.West: currentSide = Side.North; sliderH = minSlideH; slideMod = -1; break;
            case Side.East: currentSide = Side.South; sliderH = maxSlideH; slideMod = 1; break;
            case Side.North: currentSide = Side.East; sliderH = maxSlideH; slideMod = 1; break;
        }
        CorrectRotation();
    }

    /// <summary>
    /// Sets the desired rotation to be 90 degrees to the left and updates the currentSide and slideMod accordingly.
    /// </summary>
    void RotateLeft()
    {
        rotation += rotateLeft;
        switch (currentSide)
        {
            case Side.South: currentSide = Side.East; sliderH = minSlideH; slideMod = 1; break;
            case Side.West: currentSide = Side.South; sliderH = minSlideH; slideMod = 1; break;
            case Side.East: currentSide = Side.North; sliderH = maxSlideH; slideMod = -1; break;
            case Side.North: currentSide = Side.West; sliderH = maxSlideH; slideMod = -1; break;
        }
        CorrectRotation();
    }

    /// <summary>
    /// Corrects the rotation value so that it stays within the range [0,360).
    /// </summary>
    void CorrectRotation() { rotation = (rotation + 360) % 360; }
}
