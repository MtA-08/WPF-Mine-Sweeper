# WPF Mine Sweeper

Ein Minesweeper-Spiel für Windows, entwickelt mit C# und WPF. Auf einem verdeckten Spielfeld liegen zufällig verteilte Minen. Zahlen zeigen, wie viele Minen sich in den bis zu acht benachbarten Feldern befinden.

Das Ziel ist, alle sicheren Felder aufzudecken und alle Minen mit Flaggen zu markieren. Ein Klick auf eine Mine beendet die Runde mit einer Niederlage.

[Spiel herunterladen](https://github.com/MtA-08/WPF-Mine-Sweeper/releases/latest)

## Funktionen

- **Drei Schwierigkeitsstufen:** Unterschiedliche Spielfeldgrößen und Minenzahlen.
- **Flaggen setzen und entfernen:** Verdächtige Felder lassen sich mit der rechten Maustaste markieren.
- **Zähler für übrige Markierungen:** Die Anzeige entspricht der Minenzahl abzüglich gesetzter Flaggen. Sie prüft nicht, ob die Flaggen richtig gesetzt sind.
- **Ergebnisse speicehern:** Abgeschlossene Runden werden lokal als JSON gespeichert.

## Herunterladen und starten

1. Im [Release-Bereich](https://github.com/MtA-08/WPF-Mine-Sweeper/releases/latest) die Datei **`WPFMineSweeper.zip`** herunterladen.
2. Die gesamte ZIP-Datei entpacken.
3. **`Mine Sweeper.exe`** im entpackten Ordner starten.

Benötigt werden Windows und eine kompatible Installation von **.NET Framework 4.7.2 oder neuer innerhalb der 4.x-Reihe**. Die mitgelieferten DLLs müssen bei der Anwendung bleiben.

## Gespeicherte Ergebnisse

Für jede abgeschlossene Runde werden diese Angaben gespeichert:

- Spielnummer.
- Spielzeit in Sekunden.
- Ergebnis: gewonnen oder verloren.
- Schwierigkeitsstufe.
- Uhrzeit des Abschlusses.
