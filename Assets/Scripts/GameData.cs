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
    ����,
    ����,
    ����,
    �Ĳ�,
};

public enum ColorMixToolEnum
{
    ��,
    ��,
    ��,
    ��,
    ��Ⱦ���,
    ��,
    ˮ,
    �ǽ�
}

public enum ColorWorkState
{
    ����,
    �ʻ�,
    ����
}

public enum GameProgressState
{
    ľ��,
    ʯ��,
    ����,
    �
}

[System.Serializable] public class IconDictionary : SerializableDictionary<string, Sprite> { };
[System.Serializable] public class BuildingScenarioDictionary : SerializableDictionary<BlueprintType, string> { };
[System.Serializable] public class SceneDictionary : SerializableDictionary<string, string> { };
