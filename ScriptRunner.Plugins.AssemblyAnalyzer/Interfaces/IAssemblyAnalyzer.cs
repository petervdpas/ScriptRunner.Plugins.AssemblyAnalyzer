using System.Collections.Generic;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.AssemblyAnalyzer.Interfaces;

/// <summary>
///     Defines methods for analyzing assemblies and namespaces to extract information
///     about entities (classes, enums, etc.) and their relationships (inheritance, foreign keys, etc.).
/// </summary>
public interface IAssemblyAnalyzer
{
    /// <summary>
    ///     Analyzes the specified assembly to extract entities and relationships.
    /// </summary>
    /// <param name="assemblyPath">The full path to the assembly file to analyze.</param>
    /// <param name="useCustomLogic">
    ///     A boolean flag indicating whether to apply custom logic for detecting relationships based on naming conventions.
    ///     If set to <c>true</c>, relationships are inferred from property names matching the specified foreign key suffix.
    /// </param>
    /// <param name="foreignKeySuffix">
    ///     The suffix used to identify foreign key properties when <paramref name="useCustomLogic" /> is enabled.
    ///     Properties with names ending in this suffix are treated as potential foreign keys.
    /// </param>
    /// <param name="primaryKeyName">
    ///     The name of the primary key property used to establish relationships when <paramref name="useCustomLogic" /> is
    ///     enabled.
    ///     This identifies the property in related entities that foreign keys will reference.
    /// </param>
    /// <returns>
    ///     A tuple containing:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>A list of <see cref="Entity" /> objects representing classes and enums from the assembly.</description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Relationship" /> objects representing relationships such as inheritance
    ///                 and key-based associations.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    (List<Entity> Entities, List<Relationship> Relationships) AnalyzeAssembly(
        string assemblyPath,
        bool useCustomLogic = false,
        string foreignKeySuffix = "Id",
        string primaryKeyName = "Id");

    /// <summary>
    ///     Analyzes all loaded assemblies in the current application domain and extracts entities and relationships within a
    ///     specified namespace.
    /// </summary>
    /// <param name="namespaceName">
    ///     The namespace to analyze. Only classes and enums within this namespace are considered.
    /// </param>
    /// <param name="useCustomLogic">
    ///     A boolean flag indicating whether to apply custom logic for detecting relationships based on naming conventions.
    ///     If set to <c>true</c>, relationships are inferred from property names matching the specified foreign key suffix.
    /// </param>
    /// <param name="foreignKeySuffix">
    ///     The suffix used to identify foreign key properties when <paramref name="useCustomLogic" /> is enabled.
    ///     Properties with names ending in this suffix are treated as potential foreign keys.
    /// </param>
    /// <param name="primaryKeyName">
    ///     The name of the primary key property used to establish relationships when <paramref name="useCustomLogic" /> is
    ///     enabled.
    ///     This identifies the property in related entities that foreign keys will reference.
    /// </param>
    /// <returns>
    ///     A tuple containing:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>A list of <see cref="Entity" /> objects representing classes and enums within the namespace.</description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Relationship" /> objects representing relationships such as inheritance
    ///                 and key-based associations.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    (List<Entity> Entities, List<Relationship> Relationships) AnalyzeNamespace(
        string namespaceName,
        bool useCustomLogic = false,
        string foreignKeySuffix = "Id",
        string primaryKeyName = "Id");
}