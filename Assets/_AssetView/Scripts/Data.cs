using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour {

    private List<Asset> assets;

    private void OnEnable()
    {
        AssetEvent.OnAssetImported += AssetEvent_OnAssetImported;
    }
    private void OnDisable()
    {
        AssetEvent.OnAssetImported -= AssetEvent_OnAssetImported;
    }


    private void AssetEvent_OnAssetImported(Asset asset)
    {
        if (asset && !assets.Contains(asset)) { assets.Add(asset); }
    }
}
