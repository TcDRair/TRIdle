

using System;
using System.IO;
using System.Collections.Generic;

namespace TRIdle.Game.Item
{
  public partial class Tag
  {
    public string Name { get; protected set; }
    public string Tooltip { get; protected set; }
    public int MaxLevel { get; protected set; } = -1;
    public int Level { get; protected set; } = -1;
  }

  public static class TagSystem
  {
    public static Dictionary<string, Tag> Tags { get; private set; } = new();

    public static void LoadTags(string csv)
    {
      // Load tags from csv with CsvHelper
      /*using var reader = new StringReader(csv);
      using var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
      var records = csvReader.GetRecords<Tag>();
      foreach (var tag in records) {
        Tags[tag.Name] = tag;
      }*/
    }
  }
}