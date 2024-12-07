using Gtk;

public class Emulator
{
    /*
        Gameboy Components
    */
    private Cartridge _cartridge;
    private MMU _mmu;
    private PPU _ppu;
    private CPU _cpu;
    private Timer _timer;
    private Keyboard _keyboard;

    /*
        Emulator State
    */
    private bool _running = false;
    public Emulator(string filePath, DrawingArea drawingArea, Keyboard keyboard)
    {
        _cartridge = new Cartridge(filePath);
        _cartridge.Load();
        ICartridgeType mbc = _cartridge.GetMBC();
        _mmu = new MMU(ref mbc);
        _cpu = new CPU(ref _mmu);
        _ppu = new PPU(ref _mmu, ref drawingArea);
        _timer = new Timer(ref _mmu);
        _keyboard = keyboard;
        _keyboard.Init(ref _mmu);

        _running = true;

        Task t = Task.Run(() => Run());
    }

    public async Task Run()
    {
        int cycles = 0;
        int returnCycles = 0;
        while (_running)
        {
            while (cycles < 70224) // 一幀需要的時鐘週期數
            {
                returnCycles = _cpu.Step();
                cycles += returnCycles;

                // 更新 Timer
                _timer.UpdateDIV(returnCycles);
                _timer.UpdateTIMA(returnCycles);

                // 更新 PPU
                _ppu.Update(returnCycles);

                // 更新鍵盤輸入
                _keyboard.Update();

                // 中斷處理
                for (int i = 0; i < 5; i++)
                {
                    if ((((_mmu.IERegister & _mmu.IFRegister) >> i) & 0x1) == 1)
                    {
                        _cpu.Interrupt(i);
                    }
                }
                _cpu.UpdateIME();
            }

            cycles -= 70224;
        }
    }
}