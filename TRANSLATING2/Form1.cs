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

                GenerateNamespace(json.sub_name);

                foreach (var model in json.model)
                {
                    switch (model.type)
                    {
                        case "class":
                            GenerateClassStruct(model);
                            break;
                        case "association":
                            GenerateAssociation(model);
                            break;
                        case "imported_class":
                            GenerateImportedClassStruct(model);
                            break;
                    }
                }

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
        private void GenerateClassStruct(JsonData.Model classModel)
        {
            string className = classModel.class_name;
            sourceCodeBuilder.AppendLine($"struct {className} {{");

            GenerateAttributes(classModel);
            GenerateStatesEnum(classModel);
            GenerateEventsEnums(classModel);
            //GenerateEventName(classModel); 


            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();

            //GenerateConstructor(classModel);
            GenerateStateFunctions(classModel);
            GenerateStateTransitions(classModel);
            //GenerateAttributeSetters(classModel);
            //GeneratePrintAttributeNames(classModel);
            GenerateSetter(classModel); 

        }
        private void GenerateStateTransitions(JsonData.Model classModel)
        {
            if (classModel.type == "state" && classModel.states != null && classModel.states.Count > 0)
            {
                foreach (var state in classModel.states)
                {
                    GenerateTransitionEnum(classModel.class_name, state);
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

        private void GenerateEventName(JsonData.Model classModel)
        {
            foreach (var attribute in classModel.attributes)
            {
                // Check if the 'event_name' property is not null or empty
                if (!string.IsNullOrEmpty(attribute.event_name))
                {
                    // Use 'event_name' for generating the structure and display
                    string eventName = attribute.event_name;

                    // Generate code for the special structure
                    sourceCodeBuilder.AppendLine($"    struct {eventName} {eventName};");
                }
            }
        }

        private void GenerateSetter(JsonData.Model classModel)
        {
            string className = classModel.class_name;
            foreach (var attribute in classModel.attributes)
            {
                string dataType = MapDataType(attribute.data_type);
                string attributeName = attribute.attribute_name;

                if (attribute.data_type == "state")
                {
                    // Handle state attribute separately
                    sourceCodeBuilder.AppendLine($"void set_{attributeName}(struct {className} *instance, {dataType} new_{attributeName}) {{");
                    sourceCodeBuilder.AppendLine($"    instance->{attributeName} = new_{attributeName};");
                    sourceCodeBuilder.AppendLine("}");
                    sourceCodeBuilder.AppendLine();
                }
                else if (attribute.data_type == "inst_ref")
                {
                    // Handle inst_ref attribute separately
                    sourceCodeBuilder.AppendLine($"void set_{attributeName}_ref(struct {className} *instance, struct {dataType} new_{attributeName}_ref) {{");
                    sourceCodeBuilder.AppendLine($"    instance->{attributeName}_ref = new_{attributeName}_ref;");
                    sourceCodeBuilder.AppendLine("}");
                    sourceCodeBuilder.AppendLine();
                }
                else if (attribute.data_type == "inst_ref_set")
                {
                    // Handle inst_ref_set attribute separately
                    sourceCodeBuilder.AppendLine($"void set_{attributeName}_ref_set(struct {className} *instance, struct {dataType} new_{attributeName}_ref_set) {{");
                    sourceCodeBuilder.AppendLine($"    // Implementation for adding to the ref_set");
                    sourceCodeBuilder.AppendLine("}");
                    sourceCodeBuilder.AppendLine();
                }
                else if (attribute.data_type == "inst_event")
                {
                    // Skip generating setter for inst_event
                    continue;
                }
                else
                {
                    sourceCodeBuilder.AppendLine($"void set_{attributeName}(struct {className} *instance, {dataType} new_{attributeName}) {{");
                    sourceCodeBuilder.AppendLine($"    instance->{attributeName} = new_{attributeName};");
                    sourceCodeBuilder.AppendLine("}");
                    sourceCodeBuilder.AppendLine();
                }
            }
        }
        private void GeneratePrintAttributeNames(JsonData.Model classModel)
        {
            string className = classModel.class_name;
            sourceCodeBuilder.AppendLine($"void Print{className}AttributeNames(const struct {className} *{className.ToLower()})");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"    printf(\"Attribute names for {className} class:\\n\");");

            foreach (var attribute in classModel.attributes)
            {
                sourceCodeBuilder.AppendLine($"    printf(\"- {attribute.attribute_name}\\n\");");
            }

            sourceCodeBuilder.AppendLine("}");
        }
            private void GenerateConstructor(JsonData.Model classModel)
        {
            string className = classModel.class_name;

            // Constructor signature
            sourceCodeBuilder.AppendLine($"void cons{className}(struct {className} *instance,");
            sourceCodeBuilder.AppendLine($" {GetParameterList(classModel)})");
            sourceCodeBuilder.AppendLine("{");

            // Constructor body
            foreach (var attribute in classModel.attributes)
            {
                if (attribute.attribute_type != "inst_event")
                {
                    sourceCodeBuilder.AppendLine($"    strcpy(instance->{attribute.attribute_name}, {GetAttributeFromJsonData(classModel, attribute)});");
                }
            }

            // Close constructor
            sourceCodeBuilder.AppendLine("}");
            sourceCodeBuilder.AppendLine();
        }
        private string GetParameterList(JsonData.Model classModel)
        {
            var parameters = classModel.attributes
                .Select(attribute => $"{MapDataType(attribute.data_type.ToLower())} {attribute.attribute_name}")
                .ToList();
            parameters.Add("char *status"); // Adding 'status' as an additional parameter
            return string.Join(", ", parameters);
        }
        private string GetAttributeFromJsonData(JsonData.Model classModel, JsonData.Attribute1 attribute)
        {
            // Assuming the attribute value is always available in the JSON data
            if (attribute.data_type.ToLower() == "string")
            {
                return $"\"{attribute.default_value}\"";
            }
            else
            {
                return attribute.default_value;
            }
        }
        private void GenerateAttributeSetters(JsonData.Model classModel)
        {
            if (classModel != null && classModel.attributes != null && classModel.states != null && classModel.states.Count > 0)
            {
                foreach (var attribute in classModel.attributes)
                {
                    if (attribute.attribute_type != "inst_event")
                    {
                        GenerateAttributeSetter(classModel.class_name, attribute);
                    }
                }
            }
        }

        private void GenerateAttributeSetter(string className, JsonData.Attribute1 attribute)
        {
            string attributeType = MapDataType(attribute.data_type.ToLower());

            sourceCodeBuilder.AppendLine($"void set_{attribute.attribute_name}(struct {className} *instance, {attributeType} new_{attribute.attribute_name})");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"    instance->{attribute.attribute_name} = new_{attribute.attribute_name};");
            sourceCodeBuilder.AppendLine("}");
            sourceCodeBuilder.AppendLine();
        }

        private string GetSetterLogic(string attributeType, string attributeName)
        {
            if (attributeType == "char")
            {
                return $"    strcpy(instance->{attributeName}, new_{attributeName});";
            }
            else
            {
                return $"    instance->{attributeName} = new_{attributeName};";
            }
        }
        private void GenerateAttributes(JsonData.Model classModel)
        {
            foreach (var attribute in classModel.attributes)
            {
                

                // Adjust data types as needed
                string dataType = MapDataType(attribute.data_type.ToLower());

                if (attribute.data_type != "state" && attribute.data_type != "inst_event" &&
                    attribute.data_type != "inst_ref" && attribute.data_type != "inst_ref_set" &&
                    attribute.data_type != "inst_ref_<timer>")
                {
                    sourceCodeBuilder.AppendLine($"    {dataType} {attribute.attribute_name};");
                }
                else if (attribute.data_type == "state" || attribute.data_type == "inst_ref_<timer>")
                {
                    sourceCodeBuilder.AppendLine($"    {dataType} {attribute.attribute_name};");
                }
                else if (attribute.data_type == "inst_ref")
                {
                    sourceCodeBuilder.AppendLine($"    struct {attribute.related_class_name}Ref* {attribute.attribute_name}Ref;");
                }
                else if (attribute.data_type == "inst_ref_set")
                {
                    sourceCodeBuilder.AppendLine($"    struct {attribute.related_class_name}RefSet* {attribute.attribute_name}RefSet;");
                }
            }
        }

        private void GenerateStatesEnum(JsonData.Model classModel)
        {
            if (classModel.states != null && classModel.states.Count > 0)
            {
                sourceCodeBuilder.AppendLine($"    enum {classModel.class_name}State {{");

                // Generate enum values without state names
                foreach (var state in classModel.states)
                {
                    sourceCodeBuilder.AppendLine($"        {state.state_value},");
                }

                sourceCodeBuilder.AppendLine($"    }};");
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
                        // Check if it's an event attribute with type inst_event
                        if (@event.attribute_type == "inst_event")
                        {
                            // Skip inst_event declaration
                            continue;
                        }

                        string eventType = MapDataType(@event.data_type);

                        sourceCodeBuilder.AppendLine($"    enum {classModel.class_name}_{@event.event_name.ToUpper()} {{");
                        sourceCodeBuilder.AppendLine($"        {eventType} {@event.event_name};");
                        sourceCodeBuilder.AppendLine("    };");
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
                    GenerateSetActiveFunctionLogic(classModel.class_name, state);
                }
            }
        }
        private void GenerateSetActiveFunctionLogic(string className, JsonData.State state)
        {
            // Generate set_active function logic
            string functionName = $"set_{state.state_name.ToLower()}";
            string statusValue = state.state_value.ToLower(); // Assuming state_value is already in lowercase

            sourceCodeBuilder.AppendLine($"void {functionName}(struct {className} *{className.ToLower()})");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"    strcpy({className.ToLower()}->{state.state_name}, \"{statusValue}\");");
            sourceCodeBuilder.AppendLine("}");
            sourceCodeBuilder.AppendLine();
        }
        private void GenerateAssociation(JsonData.Model associationModel)
        {
            if (associationModel.model != null)
            {
                GenerateAssociationClassStruct(associationModel.model, associationModel.name);
                GenerateAssociationSetter(associationModel.model, associationModel.name);
            }
        }

        private void GenerateAssociationSetter(JsonData.Model associatedClass, string associationName)
        {
            foreach (var attribute in associatedClass.attributes)
            {
                // Adjust data types as needed
                string dataType = MapDataType(attribute.data_type);
                sourceCodeBuilder.AppendLine($"void set_{attribute.attribute_name}(struct {attribute.attribute_name}* instance, {dataType} new_{attribute.attribute_name}) {{");
                sourceCodeBuilder.AppendLine($"    instance->{attribute.attribute_name} = new_{attribute.attribute_name};");
                sourceCodeBuilder.AppendLine("}");
                sourceCodeBuilder.AppendLine();
            }
        }

        private void GenerateAssociationClassStruct(JsonData.Model associationClassModel, string associationName)
        {
            string classType = MapDataType(associationClassModel.class_name.ToLower());
            sourceCodeBuilder.AppendLine($"struct {associationClassModel.class_name} {{");

            GenerateAssociationAttributes(associationClassModel);

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();

            GenerateAssociationClassMethods(associationClassModel);

        }
       
        private void GenerateAssociationAttributes(JsonData.Model associationClassModel)
        {
            foreach (var attribute in associationClassModel.attributes)
            {
                string attributeType = MapDataType(attribute.data_type.ToLower());
                sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name};");
            }
        }

        private void GenerateAssociationClassMethods(JsonData.Model associationClassModel)
        {
            GenerateAssociationStatesEnum(associationClassModel);
            GenerateAssociationEventsEnums(associationClassModel);
            GenerateAssociationStateFunctions(associationClassModel);
            GenerateAssociationAttributeSetters(associationClassModel);
        }

        private void GenerateAssociationStatesEnum(JsonData.Model associationClassModel)
        {
            if (associationClassModel.states != null && associationClassModel.states.Count > 0)
            {
                sourceCodeBuilder.AppendLine($"    enum {associationClassModel.class_name}Status {{");

                foreach (var state in associationClassModel.states)
                {
                    sourceCodeBuilder.AppendLine($"        {state.state_value},");
                }

                sourceCodeBuilder.AppendLine($"    }};");
                sourceCodeBuilder.AppendLine();
            }
        }

        private void GenerateAssociationEventsEnums(JsonData.Model associationClassModel)
        {
            if (associationClassModel.attributes != null)
            {
                foreach (var @event in associationClassModel.attributes)
                {
                    if (@event != null && @event.event_name != null)
                    {
                        if (@event.attribute_type == "inst_event")
                        {
                            continue;
                        }

                        string eventType = MapDataType(@event.data_type);

                        sourceCodeBuilder.AppendLine($"    enum {associationClassModel.class_name}_{@event.event_name.ToUpper()} {{");
                        sourceCodeBuilder.AppendLine($"        {eventType} {@event.event_name};");
                        sourceCodeBuilder.AppendLine("    };");
                        sourceCodeBuilder.AppendLine();
                    }
                }
            }
        }

        private void GenerateAssociationStateFunctions(JsonData.Model associationClassModel)
        {
            if (associationClassModel.states != null)
            {
                foreach (var state in associationClassModel.states)
                {
                    GenerateAssociationSetActiveFunctionLogic(associationClassModel.class_name, state);
                }
            }
        }

        private void GenerateAssociationSetActiveFunctionLogic(string className, JsonData.State state)
        {
            string functionName = $"set_{state.state_name.ToLower()}";
            string statusValue = state.state_value.ToLower();

            sourceCodeBuilder.AppendLine($"void {functionName}(struct {className} *{className.ToLower()})");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"    strcpy({className.ToLower()}->{state.state_name}, \"{statusValue}\");");
            sourceCodeBuilder.AppendLine("}");
            sourceCodeBuilder.AppendLine();
        }

        private void GenerateAssociationAttributeSetters(JsonData.Model associationClassModel)
        {
            if (associationClassModel != null && associationClassModel.attributes != null && associationClassModel.states != null && associationClassModel.states.Count > 0)
            {
                foreach (var attribute in associationClassModel.attributes)
                {
                    if (attribute.attribute_type != "inst_event")
                    {
                        GenerateAssociationAttributeSetter(associationClassModel.class_name, attribute);
                    }
                }
            }
        }

        private void GenerateAssociationAttributeSetter(string className, JsonData.Attribute1 attribute)
        {
            string attributeType = MapDataType(attribute.data_type.ToLower());

            sourceCodeBuilder.AppendLine($"void set_{attribute.attribute_name}(struct {className} *instance, {attributeType} new_{attribute.attribute_name})");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"    {GetSetterLogic(attributeType, attribute.attribute_name)}");
            sourceCodeBuilder.AppendLine("}");
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
            GenerateStatesEnum(importedClassModel);
            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();

            // Generate additional sections for the imported class
            GenerateEventsEnums(importedClassModel);
            GenerateStateFunctions(importedClassModel);
            //GenerateAttributeSetters(importedClassModel);
            GenerateSetter(importedClassModel);
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
                case "inst_ref_<timer>":
                    return "TIMER";
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
                public string related_class_name { get; set; }
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

                         "1. Click 'Select File' to select a file JSON.\n" +
                         "2. Click 'Parse' to check JSON data.\n" +
                         "Interpret Checking Results.\n" +
                         "   - Review the checking results in the output text box.\n" +
                         "   - If there are errors, error messages will be provided to guide the corrections.\n" +
                         "3. Click 'Visualiza' to visualization all data\n" +
                         "4. Click 'Translate' to translate file JSON to C programming language.\n" +
                         "5. Click 'Simulate' to simulation this program\n" +
                         "6. Click 'Copy' to copy C code in textbox\n" +
                         "7. Click 'Save' to save the translated data in a C programming language\n" +
                         "8. Click 'Reset' to clear all data that been upload and translate\n";



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
