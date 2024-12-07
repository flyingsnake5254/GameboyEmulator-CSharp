public class CPU
{
    // 暫存器
    public Registers Regs;

    // 指令集
    private Instructions _insc;

    // Memory
    private MMU _mmu;

    private long tick = 0;


    public CPU(ref MMU mmu)
    {
        this._mmu = mmu;
        Regs = new Registers();
        Regs.Init();
        _insc = new Instructions(ref Regs, ref _mmu);
    }

    public int Step()
    {
        
        u8 opcode = _mmu.Read(Regs.PC ++);
        if (tick >= 10000) {
            Console.WriteLine($"{tick ++, 0:D10} {opcode, 0:X2} AF:{Regs.AF, 0:X2} BC:{Regs.BC, 0:X2} DE:{Regs.DE, 0:X2} HL:{Regs.HL, 0:X2} PC:{Regs.PC, 0:X2} SP:{Regs.SP, 0:X2} {(Regs.GetFlag(Registers.Flag.Z) ? "Z" : "-")}{(Regs.GetFlag(Registers.Flag.N) ? "N" : "-")}{(Regs.GetFlag(Registers.Flag.H) ? "H" : "-")}{(Regs.GetFlag(Registers.Flag.C) ? "C" : "-")}");
        }
        else if (tick == 200)
        {
            Console.WriteLine("Stop");
        }
        
        return _insc.Execute(opcode);
    }

    public void Interrupt(int value)
    {
        _insc.Interrupt(value);
    }

    public void UpdateIME()
    {
        _insc.UpdateIME();
    }

}