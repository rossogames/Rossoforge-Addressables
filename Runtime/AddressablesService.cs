using RossoForge.Core.Addressables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RossoForge.Addressables
{
    public class AddressableService : IAddressableService
    {
        private const string _defaultContainerKey = "default";
        private Dictionary<string, AsyncOperationHandle> _inProgressLoads = new();
        private Dictionary<string, Dictionary<string, AsyncOperationHandle>> _handleMap = new();

        public bool IsLoaded(string address)
        {
            return IsLoaded(_defaultContainerKey, address);
        }

        public bool IsLoaded(string containerKey, string address)
        {
            return _handleMap.TryGetValue(containerKey, out var container)
                && container.ContainsKey(address);
        }

        //address
        public Awaitable<T> LoadAsync<T>(string address) where T : Object
        {
            return LoadAsync<T>(_defaultContainerKey, address);
        }
        public Awaitable<T> LoadAsync<T>(string containerKey, string address) where T : Object
        {
            return LoadAsync<T>(
                containerKey,
                address,
                () => UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(address)
            );
        }

        //AssetReference 
        public Awaitable<T> LoadAsync<T>(AssetReferenceT<T> assetReference) where T : Object
        {
            return LoadAsync<T>(_defaultContainerKey, assetReference);
        }
        public Awaitable<T> LoadAsync<T>(string containerKey, AssetReferenceT<T> assetReference) where T : Object
        {
            return LoadAsync<T>(
                containerKey,
                assetReference.AssetGUID,
                () => assetReference.LoadAssetAsync<T>()
            );
        }

        public void Release(string address)
        {
            Release(_defaultContainerKey, address);
        }

        public void Release(string containerKey, string address)
        {
            if (!_handleMap.TryGetValue(containerKey, out var container))
                throw new KeyNotFoundException($"Addressable container key '{containerKey}' not found.");

            if (!container.TryGetValue(address, out var handler))
                throw new KeyNotFoundException($"Addressable address '{address}' not found in container '{containerKey}'");

            container.Remove(address);
            UnityEngine.AddressableAssets.Addressables.Release(handler);

#if UNITY_EDITOR
            Debug.Log($"Addressable released: {address}");
#endif
        }
        public void ReleaseAll()
        {
            var containers = _handleMap.Keys.ToList();
            foreach (var container in containers)
            {
                ReleaseAll(container);
            }
            _handleMap.Clear();
        }
        public void ReleaseAll(string containerKey)
        {
            if (!_handleMap.TryGetValue(containerKey, out var container))
                throw new KeyNotFoundException($"Addressable container key '{containerKey}' not found.");

            foreach (var handle in container)
            {
                if (handle.Value.IsValid())
                    UnityEngine.AddressableAssets.Addressables.Release(handle.Value);
            }

            _handleMap.Remove(containerKey);
#if UNITY_EDITOR
            Debug.Log($"Addressable container released: {containerKey}");
#endif
        }

        private void RegisterHandle(string containerKey, string address, AsyncOperationHandle handle)
        {
            if (!_handleMap.ContainsKey(containerKey))
                _handleMap[containerKey] = new Dictionary<string, AsyncOperationHandle>();

            _handleMap[containerKey].TryAdd(address, handle);
#if UNITY_EDITOR
            Debug.Log($"Addressable loaded: {address}");
#endif
        }
        private bool TryGetAddressable<T>(string containerKey, string address, out T result) where T : Object
        {
            result = default;

            if (!_handleMap.TryGetValue(containerKey, out var container))
                return false;

            if (container.TryGetValue(address, out var handler))
            {
                if (handler.Result is not T)
                    throw new System.InvalidCastException($"It is not possible to convert the addressable '{address}' of type {handler.Result.GetType().Name} to type {typeof(T).Name}");

                result = handler.Result as T;
                return true;
            }

            return false;
        }
        private async Awaitable<T> LoadAsync<T>(string containerKey, string address, System.Func<AsyncOperationHandle<T>> loadAsset) where T : Object
        {
            if (TryGetAddressable<T>(containerKey, address, out var result))
                return result;

            if (!_inProgressLoads.TryGetValue(address, out AsyncOperationHandle handle))
            {
                handle = loadAsset();
                _inProgressLoads.TryAdd(address, handle);
            }

            try
            {
                await handle.Task;
            }
            finally
            {
                _inProgressLoads.Remove(address);
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(containerKey, address, handle);
                return handle.Result as T;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {address}");
#endif
            return null;
        }
    }
}
