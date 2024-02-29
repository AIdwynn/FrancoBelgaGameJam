using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial Text")]
    [SerializeField] [TextArea] private string _movementIntroduction;
    [SerializeField] [TextArea] private string _meleeIntroduction;
    [SerializeField] [TextArea] private string _stunIntroduction;
    [SerializeField] [TextArea] private string _skipIntrodutcion;
    [SerializeField] [TextArea] private string _enemy1Introducton;
    [SerializeField] [TextArea] private string _enemy2Introduction;

    [Header("Other")] [SerializeField] private TextMeshProUGUI Text;
    
    
}
