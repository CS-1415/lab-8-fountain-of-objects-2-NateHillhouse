
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;

namespace Lab08;

public class GameLoop
{
    public static bool FountainActive = false;
    static bool loop = true;
    public static Movement? movement;
    public static readonly UserInterface _interface = new();
    public static readonly UserInterface.ChangeUserOptions changeUserOptions = new();
    //Obstacles obstacles = new(); //Used if needed to print the grid
    public static int size;
    public GameLoop()
    {
        size = _interface.WorldSize("How big would you like the world to be? (4x4, 6x6, 8x8) ");
        movement = new Movement(size); 

        Console.Write("What type of game would you like to play? (Obstacle, Monster)");
        MapType gameType = UserInterface.WorldType(Console.ReadLine());
        
        Movement move = new(size);

        if (gameType == MapType.Obstacle) ObstacleMap(move);
        else if (gameType == MapType.Monster) MonsterFighter(movement);
    }

    public GameLoop(bool test)
    {
        size = 6;
        movement = new(size);
        MonsterFighter(movement, test);
    }
    public void ObstacleMap(Movement movement)
    {
        Obstacles.RandomizeObstacles(size, movement.worldGrid);
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
            movement.Move(this, ref loop, size, FountainActive, _interface, changeUserOptions);
        }
    }

    public void MonsterFighter(Movement movement, bool test = false)
    {
        //Add inventory variable
        
        MonsterMap.RandomizeMonsters(ref movement.worldGrid, movement.MonsterList);
        if (test) PrintGrid(movement.worldGrid, size, (0,0));

        while(loop && !test)
        {
            _interface.PrintMessages(); //May need to change up to suit this gamemode
           
            Obstacles.SenseObstacles(movement.worldGrid, movement.location, size); //May be used just to sense the fountain

            movement.Move(this, ref loop, size, FountainActive, _interface, changeUserOptions);
        }
    }

    public void PrintGrid(Dictionary<(int, int), string> worldGrid, int size, (int, int) location)
    {
        for (int i = 1; i <= size; i ++)
        {
            for (int j = 1; j <= size; j++)
            {
                if (worldGrid[(j,i)] != "") Console.Write(worldGrid[(j,i)] + "        ");
                else if ((j,i) == location) Console.Write("Player ");
                else Console.Write("Empty ");
            }
            Console.WriteLine();
        }
    }
}

public class UserInterface
{
    public static Dictionary<string, string[]> movementOptions = new()
    {
        {"Exit", ["exit"]},
        {"East", ["move east", "east", "e"]},
        {"West", ["move west", "west", "w"]},
        {"North", ["move north", "north", "n"]},
        {"South", ["move south", "south", "s"]},
    };

    public class ChangeUserOptions //Used for Obstacle Map
    {
        public void AddShooting(int size)
        {
            if (size != 4)
            {
                Dictionary<string, string[]> shootingOptions = new()
                {
                    {"Shoot South", ["shoot south"]},
                    {"Shoot North", ["shoot north"]},
                    {"Shoot East", ["shoot east"]},
                    {"Shoot West", ["shoot west"]}
                };
                foreach(KeyValuePair<string, string[]> items in shootingOptions) movementOptions.Add(items.Key, items.Value);
            }
        }
        
        public void AddFountain()
        {
            movementOptions.Add("Activate Fountain", ["activate fountain"]);
        }
    }
    public void PrintMessages()
    {
        for (int length = 0; length < Console.WindowWidth; length ++)
        {
            Console.Write("-");
        }
        if (GameLoop.movement != null)
        {
            Console.WriteLine($"You are in the room at {GameLoop.movement.location.x}, {GameLoop.movement.location.y}");
        }
    }

    public string ReadInput(string message, Dictionary<string, string[]> dict)
    {
        Console.Write(message);
        string? input;
        //Get correct input
        input = Console.ReadLine();
        input = CheckInput(input, dict);

        return input;
    }

    public string CheckInput(string? input,  Dictionary<string, string[]> options)
    {
        bool match = false;
        //loop through all the possible options to make sure the input is valid
        foreach (KeyValuePair<string, string[]> item in options)
        {
            foreach (string val in item.Value) if (input != null && val == input.ToLower()) 
            {
                return item.Key;
            }
        }
        if (match == false || input == null) return ReadInput("Please enter a valid input. ", options);
        else return input;
    }

    public int WorldSize(string message)
    {
        Dictionary<string, string[]> validInputs = new Dictionary<string, string[]>
        {
            {"Exit", ["Exit", "exit"]},
            {"4x4", ["4x4", "4"]},
            {"6x6", ["6x6", "6"]},
            {"8x8", ["8x8", "8"]}
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

    public static MapType WorldType(string? input, bool test = false)
    {
        if (input!= null && input.ToLower() == "obstacle") return MapType.Obstacle;
        else if (input != null && input.ToLower() == "monster") return MapType.Monster;
        else if (input != null && input.ToLower() == "exit") return MapType.Exit;
        else if (test) return MapType.Test;
        else
        {
            Console.Write("Please enter a valid input (obstacle or monster): ");
            return WorldType(Console.ReadLine());
        }
    }
}


public interface IMonster
{
    string Name { get; }
    string Type { get; }
    int Health { get; }
    static int Defense { get; }
    static string[] NameList => File.ReadAllLines("names.txt");
    static string[] DescriptorList => File.ReadAllLines("adjectives.txt");
    static string[] PossibleItems => File.ReadAllLines("items.txt");
    static string[] UsefulItems => 
    [
        "Small health potion (restores 5 hp)", 
        "Large heatlth potion (restores 10 hp)", 
        "Chain mail (increases armor by 5)", 
        "Armor piece (increases armor by 10)",
        "Sword"
    ];
    
    public static Random Rand => new();

    int Attack();
    void TakeDamage(int damage);
    bool IsAlive();
}

public class Amarok : IMonster
{
    static Random Rand => IMonster.Rand;
    public string Name { get; set; }
    public int Health { get; set; } = 40;
    public static int Defense => 8;
    public static int Level => Rand.Next(2, 5);
    public static (string, int)? Item => GenerateItem();
    public string Type => "Amarok";

    static string[] NameList => IMonster.NameList;
    static string[] DescriptorList => IMonster.DescriptorList;
    static string[] PossibleItems => IMonster.PossibleItems;
    static string[] UsefulItems => IMonster.UsefulItems;


    public Amarok(Dictionary<string, IMonster> MonsterList)
    {
        Name = GenerateName(MonsterList);
    }
    public int Attack()
    {
        return Rand.Next(5, 10);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Console.WriteLine($"The {Name} takes {damage} damage!");
    }

    public bool IsAlive()
    {
        return Health > 0;
    }

    string GenerateName(Dictionary<string, IMonster> monsters)
    {
        //Dictionary<string, Monster> MonsterList = MonsterList;
        string name = NameList[Rand.Next(NameList.Length)] + " the " + DescriptorList[Rand.Next(DescriptorList.Length)];
        
        if (monsters.Count == 0) return name;
        //Make each name unique
        else foreach (string monsterName in monsters.Keys) if (monsterName == name) return GenerateName(monsters);
        return name;
    }

    static (string, int)? GenerateItem()
    {
        if (Rand.NextDouble() < 0.25)
        {
            if (Rand.NextDouble() < 0.75)
            {
                return (UsefulItems[Rand.Next(UsefulItems.Length)], Rand.Next(1,5));
            }
            else return (PossibleItems[Rand.Next(PossibleItems.Length)], 0);
        }
        else return null;
    }

}

public class Goblin : IMonster
{
    public string Name { get; set; }
    public string Type => "Goblin";
    public int Health { get; set; }
    public static int Defense => 4;
    public (string, int)? Item => GenerateItem();
    public static int Level => Rand.Next(1,5);
    static Random Rand => IMonster.Rand;
    static string[] NameList => IMonster.NameList;
    static string[] DescriptorList => IMonster.DescriptorList;
    static string[] PossibleItems => IMonster.PossibleItems;
    static string[] UsefulItems => IMonster.UsefulItems;

    public Goblin(Dictionary<string, IMonster> monsters)
    {
        Name = GenerateName(monsters);
        Health = Rand.Next(15, 25);
    }

    public int Attack()
    {
        return 0;
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    public bool IsAlive()
    {
        return Health <= 0;
    }

    static string GenerateName(Dictionary<string, IMonster> monsters)
    {
        //Dictionary<string, Monster> MonsterList = MonsterList;
        string name = NameList[Rand.Next(NameList.Length)];
        
        if (monsters.Count == 0) return name;
        //Make each name unique
        else foreach (string monsterName in monsters.Keys) if (monsterName == name) return GenerateName(monsters);
        return name;
    }

    static (string, int)? GenerateItem()
    {
        if (Rand.NextDouble() < 0.25)
        {
            if (Rand.NextDouble() < 0.75)
            {
                return (UsefulItems[Rand.Next(UsefulItems.Length)], Rand.Next(1,5));
            }
            else return (PossibleItems[Rand.Next(PossibleItems.Length)], 0);
        }
        else return null;
    }
}

public class Maelstrom : IMonster
{
    public string Name {get; set;}
    public string Type => "Maelstrom";
    public int Health { get; set; } = 30;
    public static int Defense => 4;
    public (string, int)? Item = null;
    public static int Level => Rand.Next(1,5);
    static Random Rand => IMonster.Rand;
    public Maelstrom(Dictionary<string, IMonster> monsters)
    {
        Name = GenerateName(monsters);
    }
    public int Attack()
    {
        return 0;
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
    public bool IsAlive()
    {
        return Health <= 0;
    }
    
    static string GenerateName(Dictionary<string, IMonster> monsters)
    {
        string[] NameList = IMonster.NameList;
        //Dictionary<string, Monster> MonsterList = MonsterList;
        string name = NameList[Rand.Next(NameList.Length)];
        
        if (monsters.Count == 0) return name + " Melstrom";
        //Make each name unique
        else foreach (string monsterName in monsters.Keys) if (monsterName == name) return GenerateName(monsters);
        return name;
    }
}