using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchReceiver2D : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Touch Receiver2D OnTriggerEnter2D", other);
    }

    void OnTriggerExit2D(Collider2D other){
        Debug.Log("Touch Receiver2D OnTriggerExit2D", other);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
