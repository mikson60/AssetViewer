using System;
using UnityEngine;

public class AssetService : RestServiceBase {

    public const string MODELS_URI = "models/";

    public void DownloadModel(int index, Delegates.AssetServiceCallback callback)
    {
        Delegates.AssetServiceCallback requestCallback = (success, message, result) =>
        {
            Debug.Log("requestCallback");
            if (callback != null)
            {
                callback(success, message, result);
            }
        };

        AsyncServerRequest(MODELS_URI+(index.ToString()), requestCallback);
    }

    [Serializable]
    public class AssetData
    {
        public byte[] data;
    }
}
