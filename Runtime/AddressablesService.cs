using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RossoForge.Addressables
{
    public class AddressableService: IAddressableService
    {
        private Dictionary<object, List<AsyncOperationHandle>> _handleMap = new();

        public async Awaitable<T> LoadAsync<T>(string address, object owner = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(owner, handle);
                return handle.Result;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {address}");
#endif
            return null;
        }

        public async Awaitable<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null, object owner = null)
        {
            AsyncOperationHandle<GameObject> handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync(address, position, rotation, parent);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(owner, handle);
                return handle.Result;
            }

#if UNITY_EDITOR
            Debug.LogError($"Addressable load fail: {address}");
#endif
            return null;
        }

        public async Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label, object owner = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<T>> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(label, null);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                RegisterHandle(owner, handle);
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

        public void ReleaseAll(object owner)
        {
            if (owner == null || !_handleMap.ContainsKey(owner)) return;

            foreach (var handle in _handleMap[owner])
            {
                if (handle.IsValid())
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
            }

            _handleMap.Remove(owner);
        }

        private void RegisterHandle<T>(object owner, AsyncOperationHandle<T> handle)
        {
            if (owner == null) return;

            if (!_handleMap.ContainsKey(owner))
                _handleMap[owner] = new List<AsyncOperationHandle>();

            _handleMap[owner].Add(handle);
#if UNITY_EDITOR
            Debug.Log($"Addressable loaded: {handle.DebugName}");
#endif
        }
    }
}
