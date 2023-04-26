using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.IO;

namespace CalculatorLibrary
{
    internal class Calculator
    {
        readonly JsonWriter writer;
        public Calculator()
        {
            //storing user inputs and calculations in json file
            StreamWriter logFile = File.CreateText("calculatorlog.json");
            logFile.AutoFlush = true;
            writer = new JsonTextWriter(logFile)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartObject();
            writer.WritePropertyName("Operations");
            writer.WriteStartArray();
        }

        public double DoOperation(double num1, double num2, string op)
        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            writer.WriteStartObject();
            writer.WritePropertyName("Operand1");
            writer.WriteValue(num1);
            writer.WritePropertyName("Operand2");
            writer.WriteValue(num2);
            writer.WritePropertyName("Operation");
            // Use a switch statement to do the math.
            switch (op)
            {
                case "a":
                    result = num1 + num2;
                    writer.WriteValue("Add");
                    break;
                case "s":
                    result = num1 - num2;
                    writer.WriteValue("Subtract");
                    break;
                case "m":
                    result = num1 * num2;
                    writer.WriteValue("Multiply");
                    break;
                case "d":
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                    }
                    writer.WriteValue("Divide");
                    break;
                case "p":
                    result = Math.Pow(num1, num2);
                    writer.WriteValue("Power");
                    break;
                case "r":
                    while (true)
                    {
                        Console.WriteLine("Choose which number you would like to use, 1 or 2");
                        string? numChoice = Console.ReadLine();
                        if (numChoice == "1")
                        {
                            result = Math.Sqrt(num1);
                            break;
                        }
                        else if (numChoice == "2")
                        {
                            result = Math.Sqrt(num2);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid choice");
                        }
                    }
                    writer.WriteValue("Square Root");
                    break;
                case "c":
                    result = Circle(num1, num2, op);
                    break;
                // Return text for an incorrect option entry.
                default:
                    break;
            }
            double roundedResult = Math.Round(result, 2);
            writer.WritePropertyName("Result");
            writer.WriteValue(roundedResult);
            writer.WriteEndObject();
            return roundedResult;
        }

        public double Circle(double num1, double num2, string op)

        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            //circle equations
            Console.WriteLine("Choose which calculation do you want to perform:");
            Console.WriteLine("\td - Diameter");
            Console.WriteLine("\tc - Circumference");
            Console.WriteLine("\ta - Area");
            string? calculationChoice = Console.ReadLine();

            Console.WriteLine($"Which number would you like to choose \n\t1: {num1} \n\t2: {num2}");
            bool validInput = false;
            double numChoice1 = 0;
            while (!validInput)
            {
                string? input = Console.ReadLine();
                if (input == "1")
                {
                    numChoice1 = num1;
                    validInput = true;
                }
                else if (input == "2") 
                {
                    numChoice1 = num2;
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter either 1 or 2.");
                }
            }

            switch (calculationChoice)
            {
                case "d":
                    result = numChoice1 * 2;
                    writer.WriteValue("Circle Diameter");
                    writer.WritePropertyName("Chosen Number");
                    writer.WriteValue(numChoice1);
                    break;
                case "c":
                    result = 2 * Math.PI * numChoice1;
                    writer.WriteValue("Circle Circumference");
                    writer.WritePropertyName("Chosen Number");
                    writer.WriteValue(numChoice1);
                    break;
                case "a":
                    result = Math.PI * Math.Pow(numChoice1, 2);
                    writer.WriteValue("Circle Area");
                    writer.WritePropertyName("Chosen Number");
                    writer.WriteValue(numChoice1);
                    break;
                default:
                    break;
            }
            return result;
        }

        public void DisplayHistory()
        {
            //change so that is open and able to be written in again, currently it starts new object and history is not accessible again
                //removing EndArray and EndObject causes errors in trying to read log file
            //remove curly brackets so that display looks more nice
            writer.WriteEndArray();
            writer.WriteEndObject();
            Console.Clear();

            try
            {
                using (FileStream fs = new FileStream("calculatorlog.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader r = new StreamReader(fs))
                {
                    string? json = r.ReadToEnd();
                    dynamic? data = JsonConvert.DeserializeObject(json);
                    if (data != null)
                    {
                        dynamic[]? operations = data.Operations.ToObject<object[]>();
                        if (operations != null)
                        {
                            for (int i = 0; i < operations.Length; i++)
                            {
                                Console.WriteLine($"{i + 1}-------------");
                                Console.WriteLine(JsonConvert.SerializeObject(operations[i], Formatting.Indented));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No operations found in calculatorlog.json");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found in calculatorlog.json");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error accessing calculatorlog.json: {ex.Message}");
            }
        } 

        public void Finish()
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Close();
        }
    }
}