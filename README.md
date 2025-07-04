# 🐍 Snake Game (Console, C#)

Jednoduchá konzolová verze klasické hry **Snake** napsaná v jazyce C#. 
Projekt je refaktorovaný podle zásad *Clean Code* – důraz na čitelnost, oddělení logiky od UI, smysluplné názvy a konzistentní architekturu.

---

## 💡 Vlastnosti

- Ovládání pomocí šipek (← ↑ → ↓)
- Hra končí nárazem do zdi nebo vlastního těla
- Dynamické generování "berry" (jídla)
- Skóre odpovídá délce hada
- Jasně oddělené části:
  - `Snake` – logika hada
  - `Berry` – jídlo
  - `GameBoard` – vykreslování
  - `SnakeGame` – hlavní herní smyčka
  - `Pixel`, `Direction` – pomocné typy

---

## ▶️ Spuštění

1. **Klonuj repozitář:**
   ```bash
   git clone https://github.com/uzivatel/snake-game-clean.git
   cd snake-game-clean
