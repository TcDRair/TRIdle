using System;
using System.Collections.Generic;
using UnityEngine;
using TRIdle.Game.Item;
using TRIdle.Game.Item.Attributes;

namespace TRIdle.Game.Recipe
{
  [CreateAssetMenu(fileName = "Recipe", menuName = "TRIdle/Recipe")]
  public class RecipeSerialized : ScriptableObject
  {
    public string uName;
    public string tooltip;
    public RequiredItem[] requirements;
    public RequiredItem[] tools;
    public RecipeResult result;
    //TODO Check Validity (e.g. tag level range) in editor
    //TODO Tag 상관관계에 따른 결과물 유효성 검사 필요
  }

  [Serializable]
  public class RequiredItem
  {
    public bool _foldout = true;
    public string uName;
    public int amount;
    public LevelRange levelRange;
    public List<RequiredTag> tags = new();
    public List<Attr> attributes = new();
    // public Rarity Rarity;
    // public Quality Quality;
  }
  [Serializable]
  public class RequiredTag
  {
    public bool _foldout = true;
    public TagData data;
    public LevelRange levelRange;
  }

  [Serializable]
  public class LevelRange
  {
    public bool use = false;
    public int min = 1;
    public int max = 100;

    public int cap;
  }
}
