# Bachelorarbeit AI Ansätze zur Umsetzung eines Flughafensteuerungssystems

Die Simulation, die in der Bachelorarbeit entwickelt wurde, befindet sich als *Airportsystem 2D.exe* unter dem Ordner *Simulation* auf dem Stick. Zudem sind Videos auf dem Stick vorhanden, die zeigen, die die Bedienung der Simulation zeigen.

Zusätzlich dazu befindet sich der Code und die Assets für die Anwendung in dem Ordner *Airportsystem 2D* und in einem [Github Repository](https://github.com/EricCpy/AirportSimulator). 
## Set up
Zum Starten der beigefügten Anwendung in Windows ist es nicht erforderlich, zusätzliche Programme herunterzuladen. Falls Bedarf für die Unterstützung anderer Plattformen wie Mac oder Linux besteht, können Sie mir gerne eine E-Mail schreiben. 

Falls jedoch beabsichtigt wird, die Simulation in Unity zu testen oder die Simulation für andere Plattformen als Windows selbst zu erstellen, ist der Download folgender Programme erforderlich:
- [Unity Hub](https://unity.com/de/download) wird benötigt, um die Simulation in der Spieleengine zur Starten
- [Unity Version 2021.3.17](https://unity.com/releases/editor/whats-new/2021.3.17) wird zusätzlich zum Unity Hub benötigt
- [Python 3.*](https://www.python.org/downloads/) wird benötigt, um die Daten der Developer API des Flughafens Hamburg für die Simulation zu konvertieren

## Anleitung zum Builden der Simulation
Es ist nicht garantiert, dass die Simulation ohne Fehler auf anderen Plattformen als Windows ohne zusätzliche Anpassungen läuft. Daher wäre es bei Bedarf von Vorteil, mir eine E-Mail zu schreiben. Falls Sie die Simulation dennoch selbst erstellen möchten, sind folgende Schritte notwendig:

1. Öffnen Sie das Unity Hub.
2. Klicken Sie auf den Menüpunkt *Installs* und klicken Sie den *Locate* Button an, um die installierte Unity.exe Datei der Unity Version auszuwählen.
3. Klicken Sie auf den Menüpunkt *Projects* und klicken Sie den *Open* Button an. Wählen Sie anschließend den Projektordner aus, der auf dem Stick *AirportSystem 2D* heißt.
4. Starten Sie das Projekt in Unity.
5.  Klicken Sie in Unity den Menüpunkt *Files* an und anschließend *Build Settings*.
6.  Stellen Sie in den Build Settings die gewünschte Plattform ein und klicken Sie den Button *Build* an.


## Anleitung zum Importieren des Flughafens Hamburg unter Windows
Um das Anwendungsbeispiel des Flughafens Hamburg in die Simulation zu importieren, sind unter Windows folgende Schritte notwendig:

1. Starten Sie die Simulation und erstellen Sie einen Flughafen.
2. Schließen Sie die Simulation.
3. Kopieren Sie den Ordner *HamburgAirport* vom Stick in den Ordner *C:\Users\User\AppData\LocalLow\EricSimulation\Airportsystem 2D* auf Ihrem lokalen Computer.
4. Starten Sie die Simulation erneut und laden Sie den importierten Flughafen.

## Expertensystem
In dieser Arbeit wurde ein [Expertensytem](https://github.com/chen0040/cs-expert-system-shell) verwendet, welches nicht in der Arbeit enstanden ist. 

## Pythonskripte
Im Projektordner *AirportSystem 2D* befindet sich der Ordner *PythonSkripte*, der den Code für das Pythonskript *convert-json.py* zur Formatanpassung enthält. Dieses Skript dient dazu, die Daten der API des Flughafens Hamburg in ein Format umzuwandeln, welches von der Simulation verwendet werden kann. Im Unterordner *data* befinden sich die Dateien *arrivals.json* und *departures.json*, die Daten aus der API des Flughafens Hamburg enthalten.

Das Pythonskript sucht im Ordner *data* nach diesen beiden JSON-Dateien, konvertiert sie in das JSON-Format der Simulation und speichert die Ausgabe in der Datei *results.json*. Der Inhalt dieser Datei kann in die Simulation importiert werden. Darüber hinaus kann über die Variable *custom_date* im Skript festgelegt werden, welches Datum die Flüge haben sollen.