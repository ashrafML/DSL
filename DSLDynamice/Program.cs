using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Data;

public class Patient
{
    public string PatientId { get; }
    public string MedicationsName { get; }
    public List<string> Conditions { get; } = new List<string>();
    public List<string> Medications { get; } = new List<string>();
    string methodName = "";
    public Patient(string patientId,string MedName, string medName)
    {
        PatientId = patientId;
        MedicationsName = MedName;
        ExecuteDynamicMethod(medName);
        methodName = medName;

    }

    private void ExecuteDynamicMethod(string medName)
    {
        string methodDefinition = RetrieveMethodDefinitionFromDatabase();

        if (!string.IsNullOrEmpty(methodDefinition))
        {
            string className = "DynamicPatient";
       

            string code = $@"
                using System;
                using System.Collections.Generic;
                public class {className}
                {{
    public List<string> Medications {{ get; }} = new List<string>();

                   
                        {methodDefinition}
                    
                }}
            ";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(List<>).GetTypeInfo().Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());

                    dynamic instance = assembly.CreateInstance(className);
                    MethodInfo dynamicMethod = instance.GetType().GetMethod(medName);
                    dynamicMethod.Invoke(instance, new object[] { className });
                    // Conditions.AddRange(instance.Conditions);
                    Medications.AddRange(instance.Medications);
                }
                else
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine(diagnostic.GetMessage());
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Method definition not found in the database.");
        }
    }

    private string RetrieveMethodDefinitionFromDatabase()
    {
        string connectionString = "connection string"; // Replace with your actual connection string
        string methodName = "PrescribeMedication"; // Replace with the name of the method

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = $"SELECT MethodDefinition FROM DSL_Methods WHERE MethodName = @MethodName"; // Replace with your actual table and column names
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MethodName", methodName);

            connection.Open();
            string methodDefinition = command.ExecuteScalar()?.ToString();

            return methodDefinition;
        }

    }
    /// <summary>
    /// Use this if there logic inside function from db
    /// </summary>
    /// <param name="query"></param>
    /// <param name="Paramsdetails"></param>
    /// <returns></returns>
    private string RetrieveMethodDefinitionFromDatabase(string query, Dictionary<string,string> Paramsdetails)
    {
        string connectionString = "connection string"; // Replace with your actual connection string
        string methodName = "PrescribeMedication"; // Replace with the name of the method

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          //  string query = $"SELECT MethodDefinition FROM DSL_Methods WHERE MethodName = @MethodName"; // Replace with your actual table and column names
            SqlCommand command = new SqlCommand(query, connection);
            foreach (var item in Paramsdetails)
            {
                command.Parameters.AddWithValue("@"+item.Key, item.Value);
            }
            if (command.Connection.State == ConnectionState.Open)
            {
                connection.Open();
            }
            
            string methodDefinition = command.ExecuteScalar()?.ToString();

            return methodDefinition;
        }

    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Create a patient
        Patient johnSmith = new Patient("JohnSmith", "Insuline", "PrescribeMedication");
      

        // Print the patient's medications
        Console.WriteLine("Patient: " + johnSmith.PatientId);
        Console.WriteLine("Medications: " + string.Join(", ", johnSmith.Medications));
    }
}
