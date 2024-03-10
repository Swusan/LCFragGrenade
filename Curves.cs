using UnityEngine;

namespace LethalFragGrenade;

[CreateAssetMenu(fileName = "Curves", menuName = "ScriptableObjects/Curves")]
public class Curves : ScriptableObject
{
    public AnimationCurve[] curves;
}