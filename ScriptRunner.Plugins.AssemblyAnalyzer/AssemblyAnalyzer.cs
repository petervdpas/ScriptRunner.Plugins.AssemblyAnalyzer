using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptRunner.Plugins.AssemblyAnalyzer.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.AssemblyAnalyzer;

/// <summary>
///     Implements the <see cref="IAssemblyAnalyzer" /> interface to analyze assemblies and extract information
///     about classes and their relationships.
/// </summary>
public class AssemblyAnalyzer : IAssemblyAnalyzer
{
    /// <summary>
    ///     Analyzes the specified assembly to extract entities and relationships between them, including inheritance
    ///     and key-based relationships.
    /// </summary>
    /// <param name="assemblyPath">The full path to the assembly file to analyze.</param>
    /// <param name="useCustomLogic">
    ///     A boolean flag indicating whether to apply custom logic for detecting relationships based on naming conventions.
    ///     If set to <c>true</c>, the method will look for relationships inferred from property names that match the specified
    ///     foreign key suffix.
    /// </param>
    /// <param name="foreignKeySuffix">
    ///     The suffix used to identify foreign key properties when <paramref name="useCustomLogic" /> is enabled.
    ///     Properties with names ending in this suffix will be considered as potential foreign keys.
    /// </param>
    /// <param name="primaryKeyName">
    ///     The name of the primary key property used to establish relationships when
    ///     <paramref name="useCustomLogic" /> is enabled. This identifies the property in related entities
    ///     that foreign keys will reference.
    /// </param>
    /// <returns>
    ///     A tuple containing:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Entity" /> objects representing classes from the assembly,
    ///                 with their properties collected as attributes.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Relationship" /> objects representing both inheritance and key-based
    ///                 relationships,
    ///                 where the base class is "FromEntity" and the derived class is "ToEntity".
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    public (List<Entity> Entities, List<Relationship> Relationships) AnalyzeAssembly(
        string assemblyPath,
        bool useCustomLogic = false,
        string foreignKeySuffix = "Id",
        string primaryKeyName = "Id")
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        var entities = new List<Entity>();
        var relationships = new List<Relationship>();
        var entityMap = new Dictionary<string, Entity>();

        var types = assembly.GetTypes();

        CollectEntitiesAndEnums(types, entities, entityMap);
        EstablishRelationships(types, relationships, entityMap, useCustomLogic, foreignKeySuffix);

        return (entities, relationships);
    }

    /// <summary>
    ///     Analyzes all loaded assemblies in the current AppDomain and extracts information about entities
    ///     and their relationships within a specified namespace.
    /// </summary>
    /// <param name="namespaceName">
    ///     The namespace to analyze for extracting classes and their relationships.
    ///     Only classes within this namespace will be considered.
    /// </param>
    /// <param name="useCustomLogic">
    ///     A boolean flag indicating whether to apply custom logic for detecting relationships based on
    ///     naming conventions. If set to <c>true</c>, the method will look for relationships inferred from
    ///     property names that match the specified foreign key suffix.
    /// </param>
    /// <param name="foreignKeySuffix">
    ///     The suffix used to identify foreign key properties when <paramref name="useCustomLogic" /> is enabled.
    ///     Properties with names ending in this suffix will be considered as potential foreign keys.
    /// </param>
    /// <param name="primaryKeyName">
    ///     The name of the primary key property used to establish relationships when
    ///     <paramref name="useCustomLogic" /> is enabled. This identifies the property in related entities
    ///     that foreign keys will reference.
    /// </param>
    /// <returns>
    ///     A tuple containing:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Entity" /> objects representing classes within the specified namespace,
    ///                 with their properties collected as attributes.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A list of <see cref="Relationship" /> objects representing relationships between classes.
    ///                 This includes inheritance relationships, where the base class is specified as "FromEntity"
    ///                 and the derived class as "ToEntity". Additional relationships may be inferred based on
    ///                 custom logic if <paramref name="useCustomLogic" /> is enabled.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </returns>
    public (List<Entity> Entities, List<Relationship> Relationships) AnalyzeNamespace(
        string namespaceName,
        bool useCustomLogic = false,
        string foreignKeySuffix = "Id",
        string primaryKeyName = "Id")
    {
        var entities = new List<Entity>();
        var relationships = new List<Relationship>();
        var entityMap = new Dictionary<string, Entity>();

        // Step 1: Collect all entities in the specified namespace
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = loadedAssemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && t.Namespace == namespaceName)
            .ToList();

        CollectEntitiesAndEnums(types, entities, entityMap);
        EstablishRelationships(types, relationships, entityMap, useCustomLogic, foreignKeySuffix);

        return (entities, relationships);
    }

    /// <summary>
    ///     Collects all class and enum entities from the specified types.
    /// </summary>
    /// <param name="types">The collection of types to analyze.</param>
    /// <param name="entities">The list to store the collected entities.</param>
    /// <param name="entityMap">The dictionary to map entity names to entities for easy lookup.</param>
    private static void CollectEntitiesAndEnums(IEnumerable<Type> types, List<Entity> entities,
        Dictionary<string, Entity> entityMap)
    {
        foreach (var type in types)
        {
            if (type.IsClass && !entityMap.ContainsKey(type.Name))
            {
                var entity = CreateEntityFromClass(type);
                entities.Add(entity);
                entityMap[type.Name] = entity;
            }
            else if (type.IsEnum && !entityMap.ContainsKey(type.Name))
            {
                var entity = CreateEntityFromEnum(type);
                entities.Add(entity);
                entityMap[type.Name] = entity;
            }
        }
    }

    /// <summary>
    ///     Creates an entity from a class type.
    /// </summary>
    private static Entity CreateEntityFromClass(Type type)
    {
        var attributes = new Dictionary<string, object>();

        foreach (var prop in type.GetProperties())
        {
            var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // Handle generic types like List<T>
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = propertyType.GetGenericArguments().FirstOrDefault();
                attributes[prop.Name] = new Dictionary<string, object>
                {
                    { "Type", $"List<{genericType?.Name ?? "Unknown"}>" }
                };
            }
            else
            {
                attributes[prop.Name] = new Dictionary<string, object>
                {
                    { "Type", propertyType.Name }
                };
            }
        }

        return new Entity(type.Name, attributes);
    }

    /// <summary>
    ///     Creates an entity from an enum type.
    /// </summary>
    private static Entity CreateEntityFromEnum(Type type)
    {
        var enumValues = Enum.GetNames(type);
        var attributes = new Dictionary<string, object>
        {
            { "Values", enumValues }
        };

        return new Entity(type.Name, attributes);
    }

    /// <summary>
    ///     Establishes relationships between entities in the assembly.
    /// </summary>
    private static void EstablishRelationships(
        IEnumerable<Type> types,
        List<Relationship> relationships,
        Dictionary<string, Entity> entityMap,
        bool useCustomLogic,
        string foreignKeySuffix)
    {
        foreach (var type in types.Where(t => t.IsClass))
        {
            CreateForeignKeyRelationships(type, relationships, entityMap, useCustomLogic, foreignKeySuffix);
            CreateInheritanceRelationships(type, relationships);
        }
    }

    /// <summary>
    ///     Creates foreign key relationships based on custom logic.
    /// </summary>
    private static void CreateForeignKeyRelationships(
        Type type,
        List<Relationship> relationships,
        Dictionary<string, Entity> entityMap,
        bool useCustomLogic,
        string foreignKeySuffix)
    {
        foreach (var prop in type.GetProperties())
        {
            // Check for foreign key relationships
            if (useCustomLogic && prop.Name.EndsWith(foreignKeySuffix) &&
                (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int)))
            {
                var relatedEntityName = prop.Name[..^foreignKeySuffix.Length];
                if (entityMap.ContainsKey(relatedEntityName))
                {
                    relationships.Add(new Relationship
                    {
                        FromEntity = type.Name,
                        ToEntity = relatedEntityName,
                        Key = "references"
                    });
                }
            }

            // Check for navigation properties (e.g., List<T>)
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                if (genericType != null && entityMap.ContainsKey(genericType.Name))
                {
                    relationships.Add(new Relationship
                    {
                        FromEntity = type.Name,
                        ToEntity = genericType.Name,
                        Key = "has_children"
                    });
                }
            }

            // Check for enum relationships
            if (!prop.PropertyType.IsEnum) continue;
            
            var enumTypeName = prop.PropertyType.Name;
            if (entityMap.ContainsKey(enumTypeName))
            {
                relationships.Add(new Relationship
                {
                    FromEntity = type.Name,
                    ToEntity = enumTypeName,
                    Key = "enum"
                });
            }
        }
    }

    /// <summary>
    ///     Creates inheritance relationships between classes.
    /// </summary>
    private static void CreateInheritanceRelationships(Type type, List<Relationship> relationships)
    {
        if (type.BaseType != null && type.BaseType != typeof(object))
            relationships.Add(new Relationship
            {
                FromEntity = type.BaseType.Name,
                ToEntity = type.Name,
                Key = "inherits"
            });
    }
}