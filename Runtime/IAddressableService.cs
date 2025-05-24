using RossoForge.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RossoForge.Addressables
{
    public interface IAddressableService : IService
    {
        Awaitable<T> LoadAsync<T>(string address) where T : Object;
        Awaitable<T> LoadAsync<T>(string address, string containerKey = "") where T : Object;

        Awaitable<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null);
        Awaitable<GameObject> InstantiateAsync(string address, string containerKey, Vector3 position, Quaternion rotation, Transform parent = null);

        Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label) where T : Object;
        Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label, string containerKey) where T : Object;

        void Release<T>(T asset) where T : Object;
        void ReleaseHandle(AsyncOperationHandle handle);

        void ReleaseAll();
        void ReleaseAll(string containerKey);
    }
}
