using UnityEngine;

public class RestServiceBase : MonoBehaviour {

    protected void AsyncServerRequest(string path, Delegates.AssetServiceCallback requestCallback)
    {
        string url = (Configuration.API_URL + path).Replace(@"\", @"/");
        Debug.Log("url: " + url);

        Core.Instance.NetworkService.MakeRequest(url, requestCallback);
    }
}
