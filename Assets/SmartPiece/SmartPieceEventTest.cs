using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayTable;

public class SmartPieceEventTest : MonoBehaviour
{
    public GameObject aTouchReceiverGO;

    // Start is called before the first frame update
    void Start()
    {
        if (aTouchReceiverGO != null) {
            TouchReceiver aTouchReceiver = aTouchReceiverGO.GetComponent<TouchReceiver>();
            aTouchReceiver.smartPieceTouchStart += OnSmartPieceTouchStart;
            aTouchReceiver.smartPieceTouchEnd += OnSmartPieceTouchEnd;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSmartPieceTouchStart(String smartPieceID, Vector3 position, String receiverName) {
        Debug.Log(String.Format("SmartPieceTouchStart {0} {1} {2}",smartPieceID, position, receiverName));
    }

    public void OnSmartPieceTouchEnd(String smartPieceID, Vector3 position, String receiverName) {
        Debug.Log(String.Format("SmartPieceTouchEnd {0} {1} {2}",smartPieceID, position, receiverName));

    }
}
