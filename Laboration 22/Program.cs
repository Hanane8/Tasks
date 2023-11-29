
using Microsoft.VisualBasic;
using System;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Laboration_22

{
    class TaskUppgft
    {
        public int ID {  get; set; }    
        public string Task { get; set; }
        public DateTime Deadline { get; set; }
        public double EstimatedHours { get; set; }
        public bool IsCompleted { get; set; }

    }
    internal class Program
    {
        StreamWriter sw = new StreamWriter("todo-list.csv"); // Skapar en StreamWriter för att skriva till en CSV-fil
        private static string filelist = "todo-list.csv"; // Namn på CSV-filen som lagrar uppgifter

        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(filelist))
                {
                    using (StreamWriter sw = new StreamWriter(filelist))
                    {
                        sw.WriteLine("Task;Deadline;EstimatedHours;IsCompleted");
                    }
                }

                List<TaskUppgft> todoList = File.ReadAllLines(filelist)
                    .Skip(1)
                    .Select(line => line.Split(';'))
                    .Select(data => new TaskUppgft
                    {
                        Task = data[0],
                        Deadline = DateTime.ParseExact(data[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        EstimatedHours = double.Parse(data[2]),
                        IsCompleted = bool.Parse(data[3])
                    })
                    .ToList();

                while (true)
                {
                    Console.WriteLine("Vad vill du göra? ");
                    Console.WriteLine("1 - Lägg till uppgift: ");
                    Console.WriteLine("2 - Markera uppgift som klar:");
                    Console.WriteLine("3 - Visa aktuella uppgifter: ");
                    Console.WriteLine("4 - Avsluta programmet");

                    string val = Console.ReadLine();

                    switch (val)
                    {
                        case "1":
                            LäggUppgift(todoList);
                            break;
                        case "2":
                            MarkUppgKlar(todoList);
                            break;
                        case "3":
                            VisaAktuellaUppgift(todoList);
                            break;
                        case "4":
                            Console.WriteLine("Programmet avslutat");
                            return;
                        default:
                            Console.WriteLine("Ogiltigt val. Försök igen.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ett fel uppstod: " + e.Message);
            }
        }
        private static void LäggUppgift(List<TaskUppgft> todoList)
        {
            Console.Write("Ange uppgiftens beskrivning: ");
            string task = Console.ReadLine();

            DateTime deadline;
            string input;
            while (true)
            {
                Console.Write("Ange deadline (YYYY-MM-DD): ");
                input = Console.ReadLine();

                try
                {
                    deadline = DateTime.Parse(input); // Försöker konvertera inmatningen till ett DateTime-objekt
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ogiltigt datumformat. Försök igen."); // Felmeddelande vid ogiltigt datumformat
                }
            }

            double estimatedHours;

            while (true)
            {
                Console.Write("Ange uppskattad tidsåtgång (timmar): ");
                string tid = Console.ReadLine();

                try
                {
                    estimatedHours = double.Parse(tid); // Försöker konvertera inmatningen till en double
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ogiltig inmatning. Ange ett nummer.");
                }
            }
            TaskUppgft newTask = new TaskUppgft // Skapar en ny uppgift
            {
                Task = task,
                Deadline = deadline,
                EstimatedHours = estimatedHours,
                IsCompleted = false
            };
            todoList.Add(newTask);
            using (StreamWriter sw = new StreamWriter(filelist, true)) // Öppnar en StreamWriter för att skriva till filen
            {
                sw.WriteLine($"{newTask.Task};{newTask.Deadline:yyyy-MM-dd};{newTask.EstimatedHours};{newTask.IsCompleted}");
            }
            Console.WriteLine("Uppgift tillagd.");
        }
        private static void MarkUppgKlar(List<TaskUppgft> todoList)
        {
            Console.WriteLine("Välj uppgift att markera som klar:");
            for (int i = 0; i < todoList.Count; i++) // Visar lista med uppgifter för att användaren ska välja
            {
                Console.WriteLine($"{i + 1}. {todoList[i].Task} (Deadline: {todoList[i].Deadline:yyyy-MM-dd}, Status: {(todoList[i].IsCompleted ? "Klar" : "Ej klar")})");
            }
            int ID;
            while (true)
            {
                Console.Write("Ange numret på uppgiften att markera som klar (0 för att avbryta): ");
                if (int.TryParse(Console.ReadLine(), out ID))
                {
                    TaskUppgft selectedTask;
                    if (ID == 0)
                    {
                        return; // Avbryt om användaren väljer 0
                    }
                    else if (ID > 0 && ID <= todoList.Count)
                    {
                        selectedTask = todoList[ID - 1];
                        selectedTask.IsCompleted = true; // Markera uppgiften som klar
                        using (StreamWriter sw = new StreamWriter(filelist, false)) // Öppna filen för att skriva om hela filen
                        {
                            sw.WriteLine("Task;Deadline;EstimatedHours;IsCompleted");
                            foreach (var task in todoList)
                            {
                                sw.WriteLine($"{task.Task};{task.Deadline:yyyy-MM-dd};{task.EstimatedHours};{task.IsCompleted}"); // Skriv om hela filen med uppdaterade uppgifter
                            }
                        }
                        Console.WriteLine($"{selectedTask.Task} är markerad som klar.");
                    }
         
                }
                else
                {
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                }
            }
        }
        private static void VisaAktuellaUppgift(List<TaskUppgft> todoList)
        {
            var uppgifter = todoList.OrderBy(task => task.Deadline).ToList(); // Sorterar uppgifterna efter deadline
            int ID = 1;
            
            if (uppgifter.Count == 0)
            {
                Console.WriteLine("Inga uppgifter tillgängliga.");
            }
            else
            {
                Console.WriteLine("Aktuella uppgifter:");
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("ID\t\tDeadline\t\tTid     \t\tVad");
                Console.WriteLine("------------------------------------------------------------------------------------");
                
                foreach (var task in uppgifter)
                {
                    
                    // Check the task's status and proximity to the deadline for color-coding
                    if (task.IsCompleted)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Green for completed tasks
                        
                    }
                    else if (task.Deadline < DateTime.Now)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Röd för förfallen uppgift

                    }
                    else if ((task.Deadline - DateTime.Now).Days <3)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; // Yellow for tasks with less than 3 days until the deadline
                        
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White; // White for other cases
                    }

                    // Display the task
                    Console.WriteLine($"{ID ++}\t\t{task.Deadline:yyyy-MM-dd}\t\t{task.EstimatedHours}h\t\t{task.Task}");
                    Console.ResetColor(); // Reset text color for the next task
                }
            }
        }

        
    }
}       
        

    
      