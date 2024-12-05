public class MBC1 : ICartridgeType
{
    public u8 ReadExternalRAM(u16 address)
    {
        throw new NotImplementedException();
    }

    public u8 ReadROM(u16 address)
    {
        throw new NotImplementedException();
    }

    public void WriteExternalRAM(u16 address, u8 value)
    {
        throw new NotImplementedException();
    }

    public void WriteROM(u16 address, u8 value)
    {
        throw new NotImplementedException();
    }
}