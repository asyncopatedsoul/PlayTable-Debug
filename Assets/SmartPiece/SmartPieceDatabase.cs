using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayTable {
 
    public class SmartPieceDatabase : MonoBehaviour
    {
        public static SmartPieceDatabase instance;
        // public Dictionary<String, Dictionary> library;
        public bool deviceOffline = true;

        public void SetDeviceOffline() {
            deviceOffline = true;
        }

        public void SetDeviceOnline() {
            deviceOffline = false;
        }


        // Start is called before the first frame update
        void Start()
        {
            // library = new Dictionary<String, Dictionary>();

        }
        // Update is called once per frame
        void Update()
        {
            
        }
    }

}
