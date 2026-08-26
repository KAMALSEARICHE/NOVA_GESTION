# NovaGestion

NovaGestion is a WinForms desktop shell for managing contracts (Contrats) and amendments (Avenants). This repository targets .NET 8 (net8.0-windows) and builds a single WinForms application.

Quick notes:
- Requires .NET 8 SDK or Visual Studio supporting .NET 8 and WinForms.
- Run:

```bash
dotnet build NovaGestion.csproj
dotnet run --project NovaGestion.csproj
```

What I changed in the repository (quick summary):
- Added a .gitignore (Visual Studio / .NET typical entries) to avoid committing build artifacts.
- Cached commonly used Font instances in UIControls.Theme to reduce GDI usage.
- Fixed ApplyRoundedRegion in FrmLogin to dispose old Region objects when replacing them (avoids potential GDI leaks).
- Added this README with run instructions and change notes.

Recommended next steps (I can do these if you want):
- Remove the `bin/` and `obj/` folders from the repo history and delete `NovaGestionSolution.rar` and `*.user` files (I added .gitignore but deletions still need commits). I can open a PR that removes these files.
- Replace hardcoded dev credentials ("1234") with a configuration or a pluggable auth provider before any production usage.
- Consider introducing a lightweight repository/DAO layer to replace AppData static lists when you start using a real database (SQLite / SQL Server).

If you want, أقدر أفتح Pull Request يتضمن تنظيف إضافي (حذف NovaGestionSolution.rar, NovaGestion.csproj.user, حذف bin/ obj من git) وأجرّب البناء عبر actions أو على جهاز CI. قلّي أعمل PR أو أعدّل مباشرة المزيد من الملفات.