using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TRANSLATING2.Parsing;

namespace TRANSLATING2
{
    public partial class Form1 : Form
    {
        private StringBuilder sourceCodeBuilder;
        private string selectedJsonFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Open Json Diagram File",
                Filter = "Json Diagram Files|*.json"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedJsonFilePath = dialog.FileName;
                string displayJson = File.ReadAllText(selectedJsonFilePath);
                textBox2.Text = displayJson;
                textBox1.Text = dialog.FileName;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedJsonFilePath = null;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedJsonFilePath) && File.Exists(selectedJsonFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "C Files|*.c";
                saveFileDialog.Title = "Save C File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {
                            sw.Write(textBox3.Text);
                        }

                        MessageBox.Show("File successfully saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please generate C code before exporting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHitung_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedJsonFilePath) && File.Exists(selectedJsonFilePath))
                {
                    sourceCodeBuilder = new StringBuilder();
                    GenerateCCode(selectedJsonFilePath);
                    textBox3.Text = sourceCodeBuilder.ToString();
                }
                else
                {
                    MessageBox.Show("Please select a valid JSON file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating C code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnParsing_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedJsonFilePath))
                {
                    MessageBox.Show("Please select a JSON file first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                JArray jsonArray = this.ProcessJson(new string[] { selectedJsonFilePath });

                textBox3.Clear(); // Clear TextBox3 before appending new results

                CheckParsing15.Point1(this, jsonArray);
                CheckParsing15.Point2(this, jsonArray);
                CheckParsing15.Point3(this, jsonArray);
                CheckParsing15.Point4(this, jsonArray);
                CheckParsing15.Point5(this, jsonArray);
                CheckParsing610.Point6(this, jsonArray);
                CheckParsing610.Point7(this, jsonArray);
                CheckParsing610.Point8(this, jsonArray);
                CheckParsing610.Point9(this, jsonArray);
                CheckParsing610.Point10(this, jsonArray);
                CheckParsing1115.Point11(this, jsonArray);
                CheckParsing1115.Point13(this, jsonArray);
                CheckParsing1115.Point14(this, jsonArray);
                CheckParsing1115.Point15(this, jsonArray);

                ParsingPoint.Point25(this, jsonArray);
                ParsingPoint.Point27(this, jsonArray);
                ParsingPoint.Point28(this, jsonArray);
                ParsingPoint.Point29(this, jsonArray);
                ParsingPoint.Point30(this, jsonArray);
                ParsingPoint.Point34(this, jsonArray);
                ParsingPoint.Point35(this, jsonArray);

                CheckParsing1115.Point99(this, jsonArray);


                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    textBox3.Text = "Model has successfully passed parsing";
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public RichTextBox GetMessageBox()
        {
            return textBox3;
        }
        private JArray ProcessJson(string[] fileNames)
        {
            List<string> jsonArrayList = new List<string>();

            foreach (var fileName in fileNames)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileName);
                    jsonArrayList.Add(jsonContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading the file {Path.GetFileName(fileName)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

            JArray jsonArray = new JArray(jsonArrayList.Select(JToken.Parse));

            textBox2.Text = jsonArray.ToString();
            return jsonArray;
        }
        private void CheckJsonCompliance(string jsonContent)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonContent);

                // Dictionary to store state model information
                Dictionary<string, string> stateModels = new Dictionary<string, string>();
                HashSet<string> usedKeyLetters = new HashSet<string>();
                HashSet<int> stateNumbers = new HashSet<int>();

                JToken subsystemsToken = jsonObj["subsystems"];
                if (subsystemsToken != null && subsystemsToken.Type == JTokenType.Array)
                {
                    // Iterasi untuk setiap subsystem dalam subsystemsToken
                    foreach (var subsystem in subsystemsToken)
                    {
                        JToken modelToken = subsystem["model"];
                        if (modelToken != null && modelToken.Type == JTokenType.Array)
                        {
                            foreach (var model in modelToken)
                            {
                                ValidateClassModel(model, stateModels, usedKeyLetters, stateNumbers);
                            }
                        }
                    }

                    // Setelah memvalidasi semua model, panggil ValidateEventDirectedToStateModelHelper untuk setiap subsystem
                    foreach (var subsystem in subsystemsToken)
                    {
                        ValidateEventDirectedToStateModelHelper(subsystem["model"], stateModels, null);
                    }
                }

                ValidateTimerModel(jsonObj, usedKeyLetters);
            }
            catch (Exception ex)
            {
                HandleError($"Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateClassModel(JToken model, Dictionary<string, string> stateModels, HashSet<string> usedKeyLetters, HashSet<int> stateNumbers)
        {
            string objectType = model["type"]?.ToString();
            string objectName = model["class_name"]?.ToString();
            Console.WriteLine($"Running CheckKeyLetterUniqueness for {objectName}");

            if (objectType == "class")
            {
                Console.WriteLine($"Checking class: {objectName}");

                string assignerStateModelName = $"{objectName}_ASSIGNER";
                JToken assignerStateModelToken = model[assignerStateModelName];

                if (assignerStateModelToken == null || assignerStateModelToken.Type != JTokenType.Object)
                {
                    HandleError($"Syntax error 16: Assigner state model not found for {objectName}.");
                    return;
                }

                string keyLetter = model["KL"]?.ToString();

                // Pemanggilan CheckKeyLetterUniqueness
                CheckKeyLetterUniqueness(usedKeyLetters, keyLetter, objectName);

                // Check if KeyLetter is correct
                JToken keyLetterToken = assignerStateModelToken?["KeyLetter"];
                if (keyLetterToken != null && keyLetterToken.ToString() != keyLetter)
                {
                    HandleError($"Syntax error 17: KeyLetter for {objectName} does not match the rules.");
                }

                // Check uniqueness of states
                CheckStateUniqueness(stateModels, assignerStateModelToken?["states"], objectName, assignerStateModelName);

                // Check uniqueness of state numbers
                CheckStateNumberUniqueness(stateNumbers, assignerStateModelToken?["states"], objectName);

                // Store state model information
                string stateModelKey = $"{objectName}.{assignerStateModelName}";
                stateModels[stateModelKey] = objectName;
            }
        }

        private void CheckStateUniqueness(Dictionary<string, string> stateModels, JToken statesToken, string objectName, string assignerStateModelName)
        {
            if (statesToken is JArray states)
            {
                HashSet<string> uniqueStates = new HashSet<string>();

                foreach (var state in states)
                {
                    string stateName = state["state_name"]?.ToString();
                    string stateModelName = $"{objectName}.{stateName}";
                    if (!uniqueStates.Add(stateModelName))
                    {
                        HandleError($"Syntax error 18: State {stateModelName} is not unique in {assignerStateModelName}.");
                    }
                }
            }
        }

        private void CheckStateNumberUniqueness(HashSet<int> stateNumbers, JToken statesToken, string objectName)
        {
            if (statesToken is JArray stateArray)
            {
                foreach (var state in stateArray)
                {
                    int stateNumber = state["state_number"]?.ToObject<int>() ?? 0;

                    if (!stateNumbers.Add(stateNumber))
                    {
                        HandleError($"Syntax error 19: State number {stateNumber} is not unique.");
                    }
                }
            }
        }

        private void CheckKeyLetterUniqueness(HashSet<string> usedKeyLetters, string keyLetter, string objectName)
        {
            string expectedKeyLetter = $"{keyLetter}_A";
            Console.WriteLine("Running ValidateClassModel");
            Console.WriteLine($"Checking KeyLetter uniqueness: {expectedKeyLetter} for {objectName}");

            if (!usedKeyLetters.Add(expectedKeyLetter))
            {
                HandleError($"Syntax error 20: KeyLetter for {objectName} is not unique.");
            }
        }

        private void ValidateTimerModel(JObject jsonObj, HashSet<string> usedKeyLetters)
        {
            string timerKeyLetter = jsonObj["subsystems"]?[0]?["model"]?[0]?["KL"]?.ToString();
            string timerStateModelName = $"{timerKeyLetter}_ASSIGNER";

            JToken timerModelToken = jsonObj["subsystems"]?[0]?["model"]?[0];
            JToken timerStateModelToken = jsonObj["subsystems"]?[0]?["model"]?[0]?[timerStateModelName];

            // Check if Timer state model exists
            if (timerStateModelToken == null || timerStateModelToken.Type != JTokenType.Object)
            {
                HandleError($"Syntax error 21: Timer state model not found for TIMER.");
                return;
            }

            // Check KeyLetter of Timer state model
            JToken keyLetterToken = timerStateModelToken?["KeyLetter"];
            if (keyLetterToken == null || keyLetterToken.ToString() != timerKeyLetter)
            {
                HandleError($"Syntax error 21: KeyLetter for TIMER does not match the rules.");
            }
        }

        private void ValidateEventDirectedToStateModelHelper(JToken modelsToken, Dictionary<string, string> stateModels, string modelName)
        {
            if (modelsToken != null && modelsToken.Type == JTokenType.Array)
            {
                foreach (var model in modelsToken)
                {
                    string modelType = model["type"]?.ToString();
                    string className = model["class_name"]?.ToString();

                    if (modelType == "class")
                    {
                        JToken assignerToken = model[$"{className}_ASSIGNER"];

                        if (assignerToken != null)
                        {
                            Console.WriteLine($"assignerToken.Type: {assignerToken.Type}");

                            if (assignerToken.Type == JTokenType.Object)
                            {
                                JToken statesToken = assignerToken["states"];

                                if (statesToken != null && statesToken.Type == JTokenType.Array)
                                {
                                    JArray statesArray = (JArray)statesToken;

                                    foreach (var stateItem in statesArray)
                                    {
                                        string stateName = stateItem["state_name"]?.ToString();
                                        string stateModelName = $"{modelName}.{stateName}";

                                        JToken eventsToken = stateItem["events"];
                                        if (eventsToken is JArray events)
                                        {
                                            foreach (var evt in events)
                                            {
                                                string eventName = evt["event_name"]?.ToString();
                                                JToken targetsToken = evt["targets"];

                                                if (targetsToken is JArray targets)
                                                {
                                                    foreach (var target in targets)
                                                    {
                                                        string targetStateModel = target?.ToString();

                                                        // Check if target state model is in the state models dictionary
                                                        if (!stateModels.ContainsKey(targetStateModel))
                                                        {
                                                            HandleError($"Syntax error 24: Event '{eventName}' in state '{stateModelName}' targets non-existent state model '{targetStateModel}'.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void HandleError(string errorMessage)
        {
            textBox3.Text += $"{errorMessage}{Environment.NewLine}";
            Console.WriteLine(errorMessage);
        }
        private void GenerateCCode(string jsonFilePath)
        {
            try
            {
                string umlDiagramJson = File.ReadAllText(jsonFilePath);
                JsonData json = JsonConvert.DeserializeObject<JsonData>(umlDiagramJson);

                GenerateStructsAndAssociations(json);
                GenerateStateEnumsAndStructs(json);
                GenerateEnums(json);
                GenerateStateTransitions(json);

                CloseNamespace();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   
        private void GenerateNamespace(string namespaceName)
        {
            sourceCodeBuilder.AppendLine("#include <stdio.h>");
            sourceCodeBuilder.AppendLine("#include <string.h>");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"namespace {namespaceName}");
            sourceCodeBuilder.AppendLine("{");
        }
        private void GenerateStructsAndAssociations(JsonData json)
        {
            GenerateNamespace(json.sub_name);

            foreach (var model in json.model)
            {
                switch (model.type)
                {
                    case "class":
                        GenerateClassStruct(model);
                        break;                       
                    case "association":
                        GenerateAssociationStruct(model);
                        break;
                    case "imported_class":
                        GenerateImportedClassStruct(model);
                        break;
                }
            }
        }
        private void GenerateClassStruct(JsonData.Model classModel)
        {
            string className = classModel.class_name;
            sourceCodeBuilder.AppendLine($"struct {className} {{");

            GenerateAttributes(classModel);
            GenerateStatesEnum(classModel);

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();

            //GenerateStateFunctions(classModel);
        }
        private void GenerateAttributes(JsonData.Model classModel)
        {
            foreach (var attribute in classModel.attributes)
            {
                string attributeType = MapDataType(attribute.data_type.ToLower());

                if (attribute.data_type.ToLower() == "string")
                {
                    sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name}[100];");
                }
                else
                {
                    sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name};");
                }
            }
        }

        private void GenerateStatesEnum(JsonData.Model classModel)
        {
            if (classModel.states != null && classModel.states.Count > 0)
            {
                sourceCodeBuilder.AppendLine($"    enum {classModel.class_name}State;");
            }
        }
        private void GenerateEventsEnums(JsonData.Model classModel)
        {
            if (classModel.attributes != null)
            {
                foreach (var @event in classModel.attributes)
                {
                    if (@event != null && @event.event_name != null)
                    {
                        string eventType = MapDataType(@event.data_type);

                        // Check if it's an event attribute and generate the event struct
                        if (@event.attribute_type == "inst_event")
                        {
                            // Skip inst_event declaration
                        }
                        else
                        {
                            sourceCodeBuilder.AppendLine($"enum {classModel.class_name}_{@event.event_name.ToUpper()} {{");
                            sourceCodeBuilder.AppendLine($"    {eventType} {@event.event_name};");
                            sourceCodeBuilder.AppendLine("};");
                            sourceCodeBuilder.AppendLine();
                        }
                    }
                }
            }
        }

        private void GenerateEnums(JsonData json)
        {
            Dictionary<string, List<string>> statesByClass = new Dictionary<string, List<string>>();

            // Group states by class
            foreach (var model in json.model)
            {
                if (model.type == "class" && model.states != null && model.states.Count > 0)
                {
                    List<string> stateNames = model.states.Select(state => state.state_name.ToUpper().Replace(' ', '_')).ToList();
                    statesByClass[model.class_name] = stateNames;

                    // Check if attributes is not null before iterating
                    if (model.attributes != null)
                    {
                        List<string> eventNames = model.attributes
                            .Where(attribute => attribute.event_name != null)
                            .Select(attribute => attribute.event_name.ToUpper().Replace(' ', '_'))
                            .ToList();

                        statesByClass[model.class_name].AddRange(eventNames);
                    }
                }
            }

            // Generate enums for states
            GenerateEnumsForStates(statesByClass);

            // Generate enums for event attributes
            foreach (var model in json.model)
            {
                GenerateEventsEnums(model);
            }
            // Generate enums for state events
            foreach (var model in json.model)
            {
                GenerateEnumsForStateEvents(model);
            }
        }
        private void GenerateEnumsForStates(Dictionary<string, List<string>> statesByClass)
        {
            foreach (var className in statesByClass.Keys)
            {
                sourceCodeBuilder.AppendLine($"enum {className}State {{");

                // Enum members for states
                foreach (var stateName in statesByClass[className])
                {
                    sourceCodeBuilder.AppendLine($"    {stateName},");
                }

                sourceCodeBuilder.AppendLine("};");
                sourceCodeBuilder.AppendLine();
            }
        }

        private void GenerateEnumsForStateEvents(JsonData.Model model)
        {
            if (model.type == "class" && model.states != null && model.states.Count > 0)
            {
                foreach (var state in model.states)
                {
                    // Enum for state events
                    if (state.state_event != null && state.state_event.Length > 0)
                    {
                        string enumName = $"{model.class_name}StateEvent_{state.state_name}";
                        sourceCodeBuilder.AppendLine($"enum {enumName} {{");

                        foreach (var eventName in state.state_event)
                        {
                            if (eventName != null)
                            {
                                sourceCodeBuilder.AppendLine($"    {eventName.ToUpper().Replace(' ', '_')},");
                            }
                        }

                        sourceCodeBuilder.AppendLine("};");
                        sourceCodeBuilder.AppendLine();
                    }
                }
            }
        }
        private void GenerateStateFunctions(JsonData.Model classModel)
        {
            if (classModel.states != null)
            {
                foreach (var state in classModel.states)
                {
                    if (state.state_function != null)
                    {
                        foreach (var function in state.state_function)
                        {
                            // Generate state function implementation
                            sourceCodeBuilder.AppendLine($"void {classModel.class_name}StateFunctions::{function}() {{");

                            // Implement the logic based on the state details
                            if (state.state_type.ToLower() == "string")
                            {
                                sourceCodeBuilder.AppendLine($"    strcpy({classModel.class_name}.{state.state_name}, \"{state.state_value}\");");
                            }
                            else
                            {
                                // Handle other types if needed
                                sourceCodeBuilder.AppendLine($"    // TODO: Implement logic for {function} with type {state.state_type}");
                            }

                            sourceCodeBuilder.AppendLine("}");
                        }
                    }
                }
            }
        }
        private void GenerateAssociationStruct(JsonData.Model associationModel)
        {
            if (associationModel.@class.Count == 2)
            {
                var class1 = associationModel.@class[0];
                var class2 = associationModel.@class[1];

                string class1Multiplicity = class1.class_multiplicity;
                string class2Multiplicity = class2.class_multiplicity;

            }

            if (associationModel.model != null)
            {
                GenerateAssociationClassStruct(associationModel.model, associationModel.name);
            }
        }
        private void GenerateAssociationClassStruct(JsonData.Model associationClassModel, string associationName)
        {
            string classType = MapDataType(associationClassModel.class_name.ToLower());
            sourceCodeBuilder.AppendLine($"struct {associationClassModel.class_name} {{");

            foreach (var attribute in associationClassModel.attributes)
            {
                string attributeType = MapDataType(attribute.data_type.ToLower());
                sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name};");
            }

          
            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();
        }
        private void GenerateImportedClassStruct(JsonData.Model importedClassModel)
        {
            string className = importedClassModel.class_name;

            sourceCodeBuilder.AppendLine($"struct {className} {{");

            foreach (var attribute in importedClassModel.attributes)
            {
                string attributeType = MapDataType(attribute.data_type.ToLower());
                string arraySuffix = attributeType.EndsWith("[]") ? "" : "[]";

                sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name}{arraySuffix};");
            }

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();
        }
        private void GenerateStateEnumsAndStructs(JsonData json)
        {
            foreach (var model in json.model)
            {
                if (model.type == "state" && model.states != null && model.states.Count > 0)
                {
                    GenerateStateEnums(model);
                    GenerateStateStructs(model);
                }
            }
        }

        private void GenerateStateEnums(JsonData.Model classModel)
        {
            string className = classModel.class_name;
            string enumPrefix = $"{className.ToUpper()}_";

            sourceCodeBuilder.AppendLine($"enum {enumPrefix}State {{");

            foreach (var state in classModel.states)
            {
                sourceCodeBuilder.AppendLine($"    {enumPrefix}{state.state_name.ToUpper().Replace(' ', '_')},");
            }

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();
        }

        private void GenerateStateStructs(JsonData.Model stateModel)
        {
            foreach (var state in stateModel.states)
            {
                string stateName = state.state_name;
                string className = stateModel.class_name;

                sourceCodeBuilder.AppendLine($"struct {className}_{stateName} {{");

                foreach (var attribute in stateModel.attributes)
                {
                    string attributeType = MapDataType(attribute.data_type.ToLower());
                    sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name};");
                }

                // Handle events
                if (state.state_event != null && state.state_event.Length > 0)
                {
                    foreach (var eventName in state.state_event)
                    {
                        sourceCodeBuilder.AppendLine($"    {MapDataType(eventName)} {eventName};");
                    }
                }

                // Handle transitions
                foreach (var transition in state.transitions)
                {
                    string targetStateId = transition.target_state_id;
                    string targetState = transition.target_state.Replace(' ', '_').ToUpper();

                    sourceCodeBuilder.AppendLine($"    {targetStateId} {targetState};");
                }

                // Handle action
                sourceCodeBuilder.AppendLine($"    // Action logic for {stateName}");
                sourceCodeBuilder.AppendLine($"    {state.action}");

                sourceCodeBuilder.AppendLine("};");
                sourceCodeBuilder.AppendLine();
            }
        }
        private void GenerateStateTransitions(JsonData json)
        {
            foreach (var model in json.model)
            {
                if (model.type == "state" && model.states != null && model.states.Count > 0)
                {
                    foreach (var state in model.states)
                    {
                        GenerateTransitionEnum(model.class_name, state);
                    }
                }
            }
        }
        private void GenerateTransitionEnum(string className, JsonData.State state)
        {
            if (state.transitions != null && state.transitions.Count > 0)
            {
                string enumName = $"{className}StateTransition_{state.state_name}";
                sourceCodeBuilder.AppendLine($"enum {enumName} {{");

                foreach (var transition in state.transitions)
                {
                    // Logika untuk menangani target_state_id dan target_state
                    string targetStateId = transition.target_state_id;
                    string targetState = transition.target_state.Replace(' ', '_').ToUpper();

                    sourceCodeBuilder.AppendLine($"    {targetStateId},");
                    sourceCodeBuilder.AppendLine($"    {targetState},");
                }

                sourceCodeBuilder.AppendLine("};");
                sourceCodeBuilder.AppendLine();
            }
        }

        private void CloseNamespace()
        {
            sourceCodeBuilder.AppendLine("}");
        }

        private string MapDataType(string dataType)
        {
            switch (dataType)
            {
                case "id":
                    return "char";
                case "state":
                    return "char";
                case "string":
                    return "char";
                case "integer":
                    return "int";
                case "real":
                    return "float";
                default:
                    return dataType; // For unknown types, just pass through
            }
        }
        public class JsonData
        {
            public string type { get; set; }
            public string sub_id { get; set; }
            public string sub_name { get; set; }
            public List<Model> model { get; set; }

            public class Model
            {
                public string type { get; set; }
                public string class_id { get; set; }
                public string class_name { get; set; }
                public string KL { get; set; }
                public string name { get; set; }
                public List<Attribute1> attributes { get; set; }
                public List<State> states { get; set; }
                public Model model { get; set; }
                public List<Class1> @class { get; set; }
                

            }
            public class State
            {
                public string state_id { get; set; }
                public string state_name { get; set; }
                public string state_value { get; set; }
                public string state_type { get; set; }
                public string[] state_event { get; set; }
                public string[] state_function { get; set; }
                public string[] state_transition_id { get; set; }
                public List<Transition> transitions { get; set; }
                public string action { get; set; }

            }
            public class Attribute1
            {
                public string attribute_name { get; set; }
                public string data_type { get; set; }
                public string default_value { get; set; }
                public string attribute_type { get; set; }
                public string event_id { get; set; }
                public string event_name { get; set; }
                public string class_id { get; set; }
                public string state_id { get; set; }
                public string state_name { get; set; }
            }

            public class Class1
            {
                public string class_name { get; set; }
                public string class_multiplicity { get; set; }
                public List<Attribute> attributes { get; set; }
                public string type { get; internal set; }
            }

            public class Attribute
            {
                public string attribute_name { get; set; }
                public string data_type { get; set; }
                public string attribute_type { get; set; }
                public string default_value { get; set; }
            }
            public class Transition
            {
                public string target_state_id { get; set; }
                public string target_state { get; set; }
            }
        }

        private void btnCJSON_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedJsonFilePath) && File.Exists(selectedJsonFilePath))
            {
                try
                {
                    // Read the JSON file content
                    string jsonContent = File.ReadAllText(selectedJsonFilePath);

                    // Copy the JSON content to the clipboard
                    Clipboard.SetText(jsonContent);

                    MessageBox.Show("JSON content copied to clipboard.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying JSON content: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid JSON file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCCODE_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text))
            {
                try
                {
                    // Copy the generated code to the clipboard
                    Clipboard.SetText(textBox3.Text);

                    MessageBox.Show("Generated code copied to clipboard.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying generated code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please generate C code before copying.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = "This Application is to generate file JSON to C programming language.\n\n" +

                         "1. Click 'Browse' to select a file JSON.\n" +
                         "2. Click 'Parsing' to check JSON data.\n" +
                         "Interpret Checking Results.\n" +
                         "   - Review the checking results in the output text box.\n" +
                         "   - If there are errors, error messages will be provided to guide the corrections.\n" +
                         "3. Click 'Simulate' to simulation this program\n" +
                         "4. Click Visualize to visualization all data\n" +
                         "5. Click 'Translate' to translate file JSON to C programming language.\n" +
                         "6. Click 'Clear' to clear all data that been upload and translate\n" +
                         "7. Click 'Copy Json' to copy JSON data in textbox\n" +
                         "8. Click 'Copy Code' to copy C code in textbox\n" +
                         "9. Click 'Export' to save the translated data in a C programming language\n";



            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnVizualize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {

            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;

        }
    }
}
