using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagezone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay2D(Collider2D other)
    {
        BlobController controller = other.GetComponent<BlobController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }
}
