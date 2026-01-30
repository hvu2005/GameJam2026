# 🌟 Hướng Dẫn Setup Line Trail Cho Teleport

## 📋 Tổng Quan
Khi player teleport, sẽ có một **line trail kéo dài từ vị trí cũ đến vị trí mới**, và **fade dần từ điểm đầu đến điểm cuối** để tạo hiệu ứng mượt mà.

---

## 🎯 Cơ Chế Hoạt Động

### Flow:
```
Player ấn Skill lần 2 
    ↓
PlayerTeleportMarker.TeleportToMarker()
    ↓
Lưu oldPosition = vị trí hiện tại
    ↓
transform.position = validPos ← Dịch chuyển player
    ↓
PlayerTeleportTrail.ActivateTrail(oldPos, newPos) ← Tạo line
    ↓
LineRenderer tạo line từ oldPos → newPos
    ↓
Fade dần từ điểm đầu → điểm cuối (0.5s)
    ↓
Line biến mất hoàn toàn
```

### Fade Effect:
- **t = 0.0s**: Line đầy đủ từ A đến B
- **t = 0.25s**: 50% đầu đã fade, 50% cuối vẫn sáng
- **t = 0.5s**: Line biến mất hoàn toàn

---

## 🛠️ Setup Trong Unity

### Bước 1: Thêm Components Vào Player GameObject

1. **Chọn Player GameObject** trong Hierarchy
2. **Add Component: Line Renderer** (không phải Trail Renderer)
3. **Add Component: PlayerTeleportTrail script**

### Bước 2: Configure Line Renderer

#### Material:
1. Tạo Material mới:
   - Right click trong Project → Create → Material
   - Đặt tên: "TeleportLineMaterial"
   - Shader: **Sprites/Default** hoặc **Particles/Additive** (cho hiệu ứng glow)
   
2. Assign Material vào Line Renderer:
   - Kéo Material vào field "Materials" của Line Renderer

#### Settings (trong Line Renderer Inspector):

**Positions:**
- Positions: `0` (script sẽ tự động set)

**Width:**
- Width: `0.3` (cả start và end)
- Hoặc dùng curve để tạo hiệu ứng thon dần

**Color:**
- Start Color: Cyan (0, 255, 255, 255)
- End Color: Cyan (0, 255, 255, 255)
- (Gradient fade sẽ được script điều khiển)

**Corner/Cap Vertices:**
- Corner Vertices: `5`
- Cap Vertices: `5`

**Advanced:**
- Use World Space: `True` ✓
- Sorting Layer: "Default"
- Order in Layer: `10` (cao hơn player sprite)

### Bước 3: Configure PlayerTeleportTrail Script

Trong Inspector của PlayerTeleportTrail:

```
Fade Duration: 0.5 (thời gian fade hoàn toàn)
Trail Width: 0.3 (độ rộng line)
Trail Color: Cyan (0, 255, 255, 255)
Line Segments: 20 (số điểm trên line, càng nhiều càng smooth)
```

---

## 🎨 Tùy Chỉnh Trail

### 1. Thay Đổi Tốc Độ Fade:

```
Fade Duration = 0.3  // Fade nhanh
Fade Duration = 0.7  // Fade chậm
Fade Duration = 1.0  // Fade rất chậm
```

### 2. Thay Đổi Độ Rộng Line:

```
Trail Width = 0.2  // Line mỏng
Trail Width = 0.5  // Line dày
Trail Width = 1.0  // Line rất dày
```

### 3. Thay Đổi Độ Smooth:

```
Line Segments = 10  // Line ít điểm, có thể bị góc cạnh
Line Segments = 30  // Line nhiều điểm, rất smooth
Line Segments = 50  // Line cực smooth (tốn performance)
```

### 4. Thay Đổi Màu Sắc:

```csharp
// Trong code:
playerTeleportTrail.SetTrailColor(Color.red);
playerTeleportTrail.SetTrailColor(new Color(1f, 0.5f, 0f)); // Orange
```

### 5. Hiệu Ứng Nâng Cao:

#### A. Line Phát Sáng (Glow):
```
Material Shader: Particles/Additive
Color: Bright colors (White, Cyan, Yellow)
Trail Width: 0.5 - 0.7 (dày hơn)
```

#### B. Line Nhiều Màu:
Có thể modify script để dùng gradient màu thay vì một màu:
```csharp
// Thay Color bằng Gradient trong UpdateLineFade()
```

#### C. Curved Line:
Có thể thêm curve vào line thay vì thẳng:
```csharp
// Trong CreateLine(), dùng Bezier curve
Vector3 midPoint = Vector3.Lerp(_startPosition, _endPosition, 0.5f);
midPoint += Vector3.up * curveHeight;
// ... tính Bezier
```

---

## 🔧 Advanced Features

### 1. Clear Trail Ngay Lập Tức:

```csharp
_teleportTrail.ClearTrail(); // Xóa line ngay
```

### 2. Trail Theo Trạng Thái:

```csharp
// Ví dụ: Line đỏ khi teleport gần enemy
if (nearEnemy)
{
    _teleportTrail.SetTrailColor(Color.red);
}
else
{
    _teleportTrail.SetTrailColor(Color.cyan);
}
```

---

## 🎮 Testing

### Checklist:
- [ ] Player có Line Renderer component
- [ ] Player có PlayerTeleportTrail script
- [ ] Material được assign cho Line Renderer
- [ ] Throw marker và teleport → thấy line xuất hiện từ A → B
- [ ] Line fade dần từ điểm A đến điểm B
- [ ] Sau 0.5s, line biến mất hoàn toàn

### Debug:
- **Không thấy line**: Check Material, check Use World Space = true
- **Line không smooth**: Tăng Line Segments (20-30)
- **Line fade sai hướng**: Kiểm tra oldPosition và newPosition
- **Line bị giật**: Check Update() loop, đảm bảo không có lag

### Visualize trong Scene View:
- Khi đang fade, Gizmos sẽ vẽ yellow line từ start → end
- Click vào Player khi teleport để thấy debug info

---

## 📊 Performance Tips

1. **Optimization:**
   - Line Segments: 20-30 là đủ (không cần quá 50)
   - Disable LineRenderer khi không dùng
   - Fade Duration: giữ < 1 giây

2. **Best Practices:**
   - Line chỉ hiển thị khi teleport (không liên tục)
   - Auto-hide sau fade hoàn tất
   - Reuse cùng một LineRenderer (không spawn mới)

---

## 🚀 Mở Rộng

### 1. Multiple Lines:
Tạo nhiều lines cùng lúc với offset:
```csharp
// Spawn 3 lines song song
for (int i = -1; i <= 1; i++)
{
    Vector3 offset = Vector3.up * i * 0.2f;
    CreateLine(start + offset, end + offset);
}
```

### 2. Particle Trail:
Kết hợp với particles:
```csharp
// Spawn particles dọc theo line
for (int i = 0; i < 10; i++)
{
    float t = i / 10f;
    Vector3 pos = Vector3.Lerp(start, end, t);
    Instantiate(particlePrefab, pos, Quaternion.identity);
}
```

### 3. Sound Effect:
Thêm âm thanh teleport:
```csharp
// Trong TeleportToMarker():
AudioSource.PlayClipAtPoint(teleportSound, validPos);
```

### 4. Screen Shake:
Kết hợp với camera shake:
```csharp
CameraShake.Shake(0.2f, 0.1f);
```

---

## 📐 Công Thức Fade

### Fade Gradient:
```
Position 0.0 (start): Alpha = 0 (trong suốt - đã fade)
Position fadeProgress: Alpha = 0.5 (đang fade)
Position 1.0 (end): Alpha = 1.0 (hiển thị đầy đủ)
```

### Timeline:
```
t=0.0s:  [========] Line đầy đủ
t=0.2s:  [---=====] 40% đã fade
t=0.4s:  [-------=] 80% đã fade
t=0.5s:  [--------] Biến mất hoàn toàn
```

---

## 📝 Sự Khác Biệt Với Trail Renderer

### LineRenderer (Current):
✅ Tạo line tức thời từ A → B  
✅ Control fade direction (start → end)  
✅ Teleport vẫn hoạt động (không cần movement)  
✅ Đơn giản, dễ customize  

### TrailRenderer (Old):
❌ Cần object di chuyển để tạo trail  
❌ Teleport = không có trail  
❌ Fade theo thời gian, không theo vị trí  

---

✨ **Kết quả**: Mỗi lần teleport sẽ có line kéo dài từ vị trí cũ → mới, fade dần từ đầu đến cuối một cách mượt mà!

---

## 🎯 Cơ Chế Hoạt Động

### Flow:
```
Player ấn Skill lần 2 
    ↓
PlayerTeleportMarker.TeleportToMarker()
    ↓
PlayerTeleportTrail.ActivateTrail() ← Kích hoạt trail
    ↓
transform.position = validPos ← Dịch chuyển player
    ↓
Trail Renderer tự động tạo vệt từ vị trí cũ → mới
    ↓
Sau 0.5s, trail tự động fade out
```

---

## 🛠️ Setup Trong Unity

### Bước 1: Thêm Components Vào Player GameObject

1. **Chọn Player GameObject** trong Hierarchy
2. **Add Component: Trail Renderer**
3. **Add Component: PlayerTeleportTrail script**

### Bước 2: Configure Trail Renderer

#### Material:
1. Tạo Material mới:
   - Right click trong Project → Create → Material
   - Đặt tên: "TeleportTrailMaterial"
   - Shader: **Sprites/Default** hoặc **Particles/Additive**
   
2. Assign Material vào Trail Renderer:
   - Kéo Material vào field "Materials" của Trail Renderer

#### Settings (trong Trail Renderer Inspector):

**Time:**
- Time: `0.5` (thời gian trail tồn tại)

**Width:**
- Width: Curve từ `0.3` → `0`
  - Click vào Width curve
  - Set điểm đầu = 0.3, điểm cuối = 0

**Color:**
- Color Gradient:
  - Start: Cyan/Blue (Alpha = 255)
  - End: Blue/Transparent (Alpha = 0)

**Corner/Cap Vertices:**
- Corner Vertices: `5`
- Cap Vertices: `5`

**Advanced:**
- Min Vertex Distance: `0.1`
- Sorting Layer: "Default"
- Order in Layer: `10` (cao hơn player sprite)

### Bước 3: Configure PlayerTeleportTrail Script

Trong Inspector của PlayerTeleportTrail:

```
Trail Duration: 0.5
Trail Width: 0.3
Trail Color: [Gradient từ cyan → blue với alpha fade]
```

---

## 🎨 Tùy Chỉnh Trail

### 1. Thay Đổi Màu Sắc:

```csharp
// Trong code, gọi:
playerTeleportTrail.SetTrailColor(newGradient);
```

Hoặc trong Unity Inspector:
- Click vào Trail Color gradient
- Điều chỉnh các điểm màu

### 2. Thay Đổi Độ Dài Trail:

```
Trail Duration = 0.3  // Trail ngắn, nhanh
Trail Duration = 0.7  // Trail dài, chậm
Trail Duration = 1.0  // Trail rất dài
```

### 3. Thay Đổi Độ Rộng:

```
Trail Width = 0.2  // Trail mỏng
Trail Width = 0.5  // Trail dày
```

### 4. Hiệu Ứng Nâng Cao:

#### A. Trail Phát Sáng (Glow):
```
Material Shader: Particles/Additive
Color: Màu sáng (White, Cyan, Yellow)
```

#### B. Trail Nhiều Màu:
```
Gradient:
- 0%: Red
- 33%: Yellow  
- 66%: Cyan
- 100%: Transparent
```

#### C. Trail Texture:
1. Tạo texture vệt (smoke, lightning, magic...)
2. Assign vào Material
3. Adjust UV mode trong Trail Renderer

---

## 🔧 Advanced Features

### Clear Trail Khi Cần:

```csharp
// Trong PlayerTeleportMarker hoặc Player:
_teleportTrail.ClearTrail(); // Xóa trail ngay lập tức
```

### Trail Theo Màu Tùy Biến:

```csharp
// Ví dụ: Trail đỏ khi health thấp
if (health < 30)
{
    Gradient redTrail = new Gradient();
    // ... setup red gradient
    _teleportTrail.SetTrailColor(redTrail);
}
```

---

## 🎮 Testing

### Checklist:
- [ ] Player có Trail Renderer component
- [ ] Player có PlayerTeleportTrail script
- [ ] Material được assign cho Trail Renderer
- [ ] Throw marker và teleport → thấy trail xuất hiện
- [ ] Trail fade out sau ~0.5 giây
- [ ] Trail không kéo dài vô hạn

### Debug:
- **Không thấy trail**: Check Material, check Sorting Layer/Order
- **Trail quá dài/ngắn**: Điều chỉnh Trail Duration
- **Trail không smooth**: Tăng Corner/Cap Vertices
- **Trail bị giật**: Giảm Min Vertex Distance

---

## 📊 Performance Tips

1. **Optimization:**
   - Dùng `Clear()` thay vì disable/enable Trail Renderer
   - Giữ Time < 1 giây để tránh quá nhiều vertices
   - Min Vertex Distance >= 0.1 để giảm vertices

2. **Best Practices:**
   - Trail chỉ emit khi teleport (không liên tục)
   - Auto-disable sau một thời gian
   - Sử dụng Object Pooling nếu spawn nhiều trails

---

## 🚀 Mở Rộng

### 1. Particle Trail:
Thay vì Trail Renderer, có thể dùng Particle System để tạo hiệu ứng đẹp hơn:
- Stars/Sparkles
- Magic particles
- Lightning bolts

### 2. Multiple Trails:
Có thể có nhiều trail layers:
- Outer glow (to, mờ)
- Inner core (nhỏ, sáng)

### 3. Sound Effect:
Kết hợp với âm thanh:
```csharp
// Trong TeleportToMarker():
AudioManager.Play("teleport_whoosh");
```

---

## 📝 Notes

- Trail Renderer tự động tạo mesh từ vị trí cũ → mới
- `Clear()` xóa tất cả vertices để trail không kéo dài từ vị trí xa
- `emitting = false` ngăn tạo vertices mới
- Trail sẽ tự động fade theo gradient alpha

✨ **Kết quả**: Mỗi lần teleport sẽ có vệt sáng đẹp mắt theo sau!
