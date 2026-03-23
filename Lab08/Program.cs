
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        UserInterface userInterface = new();
        int size = userInterface.WorldSize("How big would you like the world to be? (4x4, 6x6, 8x8) ");
        GameLoop game;
        //Escape sequence if the user wants to exit
        if (size > 0) game = new(size);
    }
}

public class GameLoop
{
    public static bool fountainActive = false;
    static bool loop = true;
    public static Movement? movement;
    public static readonly UserInterface _interface = new();
    public static readonly UserInterface.ChangeUserOptions changeUserOptions = new();
    //Obstacles obstacles = new(); //Used if needed to print the grid
    public static int size;
    public GameLoop(int _size)
    {
        size = _size;
        movement = new Movement(size);
        changeUserOptions.AddShooting(size);
        Obstacles.RandomizeObstacles(size, ref movement.worldGrid);
        int arrowsLeft = movement.arrows;
        while (loop)
        {
            //Obstacles.PrintGrid(movement.worldGrid, size, movement.location);
            _interface.PrintMessages();
            Obstacles.SenseObstacles(movement.worldGrid, movement.location, size);
            if (arrowsLeft > movement.arrows) 
            {
                arrowsLeft = movement.arrows;
                Console.WriteLine($"You have {arrowsLeft} arrows left.");
            }
            movement.Move(this, ref loop, size, fountainActive, _interface, changeUserOptions);
        }
    }
}

public class UserInterface
{
    public static Dictionary<string, List<string>> movementOptions = new()
    {
        {"Exit", new List<string>{"Exit", "exit"}},
        {"East", new List<string>{"move east","Move East", "Move east", "move East", "East", "east"}},
        {"West", new List<string>{"move west", "Move West", "Move west", "move West", "West", "west"}},
        {"North", new List<string>{"move north", "Move North", "Move north", "move North", "North", "north"}},
        {"South", new List<string>{"move south", "Move South", "Move south", "move South", "South", "south"}},
    };

    public class ChangeUserOptions
    {
        public void AddShooting(int size)
        {
            if (size != 4)
            {
                Dictionary<string, List<string>> shootingOptions = new()
                {
                    {"Shoot South", new List<string>{"shoot south", "Shoot South", "Shoot south", "shoot South"}},
                    {"Shoot North", new List<string>{"shoot north", "Shoot North", "Shoot north", "shoot North"}},
                    {"Shoot East", new List<string>{"shoot east", "Shoot East", "Shoot east", "shoot East"}},
                    {"Shoot West", new List<string>{"shoot west", "Shoot West", "Shoot west", "shoot West"}}
                };
                foreach(KeyValuePair<string, List<string>> items in shootingOptions) movementOptions.Add(items.Key, items.Value);
            }
        }
        
        public void AddFountain()
        {
            movementOptions.Add("Activate Fountain", new List<string> {"Activate Fountain", "Activate fountain", "activate Fountain", "activate fountain"});
        }
    }
    public void PrintMessages()
    {
        for (int length = 0; length < Console.WindowWidth; length ++)
        {
            Console.Write("-");
        }
        Console.WriteLine($"You are in the room at {GameLoop.movement.location.x}, {GameLoop.movement.location.y}");
    }

    public string? ReadInput(string message, Dictionary<string, List<string>> dict)
    {
        Console.Write(message);
        string? input;
        bool match = false;
        //Get correct input
        input = Console.ReadLine();
        (input, match) = CheckInput(input, ReadInput, false, dict);

        return input;
    }

    public static (string?, bool) CheckInput(string? input, Func<string, Dictionary<string, List<string>>, string?> operation, bool match, Dictionary<string, List<string>> options)
    {
        string? key = input;
        //loop through all the possible options to make sure the input is valid
        foreach (KeyValuePair<string, List<string>> item in options)
        {
            foreach (string val in item.Value) if (val == input) 
            {
                return (item.Key, true);
            }
        }
        if (match == false || input == null) 
        {
            //Console.Write(errormessage)
            input = operation("Please enter a valid input. ", options);
            return (input, false);
        }
        else return (key, true);
    }

    public int WorldSize(string message)
    {
        Dictionary<string, List<string>> validInputs = new Dictionary<string, List<string>>
        {
            {"Exit", new List<string>{"Exit", "exit"}},
            {"4x4", new List<string>{"4x4", "4"}},
            {"6x6", new List<string>{"6x6", "6"}},
            {"8x8", new List<string>{"8x8", "8"}}
        };
        string? input = ReadInput(message, validInputs);
        switch (input)
        {
            case "4x4":
                return 4;
            case "6x6":
                return 6;
            case "8x8":
                return 8;
            case "Exit":
                return 0;
            default: 
                return WorldSize("Please enter a valid input: (4, 6, 8) ");      
        }
    }
}

