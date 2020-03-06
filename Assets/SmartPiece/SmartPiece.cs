using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleJSON;
using System.Threading;

namespace PlayTable {

    public class SmartPiece : MonoBehaviour
    {
        public PTService ptsrv;

        public bool deviceOffline = false; 
        public String smartTouchCanvasName = "SmartTouchCanvas";
        public GameObject touchMarkerPrefab;

        // TouchMap
        int xOffsetL = 64;
        int xOffsetR = 64;

        int xOverlap = 24;
        //int yOverlap = 29;
        int yOverlap = 27;

        int yOffsetT = 60;
        int yOffsetB = 95;
        //int yOffsetT = 106;
        //int yOffsetB = 69;

        int xAntSize = 171;
        int yAntSize = 181;

        int numAntBanks = 6;
        int totalNumAntX = 2 * 6; // numAntBanks (C# !)

        int numAntY = 6;
        int numAntXPerBank = 2;

        GameObject goCanvas;
        Color colorTouchFound = new Color32(255, 255, 255, 200);
        Color colorTouchNotFound = new Color32(255, 25, 25, 100);
        Color colorTouchMarker = new Color32(25, 25, 225, 100);
        int canvasHeight = 1080;
        int canvasWidth = 1920;

        int cntScans;

        string data_url = "https://vim.blok.party/api/";

        string jsonName;
        public JSONNode jsonData;
        public JSONNode js;

        int cntSp = 0;

        List<SpMarker> spMarkers;

        public class SpMarker
        {
            public int touchId;
            public GameObject go;

            public SpMarker (int newTouchId, GameObject newGo) {
            touchId = newTouchId;
            go = newGo;
            }
        }

        public void SetDeviceOffline() {
            deviceOffline = true;
        }

        public void SetDeviceOnline() {
            deviceOffline = false;
        }


        void Start()
        {
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Debug.Log(" |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| SmartPiece ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Input.multiTouchEnabled = true;
            spMarkers = new List<SpMarker>();
            InitCanvas();
            if (Application.isEditor == false)
            {
                 ptsrv = gameObject.GetComponent<PTService>();
                // ptsrv = gameObject.AddComponent<PTService>(); //added
                Debug.Log("InputManager Start");
            }
        }

        private void Update()
        {
            StartCoroutine(UpdateSpMarker());
        }


        string IssueScan(int xCoordinate, int yCoordinate)
        {
            if (Application.isEditor == true) return "null";
            string uid;
                uid = ptsrv.scan(xCoordinate, yCoordinate);

            cntScans++;

            string txt = cntScans + " SmartPiece <<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>|||||||||||||||||| SmartPiece IssueScan for ";
            txt += xCoordinate + ", " + yCoordinate + " |" + uid + "|";
            Debug.Log(txt);
            return uid;
        }


        IEnumerator UpdateSpMarker()
        {
            int n = 0;
            int xCoordinate = 0;
            int yCoordinate = 0;

            for (n = 0; n < Input.touchCount; n++)
            {
                Touch t = Input.touches[n];
                if (t.phase == TouchPhase.Began)
                {
                    Debug.Log("touch began: " + n);
                    xCoordinate = (int)t.position.x;
                    yCoordinate = canvasHeight - (int)t.position.y;
                    Vector2 touchPos = new Vector2(t.position.x, t.position.y);

                     // if offline, just create touch marker with RFID
                    if (deviceOffline) {
                        spMarkers.Add(new SpMarker(t.fingerId, CreateTouchMarker(touchPos, "OFFLINE")));

                    // if online, can fetch SmartPiece data from server and enrich touch data
                    } else {

                        string uid = IssueScan(xCoordinate, yCoordinate);
                        
                        if (uid != "null") {
                            Debug.Log("Sp exists: " + n);     
                            
                                // yield return StartCoroutine(GetCharacterNameFromUID(uid));
                                // string characterName = jsonName;
                                // Debug.Log("Sp charactername: " + characterName);
                                string characterName = String.Format("UID {0} at {1},{2}",uid,xCoordinate,yCoordinate);
                                
                                spMarkers.Add(new SpMarker(t.fingerId, CreateSpMarker(touchPos, characterName, uid)));
                            
                        } else {
                                spMarkers.Add(new SpMarker(t.fingerId, CreateTouchMarker(touchPos, String.Format("NO UID at {0},{1}",xCoordinate,yCoordinate))));
                        }
                    }
                } else if (t.phase == TouchPhase.Ended) {
                    Debug.Log("touch ended: " + n);
                    SpMarker thisSp = spMarkers.Find(SpMarker => SpMarker.touchId == t.fingerId);
                    Destroy(thisSp.go);
                    spMarkers.RemoveAt(spMarkers.IndexOf(thisSp));
                } else if (t.phase == TouchPhase.Moved) {
                    Debug.Log("touch moving: " + n);
                    SpMarker thisSp = spMarkers.Find(SpMarker => SpMarker.touchId == t.fingerId);
                    Vector2 newPos = new Vector2(t.position.x, t.position.y);
                    thisSp.go.transform.position = newPos;
                }
            }

            yield return null;
        }

        public void ClearAllTouchMarkers() 
        {
            for (int i=0; i<spMarkers.Count; i++) {
                Destroy(spMarkers[i].go);
            } 
            spMarkers.Clear();
        }

        GameObject CreateTouchMarker(Vector3 pos, string uid) 
        {
            // GameObject touchMarker = Instantiate(touchMarkerPrefab, pos, Quaternion.identity);
            // touchMarker.GetComponentInChildren<TouchMarker>().smartPieceID = uid;

            // touchMarker.transform.parent = goCanvas.transform;
            // touchMarker.transform.position = pos;

            // return touchMarker;

            string labelUid = uid;
            int uidY = (int)pos.y - 130;
            Vector3 uidPos = new Vector3(pos.x, uidY, pos.z);
            string labelName = uid;
            int nameY = (int)pos.y - 160;
            Vector3 namePos = new Vector3(pos.x, nameY, pos.z);

            GameObject go = new GameObject(uid);
            go.transform.parent = goCanvas.transform;
            go.transform.position = pos;
            Image img = go.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("yellow");
            float size = xAntSize * 1.5f;
            img.rectTransform.sizeDelta = new Vector2(size, size);
            img.GetComponent<Image>().color = colorTouchFound;
            //
            GameObject goUid = new GameObject(labelUid + "_uid");
            goUid.transform.parent = go.transform;
            goUid.transform.position = uidPos;
            Text txtUid = goUid.AddComponent<Text>();
            txtUid.text = labelUid;
            txtUid.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txtUid.color = colorTouchFound;
            txtUid.fontSize = 25;
            txtUid.fontStyle = FontStyle.Italic;
            //txt.verticalOverflow = VerticalWrapMode.Overflow;
            txtUid.horizontalOverflow = HorizontalWrapMode.Overflow;

            return go;
        }


        GameObject CreateSpMarker(Vector3 pos, string charactername, string uid)
        {
            Debug.Log("CreateSpMarker Start: " + charactername);
            string labelUid = uid;
            int uidY = (int)pos.y - 130;
            Vector3 uidPos = new Vector3(pos.x, uidY, pos.z);
            string labelName = charactername;
            int nameY = (int)pos.y - 160;
            Vector3 namePos = new Vector3(pos.x, nameY, pos.z);

            GameObject go = new GameObject(labelName);
            go.transform.parent = goCanvas.transform;
            go.transform.position = pos;
            Image img = go.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("yellow");
            float size = xAntSize * 1.5f;
            img.rectTransform.sizeDelta = new Vector2(size, size);
            img.GetComponent<Image>().color = colorTouchFound;
            //
            GameObject goUid = new GameObject(labelUid + "_uid");
            goUid.transform.parent = go.transform;
            goUid.transform.position = uidPos;
            Text txtUid = goUid.AddComponent<Text>();
            txtUid.text = labelUid;
            txtUid.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txtUid.color = colorTouchFound;
            txtUid.fontSize = 25;
            txtUid.fontStyle = FontStyle.Italic;
            //txt.verticalOverflow = VerticalWrapMode.Overflow;
            txtUid.horizontalOverflow = HorizontalWrapMode.Overflow;
            //
            GameObject goName = new GameObject(labelName + "_name");
            goName.transform.parent = go.transform;
            goName.transform.position = namePos;
            Text txtName = goName.AddComponent<Text>();
            txtName.text = labelName;
            txtName.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txtName.color = colorTouchFound;
            txtName.fontSize = 30;
            txtName.fontStyle = FontStyle.Bold;
            //txt.verticalOverflow = VerticalWrapMode.Overflow;
            txtName.horizontalOverflow = HorizontalWrapMode.Overflow;
            //
            Debug.Log("CreateSpMarker End: " + charactername);
            return go;
        }

        IEnumerator GetData(string uid)
        {
        WWW www = new WWW(data_url + uid);
        //www = new WWW(data_url + uid);
        Debug.Log("Reading URL:" + data_url + uid + "\n");
        yield return www;
        string jsonRaw = www.text;
        //jsonRaw = www.text;
        Debug.Log("JSON: " + jsonRaw + "\n");
        jsonData = SimpleJSON.JSON.Parse(jsonRaw);
        }

        IEnumerator GetCharacterNameFromUID(string uid)
        {
        yield return StartCoroutine(GetData(uid));
        string txt = "?";
        txt = jsonData["name"].Value;
        Debug.Log("SmartPiece Name: " + txt + "\n");
        jsonName = txt;
        }

        void InitCanvas()
        {
            goCanvas = GameObject.Find(smartTouchCanvasName);
            canvasHeight = 1080;
        }

    }
}