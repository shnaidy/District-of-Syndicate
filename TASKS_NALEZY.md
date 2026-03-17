# Návrhy úloh po rychlém průchodu kódem

## 1) Oprava překlepu / textace (UI/Log)
- **Typ:** typo/text fix
- **Kde:** `Assets/_Project/Scripts/BlackMarketManager.cs`
- **Nález:** Uživatelský log používá výraz `Nákup blackmarketu...`, který je stylisticky/pravopisně nejednotný vůči běžnému zápisu `black market`.
- **Návrh úlohy:** Sjednotit texty v logu na jednu variantu (např. `black market`) a projít další výskyty podobných složenin.
- **Akceptační kritéria:**
  1. Všechny relevantní logy používají stejnou terminologii (`black market`).
  2. Nezmění se funkční chování, pouze textace.

## 2) Oprava chyby (možný NullReferenceException)
- **Typ:** bug fix
- **Kde:** `Assets/_Project/Scripts/PlayerInteractor.cs`
- **Nález:** `CheckInteractable()` vždy používá `playerCamera.transform`, ale `playerCamera` je pouze volitelně doplněna z `Camera.main` a není ošetřena pro případ, že hlavní kamera ve scéně chybí.
- **Návrh úlohy:** Přidat guard clause, která při `playerCamera == null` bezpečně ukončí interakci (a případně zaloguje warning jen jednou).
- **Akceptační kritéria:**
  1. Bez kamery ve scéně nedojde k pádu na `NullReferenceException`.
  2. Prompt se skryje a interakce se neprovádí, dokud kamera není dostupná.

## 3) Oprava komentáře / nesrovnalosti v dokumentaci kódu
- **Typ:** comment/doc consistency
- **Kde:** `Assets/_Project/Scripts/GameTimeManager.cs`
- **Nález:** Komentář `// 0-24` naznačuje uzavřený interval, ale implementace resetuje čas při `>= 24f` na `0f`, tedy fakticky drží `timeOfDay` v intervalu `<0, 24)`.
- **Návrh úlohy:** Upřesnit komentář na `// 0 <= timeOfDay < 24` (nebo ekvivalent), aby odpovídal runtime chování.
- **Akceptační kritéria:**
  1. Komentář přesně popisuje hranice hodnoty.
  2. Bez změny runtime logiky.

## 4) Vylepšení testu (pokrytí edge case)
- **Typ:** test improvement
- **Kde:** `Assets/_Project/Scripts/Workbench/WorkbenchCraftingV2.cs`
- **Nález:** `TryCraft()` má několik větví (null recipe, chybějící inventory, chybějící output item, nedostatek ingrediencí), ale projekt nemá zřejmé automatické testy těchto guardů.
- **Návrh úlohy:** Přidat EditMode testy pro `CanCraft()` a `TryCraft()` se zaměřením na guard větve a správné odečtení/sumarizaci ingrediencí.
- **Akceptační kritéria:**
  1. Test pokrývá minimálně: `recipe == null`, `inventoryV2 == null`, `outputItem == null`, nedostatek ingrediencí, úspěšný craft.
  2. U úspěšného craftu se ověří odečet ingrediencí a přidání `outputItem` do inventáře.
