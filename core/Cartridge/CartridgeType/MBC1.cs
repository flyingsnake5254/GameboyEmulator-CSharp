public class MBC1 : ICartridgeType
{
    /*
        RAM size:
            0 - None
            1 - 16kBit = 2kB = 1 bank
            2 - 64kBit = 8kB = 1 bank
            3 - 256kBit = 32kB = 4 banks (MBC1 最大 4 banks)
    */
    private bool _ERAMEnable = false;
    private enum eBankMode { ROM, RAM }
    private eBankMode _bankMode = eBankMode.ROM;
    private int _romBank = 1;
    private int _ramBank;
    private u8[] _eram = new u8[0x8000];
    private u8[] _rom;

    public MBC1(u8[] rom)
    {
        _rom = rom;
    }
    
    
    public u8 ReadROM(u16 address)
    {
        // ROM Bank 0
        if (address <= 0x3FFF)
        {
            return _rom[address];
        }
        // ROM Bank 1-N
        else if (address <= 0x7FFF)
        {
            /*
                實際地址 = Bank 起始位置 + Bank 內偏移量
                起始位置 = (1 Bank Size -> 0x4000) * BankNumber
                偏移量 = address & 0x3FFF --> 將偏一輛限制在 0x0000 ~ 0x3FFF 之間，也就是一個 Bank 的大小
            */
            u16 baseAddress = (u16) (0x4000 * _romBank);
            u16 addressBias = (u16) (address & 0x3FFF);
            u16 actualAddress = (u16) (baseAddress + addressBias);
            return _rom[actualAddress];
        }

        return 0;
    }

    public void WriteROM(u16 address, u8 value)
    {
        // 設置 ERAM Flag
        if (address <= 0x1FFF)
        {
            if (value == 0x0A) _ERAMEnable = true; // 啟用
            else _ERAMEnable = false; // 禁用
        }
        // ROM Bank Number
        else if (address < 0x3FFF)
        {
            // 只有低 5 bits 為有效位
            value = (u8) (value & 0b00011111);

            if (value == 0x00)
            {
                value += 1;
            }

            _romBank = value;
        }
        // 設置 Bank 的高2位
        else if (address < 0x5FFF)
        {
            if (_bankMode == eBankMode.ROM)
            {
                // 取得低2位
                value = (u8) (value & 0b00000011);
                // 重新修正 ROM Bank Number
                _romBank = _romBank | (value << 5);
            }
            else if (_bankMode == eBankMode.RAM)
            {
                // 設置 RAM Bank Number
                _ramBank = value & 0b00000011;
            }
        }
        // ROM Bank, RAM Bank 切換 (6000-7FFF)
        else if (address < 0x7FFF)
        {
            value = (u8) (value & 0b00000001);
            _bankMode = value == 0 ? eBankMode.ROM : eBankMode.RAM;
        }
        
    }

    public u8 ReadExternalRAM(u16 address)
    {
        // 檢查 ERAM 啟用 flag
        if (_ERAMEnable)
        {
            /*
                實際地址 = Bank 起始位置 + Bank 內偏移量
                起始位置 = (1 Bank Size -> 0x2000) * BankNumber
                偏移量 = address & 0x1FFF --> 將偏一輛限制在 0x0000 ~ 0x1FFF 之間，也就是一個 Bank 的大小
            */
            u16 baseAddress = (u16) (0x2000 * _romBank);
            u16 addressBias = (u16) (address & 0x1FFF);
            u16 actualAddress = (u16) (baseAddress + addressBias);
            return _eram[actualAddress];
        }

        return 0xFF;
    }

    public void WriteExternalRAM(u16 address, u8 value)
    {
        if (_ERAMEnable)
        {
            /*
                實際地址 = Bank 起始位置 + Bank 內偏移量
                起始位置 = (1 Bank Size -> 0x2000) * BankNumber
                偏移量 = address & 0x1FFF --> 將偏一輛限制在 0x0000 ~ 0x1FFF 之間，也就是一個 Bank 的大小
            */
            u16 baseAddress = (u16) (0x2000 * _romBank);
            u16 addressBias = (u16) (address & 0x1FFF);
            u16 actualAddress = (u16) (baseAddress + addressBias);
            _eram[actualAddress] = value;
        }
    }

    
}