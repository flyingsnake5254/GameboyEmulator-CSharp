public class Cartridge
{
    public u8[] ROM;
    private string _filePath = "";
    public Cartridge(string filePath)
    {
        this._filePath = filePath;
    }

    public bool Load()
    {
        try 
        {
            ROM = File.ReadAllBytes(_filePath);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
        return false;
    }
}