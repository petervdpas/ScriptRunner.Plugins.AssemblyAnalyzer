---
Title: Exploring Assemblies with AssemblyAnalyzer  
Subtitle: Extract Entities and Relationships Dynamically  
Category: Cookbook  
Author: Peter van de Pas  
Keywords: [CookBook, AssemblyAnalyzer, Reflection, Entities, Relationships]  
Table-use-row-colors: true  
Table-row-color: "D3D3D3"  
Toc: true  
Toc-title: Table of Contents  
Toc-own-page: true
---

# Recipe: Exploring Assemblies with AssemblyAnalyzer

## Goal

Use the **AssemblyAnalyzer Plugin** to dynamically explore assemblies and extract information about entities and their
relationships.
Learn how to initialize the analyzer, analyze assemblies or namespaces, and interpret results.

---

## Steps

### Step 1: Initialize the AssemblyAnalyzer

Create an instance of the **AssemblyAnalyzer** to start analyzing assemblies or namespaces:

```csharp
var analyzer = new AssemblyAnalyzer();
Dump("AssemblyAnalyzer initialized.");
```

---

### Step 2: Analyze a Namespace

Analyze a specific namespace within the currently loaded assemblies to find all classes and their relationships:

```csharp
var namespaceToAnalyze = "ScriptRunner.Plugins.AssemblyAnalyzer.Models";
Dump($"Analyzing namespace: {namespaceToAnalyze}...");

var result = analyzer.AnalyzeNamespace(namespaceToAnalyze, true);

Dump("Namespace analysis completed.");
```

---

### Step 3: Display Detected Entities

Loop through the detected entities to understand their structure and attributes:

```csharp
Dump("Entities detected:");
foreach (var entity in result.Entities)
{
    Dump($"Entity Name: {entity.Name}");
    foreach (var (key, value) in entity.Attributes)
    {
        Dump($"  - {key}: {value}");
    }
}
```

---

### Step 4: Display Detected Relationships

Loop through the detected relationships to understand connections between entities:

```csharp
Dump("Relationships detected:");
foreach (var relationship in result.Relationships)
{
    Dump($"From: {relationship.FromEntity}, To: {relationship.ToEntity}, Key: {relationship.Key}");
}
```

---

### Step 5: Analyze an Assembly File

Analyze an entire assembly from a file path for entities and relationships:

```csharp
var assemblyPath = "path/to/your/assembly.dll";
Dump($"Analyzing assembly: {assemblyPath}...");

var assemblyResult = analyzer.AnalyzeAssembly(assemblyPath, true);

Dump("Assembly analysis completed.");
```

---

### Step 6: Understand Foreign Key Relationships

Use the custom logic flag to detect relationships based on naming conventions
(e.g., **ParentId** indicating a foreign key):

```csharp
var namespaceWithFK = "ScriptRunner.Plugins.AssemblyAnalyzer.Models";
Dump($"Analyzing namespace with custom foreign key logic: {namespaceWithFK}...");

var fkResult = analyzer.AnalyzeNamespace(namespaceWithFK, true, "Id", "ParentEntityId");

Dump("Foreign key analysis completed.");
```

---

### Step 7: Debugging and Logging

Use debugging techniques to ensure all assemblies are loaded and type resolution is correct:

```csharp
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    Dump($"Loaded Assembly: {assembly.FullName}");
}
```

---

## Example Output

After analyzing a namespace, you may see output like this:

### **Entities Detected**

```bash
Entity Name: TestParentEntity
Attributes:
  - Id: Int32
  - Name: String
  - Children: List<TestChildEntity>

Entity Name: TestChildEntity
Attributes:
  - Id: Int32
  - Name: String
  - ParentEntityId: Int32
```

### **Relationships Detected**

```bash
From: TestParentEntity, To: TestChildEntity, Key: has_children
From: TestChildEntity, To: TestParentEntity, Key: references
```

---

## Summary

The **AssemblyAnalyzer Plugin** is a powerful tool for dynamically extracting entities and their relationships
from assemblies.
By using its capabilities, you can gain insights into the structure of your code and leverage
this information for tasks like validation, documentation, or automation.

---