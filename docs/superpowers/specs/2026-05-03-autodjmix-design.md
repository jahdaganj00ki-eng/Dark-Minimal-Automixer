# AutoDJMix (Windows / .NET 8 / WPF) – Design

Datum: 2026-05-03  
Quelle der harten Regeln: [mixEngine.json](file:///workspace/mixEngine.json)

## 1. Zielbild

Ein natives Windows-Desktop-Programm (Windows 10 Pro 64-Bit+), das beliebig viele Audio-Tracks (mp3/wav/m4a) importiert, zuverlässig analysiert (BPM/Beatgrid/Downbeats/Key/Energie/Segmente), daraus ein global optimiertes DJ-Set plant (Reihenfolge + Übergänge) und das fertige Set als MP3 (320 kbps CBR) rendert. UI ist modern, black theme, vollständig bedienbar inkl. Drag&Drop und Inspectors.

Nicht-Ziel: Live-DJ-Deck, Echtzeit-Latenz-optimierte Performance. Fokus ist Offline-Planung und Offline-Rendering mit reproduzierbarem Ergebnis.

## 2. Verbindliche MixEngine-Regeln

### 2.1 Score-Formel (exakt)

Die Engine bewertet Kandidatenübergänge und globale Planvarianten strikt nach:

`final_score = (base_compat * 0.6) + (phase_boost * 0.2) + (risk_penalty * 0.2)`

Dabei gilt:
- `base_compat` wird aus den gewichteten Teil-Scores gemäß `role_weights` gebildet (groove/key/energy/bass/break + penalties hook/style_jump).
- `phase_boost` resultiert aus Set-Phasen-Regeln (`set_phases` und phase-spezifischen priority_boost rules).
- `risk_penalty` aggregiert harte Risiken/Restriktionen (Hook/Bass/Double-Break, Bridge-Pflicht, BPM/Key-Verletzungen, Diversity-Penalty etc.).

### 2.2 Decision-Matrix (exakt)

Die Entscheidungsklassen werden exakt wie definiert abgebildet:
- `>=85`: PERFECT
- `70-84`: GOOD
- `55-69`: RISKY
- `<55`: AVOID

### 2.3 Harte Transition-Overrides (exakt)

Die folgenden Konflikte überschreiben automatisch gewählte Übergänge/Automationen:
- `hook_conflict=high` ⇒ `force_transition = LOOP_BLEND_ONLY`
- `bass_clash=high` ⇒ `force_eq.bass_never_overlap = true`
- `double_break_risk=high` ⇒ `force_transition = BREAK_REBUILD_ONLY`

Bridge-Pflichten und Pair-Rules aus `rules[]` sind ebenfalls bindend (z.B. DARK_HYPNOTIC→MELODIC_OPEN nur via Bridge-Rollen).

## 3. Rollenlogik (Auto + Override/Lock)

### 3.1 Rolle pro Track

Pro Track wird eine Rolle geführt:
- DARK_HYPNOTIC
- PROGRESSIVE_DRIVER
- MINIMAL_BRIDGE
- MELODIC_OPEN
- PSY_EDGE
- NEUTRAL_TOOL

### 3.2 Quellenpriorität

Rollenquelle ist Teil des Track-Zustands:
- Default: `RoleSource = AnalysisAuto` (automatische Klassifikation aus Audioanalyse)
- User kann Rolle im Inspector setzen: `RoleSource = UserOverride`
- User kann zusätzlich locken: `RoleLock = true` verhindert spätere Auto-Neuklassifikation
- Tags/Ordnernamen liefern nur `WeakRoleHint` (Bias), niemals „Truth“

### 3.3 Weak Hints (Tags/Ordner)

Tags/Ordner werden höchstens als schwache Zusatzsignale genutzt:
- beeinflussen die initiale Auto-Klassifikation nur als kleiner Prior/Bias
- werden nie genutzt, um harte Constraints zu erfüllen oder zu ersetzen
- bei Widerspruch zu Audioanalyse bleibt Audioanalyse dominant (wenn nicht manuell overridden/gelockt)

## 4. Architektur (Schichtenmodell + Module)

### 4.1 Solution / Projects

Solution: `AutoDJMix.sln`

Produktion:
- `src/AutoDJMix.App` (WPF UI)
- `src/AutoDJMix.Application` (UseCases, Orchestrierung, Ports)
- `src/AutoDJMix.Domain` (Entities/ValueObjects, rule models)
- `src/AutoDJMix.Analysis` (Analyse-Engine)
- `src/AutoDJMix.Mixing` (Planer, MixEngine-Regeln, Optimierung)
- `src/AutoDJMix.Transition` (Übergangstypen + Automation)
- `src/AutoDJMix.Rendering` (Offline-Renderer)
- `src/AutoDJMix.Persistence` (SQLite)
- `src/AutoDJMix.Infrastructure` (Dateisystem, Tag-Reader, FFmpeg, OS-Adapter)

Tests:
- `tests/AutoDJMix.Domain.Tests` (Unit)
- `tests/AutoDJMix.Application.Tests` (Unit/Component)
- `tests/AutoDJMix.IntegrationTests` (SQLite/Import/FFmpeg)
- `tests/AutoDJMix.E2E.Tests` (Import→Analyse→Plan→Render)

Installer/Build:
- `installer/nsis`
- `build`

### 4.2 Abhängigkeitsregeln

- App/UI referenziert nur `Application` (und UI-spezifische libs)
- Application referenziert Domain + Engine-Module (Analysis/Mixing/Transition/Rendering) nur über Interfaces/Ports, wo sinnvoll
- Domain hat keine Abhängigkeit nach außen
- Persistence und Infrastructure implementieren Ports aus Application
- Engine-Module dürfen Domain referenzieren; IO erfolgt nur über bereitgestellte Ports

## 5. Datenmodelle (Domain)

### 5.1 Track

Track ist eine Entity mit stabiler ID:
- `TrackId` (GUID)
- `SourceUri` (Dateipfad)
- `DisplayName` (aus Tags/Dateiname)
- `Duration`
- `Fingerprint` (für Dedupe)
- `CodecInfo` (Container/Codec/SR/Channels)
- `RoleAssignment` (Role, RoleSource, RoleLock, WeakRoleHint optional)

### 5.2 Analyse-Ergebnisse

`TrackAnalysis` (ValueObject-ähnlich, versioniert):
- `Bpm` (float) + `BpmConfidence`
- `BeatGrid` (StartTime, BeatInterval, DriftModel)
- `Downbeats` (Liste + Confidence)
- `KeyCamelot` (string, z.B. 3A) + Confidence
- `Energy` (0..10)
- `Brightness` (0..1)
- `BassIntensity` (0..1)
- `Percussiveness` (0..1)
- `StructureMarkers` (IntroStart/IntroEnd/Breaks/Hooks/OutroStart/OutroEnd)
- `AnalysisVersion` (SemVer + Hash der Algorithmenparameter)

### 5.3 MixPlan / TransitionPlan

`MixPlan`:
- `ProjectId`
- `OrderedTracks[]` mit optionalen `TrackLock`-Flags (Position lock)
- `SetPhases[]` (zugewiesene Tracks/Abschnitte)
- `Transitions[]` (zwischen Track i und i+1)
- `FinalScore` + per-edge Details

`TransitionPlan`:
- `FromTrackId`, `ToTrackId`
- `TransitionType` (Enum; deckt MixEngine-Typen ab)
- `StartTime`/`DurationBeats`/`LoopBars` optional
- `EqAutomation`/`FilterAutomation`/`LoudnessAutomation`
- `HardOverridesApplied[]` (als Daten, nicht als Kommentar)

## 6. Persistence (SQLite)

### 6.1 Datenbank-Aufteilung

Eine SQLite-DB (Datei im AppData):
- `Library` (Tracks + Metadaten + Fingerprints)
- `AnalysisCache` (Analyseergebnisse inkl. Version + Hash)
- `Projects` (Mix-Projekte, Setlisten, Locks)
- `Settings` (UI/Engine/Paths)

### 6.2 Migrations

Schema-Version wird geführt und mit Migrations gepflegt. DB-Änderungen müssen abwärtskompatibel migrierbar sein.

### 6.3 Deduplizierung

Dedupe-Key:
- Primär: Hash über Dateiinhalt (Streaming SHA-256) + Dateigröße
- Optional/ergänzend: Audio-Fingerprint (robuster gegen Tag-Edits; später ausbaubar)

## 7. Audio-Pipeline (Decoding/Resampling/MP3 Export)

### 7.1 Core + Fallback (Hybrid)

Core:
- NAudio für WAV/MP3 Decoding (wo möglich) und Audio-Pipeline-Bausteine

Fallback:
- mitgelieferte `ffmpeg.exe`/`ffprobe.exe` (Installer) für M4A/Codec-Edgecases und finalen MP3 Export via LAME (ffmpeg)

### 7.2 Interner Arbeits-PCM-Standard

Interne Verarbeitung arbeitet standardisiert auf:
- 48 kHz, Stereo, 32-bit float PCM

Resampling/Channel-Mix erfolgt deterministisch (gleiche Eingabe -> gleiche Ausgabe).

## 8. Analyse-Engine (Offline, Cached)

### 8.1 Job-Modell

Analyse läuft als Hintergrund-Jobs pro Track mit:
- Priorisierung (neue Imports zuerst)
- CancellationTokens
- Persistenter Cache (AnalysisVersion + InputFingerprint)

### 8.2 Ergebnisqualität

Analyse produziert Konfidenzen; niedrige Konfidenz beeinflusst Mixplanung:
- Key/BPM unsicher ⇒ höhere Risiko-Penalties, konservativere Transition-Wahl

## 9. Mixplanung (globale Optimierung)

### 9.1 Ziel-Funktion

Globales Optimum über Setliste:
- maximiert Summe der Transition-Scores
- respektiert Set-Phasen (Dauer-Minima und Rolle/Energie-Ziele)
- respektiert harte Pair-Regeln + Overrides
- respektiert Locks (Track-Role-Lock, Order-Lock)

### 9.2 Suchstrategie

Planung als kombinatorisches Optimierungsproblem:
- Kandidaten-Kanten pro Track-Paar werden vorbewertet (Score + Constraints)
- globale Suche mit Heuristiken (beam search / constrained A* / ILP-ähnliche Heuristik), konfigurierbar
- Bridge-Track Vorschläge, wenn direkte Kante verbietet oder stark risky ist

## 10. Transition-Engine (Offline-Automation)

TransitionType deckt MixEngine-Typen ab:
- LONG_EQ_BLEND
- INTRO_OVER_OUTRO
- PERCUSSION_LOOP_BLEND
- BREAK_REBUILD
- BREAK_EXIT_BLEND
- HARMONIC_BLEND
- ECHO_OUT_SWITCH
- TOOL_BRIDGE / SEAMLESS_TOOL
- weitere aus JSON

Automationen:
- EQ/Filter/Loudness als zeitbasierte Kurven (beats -> time über BeatGrid)
- harte Overrides erzwingen Parameter (z.B. bass_never_overlap)

## 11. Rendering

Offline-Renderer:
- liest Track-PCM Streams (decoded/resampled)
- time-stretch für Beatmatching (qualitativ hochwertig, offline)
- wendet Transition-Automationen an
- schreibt ein finales PCM-Stream
- finaler MP3 Export via ffmpeg: 320 kbps CBR

Progress + Cancel:
- jede Pipeline-Stufe meldet Fortschritt (pro Track und global)
- Abbruch führt zu sauberem Cleanup (Tempfiles, Prozesse)

## 12. UI/UX (WPF, Black Theme)

Layouts:
- links: Import/Bibliothek
- center: Setliste (Tracks, Scores, Phasen)
- rechts: Inspector (Track/Transition Details, Locks/Overrides)
- Timeline/Preview: Waveform/Beatgrid Preview (ohne Live-Deck)

Drag&Drop:
- Files/Folders in Library
- Reorder in Setliste mit Locks

## 13. Tests

Unit:
- Domain: Score-Berechnung, DecisionMatrix, Rule-Overrides, Key-Wheel-Kompatibilität
- Mixing: Kantenbewertung, Diversity-Penalty, Bridge-Pflichten, Phase-Boost Regeln

Integration:
- SQLite Migrations + CRUD
- Import Pipeline (inkl. ffprobe Metadata)
- Rendering Pipeline mit kurzem synthetischem Audio (deterministische Assertions)

E2E:
- import -> analyse -> plan -> render -> mp3
- verifiziert: exit code, mp3 existiert, Dauer plausibel, keine Clipping-Exzesse (Peak-Meter), Logging ohne Secrets

## 14. Build/Release/Installer

Build:
- `dotnet restore/build/test` für alle Projekte
- Release-Layout: Versionierter Ordner mit `AutoDJMix.exe`, `ffmpeg.exe`, `ffprobe.exe`, Ressourcen, DB-Migrations

Installer:
- NSIS: Install/Uninstall, Startmenü-Shortcut, optional Desktop-Shortcut
- Upgrade-fähig (gleiche AppId), Cleanup beim Uninstall
- FFmpeg-Binaries werden mitinstalliert (Lizenztexte werden mitgeliefert)

## 15. Sicherheits- und Robustheitsanforderungen

- Keine Logs mit Pfaden, die Secrets enthalten könnten (generell: keine Secrets)
- Robust gegen defekte Dateien: Import markiert Track als „Failed“ inkl. Fehlergrund, ohne Crash
- Prozesse (ffmpeg) mit Timeouts und Cancel

