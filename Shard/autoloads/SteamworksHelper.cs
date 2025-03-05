namespace DefaultNamespace;
using Steamworks;
public class SteamworksHelper : Node
{
    public override void _Ready()
    {
        Console.WriteLine("Steam is running " + SteamAPI.IsSteamRunning());
        try
        {
            if (SteamAPI.Init())
            {
                Console.WriteLine("SteamworksHelper init success");
            }
            else
            {
                Console.WriteLine("SteamworksHelper init failed");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No gud: " + e);
        }
    }

    public override void _ExitTree()
    {
        try
        {
            SteamAPI.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine("Also no gud: " + e);
        }
    }
}