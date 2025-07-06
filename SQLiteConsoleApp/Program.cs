using SQLiteConsoleApp;
using System.Data.SQLite;

const string dbfile = "URI=file:test.db";
string firstName = "";
string lastName = "";
int empID = 0;
string input = "";
int num;
ConsoleKeyInfo cki;


//CreateTable();

do
{
    DisplayMenu();
    cki = Console.ReadKey(true);
    switch (cki.KeyChar.ToString())
    {
        case "1"://add
            
            Console.Write("Enter first name: ");
            firstName = Console.ReadLine();
            firstName = firstName.Trim();
            if (!Valid(firstName)) break;
            
            Console.Write("Enter last name: ");
            lastName = Console.ReadLine();
            lastName = lastName.Trim();
            if (!Valid(lastName)) break;

            Random random = new Random();
            do
            {
                empID = (random.Next(1000000, 9999999));
            } while (IDFound(empID));

            DateTime today = DateTime.Now;
            string sqlDate = today.ToString();

            Employee emp = new Employee() { ID = empID, firstName = firstName, lastName = lastName, lastUpdate = sqlDate };

            AddRow(emp);
            break;

        case "2"://remove

            if (emptyTable())
            {
                Console.WriteLine("Table is empty, add an employee first. Press Enter to continue.\n");
                Console.ReadLine();
                break;
            }

            Console.Write("Enter employee ID of employee to remove: ");
            input = Console.ReadLine();
            input = input.Trim();

            if (!validID(input))
            {
                Console.WriteLine("\nTry again, valid ID is a 7 digit number, Press Enter to continue.");
                Console.ReadLine();
                break;
            }
     
            if(IDFound(num))
            {
                DeleteRow(num); 
            }
            else
            {
                Console.WriteLine($"\nEmployee {num} not found. Press Enter to continue.");
                Console.ReadLine();
            }
            break;

        case "3": //update

            if(emptyTable())
            {
                Console.WriteLine("Table is empty, add an employee first. Press Enter to continue.\n");
                Console.ReadLine();
                break;
            }

            Console.Write("Enter employee ID of employee to update: ");
            input = Console.ReadLine();
            input = input.Trim();

            if (!validID(input))
            {
                Console.WriteLine("\nTry again, valid ID is a 7 digit number, Press Enter to continue.");
                Console.ReadLine();
                break;
            }
            if (IDFound(num))
            {
                UpdateRow(num);
            }
            else
            {
                Console.WriteLine($"\nEmployee {num} not found. Press Enter to continue.");
                Console.ReadLine();
            }
            break;
        
        case "4"://show all employees

            if (emptyTable())
            {
                Console.WriteLine("Table is empty, add an employee first. Press Enter to continue.\n");
                Console.ReadLine();
                break;
            }
            ReadTable();
            Console.WriteLine("\nPress Enter to continue.");
            Console.ReadLine();
            break;
        
        case "5": //search employee by name

            if (emptyTable())
            {
                Console.WriteLine("Table is empty, add an employee first. Press Enter to continue.\n");
                Console.ReadLine();
                break;
            }
            string first = "";
            string last = "";
            Console.Write("Enter first name: ");
            first = Console.ReadLine();
            Console.Write("Enter last name: ");
            last = Console.ReadLine();
            SearchEmployee(first, last);
            Console.WriteLine("\nPress Enter to continue.");
            Console.ReadLine();
            break;
        case "6":
            Console.Write("Type DELETE to delete all records from table: ");
            if (Console.ReadLine() == "DELETE")
            {
                deleteTable();
                Console.WriteLine("\nAll records deleted. Press Enter to continue.");
                Console.ReadLine();
                break;
            }
            Console.WriteLine("\nType DELETE in all caps to delete all records. Press Enter to continue.");
            Console.ReadLine();

            break;

    }
} while (cki.Key != ConsoleKey.Escape);

Console.Clear();

void deleteTable()
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string deleteTable = "DELETE FROM Employees;";
    SQLiteCommand command = new SQLiteCommand(deleteTable, connection);
    command.ExecuteNonQuery();
    connection.Close();
}
void SearchEmployee(string first, string last)// input name and return row 
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string search =$"select * from Employees where first_name = '{first}' or last_name = '{last}';";
    string count = $"select count(*) from Employees where first_name = '{first}' or last_name = '{last}';";
    SQLiteCommand cmd = new SQLiteCommand(count, connection);
    SQLiteCommand command = new SQLiteCommand(search, connection);
    SQLiteDataReader reader = command.ExecuteReader();
    if(!reader.HasRows)
    {
        Console.WriteLine("\nNo results found!");
    }

    else
    {
        int numCount = Convert.ToInt32(cmd.ExecuteScalar());
        if (numCount == 1)
        {
            Console.WriteLine($"\n{numCount} result found:");
        }
        else
        {
            Console.WriteLine($"\n{numCount} results found:");
        }
        
        Console.WriteLine($"\n{reader.GetName(0)}\t{reader.GetName(1)}\t{reader.GetName(2)}\t{reader.GetName(3)}\n");
        Console.WriteLine("---------------------------------------------------------------------");
        while (reader.Read())
        {
            Console.WriteLine($"{reader["employee_ID"]}\t\t{reader["first_name"]}\t\t{reader["last_name"]}\t\t{reader["last_update"]}");
        }
    }
   
    connection.Close();

}

bool validID(string input)
{
    return (Int32.TryParse(input, out num) && input.Length == 7);  
}
bool IDFound(int id)
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string checkID = $"select count(*) from Employees where employee_id = {id};";
    SQLiteCommand command = new SQLiteCommand(checkID, connection);

    return int.Parse(command.ExecuteScalar().ToString()) != 0;
}

bool emptyTable()
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string checkTable = $"select count(*) from Employees;";
    SQLiteCommand command = new SQLiteCommand(checkTable, connection);

    return int.Parse(command.ExecuteScalar().ToString()) == 0;

    
}
bool Valid(string name) //verifies name does not have numbers or is empty
{
    if (!name.All(char.IsLetter) || name == "")
    {
        Console.WriteLine("Invalid input, name cannot contain numbers or be empty, try again\n");
        return false;
    }
    return true;
}
void DisplayMenu()
{
    Console.WriteLine("Welcome to the employee database! Select an option or press ESC to exit\n");
    Console.WriteLine("1. Add employee");
    Console.WriteLine("2. Remove employee");
    Console.WriteLine("3. Update employee");
    Console.WriteLine("4. List all employees");
    Console.WriteLine("5. Search employee by name");
    Console.WriteLine("6. Clear all records\n");
}
void CreateTable()
{ 
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string table = "create table Employees (employee_ID integer primary key, first_name text, last_name text, last_update text);";
    SQLiteCommand command = new SQLiteCommand(table, connection);
    command.ExecuteNonQuery();
    connection.Close();
}

void AddRow(Employee emp)
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string addEmp = $"insert into Employees (employee_ID, first_name, last_name, last_update) values ({emp.ID},'{emp.firstName}','{emp.lastName}','{emp.lastUpdate}');";
    SQLiteCommand command = new SQLiteCommand(addEmp, connection);
    command.ExecuteNonQuery();
    connection.Close();

    Console.WriteLine($"\nEmployee {firstName} {lastName} with employee ID {empID} added to database!\n\nPress Enter to continue");
    Console.ReadLine();
}

void DeleteRow(int id)
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string deleteEmp = $"delete from Employees where employee_id = {id};";
    SQLiteCommand command = new SQLiteCommand(deleteEmp, connection);
    command.ExecuteNonQuery();
    connection.Close();

    Console.WriteLine($"\nEmployee {num} removed from database! Press Enter to continue.");
    Console.ReadLine();
}

void UpdateRow(int id)
{
   
    Console.Write("Update first name: ");
    firstName = Console.ReadLine();
    firstName = firstName.Trim();
    if (!Valid(firstName)) return;

    Console.Write("Update last name: ");
    lastName = Console.ReadLine();
    lastName = lastName.Trim();
    if (!Valid(lastName)) return;

    DateTime today = DateTime.Now;
    string sqlDate = today.ToString();

    string updateEmp = $"update Employees set first_name = '{firstName}', last_name = '{lastName}', last_update = '{sqlDate}' where employee_id={id};";
    
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    SQLiteCommand command = new SQLiteCommand(updateEmp, connection);
    command.ExecuteNonQuery();
    connection.Close();
    
    Console.WriteLine($"\nEmployee {id} updated in database! Press Enter to continue.");
    Console.ReadLine();
}

void ReadTable()
{
    SQLiteConnection connection = new SQLiteConnection(dbfile);
    connection.Open();
    string selectAll = "select * from Employees;";
    SQLiteCommand command = new SQLiteCommand(selectAll, connection);
    SQLiteDataReader reader = command.ExecuteReader();
    Console.WriteLine($"{reader.GetName(0)}\t{reader.GetName(1)}\t{reader.GetName(2)}\t{reader.GetName(3)}\n");
    Console.WriteLine("---------------------------------------------------------------------");
    while (reader.Read())
    {
        Console.WriteLine($"{reader["employee_ID"]}\t\t{reader["first_name"]}\t\t{reader["last_name"]}\t\t{reader["last_update"]}");
    }
    connection.Close();
}