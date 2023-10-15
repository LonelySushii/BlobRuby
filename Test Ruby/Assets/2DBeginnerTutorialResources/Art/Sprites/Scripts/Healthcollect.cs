using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthcollect : MonoBehaviour
{
    public AudioClip collectedClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        BlobController controller = other.GetComponent<BlobController>();

        if (controller != null)
        {
            if(controller.Health < controller.maxHealth)
            {
            controller.ChangeHealth(1);
            Destroy(gameObject);

            controller.Playsound(collectedClip);
            }
        }
    }
}
