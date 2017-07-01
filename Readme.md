# Solitario

Gioco del solitario sviluppato in Unity


## Modalità di Gioco

Le regole rispecchiano perfettamente il classico gioco del solitario, con diverse modalità di gioco:

* Classica
** Viene assegnato un punteggio di 5 punti per ogni carta scoperta e 10 punti per ogni carta depositata nel foundation.

* A tempo
  * Un timer segna lo scorrere del tempo ed ogni 10 secondi vengono sottratti 2 punti

* Draw
  * Si scopre una carta per volta

* Draw3
  * Si scoprono 3 carte per volta

Le modalità di gioco possono essere combinate.


E' possibile annullare le mosse, ma questo aumenta il numero di mosse totali per completare il gioco.


## Specifiche tecniche

Il gioco è stato sviluppato in **Unity** versione 5.6.1f e linguaggio di programmazione **C#** per piattaforma PC.
Senza troppi stravolgimenti si può portare su diverse piattaforme.

L'interfaccia e il tavolo da gioco sono stati interamente realizzati mediante **Unity UI**.
Gli eventi di click, trascinamento e drop implementano le interfacce IPointer*Handler.

Il gioco concettualmente è suddiviso in macro aree, come il dealer, il tableu, il fondation e il waste.
Ognuna di queste aree implementa l'interfaccia IDropZone. Quando si trascina e si rilascia una carta nella macro area, la classe che implementa l'interfaccia si occuperà di spostare la carta secondo le regole del gioco, se la mossa è consentita.

L'annullamento delle mosse è possibile grazie a una lista di strutture, strutture che memorizzano la posizione da cui si è spostata la carta e il punteggio che è statto assegnato. L'annullamento della mossa ripristina i punti che si avevano in precedenza.


## Credits

Icons made by Madebyoliver from www.FlatIcon.com
