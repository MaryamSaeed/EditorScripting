using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGameObjectData : ScriptableObject
{
    [SerializeField]
    public GameObject replacingPrefab;
    [SerializeField]
    public List<GameObject> toBeReplaced;
}
