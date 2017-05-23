﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NURSESCHEDULING_FINAL_PROJECT
{
    class ChromosomeClass
    {
        //składowe pomocnicze
        public PoolOfNurses obPoolOfNurses;
        //do Algorytmu genetycznego
        public NurseClass[][][][] chromosomeVector = new NurseClass[5][][][]; //to jest poszarpana tablica (jagged) bo mamy różne ilości pielegniarke na różne zmiany
        public AbstractConstraintsClass obConstraintsClass = new ConcreteConstraintsClass();
        int penaltyOfChromosome;
        int howManyHCDoneCounter;
        bool[] tableOfHardConstraintsDone=new bool[10]{false,false,false,false,false, false, false, false, false, false}; //na początku wszystkie niespełnione

        //Właściwości
        public int PenaltyOfChromosome { get => penaltyOfChromosome; }
        public int HowManyHCDoneCounter { get => howManyHCDoneCounter; set => howManyHCDoneCounter = value; }

        public ChromosomeClass()
        {
            //przypisanie zdarzen obsługujących wystapienie HardConstraints
            obConstraintsClass.HCDone += HCDoneHandler;
            
            //bedzie wiele chromosomów wiec musze jakby zresetowac id od ktorego bd przydzielane nastepne
            NurseClass.counterOfNurseObject = 1;


            //tworze wektor
            for (int week = 0; week < 5; week++)
            {
                chromosomeVector[week] = new NurseClass[7][][]; //tworze tablice na tygodnie
                for (int day = 0; day < 7; day++)
                {
                    chromosomeVector[week][day] = new NurseClass[4][];
                    for (int shift = 0; shift < 4; shift++)
                    {
                        if (day < 5 && shift <= 2) //od pon do piatku 3 pielegniarki na zmiane od early do late (z dokumentu Bargieły)
                            chromosomeVector[week][day][shift] = new NurseClass[3];
                        if (day < 5 && shift >= 3)//od pon do piatku zmiana night
                            chromosomeVector[week][day][shift] = new NurseClass[1];


                        if (day >= 5 && shift <= 2)//od piatku do soboty 3 pierwsze zmiany
                            chromosomeVector[week][day][shift] = new NurseClass[2];
                        if (day >= 5 && shift >= 3)//od piatku do soboty zmiana night
                            chromosomeVector[week][day][shift] = new NurseClass[1];

                    }

                }
            }

            obPoolOfNurses = new PoolOfNurses();

            init("ordered"); // wypelniam wektor losowymi wartosciami("random") lub uporzadkowanymi ("ordered")
        }

        private void init(string randomOrOrdered) //tu poprostu inicjuje stworzony wczesniej wektor na pielegniarki
        {
            for(int week=0;week<5;week++)
            {
                for(int day=0;day<7;day++)
                {
                    for(int shift=0;shift<4;shift++)
                    {
                        //uzupelniam vektor po kolei p1 p2 p3 .....
                        for(int indexOfNurse=0;indexOfNurse< chromosomeVector[week][day][shift].Length; indexOfNurse++)
                        {
                            if(randomOrOrdered=="random")
                                chromosomeVector[week][day][shift][indexOfNurse] = obPoolOfNurses.getRandomNurseFromPool(); 
                            if(randomOrOrdered=="ordered")
                            {
                                NurseClass selectedNurse = obPoolOfNurses.getNurseFromPoolByOrder();

                                //Console.WriteLine(selectedNurse.ID.ToString());
                                chromosomeVector[week][day][shift][indexOfNurse] = selectedNurse;
                            }
                                
                        }
                    }

                }
            }
        }
        public void writeChromosomeVectorOnConsole()
        {
            //najpierw wypisuje literki zmian odpowiednio e d l n e d l n itd..

            for (int i=0;i<16;i++)
            {
                Console.SetWindowSize(Console.LargestWindowWidth,Console.LargestWindowHeight);
               
                Console.Write("e  d  l  n |");
            }

            //DO 16 ZNAKOW
            //wypisuje odpowiednie id pielegniarek teraz do 16 dnia bo na tylu sie wyswietla ta czesc chromosomu
            Console.Write("\n");
            int dayCounter = -1;
            for (int indexOfNurseOnShift = 0; indexOfNurseOnShift < 3; indexOfNurseOnShift++)
            {
                here:
                if (indexOfNurseOnShift == 3) break;
                for (int week = 0; week < 5; week++)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        dayCounter++;  //jesli 16 dni wydrukowało to nie niech nie drukuje dalej tylko przejdzie do nastepnej linii
                        if (dayCounter == 16)
                        {
                            Console.WriteLine();
                            indexOfNurseOnShift++;
                            dayCounter = -1;
                            goto here;
                        }
                        for (int shift = 0; shift < 4; shift++)
                        {
                            //wypisuje wartosci - pielegniarki na danych zmianach
                            if (chromosomeVector[week][day][shift].Length > indexOfNurseOnShift) //jesli na danej zmianie jest pielegniarka - tablica ma zmienny rozmiar przeciez
                            {
                                string idPielegniarkiString = chromosomeVector[week][day][shift][indexOfNurseOnShift].ID.ToString();
                                if (idPielegniarkiString.Length == 1) idPielegniarkiString = "0" + idPielegniarkiString;
                                Console.Write(idPielegniarkiString + " ");
                            }
                            else
                                Console.Write("00 "); // jesli pielegniarki nie ma na tej zmianie
                           
                        }

                    }
                }

              
               
            }
            //------------------------------------------------------------------------------------------------------------------------------------
            //DO 32 ZNAKOW
            for (int i = 0; i < 16; i++)
            {
               
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                Console.Write("e  d  l  n |");
            }

            bool czyPisac = false;
            Console.WriteLine();
            dayCounter = -1;
            for (int indexOfNurseOnShift = 0; indexOfNurseOnShift < 3; indexOfNurseOnShift++)
            {
                here:
                if (indexOfNurseOnShift == 3) break;
                for (int week = 0; week < 5; week++)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        if (week >= 2 && day >= 2)
                        {
                            czyPisac = true;       //to tylko raz na wiersz
                        }
                            dayCounter++;  //jesli 16 dni wydrukowało to nie niech nie drukuje dalej tylko przejdzie do nastepnej linii

                            if (dayCounter == 16)
                            {
                                czyPisac = false;
                                Console.WriteLine();
                                indexOfNurseOnShift++;
                                dayCounter = 16;
                                goto here;
                            }

                        if (czyPisac)
                        {
                            for (int shift = 0; shift < 4; shift++)
                            {
                                //wypisuje wartosci - pielegniarki na danych zmianach
                                if (chromosomeVector[week][day][shift].Length > indexOfNurseOnShift) //jesli na danej zmianie jest pielegniarka - tablica ma zmienny rozmiar przeciez
                                {
                                    string idPielegniarkiString = chromosomeVector[week][day][shift][indexOfNurseOnShift].ID.ToString();
                                    if (idPielegniarkiString.Length == 1) idPielegniarkiString = "0" + idPielegniarkiString;
                                    Console.Write(idPielegniarkiString + " ");
                                }
                                else
                                    Console.Write("00 "); // jesli pielegniarki nie ma na tej zmianie

                            }
                        }

                        
                       

                    }
                }

            }



            //OD 32 DO 35 DNI
            Console.Write("\n");
            for (int i = 0; i < 3; i++)
            {

                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                Console.Write("e  d  l  n |");
            }
            Console.WriteLine();
            //dayCounter = -1;
            for (int indexOfNurseOnShift = 0; indexOfNurseOnShift < 3; indexOfNurseOnShift++)
            {
                here:
                if (indexOfNurseOnShift == 3) break;
                for (int week = 0; week < 5; week++)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        dayCounter++;  //jesli 16 dni wydrukowało to nie niech nie drukuje dalej tylko przejdzie do nastepnej linii
                        if (dayCounter == 35)
                        {
                            Console.WriteLine();
                            indexOfNurseOnShift++;
                            dayCounter = 32;
                            goto here;
                        }
                        for (int shift = 0; shift < 4; shift++)
                        {
                            //wypisuje wartosci - pielegniarki na danych zmianach
                            if (chromosomeVector[week][day][shift].Length > indexOfNurseOnShift) //jesli na danej zmianie jest pielegniarka - tablica ma zmienny rozmiar przeciez
                            {
                                string idPielegniarkiString = chromosomeVector[week][day][shift][indexOfNurseOnShift].ID.ToString();
                                if (idPielegniarkiString.Length == 1) idPielegniarkiString = "0" + idPielegniarkiString;
                                Console.Write(idPielegniarkiString + " ");
                            }
                            else
                                Console.Write("00 "); // jesli pielegniarki nie ma na tej zmianie

                        }

                    }
                }

            }

        }

        public void writeNursesFromPoolWhichChromosomeUsing()
        {
            NurseClass currentNurse = NurseNavigator.getNextNurseFromNursePool(obPoolOfNurses);

            while(currentNurse!=null)
            {    
                Console.WriteLine(currentNurse.ID.ToString());
                currentNurse = NurseNavigator.getNextNurseFromNursePool(obPoolOfNurses);
            }

        }

        public void writeNursesFromChromosomeFromEachShift()
        {
            NurseNavigator.clearChromosomeStatements();

            NurseClass currentNurse = NurseNavigator.getNextNurseFromChromosome(chromosomeVector);
            int lastDay = 0;
            Console.WriteLine("Monday");

            Console.SetBufferSize(400, 400);
            while (currentNurse != null)
            {
                if(lastDay != NurseNavigator.CurrentDay) // jezeli zmiana dnia to wypisz ten dzien
                {
                    if (NurseNavigator.CurrentDay == 0)
                        Console.WriteLine("Monday");
                    if (NurseNavigator.CurrentDay == 1)
                        Console.WriteLine("Tuesday");
                    if (NurseNavigator.CurrentDay == 2)
                        Console.WriteLine("Wednesday");
                    if (NurseNavigator.CurrentDay == 3)
                        Console.WriteLine("Thursday");
                    if (NurseNavigator.CurrentDay == 4)
                        Console.WriteLine("Friday");
                    if (NurseNavigator.CurrentDay == 5)
                        Console.WriteLine("Saturday");
                    if (NurseNavigator.CurrentDay == 6)
                        Console.WriteLine("Sunday");
                }
                lastDay = NurseNavigator.CurrentDay;

                Console.SetCursorPosition(0, Console.CursorTop); Console.Write(currentNurse.ID.ToString());  Console.SetCursorPosition(3,Console.CursorTop); Console.Write(NurseNavigator.CurrentShift);
                Console.WriteLine();
                currentNurse = NurseNavigator.getNextNurseFromChromosome(chromosomeVector);

                
            }

        }

        /// <summary>
        /// włącznie z podanymi granicami przedziału dla podanych week , day, shift
        /// </summary>
        public void writeNursesFromChromosomeFromIntervalShift(sbyte fromWeekInterval, sbyte toWeekInterval, sbyte fromDayInterval, sbyte toDayInterval, sbyte fromShiftInterval, sbyte toShiftInterval)
        {
            NurseNavigator.setIntervalForReturnedNursesFromChromosome(fromWeekInterval, toWeekInterval, fromDayInterval, toDayInterval, fromShiftInterval, toShiftInterval);
            NurseClass currentNurse = NurseNavigator.getNextNurseFromChromosome(chromosomeVector);

            while (currentNurse != null)
            {
                Console.WriteLine(currentNurse.ID.ToString()+ " " /*+ NurseNavigator.CurrentDay.ToString()*/);
                currentNurse = NurseNavigator.getNextNurseFromChromosome(chromosomeVector);
            }
        }

        public int checkConstraints()
        {
           return penaltyOfChromosome = obConstraintsClass.checkConstraints(chromosomeVector,obPoolOfNurses);
        }


        public void mutation(int howMuchMutationWillBeDone)
        {
            //wybieram całkowicie losową zmiane i zamieniam ją z całkowicie losowo wybraną zmianą
            int counterOfMutationDone = 0;
            Random rnd = new Random();  //do losowania z week, day, shift
            //zmienne pomocnicze do zamiany

            //zamieniana zmiana
            int weekOfShift1;
            int dayOfShift1;
            int shiftOfShift1;

            //z którą zamienić
            int weekOfShift2;
            int dayOfShift2;
            int shiftOfShift2;

            //gdzie chwilowo przechować jedną zmiane(te pare pielegniarek)
            NurseClass[] tempTableForNurses;

            while (counterOfMutationDone<howMuchMutationWillBeDone)//z kazdym obraotem zamiana zmian
            {
                //zmiana 1
                weekOfShift1 = rnd.Next(0, 4);
                dayOfShift1 = rnd.Next(0, 6);
                shiftOfShift1 = rnd.Next(0, 3);

                //zmiana2
                weekOfShift2 = rnd.Next(0, 4);
                dayOfShift2 = rnd.Next(0, 6);
                shiftOfShift2 = rnd.Next(0, 3);

                //mamy 2 zmiany wiec je teraz zamieniamy ze soba 2 w miejsce 1 i 1 w miejsce 2
                //najpierw 1 zmiana do schowka
                tempTableForNurses = chromosomeVector[weekOfShift1][dayOfShift1][shiftOfShift1];

                chromosomeVector[weekOfShift1][dayOfShift1][shiftOfShift1] = chromosomeVector[weekOfShift2][dayOfShift2][shiftOfShift2];

                chromosomeVector[weekOfShift2][dayOfShift2][shiftOfShift2] = tempTableForNurses;

                counterOfMutationDone++;
            }
        }

        //Obsługa zdarzeń (odpowiednie inkrementowanie zmiennej LevelOfHardConstraintsExecuted) - jesli 1 spelnione to 1 HC spełniony   
        //Zdatrzenia potrzebne są przy debugowaniu algorytmu - żeby widać było w jaki jesteśmy momencie (ile hard constraints jest spełnionych)
        public void HCDoneHandler(int whichConstraintsDone)
        {
            tableOfHardConstraintsDone[whichConstraintsDone-1] = true;

            Console.WriteLine("HC " + whichConstraintsDone + "Is Done");
            //Console.Read();


        }

        public int checkHowManyHCIsDone()
        {
            int counterOfHCDone = 0;
            for(int i=0;i<tableOfHardConstraintsDone.Length;i++)
            {
                if (tableOfHardConstraintsDone[i]) counterOfHCDone++;
            }

            return howManyHCDoneCounter = counterOfHCDone;  //uzupełnij pole w klasie i zwróc wartosc
        }

    }
}
