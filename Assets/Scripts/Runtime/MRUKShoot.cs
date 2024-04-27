using UnityEngine;

public class MRUKShoot : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;

    [SerializeField]
    private GameObject bulletPrefab;

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Shoot(ray);
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            Debug.Log("Input primary index trigger");
            Vector3 rayOrigin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            Vector3 rayDirection = controllerRotation * Vector3.forward;

            var ray = new Ray(rayOrigin, rayDirection);
            Shoot(ray);
        }
    }

    private void Shoot(Ray shootingRay)
    {
        Debug.Log("Shoot");
        var bullet = Instantiate(bulletPrefab);
        var ball = bullet.GetComponentInChildren<Rigidbody>();
        ball.velocity = Vector3.zero;

        // Position slightly ahead, since controller collision may intersect
        ball.transform.position = shootingRay.origin + shootingRay.direction * 0.1f;
        ball.AddForce(shootingRay.direction * 200.0f);
        Destroy(bullet, 2f);
    }
}
