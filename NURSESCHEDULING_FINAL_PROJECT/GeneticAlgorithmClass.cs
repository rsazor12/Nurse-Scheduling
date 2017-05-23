﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NURSESCHEDULING_FINAL_PROJECT
{
     class GeneticAlgorithmClass
    {
        protected List<ChromosomeClass> listOfParentChromosomes;
        protected int[] tableOfPenaltyForSpecifiedChromosome;

        protected int howMuchChromosomesSelectToCrossover;
        protected int howMuchChromosomesCreate;

        protected int acceptablePenalty;
        protected int maximumNumberOfGenerations;
        protected int minimumNumberOfGenerations;
        protected int howMuchMutationPerGeneration;

        public GeneticAlgorithmClass(sbyte howMuchChromosomesCreate,int acceptablePenalty,int minimumNumberOfGenerations ,int maximumNumberOfGenerations,int howMuchMutationPerGeneration,int howMuchChromosomesSelectToCrossover)
        {
            listOfParentChromosomes = new List<ChromosomeClass>();
            this.howMuchChromosomesCreate = howMuchChromosomesCreate;
            this.acceptablePenalty = acceptablePenalty;
            this.minimumNumberOfGenerations = minimumNumberOfGenerations;
            this.maximumNumberOfGenerations = maximumNumberOfGenerations;
            this.howMuchMutationPerGeneration = howMuchMutationPerGeneration;
            this.howMuchChromosomesSelectToCrossover = howMuchChromosomesSelectToCrossover;
            


            //najpierw musze stworzyć liste chromosomów
            for(int i=0;i<howMuchChromosomesCreate;i++)
            {
                listOfParentChromosomes.Add(new ChromosomeClass()); //tworze chromosom i dodaje go do listy
                //PoolOfNurses.clearStaticCounter();
                
            }
            tableOfPenaltyForSpecifiedChromosome = new int[listOfParentChromosomes.Count]; // inicjuje tez odpowiednio tablice na kare(penalty)
        }

        ///<summary> 
        ///sprawdza funkcje przystosowania dla każdego chromosomu (osobnika) którego używa algorytm
        ///</summary>
        public virtual void fitnessFunction()
        {
            //dla każdego chromosomu obliczam jego penalty
            for(int i=0;i<listOfParentChromosomes.Count;i++)
            {
                listOfParentChromosomes[i].checkConstraints();
            }
        }

        /// <summary>
        /// Funkcja do krzyżowania chromosomów ze sobą , ale tylko z tymczasowej listy bestChromosomesSelectedToCrossover
        /// </summary>
        public virtual void CrossoverOfBestChromosomes(List<ChromosomeClass> bestChromosomesSelectedToCrossover)
        {
           
            //krzyżowanie musimy przeprowadzać dopóki nie uzyskamy howMuchChromosomeCreate nowych chromosomów
            int howMuchNewChromosomesWasCreated = 0;
            int indexOfFirstChromosome=-1;
            int indexOfSecondChromosome = 0;

            Random rnd = new Random();
            ChromosomeClass[] newChromosomesAfterCrossover = new ChromosomeClass[2];



            while (howMuchNewChromosomesWasCreated < howMuchChromosomesCreate)
            {               
                if( howMuchNewChromosomesWasCreated >= howMuchChromosomesSelectToCrossover -1)  //jezeli już skrzyżowaliśmy ze sobą chromosomy parzyście czyli 1 z 2 , 3 z 4 itd
                {
                    //to tutaj wybieramy losowo chromosomy aby stworzyć z nich pozostałe osobniki do populacji - bo paru nam brakuje do howMUchNewChromosomesWasCreated
                    do
                    {                       
                        indexOfFirstChromosome = rnd.Next(0, bestChromosomesSelectedToCrossover.Count);
                        indexOfSecondChromosome = rnd.Next(0, bestChromosomesSelectedToCrossover.Count);
                    } while (indexOfFirstChromosome == indexOfSecondChromosome);            //jesli bedą sie powtarzac te indeksy to żeby nie krzyżowało jednego chromosomu ze sobą    
                    
                    //no i krzyżuje te dwa losowo wybrane chromosomy i dodaje je do listy
                    newChromosomesAfterCrossover = crossoverOfTwoChromosomesByWeekends(bestChromosomesSelectedToCrossover[indexOfFirstChromosome], bestChromosomesSelectedToCrossover[indexOfSecondChromosome], howMuchNewChromosomesWasCreated % 2);
                    //dodaje do listy nowych
                    bestChromosomesSelectedToCrossover.Add(newChromosomesAfterCrossover[0]); //dodaje 
                    bestChromosomesSelectedToCrossover.Add(newChromosomesAfterCrossover[1]);
                }
                else
                {
                   
                    indexOfFirstChromosome++;
                    indexOfSecondChromosome++;
                    //krzyżuje parzyście - czyli pierwszy z drugim , trzeci z czwartym itd i przypisuje je odpowidnio do miejsc w liscie (zastepuje 2 inne chromosomy w liscie tymczasowej)                                                                                      // ↓↓↓ tu na dole w argumencie raz zaczynamy krzyżowanie od 0 week a raz od 1 week - tak żeby było różnorodnie
                    newChromosomesAfterCrossover = crossoverOfTwoChromosomesByWeekends(bestChromosomesSelectedToCrossover[indexOfFirstChromosome], bestChromosomesSelectedToCrossover[indexOfSecondChromosome], howMuchNewChromosomesWasCreated % 2);
                    bestChromosomesSelectedToCrossover[indexOfFirstChromosome]=newChromosomesAfterCrossover[0];    //przypisuje pierwszy nowy chromosom
                    bestChromosomesSelectedToCrossover[indexOfSecondChromosome] = newChromosomesAfterCrossover[1]; //no i drugi nowy chromosom

                }              
                howMuchNewChromosomesWasCreated+=2; //co iteracje mam 2 nowe chromosomy bo krzyżuje 2 i z nich powstaja 2 nowe
            }       
        }

        /// <summary>
        /// Krzyżuje dwa chromosomy podane na wejściu i zwracam całkowicie nowe chromosomy
        /// </summary>
        public virtual ChromosomeClass[] crossoverOfTwoChromosomesByWeekends(ChromosomeClass chromosome1, ChromosomeClass chromosome2, int fromWhichWeekStartCrossover) //tu pracuje na orginałach chromosomów dlatego nic nie zwracam
        {
            int whichWeekChange = fromWhichWeekStartCrossover;
            NurseClass[][][] tempTableOnWeek; //to jakby wskaźnik na tablice 3 wymiarową
            ChromosomeClass[] newChromosomes = new ChromosomeClass[2] {new ChromosomeClass(), new ChromosomeClass() }; // miejsce na 2 nowe chromosomy
            //krzyżuje 2 chromosomy wymieniając między nimi tygodnie

            //najpierw przypisuje do nowych chromosomów stare chromosomy ( bo jak bd puste to co bd Krzyżować ? xd)
            //przypisujemy do tych nowych chromosomów kopie tablic chrosomome vector - musi to byc kopia wartościowa a nie kopia referencyjna
            newChromosomes[0].chromosomeVector = new List<NurseClass[][][]>(chromosome1.chromosomeVector).ToArray(); //tworze liste i pozniej tworze z niej tablice - po wartosciach wszystko kopiuje (czyli mamy całkowicie nowe tablice z tymi samymi wartosciami)
            newChromosomes[1].chromosomeVector = new List<NurseClass[][][]>(chromosome2.chromosomeVector).ToArray();   
          
            
            

            while(whichWeekChange<5)//jak wyjedziemy na 6 tydzien chromosomu to koniec krzyżowania
            {
                tempTableOnWeek = newChromosomes[0].chromosomeVector[whichWeekChange];
                newChromosomes[0].chromosomeVector[whichWeekChange] = newChromosomes[1].chromosomeVector[whichWeekChange]; //wymieniam cały tydzien miedzy dwoma chromosomami
                newChromosomes[1].chromosomeVector[whichWeekChange] = tempTableOnWeek; //tu też wymiana

                whichWeekChange+=2;//bedziemy krzyżować co 2 week - bo krzyżowanie co jeden nie ma sensu bo uzyskalibyśmy 2 identyczne chromosomy takie jak na wejsciu
            }

            //return new ChromosomeClass[2] { chromosome1, chromosome2 }; //zwracam 2 przekrzyżowane juz chromosomy
            return newChromosomes;
        }


        /// <summary>
        /// Funkcja uruchamia algorytm genetyczny - zwraca najlepszy chromosom
        /// </summary>
        public virtual ChromosomeClass runAlgorithm() // główna funkcja algorytmu 
        {
            //!!!narazie robie tak żeby hard constraints były spełnione
            List<ChromosomeClass> listOfChildChromosomes = listOfParentChromosomes.GetRange(0,howMuchChromosomesSelectToCrossover); //lista na nowe pokolenie , na początku taka sama jak rodziców tylko troszke mniejsza
            
            //1 - wybieram osobniki(chromosomy) z najlepszym przystosowaniem - najmniejszym penalty
            // musze znalezc te chromosomy ktore będą mieć najmniejsza wartość w tableOfPenalty
            //dodaje je do listy wybranych dopóki lista nie będzie miała długości określonej przez użytkownika
            int counterOfGenerations = 0;

            while(true)//counterOfGenerations<maximumNumberOfGenerations
            {
                counterOfGenerations++;

                //SELEKCJA
                //1 - jesli osobniki nowe osobniki lepsze od rodziców to je zostawiam , jesli nie to wracam do rodziców (lepsze pokolenie takie których suma HowManyHCDoneCOunter jest większa)               
                    //wyliczam ile HC zostało spełnione w rodzicach i dzieciach (suma z wszystkich chromosomów)      
                    listOfParentChromosomes.ForEach(o => o.checkConstraints());//musze najpierw odpalic funkcje sprawdzajace constraints
                    listOfChildChromosomes.ForEach(o => o.checkConstraints());

                    //zsumować wszystkie howManyHCDone z wszystkich chromosomów
                    int sumOfHowManyHCDoneInParents = 0;
                    int sumOfHowManyHCDoneInChlidren = 0;
                    listOfParentChromosomes.ForEach(o => { o.checkHowManyHCIsDone(); sumOfHowManyHCDoneInParents += o.HowManyHCDoneCounter; }); //sumuje ile HC spełniono w rodzicach
                    listOfChildChromosomes.ForEach(o => { o.checkHowManyHCIsDone(); sumOfHowManyHCDoneInChlidren += o.HowManyHCDoneCounter; }); //sumuje ile HC spełniono w dzieciach

                    //teraz decyzja czy wybieramy dzieci do nowego pokolenia czy zostajemy przy rodzicach i ich mutujemy i krzyżujemy dalej

                    if (sumOfHowManyHCDoneInChlidren <= sumOfHowManyHCDoneInParents)
                    {
                    //zostajemy przy rodzicach - krzyżujemy i mutujemy stare pokolenie dalej
                    listOfChildChromosomes = listOfParentChromosomes.OrderBy(o => o.HowManyHCDoneCounter).ToList().GetRange(0, howMuchChromosomesSelectToCrossover);
                }
                    else
                    {
                        //do nowego pokolenia przechodzą dzieci - wybieram liczbe dzieci ktore maja najlepszy wskaznik spelnienia HC
                        //dzieci staja się rodzicami
                        listOfParentChromosomes = listOfChildChromosomes.OrderBy(o => o.HowManyHCDoneCounter).ToList().GetRange(0, howMuchChromosomesSelectToCrossover); //zwraca liste posortowaną rosnąco - tylko te elementy które nadają się do krzyżowania(odpowiednie penalty)                        
                    }

                Console.WriteLine("\n\nSelekcja epoka" + counterOfGenerations.ToString());

                //2 - dokonuje krzyżowania na tych osobnikach (nowe osobniki generowane do listOfChildChromosomes - zmienimy ta liste po tej funkcji)
                CrossoverOfBestChromosomes(listOfChildChromosomes);
                Console.WriteLine("Krzyżowanie epoka" + counterOfGenerations.ToString());                         

                //3 - dokonuje mutacji na skrzyżowanych osobnikach(dzieciach)
                listOfChildChromosomes.ForEach(o=> { o.mutation(howMuchMutationPerGeneration);}); 
                Console.WriteLine("Mutacja epoka" + counterOfGenerations.ToString());             
                



                //5 KONIEC ALGORYTMU GDY wskaźnik spełnienia HC w najlepszym chromosomie wynosi 10 i penalty mniejsze od założonego 
                if ( (listOfParentChromosomes.OrderByDescending(o => o.HowManyHCDoneCounter).ToList()[0].HowManyHCDoneCounter == 10) && counterOfGenerations > minimumNumberOfGenerations ) //jezeli HC spełnione i osiągnięto założoną liczbe minimalna generacji
                {
                    if(listOfParentChromosomes.OrderByDescending(o => o.HowManyHCDoneCounter).ToList()[0].PenaltyOfChromosome<acceptablePenalty) // jezeli soft constraints na odpowiednim poziomie
                    {
                        Console.WriteLine("Mamy Rozwiązanie !");
                        Console.Read();

                        return listOfParentChromosomes.OrderBy(o => o.PenaltyOfChromosome).ToList()[0]; //mamy rozwiązanie wiec zwracamy chromosom z najmniejszą karą
                       
                    }
                    
                }
            }

            return listOfParentChromosomes.OrderByDescending(o => o.HowManyHCDoneCounter).ToList()[0]; //zwracam najlepszy chromosom gdy nie ma rozwiązania

        }


    }
}
