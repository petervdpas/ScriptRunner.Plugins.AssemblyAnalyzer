# ScriptRunner.Plugins.AssemblyAnalyzer

![License](https://img.shields.io/badge/license-MIT-green)  
![Version](https://img.shields.io/badge/version-1.0.0-blue)

A robust plugin for **ScriptRunner** designed to analyze assemblies and extract detailed information about entities and their relationships, including inheritance hierarchies and property attributes. Ideal for developers needing insights into assemblies or generating documentation.

---

## 🚀 Features

- **Entity Extraction**: Identify classes, enums, and their properties from assemblies.
- **Relationship Analysis**: Detect inheritance hierarchies, foreign key relationships, and more.
- **Namespace Filtering**: Analyze entities within a specific namespace.
- **Custom Logic Support**: Leverage custom naming conventions for detecting foreign key relationships.
- **ScriptRunner Integration**: Seamlessly integrates into the ScriptRunner ecosystem.

---

## 📦 Installation

### Plugin Activation
Place the `ScriptRunner.Plugins.AssemblyAnalyzer` plugin assembly in the `Plugins` folder of your ScriptRunner project. The plugin will be automatically discovered and activated.

---

## 📖 Usage

### Writing a Script

Here’s an example script that analyzes a specific assembly:

```csharp
/*
{
    "TaskCategory": "Analysis",
    "TaskName": "Analyze Assembly",
    "TaskDetail": "This script analyzes an assembly to extract entities and their relationships."
}
*/

var assemblyAnalyzer = PluginLoader.GetPlugin<ScriptRunner.Plugins.AssemblyAnalyzer.IAssemblyAnalyzer>();

var assemblyPath = "path/to/your/assembly.dll";
var result = assemblyAnalyzer.AnalyzeAssembly(assemblyPath, useCustomLogic: true);

Dump("Entities:", result.Entities);
Dump("Relationships:", result.Relationships);

return "Assembly analysis completed.";
```

---

## 🔧 Configuration

### Initialize the Plugin
```csharp
var configuration = new Dictionary<string, object>
{
    { "AssemblyAnalyzerKey", "optional-value" }
};

var assemblyAnalyzer = PluginLoader.GetPlugin<ScriptRunner.Plugins.AssemblyAnalyzer.IAssemblyAnalyzer>();
assemblyAnalyzer.Initialize(configuration);
```

### Custom Logic
Customize foreign key detection:
- Set `useCustomLogic` to `true`.
- Define `foreignKeySuffix` (default: `"Id"`) to identify foreign key properties.
- Define `primaryKeyName` (default: `"Id"`) to identify primary key properties.

```csharp
var result = assemblyAnalyzer.AnalyzeAssembly(
    "path/to/your/assembly.dll", 
    useCustomLogic: true, 
    foreignKeySuffix: "Key", 
    primaryKeyName: "PrimaryKey");
```

---

## 🌟 Advanced Features

### Analyze Namespace
Analyze all loaded assemblies for a specific namespace:
```csharp
var namespaceName = "MyProject.Models";
var result = assemblyAnalyzer.AnalyzeNamespace(namespaceName, useCustomLogic: true);

Dump("Entities in namespace:", result.Entities);
Dump("Relationships in namespace:", result.Relationships);
```

### Detailed Relationships
- **Inheritance**: Base and derived class hierarchies.
- **Foreign Keys**: Relationships inferred from property naming conventions.
- **Enums**: Properties referencing enum types.

---

## 🧪 Testing

- Use test assemblies with well-defined inheritance and relationships for validation.
- Ensure the `Plugins` folder contains all dependencies of the assembly being analyzed.

---

## 📄 Contributing

1. Fork this repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add YourFeature'`).
4. Push the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

---

## Author

Developed with **🧡 passion** by **Peter van de Pas**.

For questions or feedback, feel free to open an issue or contact me directly!

---

## 🔗 Links

- [ScriptRunner Plugins Repository](https://github.com/petervdpas/ScriptRunner.Plugins)

---

## License

This project is licensed under the [MIT License](./LICENSE).
