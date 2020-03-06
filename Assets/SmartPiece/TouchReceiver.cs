using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayTable {

    public class TouchReceiver : MonoBehaviour
    {

        public String receiverName;

        public delegate void OnSmartPieceTouchEvent(String smartPieceID, Vector3 position, String receiverName);
        public event OnSmartPieceTouchEvent smartPieceTouchStart;
        public event OnSmartPieceTouchEvent smartPieceTouchEnd;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter",other);

            // extract smartPiece info
            TouchMarker touchMarker = other.transform.GetComponent<TouchMarker>();
            String smartPieceID = touchMarker.smartPieceID;
            touchMarker.touchMarkerDestroyed += OnTouchMarkerDestroyed;

            smartPieceTouchStart(smartPieceID, transform.position, receiverName);
        }

        public void OnTouchMarkerDestroyed(String smartPieceID, Vector3 position)
        {
            smartPieceTouchEnd(smartPieceID, position, receiverName);
        }

        //When the Primitive exits the collision, it will change Color
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("OnTriggerExit",other);

            // extract smartPiece info
            TouchMarker touchMarker = other.transform.GetComponent<TouchMarker>();
            String smartPieceID = touchMarker.smartPieceID;
            touchMarker.touchMarkerDestroyed -= OnTouchMarkerDestroyed;

            smartPieceTouchEnd(smartPieceID, transform.position, receiverName);

            // switch (colorPicker)
            // {
            //     case 0: rend.material.color = Color.white; break;
            //     case 1: rend.material.color = Color.cyan; break;
            //     case 2: rend.material.color = Color.blue; break;
            //     case 3: rend.material.color = Color.black; break;
            //     case 4: rend.material.color = Color.red; break;
            //     case 5: rend.material.color = Color.green; break;
            //     case 6: rend.material.color = Color.grey; break;
            //     case 7: rend.material.color = Color.magenta; break;
            //     case 8: rend.material.color = Color.yellow; break;
            //     case 9: rend.material.color = Color.gray; break;
            // }
        }
    }
}