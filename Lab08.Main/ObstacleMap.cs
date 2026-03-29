namespace Lab08;

public class Obstacles
{
    public static Dictionary<(int, int), string> RandomizeObstacles(int size, Dictionary<(int x, int y), string> worldGrid)
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