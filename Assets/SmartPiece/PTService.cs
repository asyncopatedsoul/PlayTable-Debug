using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTService : MonoBehaviour {
private static string fullClassName = "com.blokparty.ptserviceproxy.PTServiceProxy";
AndroidJavaObject proxyObject = null;
string id;
	// Use this for initialization
	void Start () {
         Debug.Log("PTService->Start");
		createProxyObject();
	}

	// Update is called once per frame
	void Update () {

	}

	void  createProxyObject() {
		Debug.Log("in PTService: createProxyObject");
	var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
 	var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
	     AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
         	proxyObject = pluginClass.CallStatic<AndroidJavaObject>("createInstance", context);
         	Debug.Log("createProxyObject");
        }
	}

	public void showControlButtonMenu() {
		if(proxyObject == null) {
			return;
		}
		proxyObject.Call("showMenu");
	}

	public void autoScan() {
		if(proxyObject == null) {
			return;
		}
		AndroidJavaObject[] scanResults = proxyObject.Call<AndroidJavaObject[]>("autoScan");
		if(scanResults.Length<=0) {
			Debug.Log("No smart piece found");
		} else {
			for(int i =0; i< scanResults.Length; i++) {
				string id = scanResults[i].Get<string>("id");
				AndroidJavaObject loc = scanResults[i].Get<AndroidJavaObject>("location");
				string bank = loc.Get<string>("first");
				string index = loc.Get<string>("second");
				Debug.Log("SM #"+(i+1) + " with id: " + id+ "at: " + bank + " " + index);
			}
		}
	}

	public string scan(int x, int y) {
		if(proxyObject == null) {
			return id;
		}
		AndroidJavaObject scanResult = proxyObject.Call<AndroidJavaObject>("scan", x, y);
		if(scanResult == null) {
			id = "null";
			Debug.Log("No smart piece found at ("+x + ","+y +")");
		} else {
			id = scanResult.Get<string>("id");
			AndroidJavaObject loc = scanResult.Get<AndroidJavaObject>("location");
			string bank = loc.Get<string>("first");
			string index = loc.Get<string>("second");
			Debug.Log("Found smart piece at (x,y):" + x +","+y + " with id: " + id + " at :" + bank + " " + index);
		}
		return id; //added
	}

	public void enableMenu(int flag) {
		if(proxyObject == null) {
			return;
		}
		proxyObject.Call("enableMenu", flag);

	}

	private AndroidJavaObject javaArrayFromCS(string [] values) {
    	AndroidJavaClass arrayClass  = new AndroidJavaClass("java.lang.reflect.Array");
    	AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Length);
    	for (int i=0; i<values.Length; ++i) {
        	arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
    	}

    	return arrayObject;
	}
}
