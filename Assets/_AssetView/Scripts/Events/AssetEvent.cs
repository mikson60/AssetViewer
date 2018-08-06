
public class AssetEvent
{

    public delegate void AssetAction(Asset asset);

    public static event AssetAction OnAssetImported;

    public static void AssetImported(Asset asset)
    {
        if (OnAssetImported != null) { OnAssetImported(asset); }
    }
}
