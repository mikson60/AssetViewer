using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetView : MonoBehaviour {

    [SerializeField] Transform assetParent;
    [SerializeField] BoxCollider assetScaleCollider;

    [SerializeField] private GameObject activeAsset;

    Vector3 scaleStep = new Vector3(0.01f, 0.01f, 0.01f);

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
        if (asset) { StartCoroutine(DisplayAsset(asset)); }
    }

    IEnumerator DisplayAsset(Asset asset, bool destroy = true)
    {
        if (!asset || !assetParent) { yield break; }

        if (destroy && activeAsset) { Destroy(activeAsset.gameObject); }

        asset.transform.SetParent(assetParent);
        asset.transform.localPosition = Vector3.zero;

        yield return StartCoroutine(ScaleAsset(asset));

        activeAsset = asset.gameObject;
    }

    IEnumerator ScaleAsset(Asset asset)
    {
        if (!asset) { yield break; ; }

        Collider scaleLocalCollider = asset.gameObject.AddComponent<BoxCollider>();
        asset.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        

        bool isContained = true;
        int cycles = 0;

        while (isContained)
        {
            cycles++;

            if (cycles > 10000000) { Debug.LogWarning("Scaling takes too many steps. Breaking."); break; }

            Bounds mrBounds = scaleLocalCollider.bounds;

            Vector3[] pts = new Vector3[8];

            pts[0] = new Vector3(mrBounds.center.x + mrBounds.extents.x, mrBounds.center.y + mrBounds.extents.y, mrBounds.center.z + mrBounds.extents.z);
            pts[1] = new Vector3(mrBounds.center.x + mrBounds.extents.x, mrBounds.center.y + mrBounds.extents.y, mrBounds.center.z - mrBounds.extents.z);
            pts[2] = new Vector3(mrBounds.center.x + mrBounds.extents.x, mrBounds.center.y - mrBounds.extents.y, mrBounds.center.z + mrBounds.extents.z);
            pts[3] = new Vector3(mrBounds.center.x + mrBounds.extents.x, mrBounds.center.y - mrBounds.extents.y, mrBounds.center.z - mrBounds.extents.z);
            pts[4] = new Vector3(mrBounds.center.x - mrBounds.extents.x, mrBounds.center.y + mrBounds.extents.y, mrBounds.center.z + mrBounds.extents.z);
            pts[5] = new Vector3(mrBounds.center.x - mrBounds.extents.x, mrBounds.center.y + mrBounds.extents.y, mrBounds.center.z - mrBounds.extents.z);
            pts[6] = new Vector3(mrBounds.center.x - mrBounds.extents.x, mrBounds.center.y - mrBounds.extents.y, mrBounds.center.z + mrBounds.extents.z);
            pts[7] = new Vector3(mrBounds.center.x - mrBounds.extents.x, mrBounds.center.y - mrBounds.extents.y, mrBounds.center.z - mrBounds.extents.z);

            foreach (Vector3 point in pts)
            {
                if (!assetScaleCollider.bounds.Contains(point))
                {
                    Debug.Log("breaking");

                    isContained = false;
                    break;
                }
            }

            if (isContained)
            {
                asset.transform.localScale += scaleStep;
            }

            yield return null;
        }

        Destroy(asset.GetComponent<BoxCollider>());
    }
}
