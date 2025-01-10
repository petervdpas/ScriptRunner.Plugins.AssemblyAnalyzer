/*
{
    "TaskCategory": "Plugins",
    "TaskName": "AssemblyAnalyzer Plugin",
    "TaskDetail": "Analyze TestParentEntity and TestChildEntity classes in the namespace and display entities and relationships.",
    "RequiredPlugins": ["AssemblyAnalyzer"]
}
*/

var analyzer = new AssemblyAnalyzer();
var result = analyzer.AnalyzeNamespace("ScriptRunner.Plugins.AssemblyAnalyzer.Models", true);

// Deduplicate entities by their name
var distinctEntities = result.Entities.GroupBy(e => e.Name).Select(g => g.First()).ToList();

// Dump entities
Dump("Entities detected:");
foreach (var entity in distinctEntities)
{
    Dump($"Entity Name: {entity.Name}");
    Dump("Attributes:");
    foreach (var attribute in entity.Attributes)
    {
        Dump($"- {attribute.Key}:");

        if (attribute.Value is Dictionary<string, object> attributeDetails)
        {
            if (attributeDetails.TryGetValue("Type", out var typeValue) && typeValue is string typeString)
            {
                // Safely handle collection types like List<T>
                if (typeString.StartsWith("List"))
                {
                    string genericType = "Unknown";
                    int startIndex = typeString.IndexOf('[');
                    int endIndex = typeString.IndexOf(']');

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        genericType = typeString.Substring(startIndex + 1, endIndex - startIndex - 1);
                    }

                    Dump($"  Type: List<{genericType}>");
                }
                else
                {
                    Dump($"  Type: {typeString}");
                }
            }
            else
            {
                foreach (var detail in attributeDetails)
                {
                    Dump($"  {detail.Key}: {detail.Value}");
                }
            }
        }
        else
        {
            Dump($"  Value: {attribute.Value}");
        }
    }
    Dump(""); // Blank line for separation
}

// Dump relationships
Dump("Relationships detected:");
if (result.Relationships.Any())
{
    foreach (var relationship in result.Relationships)
    {
        Dump($"From: {relationship.FromEntity}, To: {relationship.ToEntity}, Key: {relationship.Key}");
    }
}
else
{
    Dump("No relationships detected.");
}

return "Task completed successfully with dynamic classes.";
