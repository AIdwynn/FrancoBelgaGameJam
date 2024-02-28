using System;
using UnityEngine;

namespace FrancoGameJam.Genration
{
    [Serializable]
    public class Category
    {
        [SerializeField] private string _name;
        [SerializeField] private int _spawnChance;
        [SerializeField] private Area[] _areas;
        public int SpawnChance
        {
            get { return _spawnChance; }
        }
        public Area[] Areas
        {
            get { return _areas; }
        }

        public int[] RoomSpawnChances
        {
            get
            {
                 var _spawnChances = new int[_areas.Length];
                _spawnChances[0] = _areas[0].SpawnChance;
                for (int i = 1; i < _areas.Length; i++)
                {
                    _spawnChances[i] = _areas[i].SpawnChance + _areas[i-1].SpawnChance;
                }
                return _spawnChances;
            }
        }
        
    }

    [Serializable]
    public class Area
    {
        [SerializeField] private int _spawnChance;
        public int SpawnChance
        {
            get { return _spawnChance; }
        }

        [SerializeField] private RoomScript _room;

        public RoomScript Room
        {
            get { return _room; }
        }
    }
}