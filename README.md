# Rosso Games

<table>
  <tr>
    <td><img src="https://github.com/rossogames/Rossoforge-Addressables/blob/master/logo.png?raw=true" alt="Rossoforge" width="64"/></td>
    <td><h2>Rossoforge - Addressables</h2></td>
  </tr>
</table>

**Rossoforge-Addressables** Is a lightweight wrapper for Unity Addressables, designed to simplify asset loading, releasing, and reference handling across your project

**Version**: Unity 6 or higher

**Tutorial**: [Pending]

**Dependencies**
* com.unity.addressables
* [Rossoforge-Core](https://github.com/rossogames/Rossoforge-Core.git)

#
With this package, you can:
- sync asset loading via string addresses or AssetReferenceT<T>
- Support for named containers (containerKey) to group assets contextually
- Prevention of duplicate simultaneous loads
- Safe release methods for individual assets or entire containers
- Awaitable-based API for clean and readable async workflows

#

```csharp
// Setup
ServiceLocator.SetLocator(new DefaultServiceLocator());
ServiceLocator.Register<IAddressableService>(new AddressableService());
ServiceLocator.Initialize();

// Anywhere in your code
var _addressableService = ServiceLocator.Get<IAddressableService>();

// Load one by address
await _addressableService.LoadAssetAsync<GameObject>("enemy");
await _addressableService.LoadAssetAsync<Sprite>("icon");

// Load many by label
await _addressableService.LoadAssetsAsync<GameObject>("World1");

_addressableService.Release("enemy");
_addressableService.ReleaseAll();
```

#
This package is part of the **Rossoforge** suite, designed to streamline and enhance Unity development workflows.

Developed by Agustin Rosso
https://www.linkedin.com/in/rossoagustin/
