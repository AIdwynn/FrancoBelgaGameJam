using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace FrancoGameJam.Genration
{
    public class LevelGeneratorScript : MonoBehaviour
    {
        [SerializeField] private Category[] _categories;
        [SerializeField] private Category _endPoints;
        [SerializeField] private Transform _startPoint;
        
        private int[] _spawnChances;
        private List<RoomScript> _spawnedRooms;

        public bool GenerateLevel()
        {
            _spawnChances = new int[_categories.Length + 1];
            _spawnChances[0] = _categories[0].SpawnChance;
            for (int i = 1; i < _categories.Length; i++)
            {
                _spawnChances[i] = _categories[i].SpawnChance + _categories[i-1].SpawnChance;
            }

            _spawnChances[_categories.Length] = _endPoints.SpawnChance;
            _spawnedRooms = new List<RoomScript>();
            return SpawnRoom(_startPoint);
        }
        
        public bool SpawnRoom(Transform startPoint)
        {
            var random = new Random();
            var goodSpawn = false;
            do
            {
                var numRan = random.Next(0, _spawnChances[_categories.Length]);
                var i = 0;
                for (i = 0; i < _spawnChances.Length; i++)
                {
                    if (numRan < _spawnChances[0]) break;
                }

                var categorie = _categories[0];
                try
                {
                    categorie = _categories[i];
                }
                catch (Exception e)
                {
                    Console.WriteLine("end created");
                    categorie = _endPoints;
                }
                
            } while (!goodSpawn);
        
            return true;
        }
    }
}
