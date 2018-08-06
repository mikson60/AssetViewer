
public class NetworkEvent {

    public delegate void NetworkRequestObjectAction(NetworkRequestObject requestObject);

    public static event NetworkRequestObjectAction OnNetworkRequestStart;

    public static void NetworkRequestStart(NetworkRequestObject requestObject)
    {
        if (OnNetworkRequestStart != null) { OnNetworkRequestStart(requestObject); }
    }
}
