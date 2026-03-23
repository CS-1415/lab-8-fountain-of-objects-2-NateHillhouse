public class Obstacles
{
    public static Dictionary<(int, int), string> RandomizeObstacles(int size, ref Dictionary<(int x, int y), string> worldGrid)
    {
        Random rand = new();
        Dictionary<string, int> obstacles = new Dictionary<string, int>
        {
            {"Pit", 0},
            {"Maelstroms", 0},
            {"Amaroks", 0},
            {"Fountain", 1}
        };
        switch (size)
        {
            case 4:
                obstacles["Fountain"] = 1;
                obstacles["Pit"] = 1;
                break;
            case 6:
                obstacles["Fountain"] = 1;
                obstacles["Pit"] = 2;
                obstacles["Maelstrom"] = 1;
                obstacles["Amaroks"] = 2;
                break;
            case 8:
                obstacles["Fountain"] = 1;
                obstacles["Pit"] = 4;
                obstacles["Maelstrom"] = 2;
                obstacles["Amaroks"] = 3;
                break;
        }
        
        List<(int x, int y)> usedSpaces = new List<(int x, int y)>();
        foreach (KeyValuePair<string, int> pair in obstacles)
        {
            while (obstacles[pair.Key] > 0)
            {
                (int x, int y) location = Randomize(usedSpaces);
                while (worldGrid[location] != "") 
                {
                    location = Randomize(usedSpaces);
                }
                worldGrid[location] = pair.Key;
                obstacles[pair.Key] --;
            } 
        }
        
        return worldGrid;


        (int, int) Randomize(List<(int x, int y)> spaces)
        {
            int x = rand.Next(1,size+1);
            int y = rand.Next(1,size+1);
            
            foreach ((int x, int y) item in spaces)
            {
                Console.Write($"{item.x}, {item.y}");
            }
            if (spaces.Contains((x, y))) return Randomize(spaces);
            return (x, y);
        }


    }
    public void PrintGrid(Dictionary<(int, int), string> worldGrid, int size, (int, int) location)
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
        if (worldGrid[player] == "Fountain" && !GameLoop.fountainActive) Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!"); 

        void CheckForObstacles((int, int) locationToCheck)
        {
            if (worldGrid[locationToCheck] == "Pit") Console.WriteLine("You feel a draft. There is a pit in a nearby room.");
            if (worldGrid[locationToCheck] == "Maelstrom") Console.WriteLine("You hear the growling and groaning of a maelstrom nearby."); 
            if (worldGrid[locationToCheck] == "Amaroks") Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room."); 
                                
        }
    }
}

public class Movement
{
    public (int x, int y) location = (1, 1);
    public Dictionary<(int x, int y), string> worldGrid = new();
    public int arrows = 5;

    public Movement(int size)
    {
        //Create world upon class initialization
        for (int i= 1; i<=size; i++)
        {
            for (int j = 1; j<=size; j++)
            {
                worldGrid.Add((j, i), "");
                switch (j,i)
                {
                    case (1,1):
                        worldGrid[(j, i)] = "entrance";
                        break;
                }
            }
        }
    }
    
    public void Move(GameLoop game, ref bool loop, int size, bool fountainActive, UserInterface _interface, UserInterface.ChangeUserOptions changeUserOptions)
    {
        string? movement;
        string message = $"You are in a room at {location.x}, {location.y}\rWhat do you want to do? ";
        if (size == 4) message += "(move: east, west, north, ";
        else  message += "(move or shoot: east, west, north, ";

        if (!fountainActive && UserInterface.movementOptions.ContainsKey("Activate Fountain")) message += "south, or activate fountain) ";
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
                GameLoop.fountainActive = true;
                UserInterface.movementOptions.Remove("Activate Fountain");
                break;
            case "Exit":
                loop = false;
                break;
        }
        if (loop) loop = HitObstacles(UserInterface.movementOptions, changeUserOptions);
        //if (location == (size,size)) loop = false;
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

    public bool HitObstacles(Dictionary<string, List<string>> movementOptions, UserInterface.ChangeUserOptions changeUserOptions)
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
                    if (!GameLoop.fountainActive) changeUserOptions.AddFountain();
                    else 
                    {
                        movementOptions.Remove("Activate Fountain");
                        Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
                    }
                    return true;
                case "entrance":
                    if (GameLoop.fountainActive) 
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


