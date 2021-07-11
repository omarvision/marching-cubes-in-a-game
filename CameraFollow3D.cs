using UnityEngine;

public class CameraFollow3D : MonoBehaviour
{
    public Transform LookAt = null;
    public Transform MoveTo = null;
    public float Lookspeed = 45f;
    public float Movespeed = 1.8f;

    private void LateUpdate()
    {
        Quaternion rotTarget = Quaternion.LookRotation(LookAt.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, Lookspeed * Time.deltaTime);
        this.transform.position = Vector3.Lerp(this.transform.position, MoveTo.transform.position, Movespeed * Time.deltaTime);
    }
}
