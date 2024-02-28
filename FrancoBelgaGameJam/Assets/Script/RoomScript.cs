using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrancoGameJam.Genration
{
    public class RoomScript : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform[] _endPoint;
        [SerializeField] private Vector2 _size;
        
        public Transform StartPoint
        {
            get { return _startPoint; }
            private set { _startPoint = value; }
        }

        public Transform[] EndPoint
        {
            get { return _endPoint; }
            private set { _endPoint = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
            private set { _size = value; }
        }

        public float Offset
        {
            get { return (_startPoint.position - this.transform.transform.position).magnitude; }
        }

    }
}
