using System;
using System.Collections.Generic;

namespace EDExplorix.Models.Database;

public class StarSystem
{
    public long SystemAddress { get; set; }
    public string Name { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public int BodyCount { get; set; }
    public int NonBodyCount { get; set; }
    public DateTime FirstVisited { get; set; }

    public ICollection<Body> Bodies { get; set; } = [];
}