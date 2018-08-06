using UnityEngine;

public class Core : MonoBehaviour {

    public static Core Instance;

    public Resources resources;

    private NetworkService networkService;
    public NetworkService NetworkService
    {
        get
        {
            if (!networkService)
            {
                networkService = gameObject.AddComponent<NetworkService>();
            }
            return networkService;
        }
        set
        {
            networkService = value;
        }
    }

    private AssetService assetService;
    public AssetService AssetService
    {
        get
        {
            if (!assetService)
            {
                assetService = gameObject.AddComponent<AssetService>();
            }
            return assetService;
        }
        set
        {
            assetService = value;
        }
    }

    private void Awake()
    {
        if (!Instance) { Instance = this; }
        else {
            Debug.LogWarning("Core instance already exists. Destroying.");
            Destroy(this);
        }
    }
}
