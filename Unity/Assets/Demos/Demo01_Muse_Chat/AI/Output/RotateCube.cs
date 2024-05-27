using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotateSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        // Rotates the cube at 30 degrees per second around y axis
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}