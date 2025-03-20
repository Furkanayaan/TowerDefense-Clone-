using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Tower : MonoBehaviour
{
    public class Factory : PlaceholderFactory<Tower> { }
}
