using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteMapController : MonoBehaviour
{
    [Serializable]
    private class WeightedTile
    {
        public TileBase Tile;

        [Min(1)]
        public int Weight = 1;
    }

    [Title("References")]
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private Tilemap _groundTilemap;

    [Title("Chunk Settings")]
    [SerializeField]
    private int _chunkSize = 8;

    [SerializeField]
    private int _loadRadius = 2;

    [SerializeField]
    private int _worldSeed = 12345;

    [Title("Ground Tiles")]
    [SerializeField]
    private List<WeightedTile> _groundTiles;

    private readonly HashSet<Vector2Int> _loadedChunks = new();
    private Vector2Int _currentChunk;

    private bool _isInitialized = false;
    
    public void Initialize(Transform player)
    {
        _player = player;
        _isInitialized = true;
        
        _currentChunk = GetPlayerChunkCoordinate();
        RefreshChunks();
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }
        
        Vector2Int playerChunk = GetPlayerChunkCoordinate();
        

        _currentChunk = playerChunk;
        RefreshChunks();
    }

    private Vector2Int GetPlayerChunkCoordinate()
    {
        Vector3Int cellPosition = _groundTilemap.WorldToCell(_player.position);

        int chunkX = Mathf.FloorToInt(cellPosition.x / (float)_chunkSize);
        int chunkY = Mathf.FloorToInt(cellPosition.y / (float)_chunkSize);

        return new Vector2Int(chunkX, chunkY);
    }

    private void RefreshChunks()
    {
        HashSet<Vector2Int> requiredChunks = new();

        for (int x = -_loadRadius; x <= _loadRadius; x++)
        {
            for (int y = -_loadRadius; y <= _loadRadius; y++)
            {
                Vector2Int chunkCoordinate = _currentChunk + new Vector2Int(x, y);

                requiredChunks.Add(chunkCoordinate);

                if (!_loadedChunks.Contains(chunkCoordinate))
                {
                    GenerateChunk(chunkCoordinate);
                }
            }
        }

        List<Vector2Int> chunksToRemove = new();

        foreach (Vector2Int loadedChunk in _loadedChunks)
        {
            if (!requiredChunks.Contains(loadedChunk))
            {
                chunksToRemove.Add(loadedChunk);
            }
        }


        foreach (Vector2Int chunk in chunksToRemove)
        {
            RemoveChunk(chunk);
        }
        
    }

    private void GenerateChunk(Vector2Int chunkCoordinate)
    {
        int startX = chunkCoordinate.x * _chunkSize;
        int startY = chunkCoordinate.y * _chunkSize;

        System.Random random = new(GetChunkSeed(chunkCoordinate));

        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                Vector3Int cellPosition = new(startX + x, startY + y, 0);

                TileBase tile = GetRandomTile(random);

                
                _groundTilemap.SetTile(cellPosition,tile);
            }
        }

        _loadedChunks.Add(chunkCoordinate);
    }
    
    private void RemoveChunk(Vector2Int chunkCoordinate)
    {
        int startX = chunkCoordinate.x * _chunkSize;
        int startY = chunkCoordinate.y * _chunkSize;

        for (int x = 0; x < _chunkSize; x++)
        {
            for(int y =0;y<_chunkSize;y++)
            {
                Vector3Int cellPosition = new(startX + x,startY + y, 0);
                
                _groundTilemap.SetTile(cellPosition,null);
            }
        }

        _loadedChunks.Remove(chunkCoordinate);
    }

    private TileBase GetRandomTile(System.Random random)
    {
        int totalWeight = 0;

        foreach (WeightedTile weightedTile in _groundTiles)
        {
            totalWeight += weightedTile.Weight;
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        int value = random.Next(totalWeight);

        foreach (WeightedTile weightedTile in _groundTiles)
        {
            if (value < weightedTile.Weight)
            {
                return weightedTile.Tile;
            }

            value -= weightedTile.Weight;
        }

        return _groundTiles[^1].Tile;
    }
    
    private int GetChunkSeed(Vector2Int chunkCoordinate)
    {
        unchecked
        {
            int hash = _worldSeed;

            hash = (hash * 397) ^ chunkCoordinate.x;
            hash = (hash * 397) ^ chunkCoordinate.y;

            return hash & int.MaxValue;
        }
    }
}