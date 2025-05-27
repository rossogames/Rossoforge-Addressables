using RossoForge.Services;
using UnityEngine;

namespace RossoForge.Addressables
{
    public interface IAddressableService : IService
    {
        bool IsLoaded(string address);
        bool IsLoaded(string containerKey, string address);

        Awaitable<T> LoadAsync<T>(string address) where T : Object;
        Awaitable<T> LoadAsync<T>(string containerKey, string address) where T : Object;

        void Release(string address);
        void Release(string containerKey, string address);

        void ReleaseAll();
        void ReleaseAll(string containerKey);
    }
}
