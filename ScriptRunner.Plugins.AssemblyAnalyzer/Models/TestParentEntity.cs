using System.Collections.Generic;

namespace ScriptRunner.Plugins.AssemblyAnalyzer.Models;

/// <summary>
/// Represents a parent entity in a hierarchical structure.
/// </summary>
public class TestParentEntity
{
    /// <summary>
    /// The unique identifier for the parent entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the parent entity.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A collection of child entities linked to this parent.
    /// </summary>
    public List<TestChildEntity> Children { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestParentEntity"/> class with values.
    /// </summary>
    /// <param name="id">The parent's unique identifier.</param>
    /// <param name="name">The parent's name.</param>
    /// <param name="children">The children linked to this parent.</param>
    public TestParentEntity(int id, string name, List<TestChildEntity>? children)
    {
        Id = id;
        Name = name;
        Children = children ?? [];
    }
}