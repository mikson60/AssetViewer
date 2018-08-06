using UnityEngine;

public class Delegates : MonoBehaviour {

    public delegate void AssetServiceCallback(bool success = true, string errorMessage = null, Asset result = default(Asset));
}
