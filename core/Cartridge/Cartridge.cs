public class Cartridge
{
    private u8[] _rom;
    private ICartridgeType _mbc;
    private string _filePath = "";
    public Cartridge(string filePath)
    {
        this._filePath = filePath;
    }

    public bool Load()
    {
        try 
        {
            _rom = File.ReadAllBytes(_filePath);
            SetMBC();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
        return false;
    }

    private void SetMBC()
    {
        // MBC1
        if (_rom[0x147] == 0x01) _mbc = new MBC1(_rom);
    }

    public ICartridgeType GetMBC()
    {
        return _mbc;
    }
}