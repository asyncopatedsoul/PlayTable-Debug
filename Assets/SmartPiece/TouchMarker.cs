using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayTable {

    public class TouchMarker : MonoBehaviour
    {
        public delegate void OnTouchMarkerEvent(String smartPieceID, Vector3 position);
        public event OnTouchMarkerEvent touchMarkerDestroyed;

        public String smartPieceID = null;

        void OnDestroy()
        {
            touchMarkerDestroyed(smartPieceID, transform.position);
        }
    }

}
