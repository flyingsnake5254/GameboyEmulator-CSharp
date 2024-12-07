public class MMU
{
    /*
        0000-3FFF : ROM Bank 0
        4000-7FFF : ROM Bank 1~N
        8000-9FFF : VRAM
        A000-BFFF : ERAM
        C000-DFFF : WRAM
        E000-FDFF : Echo RAM (禁用)
        FE00-FE9F : OAM
        FEA0-FEFF : Not Usable (禁用)
        FF00-FF7F : IO Registers
        FF80-FFFE : HRAM
        FFFF : IE Register
    */

    public u8 IERegister;
    public u8 IFRegister { get { return IO[0x0F]; } set { IO[0x0F] = value; }}
    public u8 LY { get { return IO[0x44]; } set { IO[0x44] = value; }}
    public u8 LCDC { get { return IO[0x40]; } set { IO[0x40] = value;}}
    public u8 STAT { get { return IO[0x41]; } set { IO[0x41] = value; }}
    public u8 SCX { get { return IO[0x43]; } set { IO[0x43] = value; }}
    public u8 SCY { get { return IO[0x42]; } set { IO[0x42] = value; }}
    public u8 BGP { get { return IO[0x47]; } set { IO[0x47] = value; }}
    public u8 WY { get { return IO[0x4A]; } set { IO[0x4A] = value; }}
    public u8 WX { get { return IO[0x4B]; } set { IO[0x4B] = value; }}
    public u8 OBP1 { get { return IO[0x49]; } set { IO[0x49] = value; }}
    public u8 OBP0 { get { return IO[0x48]; } set { IO[0x48] = value; }}
    public u8 LYC { get { return IO[0x45]; } set { IO[0x45] = value; }}
    public u8 DIV { get { return IO[0x04]; } set { IO[0x04] = value; }}
    public u8 TAC { get { return IO[0x07]; } set { IO[0x07] = value; }}
    public u8 TIMA { get { return IO[0x05]; } set { IO[0x05] = value; }}
    public u8 JOYPAD { get { return IO[0x00]; } set { IO[0x00] = value; }}

    public ICartridgeType _mbc;
    public u8[] VRAM = new u8[0x9FFF - 0x8000 + 1];
    public u8[] WRAM = new u8[0xDFFF - 0xC000 + 1];
    public u8[] OAM = new u8[0xFE9F - 0xFE00 + 1];
    public u8[] IO = new u8[0xFF7F - 0xFF00 + 1];
    public u8[] HRAM = new u8[0xFFFE - 0xFF80 + 1];
    

    public MMU(ref ICartridgeType mbc)
    {
        this._mbc = mbc;
        IOInit();
    }
    
    public u8 Read(u16 address)
    {
        // ROM
        if (address <= 0x7FFF) return _mbc.ReadROM(address);
        // VRAM
        else if (address <= 0x9FFF) return VRAM[address & (0x9FFF - 0x8000)];
        // ERAM
        else if (address <= 0xBFFF) return _mbc.ReadExternalRAM(address);
        // WRAM
        else if (address <= 0xDFFF) return WRAM[address & (0xDFFF - 0xC000)];
        // Echo RAM
        else if (address <= 0xFDFF) { /* Echo Ram - 禁用 */ }
        // OAM
        else if (address <= 0xFE9F) return OAM[address & (0xFE00 - 0xFE9F)];
        // Not Usable
        else if (address <= 0xFEFF) { /* Not Usable - 禁用 */ }
        // IO
        else if (address <= 0xFF7F) return IO[address & (0xFF7F - 0xFF00)];
        // HRAM
        else if (address <= 0xFFFF) return HRAM[address & (0xFFFE - 0xFF80)];
        // IE Register
        else if (address == 0xFFFF) return IERegister;

        return 0;
    }

    public void Write(u16 address, u8 value)
    {
        // ROM
        if (address <= 0x7FFF) _mbc.WriteROM(address, value);
        // VRAM
        else if (address <= 0x9FFF) VRAM[address & (0x9FFF - 0x8000)] = value;
        // ERAM
        else if (address <= 0xBFFF) _mbc.WriteExternalRAM(address, value);
        // WRAM
        else if (address <= 0xDFFF) WRAM[address & (0xDFFF - 0xC000)] = value;
        // Echo RAM
        else if (address <= 0xFDFF) { /* Echo Ram - 禁用 */ }
        // OAM
        else if (address <= 0xFE9F) OAM[address & (0xFE9F - 0xFE00)] = value;
        // Not Usable
        else if (address <= 0xFEFF) { /* Not Usable - 禁用 */ }
        // IO
        else if (address <= 0xFF7F) 
        {
            // DMA - 將內存中 0xXX00 - 0xXX9F 的內容複製到 OAM，其中 XX 為 value
            if (address == 0xFF46)
            {
                for (int i = 0 ; i < OAM.Length ; i ++)
                {
                    OAM[i] = Read((u16) ((value << 8) + i));
                }
            }
            else
            {
                // DIV - 寫入時，將值設成 0
                if (address == 0xFF04) value = 0;
                // LY - 寫入時，將值設成 0
                else if (address == 0xFF44) value = 0;
                // IF Register - 高三位無用，設值為 1
                else if (address == 0xFF0F) value = (u8) (value | 0b11100000);
                IO[address & 0x7F] = value;
            }
        }
        // HRAM
        else if (address <= 0xFFFE) HRAM[address & (0xFFFE - 0xFF80)] = value;
        // IE Register
        else if (address == 0xFFFF) IERegister = value;

    }


    public u16 Read16(u16 address)
    {
        /*
            value (16 bit) :
                高位：from "address + 1"
                低位: from "address"
        */
        return (u16)(Read((u16)(address + 1)) << 8 | Read(address));
    }


    public void Write16(u16 address, u16 value)
    {
        /*
            value (16bit):
                低八位寫入位置：address
                高八位寫入位置：address + 1
        */
        u8 high = (u8) (value >> 8);
        u8 low = (u8) value;
        Write((u16) (address + 1), high);
        Write(address, low);
    }


    public void IOInit()
    {
        // IO[0x05] = 0x00; // TIMA
        // IO[0x06] = 0x00; // TMA
        // IO[0x07] = 0x00; // TAC
        IO[0x4D] = 0xFF;
        IO[0x10] = 0x80; // NR10
        IO[0x11] = 0xBF; // NR11
        IO[0x12] = 0xF3; // NR12
        IO[0x14] = 0xBF; // NR14
        IO[0x16] = 0x3F; // NR21
        // IO[0x17] = 0x00; // NR22
        IO[0x19] = 0xBF; // NR24
        IO[0x1A] = 0x7F; // NR30
        IO[0x1B] = 0xFF; // NR31
        IO[0x1C] = 0x9F; // NR32
        IO[0x1E] = 0xBF; // NR33
        IO[0x20] = 0xFF; // NR41
        // IO[0x21] = 0x00; // NR42
        // IO[0x22] = 0x00; // NR43
        IO[0x23] = 0xBF; // NR30
        IO[0x24] = 0x77; // NR50
        IO[0x25] = 0xF3; // NR51
        IO[0x26] = 0xF1; // NR52 (GB模式), 可選 0xF0 (SGB模式)
        IO[0x40] = 0x91; // LCDC
        // IO[0x42] = 0x00; // SCY
        // IO[0x43] = 0x00; // SCX
        // IO[0x45] = 0x00; // LYC
        IO[0x47] = 0xFC; // BGP
        IO[0x48] = 0xFF; // OBP0
        IO[0x49] = 0xFF; // OBP1
        // IO[0x4A] = 0x00; // WY
        // IO[0x4B] = 0x00; // WX
        // IO[0xFF] = 0x00; // IE (Interrupt Enable)
    }

    public void RequestInterrupt(u8 b) {
        IO[0x0F] |= (u8)(1 << b);
    }

    public u8 ReadVRAM(u16 address)
    {
        return VRAM[address & 0x1FFF];
    }

    public u8 ReadOAM(int address)
    {
        return OAM[address];
    }

}