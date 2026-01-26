using System;
using UnityEngine;

public class StaticHazard : Hazard {
    // Không cần logic thêm vì Hazard base class đã xử lý va chạm và instant death
    protected virtual void Reset() {
        isInstantDeath = true; 
    }
}