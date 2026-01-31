# 🔧 TROUBLESHOOTING: Không thấy "New Dialogue Data" trong menu

## Vấn đề

Khi right-click → Create → Dialogue, chỉ thấy:

- ✓ New Dialogue Container (DialogueText - legacy)
- ❌ New Dialogue Data (DialogueData - mới) ← KHÔNG THẤY

## Nguyên nhân

Unity chưa compile DialogueData.cs hoặc có lỗi compile.

## Giải pháp

### Cách 1: Đợi Unity Compile

```
1. Check góc dưới phải Unity Editor
2. Nếu thấy "Compiling..." → Đợi xong
3. Nếu thấy lỗi compile → Fix lỗi trước
4. Sau khi compile xong, thử lại Create → Dialogue
```

### Cách 2: Force Recompile

```
1. Unity → Assets → Reimport All
2. Hoặc: Edit → Preferences → External Tools → Regenerate project files
3. Restart Unity Editor
```

### Cách 3: Kiểm tra File DialogueData.cs

```
Mở file: Assets/_Game/Scripts/Dialogue/DialogueData.cs

Dòng đầu phải có:
[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/New Dialogue Data")]

Nếu không có hoặc sai → Copy code từ template
```

### Cách 4: Xóa Library và Recompile

```
1. Đóng Unity
2. Xóa folder: GameJam2026/Library/
3. Mở lại Unity (sẽ recompile toàn bộ)
4. Đợi compile xong (có thể mất vài phút)
```

### Cách 5: Tạo Manually bằng Code

```csharp
// Tạo script Editor để tạo DialogueData
// Assets/Editor/CreateDialogueData.cs

using UnityEngine;
using UnityEditor;

public class CreateDialogueData
{
    [MenuItem("Assets/Create/Dialogue/Create Data Asset")]
    static void CreateAsset()
    {
        DialogueData asset = ScriptableObject.CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(asset, "Assets/NewDialogueData.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
```

## Workaround: Dùng DialogueText (Legacy)

Nếu không tạo được DialogueData, tạm thời dùng DialogueText:

```
1. Right-click → Create → Dialogue → New Dialogue Container
2. Code đã có backwards compatibility
3. DialogueController.DisplayNextParagraph() vẫn hoạt động
4. Nhưng không có multiple speakers và portraits

Khi fix được, migrate sang DialogueData sau!
```

## Kiểm tra Console

```
Unity → Window → Console
Check có lỗi compile không:
- Error CS#### → Sửa lỗi syntax
- Missing reference → Import package thiếu
- Namespace issues → Check using statements
```

## Đảm bảo Script Đúng

### DialogueData.cs phải có

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Configuration")]
    public DialogueLine[] lines;
    
    [Header("Optional Settings")]
    public bool autoClose = true;
    public float autoCloseDelay = 0.5f;
}
```

### DialogueLine.cs phải có

```csharp
using UnityEngine;

public enum SpeakerPosition { Left, Right, Center }

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    public SpeakerPosition position = SpeakerPosition.Left;
    
    [TextArea(3, 10)]
    public string text;
}
```

## Sau khi Fix

```
1. Right-click trong Project window
2. Create → Dialogue
3. Sẽ thấy CẢ HAI:
   - New Dialogue Container (old)
   - New Dialogue Data (new) ← Dùng cái này!
```

---

**Nếu vẫn không được, báo tôi check Console errors!**
