using System.Collections.Generic;
using UnityEngine;

public class NetworkService : MonoBehaviour {

    List<NetworkRequestObject> pendingRequests = new List<NetworkRequestObject>();

	public void MakeRequest(string url, Delegates.AssetServiceCallback requestCallback)
    {
        NetworkRequestObject requestObject = new NetworkRequestObject(this, OnRequestCompleted);
        pendingRequests.Add(requestObject);
        requestObject.MakeRequest(url, requestCallback);
    }

    private void OnRequestCompleted(NetworkRequestObject requestObject)
    {
        if (pendingRequests.Contains(requestObject)) { pendingRequests.Remove(requestObject); }
    }
}
