using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelPlayerInfo
{
    public int level; 
    public int exp; 
    public int maxScore; 
    public int levelScore; 
    public int score; 
    public List<SphereInfo> listSphereInfo; 
}

[Serializable]
public class SphereInfo
{
    public int id;
    public double[] pos;
    public double num;
}