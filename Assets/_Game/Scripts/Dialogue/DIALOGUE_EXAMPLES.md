# 🎬 VÍ DỤ TẠO DIALOGUE - QUICK REFERENCE

## 📝 Ví dụ 1: Dialogue Đơn Giản (2 nhân vật)

### DialogueData: "Meeting_Alice_Bob"

```
Lines: 4

[0] Speaker: Alice | Position: Left | Portrait: alice_happy
    "Chào Bob! Lâu rồi không gặp."

[1] Speaker: Bob | Position: Right | Portrait: bob_smile
    "Alice! Thật bất ngờ! Khoẻ không?"

[2] Speaker: Alice | Position: Left | Portrait: alice_smile
    "Mình khoẻ! Còn bạn?"

[3] Speaker: Bob | Position: Right | Portrait: bob_happy
    "Tuyệt vời! Đi uống cà phê nhé!"
```

---

## 💬 Ví dụ 2: Dialogue Có Narrator

### DialogueData: "Quest_Introduction"

```
Lines: 5

[0] Speaker: Narrator | Position: Center | Portrait: null
    "Một ngày đẹp trời tại làng Peaceful..."

[1] Speaker: Elder | Position: Left | Portrait: elder_wise
    "Chiến binh trẻ, ta cần ngươi giúp đỡ."

[2] Speaker: Player | Position: Right | Portrait: player_neutral
    "Ngài cần gì ạ?"

[3] Speaker: Elder | Position: Left | Portrait: elder_serious
    "Rồng đen đã trở lại. Hãy ngăn chặn nó!"

[4] Speaker: Player | Position: Right | Portrait: player_determined
    "Con sẽ làm! Con hứa!"
```

---

## 🎭 Ví dụ 3: Thay Đổi Cảm Xúc (Same Speaker, Different Portraits)

### DialogueData: "Sarah_Emotions"

```
Lines: 4

[0] Speaker: Sarah | Position: Left | Portrait: sarah_happy
    "Tuyệt vời! Ngày hôm nay thật đẹp!"

[1] Speaker: Sarah | Position: Left | Portrait: sarah_shocked
    "Ôi không! Cái gì vậy?!"

[2] Speaker: Sarah | Position: Left | Portrait: sarah_sad
    "Huhu... Mọi thứ hỏng hết rồi..."

[3] Speaker: Sarah | Position: Left | Portrait: sarah_angry
    "Ai làm điều này?! Tôi sẽ không tha!"
```

---

## ⚔️ Ví dụ 4: Dialogue Combat/Boss

### DialogueData: "Boss_Encounter"

```
Lines: 6

[0] Speaker: System | Position: Center | Portrait: null
    "⚠️ WARNING: Boss Appeared!"

[1] Speaker: Dark Lord | Position: Right | Portrait: boss_laugh
    "Kaka! Ngươi dám thách thức ta?"

[2] Speaker: Player | Position: Left | Portrait: player_brave
    "Tôi sẽ đánh bại ông!"

[3] Speaker: Dark Lord | Position: Right | Portrait: boss_angry
    "Ngạo mạn! Nhận lấy sức mạnh của ta!"

[4] Speaker: Player | Position: Left | Portrait: player_defend
    "Tôi không sợ!"

[5] Speaker: System | Position: Center | Portrait: null
    "⚔️ BATTLE START!"
```

---

## 🏪 Ví dụ 5: Shop Keeper

### DialogueData: "Shop_Welcome"

```
Auto Close: false
Auto Close Delay: 0

Lines: 3

[0] Speaker: Merchant | Position: Right | Portrait: merchant_welcome
    "Xin chào! Chào mừng đến cửa hàng!"

[1] Speaker: Merchant | Position: Right | Portrait: merchant_smile
    "Tôi có nhiều đồ tốt đây!"

[2] Speaker: Merchant | Position: Right | Portrait: merchant_thinking
    "Bạn cần gì nào?"
```

---

## 🎯 Ví dụ 6: Tutorial

### DialogueData: "Tutorial_Movement"

```
Lines: 4

[0] Speaker: Guide | Position: Left | Portrait: guide_friendly
    "Chào mừng đến game! Tôi sẽ hướng dẫn bạn."

[1] Speaker: Guide | Position: Left | Portrait: guide_point
    "Dùng WASD hoặc Arrow keys để di chuyển nhé!"

[2] Speaker: Guide | Position: Left | Portrait: guide_happy
    "Thử di chuyển xung quanh đi!"

[3] Speaker: Guide | Position: Left | Portrait: guide_thumbsup
    "Tuyệt vời! Bạn làm được rồi!"
```

---

## 💡 TIPS & TRICKS

### Tip 1: Tạo Multiple Portraits cho cùng 1 nhân vật

```
Assets/Art/Characters/Alice/
├─ alice_happy.png
├─ alice_sad.png
├─ alice_angry.png
├─ alice_surprised.png
├─ alice_thinking.png
└─ alice_confused.png
```

### Tip 2: Đặt tên DialogueData có ý nghĩa

```
✓ GOOD:
- Quest_01_Introduction
- NPC_Merchant_FirstMeet
- Boss_DarkLord_Phase1

✗ BAD:
- Dialogue1
- NewDialogueData
- Untitled
```

### Tip 3: Sử dụng Center Position cho System Messages

```
- Quest notifications
- Combat messages
- Tutorial hints
- Achievement unlocks
```

### Tip 4: Auto Close Settings

```
Cutscenes: Auto Close = true, Delay = 1.5s
Shop/Quest: Auto Close = false
Tutorial: Auto Close = false
System Msg: Auto Close = true, Delay = 2s
```

### Tip 5: Portrait Size Optimization

```
Character portraits: 512x512
Simple icons: 256x256
Full illustrations: 1024x1024
```

---

## 🎨 PORTRAIT STYLES

### Style 1: Anime Style

```
- Size: 512x512
- Format: PNG transparent
- Content: Head + shoulders
- Expression: Clear, expressive eyes
```

### Style 2: Pixel Art

```
- Size: 128x128 or 256x256
- Format: PNG transparent
- Content: Chibi style head
- Expression: Simple, bold features
```

### Style 3: Realistic

```
- Size: 1024x1024
- Format: PNG transparent
- Content: Face close-up
- Expression: Subtle, natural
```

---

## 🔧 WORKFLOW SỬ DỤNG

### Bước 1: Plan Dialogue

```
1. Viết kịch bản trên giấy/docs
2. Xác định số nhân vật
3. Xác định vị trí mỗi người (L/R/C)
4. Note down cảm xúc cần thiết
```

### Bước 2: Chuẩn Bị Assets

```
1. Tạo/Import portraits
2. Crop và resize phù hợp
3. Organize vào folders
```

### Bước 3: Tạo DialogueData

```
1. Right-click → Create → Dialogue → New Dialogue Data
2. Đặt tên có ý nghĩa
3. Set số lượng lines
```

### Bước 4: Fill Data

```
1. Copy text từ kịch bản
2. Assign portraits
3. Set positions
4. Config auto-close
```

### Bước 5: Test

```
1. Attach vào NPC/Trigger
2. Play mode test
3. Check timing, positions
4. Adjust nếu cần
```

---

## 📊 PERFORMANCE TIPS

### Optimization 1: Portrait Atlas

```
Combine multiple portraits vào 1 atlas
→ Giảm draw calls
→ Faster loading
```

### Optimization 2: Lazy Loading

```
Chỉ load portraits khi cần
→ Tiết kiệm memory
→ Faster startup
```

### Optimization 3: Portrait Pooling

```
Reuse portrait objects
→ Không tạo/destroy liên tục
→ Smoother performance
```

---

**🎉 VUI LÒNG THAM KHẢO UI_SETUP_GUIDE.MD ĐỂ SETUP UI!**
