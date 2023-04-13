using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugbox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("aa");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnColliderEnter2D(Collision2D other) {
        Debug.Log("colliderenter");
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("triggerenter");
    }
    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log("triggerexit");
    }
    private void OnColliderExit2D(Collision2D other) {
        Debug.Log("colliderexit");
    }
}
