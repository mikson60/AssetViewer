using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkRequestObject
{
    private string url;
    private UnityWebRequest webRequest;
    private string webRequestError;

    private MonoBehaviour coroutineObject;
    private Action<NetworkRequestObject> destroyCallback;

    private bool pending = false;

    public NetworkRequestObject(MonoBehaviour coroutineObject, Action<NetworkRequestObject> destroyCallback)
    {
        this.coroutineObject = coroutineObject;
        this.destroyCallback = destroyCallback;
    }

    internal void MakeRequest(string url, Delegates.AssetServiceCallback requestCallback)
    {
        this.url = url;
        coroutineObject.StartCoroutine(Request(requestCallback));
    }

    private IEnumerator Request(Delegates.AssetServiceCallback requestCallback)
    {
        pending = true;

        NetworkEvent.NetworkRequestStart(this);

        webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
#if UNITY_2017_3_OR_NEWER
        yield return webRequest.SendWebRequest();
#else
        yield return webRequest.Send();
#endif

        pending = false;

        OnRequestCompleted(requestCallback);
    }

    private void OnRequestCompleted(Delegates.AssetServiceCallback requestCallback)
    {
        Debug.Log("OnRequestCompleted");

        if (webRequest != null)
        {
            Debug.LogFormat("Size of response is {0}.", webRequest.downloadedBytes);
            Debug.LogFormat("Size of data is {0}", webRequest.downloadHandler.data.Length.ToString());

            webRequestError = webRequest.error;
            bool hasError = !string.IsNullOrEmpty(webRequestError);

            if (hasError)
            {
                Debug.LogFormat("WWW Error: {0}", webRequestError);
                KillRequest();
                if (requestCallback != null) { requestCallback(false, webRequestError); }
            }
            else
            {
                Debug.Log("Response has no errors");
                Debug.LogFormat("WWW text: {0}", webRequest.downloadHandler.text);

                Asset response = ParseResponseBody(webRequest.downloadHandler.text);
                if (requestCallback != null)
                {
                    requestCallback(true, null, response);
                }
            }
            KillRequest();
        }
        Delete();
    }

    private Asset ParseResponseBody(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.LogError("response is null or empty");
            return null;
        }

        Asset responseData = null;
        try
        {
            Asset asset = AssetImporter.ImportModelByText(response);
            responseData = asset;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }

        return responseData;
    }

    private void StartCoroutine(IEnumerator routine)
    {
        if (routine != null && coroutineObject != null) { coroutineObject.StartCoroutine(routine); }
    }

    private void KillRequest()
    {
        if (webRequest != null)
        {
            webRequest.Dispose();
            webRequest = null;
        }
    }
    private void Delete()
    {
        if (destroyCallback != null) { destroyCallback(this); }
    }

    public bool isPending
    {
        get
        {
            return pending;
        }
    }
    public float currentProgress
    {
        get
        {
            return (webRequest != null ? webRequest.downloadProgress : 0f);
        }
    }
    public string Url
    {
        get
        {
            return url;
        }
    }
}
