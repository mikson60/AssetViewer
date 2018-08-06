using UnityEngine;

public class UIController : MonoBehaviour {

	[SerializeField] GameObject downloadAssetsModal;

    public void ToggleDownloadAssetsModal(bool value)
    {
        if (downloadAssetsModal) { downloadAssetsModal.SetActive(value); }
    }
}
