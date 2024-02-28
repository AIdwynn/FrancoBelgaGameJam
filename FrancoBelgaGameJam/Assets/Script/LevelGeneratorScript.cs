using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace FrancoGameJam.Genration
{
    public class LevelGeneratorScript : MonoBehaviour
    {
        [SerializeField] private Category[] _categories;
        [SerializeField] private Category _endPoints;
        [SerializeField] private RoomScript _StartEndRoom;
        
        private int[] _spawnChances;
        private List<RoomScript> _spawnedRooms;
        
        private int depth = 0;
        private int MaxDepth = 10;
        
        public bool GenerateLevel()
        {
            depth = 0;
            _spawnChances = new int[_categories.Length + 1];
            _spawnChances[0] = _categories[0].SpawnChance;
            for (int i = 1; i < _categories.Length; i++)
            {
                _spawnChances[i] = _categories[i].SpawnChance + _categories[i-1].SpawnChance;
            }

            _spawnChances[_categories.Length] = _endPoints.SpawnChance;

            /*if (_spawnedRooms != null)
            {
                for (int i = _spawnedRooms.Count - 1; i >= 1; i--)
                {
                    Destroy(_spawnedRooms[i].gameObject);
                }
            }*/

            _spawnedRooms = new List<RoomScript>();
            _spawnedRooms.Add(_StartEndRoom);
            return SpawnRoom(_StartEndRoom, _StartEndRoom.StartPoint);
        }
        
        public bool SpawnRoom(RoomScript PreviousRoom, Transform startPoint)
        {
            var random = new Random();
            depth++;
            var tries = 0;
            var goodSpawn = false;
            if (depth >= MaxDepth)
            {
                tries++;
                var categorie = _endPoints;

                do
                {
                    var roomSpawnChances = categorie.RoomSpawnChances;
                    var numRan = random.Next(0, roomSpawnChances[categorie.Areas.Length - 1]);
                    var room = categorie.Areas[numRan];
                    
                    var direction = (startPoint.position - PreviousRoom.transform.position).normalized;
                    var position = PreviousRoom.transform.position + (PreviousRoom.Offset * direction) + (room.Room.Offset * direction);
                
                    var directionPrev = (startPoint.position - position).normalized;
                    var directionNew = (room.Room.StartPoint.position - room.Room.transform.position).normalized;
                
                    var anglePrev = ConvertToAngle(directionPrev);
                    var angleNext = ConvertToAngle(directionNew);
                    var angle = angleNext - anglePrev;
                    var overlap = false;
                    foreach (var spawnedRoom in _spawnedRooms)
                    {
                        if (AreSquaresColliding(room.Room, position, spawnedRoom))
                            overlap = true;
                    }
                    if (!overlap)
                    {
                        RoomScript SpawnedRoom = Instantiate(room.Room, position, Quaternion.Euler(0,angle,0));
                        _spawnedRooms.Add(SpawnedRoom);
                        depth--;
                        return true;
                    }
                } while (tries < 20 && !goodSpawn);
            }

            do
            {
                ++tries;
                var numRan = random.Next(0, _spawnChances[_categories.Length]);
                var i = 0;
                for (i = 0; i < _spawnChances.Length; i++)
                {
                    if (numRan < _spawnChances[i]) break;
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

                var roomSpawnChances = categorie.RoomSpawnChances;
                numRan = random.Next(0, roomSpawnChances[categorie.Areas.Length - 1]);
                for (i = 0; i < roomSpawnChances.Length; i++)
                {
                    if (numRan < roomSpawnChances[i]) break;
                }
                var room = categorie.Areas[i];

                var direction = (startPoint.position - PreviousRoom.transform.position).normalized;
                var position = PreviousRoom.transform.position + (PreviousRoom.Offset * direction) + (room.Room.Offset * direction);
                
                var directionPrev = (startPoint.position - position).normalized;
                var directionNew = (room.Room.StartPoint.position - room.Room.transform.position).normalized;
                
                var anglePrev = ConvertToAngle(directionPrev);
                var angleNext = ConvertToAngle(directionNew);
                var angle = angleNext - anglePrev;
                
                var overlap = false;
                foreach (var spawnedRoom in _spawnedRooms)
                {
                    if (spawnedRoom != PreviousRoom && AreSquaresColliding(room.Room, position, spawnedRoom))
                        overlap = true;
                }
                if (!overlap)
                {
                    RoomScript SpawnedRoom = Instantiate(room.Room, position, Quaternion.Euler(0,angle,0));
                    _spawnedRooms.Add(SpawnedRoom);
                    if (SpawnedRoom.EndPoint.Length == 0)
                    {
                        return true;
                    }
                    
                    var spawns = true;
                    var endTries = 0;
                    foreach (var ends in SpawnedRoom.EndPoint)
                    {
                        if (!SpawnRoom(SpawnedRoom, ends))
                        {
                            Debug.Log("Failed To Spawn Room");
                            /*_spawnedRooms.Remove(SpawnedRoom);
                            DestroyImmediate(SpawnedRoom.gameObject);
                            categorie = _endPoints;
                            do
                            {
                                endTries++;
                                roomSpawnChances = categorie.RoomSpawnChances;
                                numRan = random.Next(0, roomSpawnChances[categorie.Areas.Length - 1]);
                                room = categorie.Areas[numRan];
                    
                                direction = (startPoint.position - PreviousRoom.transform.position).normalized;
                                position = PreviousRoom.transform.position + (PreviousRoom.Offset * direction) + (room.Room.Offset * direction);
                
                                directionPrev = (startPoint.position - position).normalized;
                                directionNew = (room.Room.StartPoint.position - room.Room.transform.position).normalized;
                
                                anglePrev = ConvertToAngle(directionPrev);
                                angleNext = ConvertToAngle(directionNew);
                                angle = angleNext - anglePrev;
                                overlap = false;
                                foreach (var spawnedRoom in _spawnedRooms)
                                {
                                    if (AreSquaresColliding(room.Room, position, spawnedRoom))
                                        overlap = true;
                                }
                                if (!overlap)
                                {
                                    SpawnedRoom = Instantiate(room.Room, position, Quaternion.Euler(0,angle,0));
                                    _spawnedRooms.Add(SpawnedRoom);
                                    goodSpawn = true;
                                }
                            } while (endTries < 20 && !goodSpawn);*/
                        }
                    }
                    if(endTries != 0)
                        goodSpawn = true;
                }
            } while (tries < 20 && !goodSpawn);

            
            if (tries == 20)
                    return false;
               
            return true;
        }
        /*private bool AreSquaresColliding(RoomScript square1, Vector3 square1Pos, RoomScript square2)
        {
            // Check for collision by comparing positions and dimensions
            bool xOverlap = Math.Abs(square1.transform.position.x - square2.transform.position.x) * 2 < (square1.Size + square2.Size);
            bool yOverlap = Math.Abs(square1.transform.position.z - square2.transform.position.z) * 2 < (square1.Size + square2.Size);

            return xOverlap && yOverlap;
        }*/
        
        static bool AreSquaresColliding(RoomScript rectangle1,Vector3 square1Pos, RoomScript rectangle2)
        {
            // Check for collision by comparing positions and dimensions
            bool xOverlap = (square1Pos.x < rectangle2.transform.position.x + (rectangle2.Size.x/2)) && (square1Pos.x + (rectangle1.Size.x/2) > rectangle2.transform.position.x);
            bool yOverlap = (square1Pos.z < rectangle2.transform.position.z + (rectangle2.Size.y/2)) && (square1Pos.z + (rectangle1.Size.y/2) > rectangle2.transform.position.z);

            return xOverlap && yOverlap;
        }

        private float ConvertToAngle(Vector3 input)
        {
            var angle = Mathf.Atan2(input.z, input.x) * Mathf.Rad2Deg;
            return angle;
        }
    }
}
