using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Constraints.Tests")]


namespace NURSESCHEDULING_FINAL_PROJECT
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.SetBufferSize(200, 100);
            ChromosomeClass obChromosomeClass = new ChromosomeClass();
            obChromosomeClass.writeNursesFromChromosomeFromEachShift();



            GeneticAlgorithmClass obGeneticAlgorithmClass = new GeneticAlgorithmClass(6,0,2000,1000,1000,4);
            obChromosomeClass = obGeneticAlgorithmClass.runAlgorithm();

            obChromosomeClass.writeNursesFromChromosomeFromEachShift();  //wypisz ten chromosom ktory jest wynikiem

            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
