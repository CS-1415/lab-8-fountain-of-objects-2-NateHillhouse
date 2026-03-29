using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Lab08;

public class Movement
{
    public (int x, int y) location = (1, 1);
    public Dictionary<(int x, int y), string> worldGrid = [];
    public int arrows = 5;
    static readonly Random Rand = new();
    public Dictionary<string, IMonster> MonsterList = [];

    public Movement(int size)
    {
        //Create world upon class initialization
        (int x, int y) fountain = (Rand.Next(1, size+1), Rand.Next(1, size+1));
        Console.WriteLine(fountain.x +"," +fountain.y);
        for (int i= 1; i<=size; i++)
        {
            for (int j = 1; j<=size; j++)
            {
                if ((i, j) == (1, 1)) worldGrid[(i, j)] = "entrance";
                else if ((j,i) == (fountain.x, fountain.y)) worldGrid[(i, j)] = "fountain";
                else worldGrid[(i,j)] = "";
                Console.Write($"{i}, {j} = {worldGrid[(i,j)]}");
            }
            Console.WriteLine();
        }
    }
    
    public void Move(GameLoop game, ref bool loop, int size, bool FountainActive, UserInterface _interface, UserInterface.ChangeUserOptions changeUserOptions)
    {
        string? movement;
        string message = $"You are in a room at {location.x}, {location.y}\rWhat do you want to do? ";
        if (size == 4) message += "(move: east, west, north, ";
        else  message += "(move or shoot: east, west, north, ";

        if (!FountainActive && UserInterface.movementOptions.ContainsKey("Activate Fountain")) message += "south, or activate fountain) ";
        else message += "or south) ";

        movement = _interface.ReadInput(message, UserInterface.movementOptions);
        
        string bounds = "You hit the wall. ";
        switch (movement)
        {
            case "East":
                if (location.x == size) Console.WriteLine(bounds);
                else location.x += 1;
                break;
            case "West":
                if (location.x == 1) Console.WriteLine(bounds);
                else location.x -= 1;
                break;
            case "North":
                if (location.y == 1) Console.WriteLine(bounds);
                else location.y -= 1;
                break;
            case "South":
                if (location.y == size) Console.WriteLine(bounds);
                else location.y += 1;
                break; 
            case "Shoot East":
                Shoot((location.x + 1, location.y));
                break; 
            case "Shoot West":
                Shoot((location.x - 1, location.y));
                break; 
            case "Shoot North":
                Shoot((location.x, location.y - 1));
                break; 
            case "Shoot South":
                Shoot((location.x, location.y + 1));
                break;
            case "Activate Fountain":
                GameLoop.FountainActive = true;
                UserInterface.movementOptions.Remove("Activate Fountain");
                break;
            case "Exit":
                loop = false;
                break;
        }
        if (loop) loop = HitObstacles(UserInterface.movementOptions, changeUserOptions);
        //if (location == (size,size)) loop = false;
    }

    public void Move() //MonsterMap
    {
        
    }

    public void HitMaelstrom()
    {
        //One space North
        if (location.y - 1 > 0) location.y --;
        //Two spaces east
        for (int i = 0; i < 2; i++) if (location.x + 1 < GameLoop.size) location.x ++;
    }

    public void Shoot((int, int) gridSquare)
    {
        if (arrows == 0) Console.WriteLine("You cannot shoot, you are out of arrows. "); 
        else if (worldGrid[gridSquare] == "Amaroks")
        {
            worldGrid[gridSquare] = "";
        }
        arrows --;
    }

    public bool HitObstacles(Dictionary<string, string[]> movementOptions, UserInterface.ChangeUserOptions changeUserOptions)
    {
        string item = worldGrid[location];
            switch (item)
            {
                case "Pit":
                    Console.WriteLine("You fell in a pit and died. ");
                    return false;
                case "Amaroks":
                    Console.WriteLine("You got eaten by an Amarok. ");
                    return false;
                case "Maelstrom":
                    HitMaelstrom();
                    Console.WriteLine($"{location.x}, {location.y}");
                    Console.WriteLine("You hit the maelstrom! ");
                    return HitObstacles(movementOptions, changeUserOptions);
                case "Fountain":
                    if (!GameLoop.FountainActive) changeUserOptions.AddFountain();
                    else 
                    {
                        movementOptions.Remove("Activate Fountain");
                        Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
                    }
                    return true;
                case "entrance":
                    if (GameLoop.FountainActive) 
                    {
                        Console.WriteLine("You have succesfully activated the fountain and exited the cave! ");
                        return false;
                    }
                    else Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance. ");
                    return true;
                default:
                return true;
            }
    }

}


public class MonsterMap
{
    public static void RandomizeMonsters(ref Dictionary<(int x, int y), string> worldGrid, Dictionary<string, IMonster> monsters)
    {
        Random rand = new();
        
        foreach (KeyValuePair<(int, int), string> gridSquare in worldGrid)
        {
            double chance = rand.NextDouble();
            if (gridSquare.Value == "" && chance < 0.33)
            {
                IMonster monster;
                double roll = rand.NextDouble();
                if (roll <.25) monster = new Amarok(monsters);
                else if (.25 <= roll && roll < .5) monster = new Maelstrom(monsters);
                else monster = new Goblin(monsters);

                worldGrid[gridSquare.Key] = monster.Name;
                monsters.Add(monster.Name, monster);
            }
        }
    }
    public static void PrintGrid(Dictionary<(int, int), string> worldGrid, int size, (int, int) location)
    {
        for (int i = 1; i <= size; i ++)
        {
            for (int j = 1; j <= size; j++)
            {
                if (worldGrid[(j,i)] != "") Console.Write(worldGrid[(j,i)]+ " ");
                else if ((j,i) == location) Console.Write("Player ");
                else Console.Write("Empty ");
            }
            Console.WriteLine();
        }
    }
    public static void SenseObstacles(Dictionary<(int, int), string> worldGrid, (int x, int y) player, int size)
    {
        (int x, int y) location = player;
        for (int x = -1; x < 2; x++)
        {
            location.x = player.x + x;
            if (location.x > 0 && location.x <= size) 
            {
                for (int y = -1; y < 2; y++)
                {
                    location.y = player.y + y;
                    if (location.y > 0 && location.y <= size) 
                    {
                        CheckForObstacles(location);
                        
                    }
                }
            }
        }
        if (worldGrid[player] == "Fountain" && !GameLoop.FountainActive) Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!"); 

        void CheckForObstacles((int, int) locationToCheck)
        {
            if (worldGrid[locationToCheck] == "Pit") Console.WriteLine("You feel a draft. There is a pit in a nearby room.");
            if (worldGrid[locationToCheck] == "Maelstrom") Console.WriteLine("You hear the growling and groaning of a maelstrom nearby."); 
            if (worldGrid[locationToCheck] == "Amaroks") Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room."); 
                                
        }
    }
}

public class Player
{
    //public readonly int level;
    public (string name, int level) weapon;
    public int health = 100;
    public int defense = 5;
    public List<string> items = [];
    Random rand = new();

    Player()
    {
        weapon = ("Basic Sword", 1);
        items.Add(weapon.name);
    }

    public int Attack()
    {
        return rand.Next(weapon.level);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Console.WriteLine($"You take {damage} damage!");
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}