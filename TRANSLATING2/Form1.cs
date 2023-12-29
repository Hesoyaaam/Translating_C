using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static TRANSLATING2.Form1.JsonData;

namespace TRANSLATING2
{
    public partial class Form1 : Form
    {
        private StringBuilder sourceCodeBuilder;
        private string selectedJsonFilePath;
        private string[] fileNames;
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
                richTextBox1.Text = displayJson;
            }
        }
        private void btnParsing_Click(object sender, EventArgs e)
        {
            Parsing.Parsing formParsing = new Parsing.Parsing();
            formParsing.Show();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            selectedJsonFilePath = null; // Clear the selected JSON file path
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
                            // Write the generated C code to the file
                            sw.Write(richTextBox2.Text);
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
                    richTextBox2.Text = sourceCodeBuilder.ToString();
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
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = "This Application is to generate file JSON to C programming language.\n\n" +

                         "1. Click 'Browse' to select a file JSON.\n" +
                         "2. Click 'Parsing' to check JSON data\n"+
                         "3. Click 'Simulate' to simulation this program\n"+
                         "4. Click Visualize to visualization all data\n" +
                         "5. Click 'Translate' to translate file JSON to C programming language.\n" +
                         "6. Click 'Clear' to clear all data that been upload and translate\n" +
                         "7. Click 'Copy Json' to copy JSON data in textbox\n" +
                         "8. Click 'Copy Code' to copy C code in textbox\n"+
                         "9. Click 'Export' to save the translated data in a C programming language file";
                         

            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void GenerateCCode(string jsonFilePath)
        {
            try
            {
                string umlDiagramJson = File.ReadAllText(jsonFilePath);
                JsonData json = JsonConvert.DeserializeObject<JsonData>(umlDiagramJson);
 
                GenerateStructsAndAssociations(json);

                CloseNamespace();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    // Tambahkan kasus lainnya sesuai kebutuhan
                }
            }
        }

        private void GenerateNamespace(string namespaceName)
        {
            sourceCodeBuilder.AppendLine("#include <stdio.h>");
            sourceCodeBuilder.AppendLine("#include <string.h>"); 
            sourceCodeBuilder.AppendLine($"namespace {namespaceName}");
            sourceCodeBuilder.AppendLine("{");
        }
        private void GenerateClassStruct(JsonData.Model classModel)
        {
            string className = classModel.class_name;

            sourceCodeBuilder.AppendLine($"struct {className} {{");

            foreach (var attribute in classModel.attributes)
            {
                string attributeType = MapDataType(attribute.data_type.ToLower());
                sourceCodeBuilder.AppendLine($"    {attributeType} {attribute.attribute_name}; ");
            }

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();
        }
        private void GenerateAssociationStruct(JsonData.Model associationModel)
        {
            string associationName = associationModel.name;

            sourceCodeBuilder.AppendLine($"struct {associationName} {{");

            // Assuming there are two classes in the association (Mahasiswa and Matakuliah)
            if (associationModel.@class.Count == 2)
            {
                var class1 = associationModel.@class[0];
                var class2 = associationModel.@class[1];

                string class1Type = MapDataType(class1.class_name.ToLower());
                string class2Type = MapDataType(class2.class_name.ToLower());

                string multiplicity1 = class1.class_multiplicity;
                string multiplicity2 = class2.class_multiplicity;

                // Handle the case when both classes have multiplicity "1..1"
                if (multiplicity1 == "1..1" && multiplicity2 == "1..1")
                {
                    sourceCodeBuilder.AppendLine($"    struct {class1Type} {class1.class_name};");
                    sourceCodeBuilder.AppendLine($"    struct {class2Type} {class2.class_name};");
                }
                else
                {
                    sourceCodeBuilder.AppendLine($"    struct {class1Type} {class1.class_name} {class2Type} {class2.class_name};");
                }
            }

            sourceCodeBuilder.AppendLine("};");
            sourceCodeBuilder.AppendLine();
            if (associationModel.model != null)
            {
                GenerateAssociationClassStruct(associationModel.model);
            }
        }
        private void GenerateAssociationClassStruct(JsonData.Model associationClassModel)
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
        private void CloseNamespace()
        {
            sourceCodeBuilder.AppendLine("}");
            sourceCodeBuilder.AppendLine();
        }
        private string MapDataType(string dataType)
        {
            if (dataType == null)
            {
                return "UNKNOWN_TYPE";
            }

            switch (dataType.ToLower())
            {
                case "id":
                    return "int";
                case "state":
                    return "char";
                case "string":
                    return "char[]";
                case "integer":
                    return "int";
                case "real":
                    return "float";
                // Add more mappings as needed
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
            }
            public class Attribute1
            {
                public string attribute_name { get; set; }
                public string data_type { get; set; }
                public string default_value { get; set; }
                public string attribute_type { get; set; }
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
            if (!string.IsNullOrEmpty(richTextBox2.Text))
            {
                try
                {
                    // Copy the generated code to the clipboard
                    Clipboard.SetText(richTextBox2.Text);

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
    }
}
