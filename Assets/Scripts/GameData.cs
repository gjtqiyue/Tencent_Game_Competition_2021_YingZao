using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    InMenu,
    StartGame,
    InGame,
    Paused,
    FinishedGame
};

public enum BlueprintType
{
    基层,
    二层,
    三层,
    四层,
};

public enum ColorMixToolEnum
{
    无,
    白,
    青,
    红,
    凡染赤黄,
    绿,
    水,
    骨胶
}

public enum ColorWorkState
{
    调漆,
    彩画,
    上漆
}

public enum GameProgressState
{
    木作,
    石作,
    漆作,
    搭建
}

[System.Serializable] public class IconDictionary : SerializableDictionary<string, Sprite> { };
[System.Serializable] public class BuildingScenarioDictionary : SerializableDictionary<BlueprintType, string> { };
[System.Serializable] public class SceneDictionary : SerializableDictionary<string, string> { };
