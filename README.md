# 🎮 MyUnityProject

Sebuah game yang dibuat dengan Unity.

## 🛠️ Tools & Teknologi

- Unity v202x.x.x
- C#
- Git & GitHub (kolaborasi)
- Platform: Windows / WebGL / Android

## 📂 Struktur Folder

| Folder        | Isi                                                  |
|---------------|-------------------------------------------------------|
| `Assets/`     | Semua asset (script, prefab, scene, art, audio)       |
| `Packages/`   | Dependency Unity                                       |
| `ProjectSettings/` | Setting project Unity                             |

## 🚀 Cara Menjalankan

1. Clone repository ini:
    ```bash
    git clone https://github.com/username/MyUnityProject.git
    ```
2. Buka folder project ini melalui Unity Hub.

3. Jalankan scene utama di folder `Assets/Scenes/Main.unity` (atau sesuai project).

## 👥 Kolaborasi

### Branching Strategy:

- `main` = versi stabil
- `dev` = fitur baru (merge dari branch fitur)
- `feature/xyz` = buat fitur tertentu
- Gunakan Pull Request (PR) untuk menggabungkan

### Aturan Commit:

```bash
✅ Fitur     = feat: nama fitur
🐞 Bugfix    = fix: deskripsi bug
⚙️ Refactor = refactor: keterangan
🧪 Test     = test: penambahan test
