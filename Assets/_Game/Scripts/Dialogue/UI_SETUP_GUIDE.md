# 🎨 HƯỚNG DẪN SETUP UI - DYNAMIC DIALOGUE SYSTEM

## 📋 BƯỚC 1: TẠO UI STRUCTURE

### 1.1. Tạo Canvas (nếu chưa có)

```
Hierarchy → Right-click → UI → Canvas
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: 1920x1080
- Render Mode: Screen Space - Overlay
```

### 1.2. Tạo DialoguePanel

```
Canvas → Right-click → UI → Panel
Rename: "DialoguePanel"

Settings:
- Anchor: Bottom Stretch
- Height: ~300
- Pos Y: 0
- Color: Semi-transparent black (0, 0, 0, 200)
```

---

## 🎭 BƯỚC 2: TẠO LEFT SPEAKER PANEL

### 2.1. Tạo Panel

```
DialoguePanel → Right-click → UI → Panel
Rename: "LeftSpeakerPanel"

RectTransform:
- Anchor: Bottom-Left
- Pivot: (0, 0)
- Pos X: 50
- Pos Y: 50
- Width: 800
- Height: 250
```

### 2.2. Tạo Character Image (Portrait)

```
LeftSpeakerPanel → Right-click → UI → Image
Rename: "CharacterImage"

RectTransform:
- Anchor: Middle-Left
- Pivot: (0, 0.5)
- Pos X: 20
- Pos Y: 0
- Width: 200
- Height: 200

Image Component:
- Source Image: (để trống, sẽ set bằng code)
- Preserve Aspect: ✓
- Color: White
- Image Type: Simple (hoặc Sliced nếu dùng 9-slice sprite)
```

### 2.3. Tạo Name Text - RESPONSIVE

```
LeftSpeakerPanel → Right-click → UI → Text - TextMeshPro
Rename: "NameText"

RectTransform:
- Anchor: Top Stretch (giữ Alt)
  * Min: (0, 1)
  * Max: (1, 1)
  * Pivot: (0.5, 1)
  
- Left: 220 (sau portrait + margin)
- Right: 20 (margin phải)
- Pos Y: -10 (từ cạnh trên)
- Height: 40

TextMeshPro:
- Font Size: 28
- Alignment: Left, Center
- Color: Yellow (255, 220, 0)
- Font Style: Bold
- Overflow: Ellipsis (cắt text dài)

Giải thích:
✓ Top Stretch: Kéo dài theo chiều ngang panel
✓ Left/Right: Margin từ 2 cạnh (auto scale)
✓ Height cố định cho text
```

### 2.4. Tạo Dialogue Text - RESPONSIVE

```
LeftSpeakerPanel → Right-click → UI → Text - TextMeshPro
Rename: "DialogueText"

RectTransform:
- Anchor: Stretch (giữ Alt - kéo dãn cả 4 hướng)
  * Min: (0, 0)
  * Max: (1, 1)
  * Pivot: (0.5, 0.5)
  
- Left: 220 (sau portrait + margin)
- Right: 20 (margin phải)
- Top: 60 (dưới NameText)
- Bottom: 20 (margin dưới)

TextMeshPro:
- Font Size: 24
- Alignment: Left, Top
- Color: White (255, 255, 255)
- Enable Word Wrapping: ✓
- Overflow: Page (hoặc Truncate)
- Auto Size: ✗ (tắt để kiểm soát được)

Giải thích:
✓ Anchor Stretch: Auto scale theo mọi màn hình
✓ Left/Right/Top/Bottom: Margins cố định
✓ Text sẽ tự wrap và fit trong vùng responsive
```

---

## 🎭 BƯỚC 3: TẠO RIGHT SPEAKER PANEL

### 3.1. Duplicate Left Panel

```
LeftSpeakerPanel → Duplicate (Ctrl+D)
Rename: "RightSpeakerPanel"

RectTransform:
- Anchor: Bottom-Right
- Pivot: (1, 0)
- Pos X: -50
- Pos Y: 50
- Width: 800
- Height: 250
```

### 3.2. Đảo ngược Layout

```
CharacterImage:
- Anchor: Middle-Right
- Pivot: (1, 0.5)
- Pos X: -20 (đảo ngược)

NameText:
- Alignment: Right, Top
- Pos X: -240 (right)

DialogueText:
- Alignment: Right, Top
- Left: 20
- Right: 240 (đảo ngược)
```

---

## 📝 BƯỚC 4: TẠO CENTER PANEL (OPTIONAL)

```
DialoguePanel → Right-click → UI → Panel
Rename: "CenterPanel"

RectTransform:
- Anchor: Bottom Stretch
- Height: 150
- Pos Y: 50

Children:
├─ NameText (Center, Top, Yellow)
└─ DialogueText (Center, Middle, White)
```

---

## ⚙️ BƯỚC 5: ASSIGN VÀO DIALOGUECONTROLLER

### 5.1. Tạo DialogueController GameObject

```
⚠️ QUAN TRỌNG: DialogueController PHẢI nằm trên GameObject LUÔN ACTIVE!

Canvas → Right-click → Create Empty
Rename: "DialogueManager"

Add Component → DialogueController

LƯU Ý:
- KHÔNG đặt DialogueController trên DialoguePanel
- DialoguePanel sẽ được DialogueController tìm và control
- DialogueManager luôn active để có thể chạy Coroutine
```

### 5.2. Assign References

```
DialogueManager → Inspector → DialogueController Component:

Left Speaker Panel:
✓ Left Speaker Panel: DialoguePanel/LeftSpeakerPanel
✓ Left Portrait: DialoguePanel/LeftSpeakerPanel/CharacterImage
✓ Left Name Text: DialoguePanel/LeftSpeakerPanel/NameText
✓ Left Dialogue Text: DialoguePanel/LeftSpeakerPanel/DialogueText

Right Speaker Panel:
✓ Right Speaker Panel: DialoguePanel/RightSpeakerPanel
✓ Right Portrait: DialoguePanel/RightSpeakerPanel/CharacterImage
✓ Right Name Text: DialoguePanel/RightSpeakerPanel/NameText
✓ Right Dialogue Text: DialoguePanel/RightSpeakerPanel/DialogueText

Center Panel (Optional):
□ Center Panel: DialoguePanel/CenterPanel
□ Center Name Text: DialoguePanel/CenterPanel/NameText
□ Center Dialogue Text: DialoguePanel/CenterPanel/DialogueText

Settings:
✓ Type Speed: 10
✓ Active Color: White (255, 255, 255, 255)
✓ Inactive Color: Gray (128, 128, 128, 128)

⚠️ CRITICAL: DialogueManager phải LUÔN ACTIVE trong Hierarchy!
DialoguePanel có thể inactive, sẽ được DialogueController tự bật.
```

---

## 🎨 BƯỚC 6: TẠO CHARACTER PORTRAITS

### 6.1. Chuẩn bị Sprites

```
Import character sprites vào Assets/Art/Characters/
- alice_happy.png
- alice_sad.png
- bob_smile.png
- bob_angry.png
etc.

Sprite Settings:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Max Size: 512 hoặc 1024
- Pixels Per Unit: 100 (default)

LƯU Ý: Sprite này sẽ được assign vào DialogueLine.portrait
Code sẽ tự động set vào Image component khi dialogue chạy!
```

### 6.2. Crop và Resize

```
Khuyến nghị:
- Size: 512x512 hoặc 1024x1024
- Format: PNG với transparency
- Style: Bust shot (thân trên + đầu)
```

---

## 📦 BƯỚC 7: TẠO DIALOGUE DATA

### 7.1. Tạo DialogueData Asset

```
Project Window → Right-click → Create → Dialogue → New Dialogue Data
(Hoặc có thể hiện tên: "Dialogue Container" - cùng 1 script)

Nếu không thấy menu "Dialogue":
1. Đợi Unity compile xong (check góc dưới phải)
2. Restart Unity
3. Hoặc tạo script mới rồi xóa đi để force recompile

Rename asset: "Conversation_01"
```

### 7.2. Config Dialogue Lines

```
Inspector → Dialogue Data:

Lines → Size: 3

Element 0:
├─ Speaker Name: "Sarah"
├─ Portrait: girl_happy sprite
├─ Position: Left
└─ Text: "Xin chào! Mình là Sarah."

Element 1:
├─ Speaker Name: "John"
├─ Portrait: boy_smile sprite
├─ Position: Right
└─ Text: "Chào bạn! Tôi là John, rất vui được gặp."

Element 2:
├─ Speaker Name: "Sarah"
├─ Portrait: girl_happy sprite
├─ Position: Left
└─ Text: "Bạn đến đây làm gì vậy?"

Optional Settings:
✓ Auto Close: true
✓ Auto Close Delay: 1.0
```

---

## 🎮 BƯỚC 8: SETUP NPC/TRIGGER

### 8.1. Setup NPC

```
NPC GameObject → Inspector → Wraith (or custom NPC script)

Dialogue Settings:
✓ Dialogue Data: Kéo "Conversation_01" vào đây
□ Legacy Dialogue Text: (bỏ trống)
```

### 8.2. Setup Dialogue Trigger

```
Trigger GameObject → Inspector → DialogueTrigger

Dialogue Settings:
✓ Dialogue Data: Kéo "Conversation_01" vào đây
□ Legacy Dialogue Text: (bỏ trống)

Trigger Settings:
✓ Trigger Once: true
✓ Disable Player Movement: true
```

---

## ✅ BƯỚC 9: TEST

### 9.1. Disable DialoguePanel

```
DialoguePanel → Inspector → Uncheck Active
(DialogueController sẽ tự động bật khi cần)
```

### 9.2. Play Mode

```
1. Run game
2. Đi vào trigger zone hoặc nhấn E gần NPC
3. Dialogue hiện lên với:
   - Speaker name đúng
   - Portrait đúng vị trí (Left/Right)
   - Text typing animation
   - Active/Inactive highlighting
4. Nhấn Space để next
5. Dialogue tự động đóng sau dòng cuối
```

---

## 🎨 BƯỚC 10: TUỲ CHỈNH STYLE (OPTIONAL)

### 10.1. Add Background Decoration

```
LeftSpeakerPanel/RightSpeakerPanel:
- Add gradient background
- Add border frame
- Add glow effect cho active speaker
```

### 10.2. Animation

```
Add Animator:
- Slide in from bottom khi mở
- Fade out khi đóng
- Portrait bounce khi speaker thay đổi
```

### 10.3. Sound Effects

```
Add Audio Source:
- Text typing sound (tick tick)
- Speaker change sound (whoosh)
- Dialogue open/close sound
```

---

## 🐛 TROUBLESHOOTING

### Lỗi: Panel không hiện

```
✓ Check DialoguePanel inactive trong Hierarchy (đúng)
✓ Check DialogueManager LUÔN ACTIVE (quan trọng!)
✓ Check DialogueController component trên DialogueManager, KHÔNG trên DialoguePanel
✓ Check Canvas Render Mode = Screen Space Overlay
```

### Lỗi: Coroutine couldn't be started

```
✓ NGUYÊN NHÂN: DialogueController nằm trên GameObject inactive!
✓ GIẢI PHÁP: Di chuyển DialogueController sang GameObject khác LUÔN ACTIVE
✓ Tạo DialogueManager (empty GameObject) trong Canvas
✓ Add Component DialogueController vào DialogueManager
✓ Assign DialoguePanel và các references
```

### Lỗi: Portrait không hiện

```
✓ Check sprite đã import đúng (Texture Type: Sprite 2D/UI)
✓ Check DialogueLine.portrait có assign sprite
✓ Check Image component trong Inspector (Left Portrait / Right Portrait)
✓ Check Image.gameObject.SetActive = true
✓ Sprite sẽ tự động thay đổi khi dialogue chạy, không cần set manually!
```

### Lỗi: Text bị cắt

```
✓ Check TextMeshPro Overflow: Overflow
✓ Tăng Width/Height của RectTransform
✓ Giảm Font Size
✓ Enable Word Wrapping
```

### Lỗi: Position sai

```
✓ Check Anchor/Pivot settings
✓ Check Pos X/Y values
✓ Reset RectTransform nếu cần
```

---

## 📐 TEMPLATE VALUES (Copy-Paste)

### LeftSpeakerPanel

```
Anchor: Bottom-Left
Pivot: (0, 0)
Pos: (50, 50)
Size: (800, 250)
```

### LeftCharacterImage (Portrait)

```
Anchor: Middle-Left
Pivot: (0, 0.5)
Pos: (20, 0)
Size: (200, 200)
Preserve Aspect: true
Image Type: Simple
```

### LeftNameText

```
Anchor: Top Stretch
Height: 40
Left: 240, Right: 20
Top: 10
Font Size: 28
Color: #FFDC00
```

### LeftDialogueText

```
Anchor: Stretch
Left: 240, Right: 20
Top: 60, Bottom: 20
Font Size: 24
Color: #FFFFFF
```

---

## 🎯 QUICK START CHECKLIST

- [ ] Canvas created với Screen Space Overlay
- [ ] DialoguePanel created (Bottom Stretch, Height 300, INACTIVE)
- [ ] DialogueManager created (Empty GameObject, LUÔN ACTIVE)
- [ ] DialogueController component trên DialogueManager
- [ ] LeftSpeakerPanel với CharacterImage + NameText + DialogueText
- [ ] RightSpeakerPanel với CharacterImage + NameText + DialogueText (mirrored)
- [ ] CenterPanel (optional)
- [ ] DialogueController assigned tất cả references từ DialoguePanel
- [ ] DialogueData asset created với multiple lines
- [ ] Character sprites imported (512x512 PNG, Texture Type: Sprite 2D/UI)
- [ ] NPC/Trigger có DialogueData assigned
- [ ] Test trong Play Mode

⚠️ CRITICAL SETUP:

```
Canvas (ACTIVE)
├─ DialogueManager (ACTIVE) ← DialogueController component ở đây!
└─ DialoguePanel (INACTIVE) ← UI elements ở đây
    ├─ LeftSpeakerPanel
    └─ RightSpeakerPanel
```

---

**🎉 HOÀN THÀNH! Dialogue system của bạn đã sẵn sàng!**
