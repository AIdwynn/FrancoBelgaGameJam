using System.Collections;
using System.Collections.Generic;
using FrancoGameJam.Genration;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomScript))]
public class RoomSizeDisplayer : Editor
{
    private void OnSceneGUI()
    {
        RoomScript Target = (RoomScript)target;
        DrawRoomLines(Target);
    }

    private void DrawRoomLines(RoomScript roomScript)
    {
        var startPoint = roomScript.gameObject.transform.position - new Vector3(roomScript.Size.x / 2, 0, roomScript.Size.y / 2);
        DrawSquare(startPoint, roomScript.Size.x, roomScript.Size.y);
    }
    
    private void DrawSquare(Vector3 startPoint, float SizeX, float SizeZ)
    {
        Handles.DrawLine(startPoint, startPoint + new Vector3(SizeX, 0, 0));
        Handles.DrawLine(startPoint, startPoint + new Vector3(0, 0, SizeZ));
        Handles.DrawLine(startPoint + new Vector3(SizeX, 0, 0), startPoint + new Vector3(SizeX, 0, SizeZ));
        Handles.DrawLine(startPoint + new Vector3(0, 0, SizeZ), startPoint + new Vector3(SizeX, 0, SizeZ));
    }
}
