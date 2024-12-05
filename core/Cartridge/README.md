## Memory Map
| Address | Description | 讀寫規則 |
|---|---|---|
| 0x0000 - 0x3FFF | 固定 ROM ( Bank 0 ) |
| 0x4000 - 0x7FFF | 可切換 ROM ( Bank 1 - n ) |
| 0x8000 - 0x9FFF | VRAM |
| 0xA000 - 0xBFFF | ERAM |
| 0xC000 - 0xDFFF | WRAM | 無限制 |
| 0xE000 - 0xFDFF | Echo RAM | 任天堂說**禁用**此區段 |
| 0xFE00 - 0xFE9F | OAM |
| 0xFEA0 - 0xFEFF | Not Usable | 任天堂說**禁用**此區段 |
| 0xFF00 - 0xFF7F | I/O Registers |
| 0xFF80 - 0xFFFE | HRAM | 無限制 |
| 0xFFFF | Interrupt Enable register (IE) |

---

#### 控制 VBlank 和 LCD 狀態的寄存器

##### LCD 控制寄存器 (LCDC)  
- 地址：0xFF40
- 位元定義  

  | 位元 (bit) | 名稱 | 功能描述 |
  |---|---|---|
  | 0 | BG & Window Display Enable | 控制背景和窗口顯示<br>`0 = 禁用`<br>`1 = 啟用` |
  | 1 | OBJ (Sprite) Display Enable | 控制精靈顯示<br>`0 = 禁用`<br>`1 = 啟用精靈` |
  | 2 | OBJ (Sprite) Size | 控制精靈大小<br>`0 = 8x8`<br>`1 = 8x16` |
  | 3 | BG Tile Map Display Select | 選擇背景顯示的 Tile Map<br>`0 = 0x9800-0x9BFF`<br>`1 = 0x9C00-0x9FFF` |
  | 4 | BG & Window Tile Data Select | 選擇背景和窗口的 Tile Data 區域<br>`0 = 0x8800-0x97FF`<br>`1 = 0x8000-0x8FFF`|
  | 5 | Window Enable | 控制窗口功能<br>`0 = 禁用`<br>`1 = 啟用`|
  | 6 | Window Tile Map Area | 選擇窗口顯示的 Tile Map<br>`0 = 0x9800-0x9BFF`<br>`1 = 0x9C00-0x9FFF`|
  | 7 | LCD & PPU enable | `0 = 禁用` <br>`1 = 啟用`  |

##### LCD 狀態寄存器 (STAT)
- 地址：0xFF41
- 位元定義  

  | 6 | 5 | 4 | 3 | 2 | 1 - 0 |
  |---|---|---|---|---|---|
  |`1: 啟用 LYC=LY 中斷`<br>
---


## Cartridge Header
| Address | Content |
|---|---|
|0x0147|Cartridge type|

---

## MBC1

#### 特點
- 支援更大的 ROM：最大 2MB（128個 16KB 的 ROM 銀行）。
- 支援外部 RAM：最大 32KB（4個 8KB 的 RAM 銀行）。
- 切換模式：提供 ROM 模式 和 RAM 模式，影響銀行切換的行為。

#### 地址空間劃分
| Address | Content | Remark |
|---|---|---|
|**固定** ROM 區域 (Bank 0)|0x0000-0x3FFF|
|**可切換** ROM 區域 (Bank 1 - N)|0x4000-0x7FFF|
|外部 RAM 區域|0xA000-0xBFFF|外部 RAM 僅在啟用後可讀寫|



#### 控制暫存器和寫入規則
- ERAM 啟用條件  
  1. ERAM 只在 `ERAM_ENABLE = true` 時，可以讀寫
  2. 啟用/禁用 ERAM
  ```CSharp
    // address 數值須介於 0x0000 ~ 0x1FFF
    WriteROM(address, 0x0A); // 啟用外部 RAM --> ERAM_ENABLE = true
    WriteROM(address, 0x00); // 禁用外部 RAM --> ERAM_ENABLE = false
  ```

---
## Reference 
1. [Pan Docs - Cartridge type](https://gbdev.io/pandocs/The_Cartridge_Header.html#0147--cartridge-type)
2. [Pan Docs - MBC1](https://gbdev.io/pandocs/MBC1.html#mbc1)
