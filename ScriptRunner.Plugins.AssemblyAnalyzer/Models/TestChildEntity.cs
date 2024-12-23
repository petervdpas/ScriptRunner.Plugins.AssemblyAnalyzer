namespace ScriptRunner.Plugins.AssemblyAnalyzer.Models;

/// <summary>
/// Represents a child entity in a hierarchical structure.
/// </summary>
public class TestChildEntity
{
    /// <summary>
    /// The unique identifier for the child entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the child entity.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The foreign key linking this child to its parent.
    /// </summary>
    public int ParentEntityId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestChildEntity"/> class with values.
    /// </summary>
    /// <param name="id">The child's unique identifier.</param>
    /// <param name="name">The child's name.</param>
    /// <param name="parentEntityId">The ID of the parent entity this child is linked to.</param>
    public TestChildEntity(int id, string name, int parentEntityId)
    {
        Id = id;
        Name = name;
        ParentEntityId = parentEntityId;
    }
}