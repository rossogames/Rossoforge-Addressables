using RossoForge.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RossoForge.Addressables
{
    public interface IAddressableService: IService
    {
        Awaitable<T> LoadAsync<T>(string address, object owner = null) where T : UnityEngine.Object;
        Awaitable<GameObject> InstantiateAsync(string address, Vector3 position, Quaternion rotation, Transform parent = null, object owner = null);
        Awaitable<IList<T>> LoadAssetsByLabelAsync<T>(string label, object owner = null) where T : UnityEngine.Object;
        void Release<T>(T asset) where T : UnityEngine.Object;
        void ReleaseHandle(AsyncOperationHandle handle);
        void ReleaseAll(object owner);
    }
}
