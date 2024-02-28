using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrancoGameJam.Genration
{
    public class RoomScript : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform[] _endPoint;
        [SerializeField] private float _size;
        
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

        public float Size
        {
            get { return _size; }
            private set { _size = value; }
        }

        public Vector3 Offset
        {
            get { return this.transform.transform.position - _startPoint.position; }
        }

    }
}
