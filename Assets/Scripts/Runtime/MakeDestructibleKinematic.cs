using UnityEngine;

public class MakeDestructibleKinematic : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object we collided with has the layer "Destructible"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible"))  
        {
            // Get the Rigidbody of the collided object
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            // Check if the collided object has a Rigidbody component
            if (rb != null) 
            {
                // Make the object non-kinematic 
                rb.isKinematic = false;
            }
        }
    }
}
