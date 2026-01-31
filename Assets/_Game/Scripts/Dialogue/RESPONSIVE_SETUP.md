# 📱 HƯỚNG DẪN SETUP UI RESPONSIVE CHO DIALOGUE SYSTEM

## ⚠️ VẤN ĐỀ CỦA BẠN

Hiện tại UI của bạn dùng **Width/Height cố định (pixels)** → Không responsive!

```
❌ SAI: Width = 1920, Height = 358.5 (chỉ fit màn 1920x1080)
✅ ĐÚNG: Dùng ANCHORS để auto scale theo mọi màn hình
```

---

## 🎯 NGUYÊN TẮC RESPONSIVE TRONG UNITY UI

### 1. Hiểu về Anchors

```
Anchors = điểm neo để UI tự động scale theo màn hình

Min (0, 0) = Góc trái-dưới màn hình
Max (1, 1) = Góc phải-trên màn hình

Ví dụ:
- Anchor (0, 0) đến (1, 0) = STRETCH ngang ở đáy màn hình
- Anchor (0, 0) đến (0, 1) = STRETCH dọc ở cạnh trái
- Anchor (0, 0) đến (1, 1) = STRETCH toàn màn hình
```

### 2. Khi nào dùng gì?

| Mục đích | Anchor | Dùng | Không dùng |
|----------|--------|------|------------|
| Full width panel | Bottom Stretch | Left, Right, Height | Width, Pos X |
| Corner element | Bottom-Left | Pos X, Pos Y, Width, Height | Left, Right, Top, Bottom |
| Text area | Stretch | Left, Right, Top, Bottom | Width, Height, Pos X, Pos Y |

---

## 📐 SETUP DIALOGUE PANEL RESPONSIVE

### BƯỚC 1: DialoguePanel (Full Width Bottom Panel)

```
DialoguePanel → Inspector → Rect Transform

1. Click vào Anchor Preset (hình vuông 4 góc)
2. GIỮ ALT + Click vào "Bottom Stretch" (hàng dưới, ô giữa)
   → Vừa set Anchor vừa set Position

Kết quả:
- Anchors: Min (0, 0), Max (1, 0)
- Pivot: (0.5, 0)
- Left: 0
- Right: 0
- Pos Y: 0
- Height: 300

GIẢI THÍCH:
✓ Min (0, 0) và Max (1, 0) → Kéo dài từ cạnh trái (0) đến cạnh phải (1)
✓ Left = 0, Right = 0 → Không margin, phủ full width
✓ Height = 300 → Chiều cao cố định
✓ Pos Y = 0 → Dính sát đáy màn hình
```

**KẾT QUẢ:** Panel sẽ tự động kéo dài theo chiều rộng màn hình!

---

### BƯỚC 2: LeftSpeakerPanel (Bottom-Left Corner)

```
LeftSpeakerPanel → Inspector → Rect Transform

1. GIỮ ALT + Click "Bottom Left" (góc dưới-trái)

Kết quả:
- Anchors: Min (0, 0), Max (0, 0)
- Pivot: (0, 0)
- Pos X: 50 (khoảng cách từ cạnh trái)
- Pos Y: 50 (khoảng cách từ cạnh dưới)
- Width: 800 (hoặc 40% màn hình)
- Height: 200

GIẢI THÍCH:
✓ Min (0, 0) và Max (0, 0) → Neo tại góc dưới-trái
✓ Pivot (0, 0) → Điểm gốc ở góc dưới-trái
✓ Pos X, Pos Y → Offset từ anchor (luôn 50px từ góc)
✓ Width, Height → Kích thước panel
```

**KẾT QUẢ:** Panel luôn ở góc dưới-trái, cách mép 50px dù màn hình to hay nhỏ!

---

### BƯỚC 3: CharacterImage (Left Portrait - Anchor Left-Center)

```
CharacterImage → Inspector → Rect Transform

1. GIỮ ALT + Click "Middle Left" (giữa cạnh trái)

Kết quả:
- Anchors: Min (0, 0.5), Max (0, 0.5)
- Pivot: (0, 0.5)
- Pos X: 20 (từ cạnh trái panel)
- Pos Y: 0 (giữa chiều cao)
- Width: 180
- Height: 180

Image Component:
- Preserve Aspect: ✓ (QUAN TRỌNG - giữ tỷ lệ ảnh)

GIẢI THÍCH:
✓ Min/Max (0, 0.5) → Neo tại giữa cạnh trái
✓ Pivot (0, 0.5) → Điểm gốc ở giữa cạnh trái
✓ Pos Y = 0 → Luôn ở giữa chiều cao panel
✓ Preserve Aspect → Ảnh không bị méo
```

---

### BƯỚC 4: NameText (Top Stretch - Full Width)

```
NameText → Inspector → Rect Transform

1. GIỮ ALT + Click "Top Stretch" (hàng trên, ô giữa)

Kết quả:
- Anchors: Min (0, 1), Max (1, 1)
- Pivot: (0.5, 1)
- Left: 220 (sau portrait + margin)
- Right: 20 (margin phải)
- Pos Y: -10 (từ cạnh trên xuống)
- Height: 40

GIẢI THÍCH:
✓ Min (0, 1) đến Max (1, 1) → Kéo dài toàn bộ chiều ngang
✓ Left = 220 → Margin trái (sau portrait 180 + 20 + 20)
✓ Right = 20 → Margin phải
✓ Text sẽ tự co giãn theo chiều rộng panel!
```

---

### BƯỚC 5: DialogueText (Full Stretch - 4 chiều)

```
DialogueText → Inspector → Rect Transform

1. GIỮ ALT + Click "Stretch" (ô giữa - full stretch)

Kết quả:
- Anchors: Min (0, 0), Max (1, 1)
- Pivot: (0.5, 0.5)
- Left: 220 (sau portrait)
- Right: 20 (margin phải)
- Top: 60 (dưới NameText: 40 + 10 + 10)
- Bottom: 20 (margin dưới)

TextMeshPro:
- Word Wrapping: ✓ (QUAN TRỌNG)
- Overflow: Page hoặc Truncate
- Auto Size: ✗ (tắt để kiểm soát)

GIẢI THÍCH:
✓ Min (0, 0) đến Max (1, 1) → Kéo dài cả 4 chiều
✓ Left/Right/Top/Bottom → Margins cố định
✓ Vùng text tự động scale theo mọi màn hình!
```

---

### BƯỚC 6: RightSpeakerPanel (Mirror - Bottom-Right)

```
1. Duplicate LeftSpeakerPanel (Ctrl+D)
2. Rename: RightSpeakerPanel

RectTransform:
- GIỮ ALT + Click "Bottom Right"

Kết quả:
- Anchors: Min (1, 0), Max (1, 0)
- Pivot: (1, 0)
- Pos X: -50 (ÂM - từ cạnh phải vào trong)
- Pos Y: 50
- Width: 800
- Height: 200
```

**CharacterImage (Right):**

```
- GIỮ ALT + Click "Middle Right"
- Anchors: Min (1, 0.5), Max (1, 0.5)
- Pivot: (1, 0.5)
- Pos X: -20 (ÂM)
- Pos Y: 0
- Width: 180
- Height: 180
```

**NameText (Right):**

```
- GIỮ ALT + Click "Top Stretch"
- Left: 20 (đảo ngược)
- Right: 220 (sau portrait bên phải)
- Pos Y: -10
- Height: 40
- Alignment: RIGHT, Center ← ĐỔI SANG PHẢI
```

**DialogueText (Right):**

```
- GIỮ ALT + Click "Stretch"
- Left: 20
- Right: 220
- Top: 60
- Bottom: 20
- Alignment: RIGHT, Top ← ĐỔI SANG PHẢI
```

---

## 🎨 OPTIONAL: SỬ DỤNG % THAY VÌ PIXELS

### DialoguePanel Height = 25% màn hình

```
DialoguePanel:
- Anchors: Min (0, 0), Max (1, 0.25) ← Từ 0% đến 25% chiều cao
- Left: 0
- Right: 0
- Top: 0
- Bottom: 0

→ Panel sẽ chiếm 25% chiều cao màn hình, full width!
```

### LeftSpeakerPanel Width = 40% màn hình

```
LeftSpeakerPanel:
- Anchors: Min (0, 0), Max (0.4, 0.4) ← 40% width/height từ góc
- Pos X: 50
- Pos Y: 50
- Left/Right/Top/Bottom: 0 (auto fill)

→ Panel luôn chiếm 40% màn hình!
```

---

## ✅ CHECKLIST RESPONSIVE

Kiểm tra lại từng element:

### DialoguePanel

- [ ] Anchor: Bottom Stretch (0,0) → (1,0)
- [ ] Left = 0, Right = 0
- [ ] KHÔNG có Width cố định
- [ ] Height = 300 hoặc dùng Max Y = 0.25

### LeftSpeakerPanel

- [ ] Anchor: Bottom-Left (0,0) → (0,0)
- [ ] Pivot: (0, 0)
- [ ] Pos X/Y dương (+50)
- [ ] Width/Height cố định HOẶC dùng Anchor Max

### CharacterImage (Left)

- [ ] Anchor: Middle-Left (0,0.5) → (0,0.5)
- [ ] Pivot: (0, 0.5)
- [ ] Preserve Aspect: ✓
- [ ] Width = Height (vuông)

### NameText (Left)

- [ ] Anchor: Top Stretch (0,1) → (1,1)
- [ ] Left/Right margins
- [ ] KHÔNG có Width/Pos X
- [ ] Height cố định

### DialogueText (Left)

- [ ] Anchor: Stretch (0,0) → (1,1)
- [ ] Left/Right/Top/Bottom margins
- [ ] KHÔNG có Width/Height/Pos X/Pos Y
- [ ] Word Wrapping: ✓

### RightSpeakerPanel

- [ ] Anchor: Bottom-Right (1,0) → (1,0)
- [ ] Pivot: (1, 0)
- [ ] Pos X ÂM (-50)
- [ ] CharacterImage ở phải với Pos X ÂM
- [ ] Text Alignment: RIGHT

---

## 🧪 TEST RESPONSIVE

### Cách test trong Unity Editor

1. Mở **Game View**
2. Click dropdown ở góc trên-trái (default "Free Aspect")
3. Test các resolution:
   - 1920x1080 (Full HD)
   - 1280x720 (HD)
   - 3840x2160 (4K)
   - 1024x768 (4:3)
   - 2560x1440 (2K)

4. Check:
   - ✓ DialoguePanel phủ full width?
   - ✓ LeftPanel luôn ở góc trái?
   - ✓ RightPanel luôn ở góc phải?
   - ✓ Portrait không bị méo?
   - ✓ Text không bị tràn ra ngoài?

---

## 🔧 TROUBLESHOOTING

### Panel bị lệch khi đổi màn hình

```
→ Check Anchor chưa đúng
→ Dùng ALT khi click Anchor Preset để set luôn Position
```

### Text bị tràn ra ngoài

```
→ Check Word Wrapping: ✓
→ Check Overflow: Page hoặc Truncate
→ Giảm Font Size hoặc tăng vùng text
```

### Portrait bị méo

```
→ Image → Preserve Aspect: ✓
→ Sử dụng sprite vuông (512x512, 1024x1024)
```

### Panel không full width

```
→ Anchor PHẢI là Stretch: Min (0,y) Max (1,y)
→ Left = 0, Right = 0
→ KHÔNG set Width!
```

---

## 📊 SO SÁNH: CŨ VS MỚI

### Setup cũ (KHÔNG responsive)

```
DialoguePanel:
  Width: 1920 ← Chỉ đúng màn 1920px
  Height: 358.5
  Pos X: 960
  Pos Y: 0
```

### Setup mới (RESPONSIVE)

```
DialoguePanel:
  Anchor: (0,0) → (1,0) ← Auto scale mọi màn hình
  Left: 0
  Right: 0
  Height: 300
  Pos Y: 0
```

**Kết quả:**

- ✅ Màn 1920x1080 → Panel 1920px width
- ✅ Màn 1280x720 → Panel 1280px width
- ✅ Màn 3840x2160 → Panel 3840px width

---

## 🎯 QUICK FIX CHO UI HIỆN TẠI CỦA BẠN

### 1. DialoguePanel

```
Inspector → Rect Transform:
1. GIỮ ALT + Click "Bottom Stretch"
2. Set Left = 0
3. Set Right = 0
4. Set Height = 300
```

### 2. Mỗi child element

```
- Portrait: ALT + "Middle Left" hoặc "Middle Right"
- NameText: ALT + "Top Stretch"
- DialogueText: ALT + "Stretch"
```

### 3. Test ngay

```
Game View → Thử các resolution khác nhau
```

---

🎉 **DONE!** UI giờ sẽ responsive trên mọi màn hình từ mobile đến 4K!
