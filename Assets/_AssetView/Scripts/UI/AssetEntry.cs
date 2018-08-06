using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AssetEntry : MonoBehaviour {

    [SerializeField] Text nameText;

    [SerializeField] Slider progressSlider;
    [SerializeField] Text progressText;
    [SerializeField] Image progressFillImage;

    [SerializeField] Image statusImage;

    [SerializeField] int modelIndex;

    private NetworkRequestObject observableNetworkRequest;

    private void Awake()
    {
        ResetEntry();
    }

    private void OnEnable()
    {
        NetworkEvent.OnNetworkRequestStart += NetworkEvent_OnNetworkRequestStart;
    }
    private void OnDisable()
    {
        NetworkEvent.OnNetworkRequestStart -= NetworkEvent_OnNetworkRequestStart;
    }

    private void NetworkEvent_OnNetworkRequestStart(NetworkRequestObject requestObject)
    {
        if (requestObject.Url == Configuration.API_URL + AssetService.MODELS_URI + modelIndex.ToString())
        {
            observableNetworkRequest = requestObject;
            ShowProgress();
        }
    }

    public void DownloadAsset()
    {
        Core.Instance.AssetService.DownloadModel(modelIndex, OnAssetDownloaded);
    }

    private void OnAssetDownloaded(bool success = true, string errorMessage = null, Asset result = null)
    {
        StopCoroutine(ProgressRoutine());
        progressText.text = "";

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Debug.LogError(errorMessage);
            progressSlider.value = 0;
        }
        else
        {
            progressSlider.value = 100;

            Debug.Log("Invoking event with " + result.name);

            AssetEvent.AssetImported(result);
        }
    }

    private void ShowProgress()
    {
        statusImage.enabled = false;
        StartCoroutine(ProgressRoutine());
    }

    IEnumerator ProgressRoutine()
    {
        while (observableNetworkRequest != null && observableNetworkRequest.isPending)
        {
            // Debug.LogFormat("Request progress: {0}", observableNetworkRequest.currentProgress);
            progressSlider.value = (int)(observableNetworkRequest.currentProgress * 100);
            progressFillImage.color = GetProgressColor(observableNetworkRequest.currentProgress);
            progressText.text = string.Format("{0}%", (int)(observableNetworkRequest.currentProgress * 100));
            yield return null;
        }

        observableNetworkRequest = null;
        statusImage.enabled = true;
    }

    private Color GetProgressColor(float progress)
    {
        float r = progress < 0.5f ? 0.95f : (0.95f * ((1.0f - progress) * 2)) / 100f;
        float g = progress >= 0.5f ? 0.8f : (0.8f * ((0.5f - progress) * 2)) / 100f;
        float b = 0f;

        return new Color(r,g,b);
    }

    public void ResetEntry()
    {
        progressSlider.value = 0;
        progressText.text = "";

    }
}
