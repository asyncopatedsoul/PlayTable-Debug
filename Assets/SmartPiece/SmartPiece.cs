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
        public static SmartPiece instance = null;

        protected void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public PTService ptsrv;

        public String smartTouchCanvasName = "SmartTouchCanvas";
        public GameObject touchMarkerPrefab;
        public GameObject touchMarker2DPrefab;
        public LayerMask smartTouchHitLayers;

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

        public List<SpMarker> spMarkers;

        public class SpMarker
        {
            public int touchId;
            public string label;
            public GameObject go;
            public GameObject collider2D;
            public GameObject collider3D;

            // public SpMarker (int newTouchId, GameObject newGo) {
            //     touchId = newTouchId;
            //     go = newGo;
            // }

            public SpMarker (int newTouchId, GameObject _collider2D, GameObject _collider3D) {
                touchId = newTouchId;
                collider2D = _collider2D;
                collider3D = _collider3D;
            }

            public void SetSmartPieceData(string RFID, string _label)
            {
                label = _label;

                collider2D.GetComponent<TouchMarker>().smartPieceID = RFID;
                collider3D.GetComponent<TouchMarker>().smartPieceID = RFID;
            }

            public void SetDebugInfo(string labelText, string UIDText, string coordinateText, string touchIDText)
            {
                collider2D.GetComponent<TouchMarkerInfo>().UpdateInfoText(labelText, UIDText, coordinateText, touchIDText);
            }
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

                    string uid = IssueScan(xCoordinate, yCoordinate);
                    
                    if (uid != "null") {
                        Debug.Log("TOUCHID:" + n + ", UID:" + uid);     
                        
                        string label = String.Format("UID {0} at {1},{2} : {3}", uid, xCoordinate, yCoordinate, t.fingerId);
                        CreateTouchMarker2D(touchPos, t.fingerId, label, uid);
                    } else {
                        CreateTouchMarker2D(touchPos, t.fingerId, "NOLABEL", "NOUID");
                    }
                    
                } else if (t.phase == TouchPhase.Ended) {
                    Debug.Log("touch ended: " + n);
                    ClearTouchMarker(t.fingerId);

                } else if (t.phase == TouchPhase.Moved) {
                    Debug.Log("touch moving: " + n);
                    UpdateTouchMarker(t.fingerId,t.position);
                }
            }

            yield return null;
        }
        

        public void ClearAllTouchMarkers() 
        {
            for (int i=0; i<spMarkers.Count; i++) {
                // Destroy(spMarkers[i].go);
                Destroy(spMarkers[i].collider2D);
                Destroy(spMarkers[i].collider3D);
            } 
            spMarkers.Clear();
        }

        public void ClearTouchMarker(int touchId)
        {
            SpMarker marker = spMarkers.Find(SpMarker => SpMarker.touchId == touchId);
            Destroy(marker.collider2D);
            Destroy(marker.collider3D);
            spMarkers.RemoveAt(spMarkers.IndexOf(marker));
        }

        public void UpdateTouchMarker(int touchId, Vector3 updatedPosition)
        {
            // Debug.Log("touch marker moving: " + touchId);
            SpMarker thisSp = spMarkers.Find(SpMarker => SpMarker.touchId == touchId);
            
            Vector2 newPos = new Vector2(updatedPosition.x, updatedPosition.y);
            thisSp.collider2D.transform.position = newPos;

            Ray castPoint = Camera.main.ScreenPointToRay(updatedPosition);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, smartTouchHitLayers))
            {
                thisSp.collider3D.transform.position = hit.point;
            }
        }

        public SpMarker CreateTouchMarker2D(Vector3 spawnPosisiton, int touchID, string label, string uid)
        {
            GameObject touchMarker2D = Instantiate(touchMarker2DPrefab, spawnPosisiton, Quaternion.identity);
            touchMarker2D.transform.parent = goCanvas.transform;
            touchMarker2D.transform.position = spawnPosisiton;

            GameObject touchMarker3D = Instantiate(touchMarkerPrefab, spawnPosisiton, Quaternion.identity);

            SpMarker marker = new SpMarker(touchID, touchMarker2D, touchMarker3D);
            marker.SetSmartPieceData(uid, label);
            marker.SetDebugInfo(label, uid, 
                String.Format("{0},{1}",spawnPosisiton.x.ToString(),spawnPosisiton.y.ToString()), 
                touchID.ToString());
            spMarkers.Add(marker);

            return marker;
        }

        // public GameObject CreateTouchMarker(Vector3 pos, string uid) 
        // {
        //     string labelUid = uid;
        //     int uidY = (int)pos.y - 130;
        //     Vector3 uidPos = new Vector3(pos.x, uidY, pos.z);
        //     string labelName = uid;
        //     int nameY = (int)pos.y - 160;
        //     Vector3 namePos = new Vector3(pos.x, nameY, pos.z);

        //     GameObject go = new GameObject(uid);
        //     go.transform.parent = goCanvas.transform;
        //     go.transform.position = pos;
        //     Image img = go.AddComponent<Image>();
        //     img.sprite = Resources.Load<Sprite>("yellow");
        //     float size = xAntSize * 1.5f;
        //     img.rectTransform.sizeDelta = new Vector2(size, size);
        //     img.GetComponent<Image>().color = colorTouchFound;
        //     //
        //     GameObject goUid = new GameObject(labelUid + "_uid");
        //     goUid.transform.parent = go.transform;
        //     goUid.transform.position = uidPos;
        //     Text txtUid = goUid.AddComponent<Text>();
        //     txtUid.text = labelUid;
        //     txtUid.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        //     txtUid.color = colorTouchFound;
        //     txtUid.fontSize = 25;
        //     txtUid.fontStyle = FontStyle.Italic;
        //     //txt.verticalOverflow = VerticalWrapMode.Overflow;
        //     txtUid.horizontalOverflow = HorizontalWrapMode.Overflow;

        //     return go;
        // }


        // public GameObject CreateSpMarker(Vector3 pos, string charactername, string uid)
        // {
        //     Debug.Log("CreateSpMarker Start: " + charactername);
        //     string labelUid = uid;
        //     int uidY = (int)pos.y - 130;
        //     Vector3 uidPos = new Vector3(pos.x, uidY, pos.z);
        //     string labelName = charactername;
        //     int nameY = (int)pos.y - 160;
        //     Vector3 namePos = new Vector3(pos.x, nameY, pos.z);

        //     GameObject go = new GameObject(labelName);
        //     go.transform.parent = goCanvas.transform;
        //     go.transform.position = pos;


        //     // go.transform.scale = new Vector3(1,1,1);
        //     // go.transform.rotation = Quaternion.identity;

        //     Image img = go.AddComponent<Image>();
        //     img.sprite = Resources.Load<Sprite>("yellow");
        //     float size = xAntSize * 1.5f;
        //     img.rectTransform.sizeDelta = new Vector2(size, size);
        //     img.GetComponent<Image>().color = colorTouchFound;
        //     //
        //     GameObject goUid = new GameObject(labelUid + "_uid");
        //     goUid.transform.parent = go.transform;
        //     goUid.transform.position = uidPos;
        //     Text txtUid = goUid.AddComponent<Text>();
        //     txtUid.text = labelUid;
        //     txtUid.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        //     txtUid.color = colorTouchFound;
        //     txtUid.fontSize = 25;
        //     txtUid.fontStyle = FontStyle.Italic;
        //     //txt.verticalOverflow = VerticalWrapMode.Overflow;
        //     txtUid.horizontalOverflow = HorizontalWrapMode.Overflow;
        //     //
        //     GameObject goName = new GameObject(labelName + "_name");
        //     goName.transform.parent = go.transform;
        //     goName.transform.position = namePos;
        //     Text txtName = goName.AddComponent<Text>();
        //     txtName.text = labelName;
        //     txtName.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        //     txtName.color = colorTouchFound;
        //     txtName.fontSize = 30;
        //     txtName.fontStyle = FontStyle.Bold;
        //     //txt.verticalOverflow = VerticalWrapMode.Overflow;
        //     txtName.horizontalOverflow = HorizontalWrapMode.Overflow;
        //     //
        //     Debug.Log("CreateSpMarker End: " + charactername);
        //     return go;
        // }

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