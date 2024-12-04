using System;
using System.Collections;
using System.Collections.Generic;
using RaindowStudio.Attribute;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RaindowStudio.DesignPattern;
using RaindowStudio.Utility;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressableManager : SingletonUnityEternal<AddressableManager>
{
    public const string LABEL_GLOBAL = "Global";
    public const string LABEL_TITLE_SCENE = "TitleScene";
    public const string LABEL_MAP_SCENE = "MapScene";

    public Dictionary<string, CharacterDataSet> Character { get; set; }
    public Dictionary<string, Sprite> UI { get; set; }
    public Dictionary<MapBlockEventType, GameObject> MapBlockPrefabs { get; set; }
    public List<MapBlockProbability> MapBlockProbabilities { get; set; }
    
    [SerializeField] private AssetReferenceT<MapData> ar_mapData;
    [SerializeField] private AssetReferenceT<UIData> ar_ui;
    
    private bool _initialized;

    public bool Initialized => _initialized;
    public CharacterDataSet CurrentCharacterData => Instance.Character[GameManager.Instance.CharacterID];

    public Coroutine LoadAssetsByLabel<T>(string label,
        Action<T> assetLoaded,
        Action<AsyncOperationHandle> downloading = null,
        Action<AsyncOperationHandle> completed = null)
    {
        return StartCoroutine(LoadAssetsByLabelIE<T>(label, assetLoaded, downloading, completed));
    }

    private IEnumerator LoadAssetsByLabelIE<T>(string label,
        Action<T> assetLoaded,
        Action<AsyncOperationHandle> downloading,
        Action<AsyncOperationHandle> completed)
    {
        AsyncOperationHandle<long> aoh_size = default;
        try
        {
            aoh_size = Addressables.GetDownloadSizeAsync(label);
        }
        catch (Exception e)
        {
            Debug.Log($"Download bundle '{label}' failed : {e.Message}");
        }

        yield return aoh_size;

        if (aoh_size.Status == AsyncOperationStatus.Succeeded)
        {
            AsyncOperationHandle handle = default;
            if (aoh_size.Result > 0)
            {
                try
                {
                    handle = Addressables.DownloadDependenciesAsync(label);
                }
                catch (Exception e)
                {
                    Debug.Log($"Download bundle '{label}' failed : {e.Message}");
                }

                while (!handle.IsDone)
                {
                    downloading?.Invoke(handle);
                    yield return null;
                }

                Addressables.Release(handle);
            }
            
            Addressables.Release(aoh_size);

            handle = Addressables.LoadAssetsAsync<T>(label, assetLoaded);
            yield return handle;
            completed?.Invoke(handle);
        }
    }
    
    private IEnumerator InitializeIE(Action<AsyncOperationHandle> completed = null)
    {
        AsyncOperationHandle handle = Addressables.InitializeAsync();
        yield return handle;
        completed?.Invoke(handle);
    }
    
    protected override void Initialization()
    {
        base.Initialization();

        Character = new Dictionary<string, CharacterDataSet>();
        UI = new Dictionary<string, Sprite>();
        MapBlockProbabilities = new List<MapBlockProbability>();
        MapBlockPrefabs = new Dictionary<MapBlockEventType, GameObject>();
        StartCoroutine(InitializeIE(t => _initialized = true));
    }
}