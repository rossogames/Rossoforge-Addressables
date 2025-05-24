using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RossoForge.Addressables
{
    public class AddressableService : IAddressableService
    {
        private const string _defaultContainerKey = "default";
        private Dictionary<string, List<AsyncOperationHandle>> _handleMap = new();

        public Awaitable<T> LoadAsync<T>(string address) where T : UnityEngine.Object
        {
            return LoadAsync<T>(address, _defaultContainerKey);
        }
        public async Awaitable<T> LoadAsync<T>(string address, string containerKey) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(containerKey, handle);
                return handle.Result;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {address}");
#endif
            return null;
        }

        public Awaitable<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return InstantiateAsync(address, _defaultContainerKey, position, rotation, parent);
        }
        public async Awaitable<GameObject> InstantiateAsync(string address, string containerKey, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            AsyncOperationHandle<GameObject> handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync(address, position, rotation, parent);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(containerKey, handle);
                return handle.Result;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {address}");
#endif
            return null;
        }

        public Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
        { 
            return LoadAssetsByLabelAsync<T>(label, _defaultContainerKey);
        }
        public async Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label, string containerKey) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<T>> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(label, null);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(containerKey, handle);
                return handle.Result;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {label}");
#endif
            return null;
        }

        public void Release<T>(T asset) where T : UnityEngine.Object
        {
            UnityEngine.AddressableAssets.Addressables.Release(asset);
        }

        public void ReleaseHandle(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                UnityEngine.AddressableAssets.Addressables.Release(handle);
        }

        public void ReleaseAll()
        { 
            foreach (var container in _handleMap.Keys)
            {
                ReleaseAll(container);
            }
            _handleMap.Clear();
        }

        public void ReleaseAll(string containerKey)
        {
            if (string.IsNullOrWhiteSpace(containerKey) || !_handleMap.ContainsKey(containerKey))
                return;

            foreach (var handle in _handleMap[containerKey])
            {
                if (handle.IsValid())
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
            }

            _handleMap.Remove(containerKey);
        }

        private void RegisterHandle<T>(string containerKey, AsyncOperationHandle<T> handle)
        {
            if (!_handleMap.ContainsKey(containerKey))
                _handleMap[containerKey] = new List<AsyncOperationHandle>();

            _handleMap[containerKey].Add(handle);
#if UNITY_EDITOR
            Debug.Log($"Addressable loaded: {handle.DebugName}");
#endif
        }
    }
}
