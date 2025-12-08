using System.Collections.Generic;
using UnityEngine;

public interface IPlayerObjectPool
{
    GameObject GetPlayerFromPool(string playerId, Vector3 position, Quaternion rotation);
    void       ReturnPlayerToPool(string playerId, GameObject playerObject);
    void       ClearPool();
}

public class PlayerObjectPool : IPlayerObjectPool
{
    private readonly Dictionary<string, Queue<GameObject>> _playerPools = new();
    private readonly Transform                             _poolContainer;

    public PlayerObjectPool()
    {
        // 创建池容器
        _poolContainer = new GameObject("PlayerObjectPool").transform;
        Object.DontDestroyOnLoad(_poolContainer.gameObject);
    }

    public GameObject GetPlayerFromPool(string playerId, Vector3 position, Quaternion rotation)
    {
        if (_playerPools.TryGetValue(playerId, out var pool) && pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        return null;
    }

    public void ReturnPlayerToPool(string playerId, GameObject playerObject)
    {
        if (playerObject == null) return;

        playerObject.SetActive(false);
        playerObject.transform.SetParent(_poolContainer);

        if (!_playerPools.ContainsKey(playerId))
            _playerPools[playerId] = new Queue<GameObject>();

        _playerPools[playerId].Enqueue(playerObject);
    }

    public void ClearPool()
    {
        foreach (var pool in _playerPools.Values)
            while (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                if (obj != null)
                    Object.Destroy(obj);
            }

        _playerPools.Clear();
    }

    public Dictionary<string, int> GetPoolStats()
    {
        var stats = new Dictionary<string, int>();
        foreach (var kvp in _playerPools) stats[kvp.Key] = kvp.Value.Count;
        return stats;
    }
}