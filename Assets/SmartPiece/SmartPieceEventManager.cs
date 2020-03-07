using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayTable;

namespace PlayTable {
    public class SmartPieceEventManager : MonoBehaviour
    {
        public static SmartPieceEventManager instance;

        
        SmartPiece.SpMarker proxySmartTouchMarker = null;
        //the object to move
        public Transform moveThis;
        //the layers the ray can hit
        public LayerMask hitLayers;

        // Start is called before the first frame update
        void Start()
        {
            
        }


        void Update()
        {
            Vector3 mouse = Input.mousePosition;
            Vector2 mousePos = new Vector2(mouse.x, mouse.y);

            string proxyCharacterName = "proxycharacter";
            string proxyUID = "xxxxxxxxxx";
            int proxyFingerID = 99;

            if (Input.GetMouseButton(0))
            {

                if (proxySmartTouchMarker==null) 
                {
                    Debug.Log(string.Format("mouse down at {0}, {1}, {2}",mouse.x, mouse.y, mouse.z));
                    Debug.Log("Spawn proxy TouchMarker.");
                    proxySmartTouchMarker = SmartPiece.instance.CreateTouchMarker2D(mouse, proxyFingerID, proxyCharacterName, proxyUID); 
                } else 
                {
                    // proxySmartTouchMarker.go.transform.position = mousePos;
                    SmartPiece.instance.UpdateTouchMarker(proxyFingerID, mousePos);
                }
                
            }
            if (!Input.GetMouseButton(0))
            {
                if (proxySmartTouchMarker!=null)
                {
                    Debug.Log("Destroy proxy TouchMarker.");
                    SmartPiece.instance.ClearTouchMarker(proxyFingerID);
                    proxySmartTouchMarker = null;
                }
            }
        }
    }
}
