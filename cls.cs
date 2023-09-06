using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace cls
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initial dataset variables
            List<double> dataX = new List<double>();
            List<int> dataClass = new List<int>();

            //Regex paterns used later for modifying input data
            Regex dotPattern = new Regex("[.]");
            Regex columnPattern = new Regex("[,]");

            //Training data loading
            try
            {
                string line;
                StreamReader sr = new StreamReader("cls_train.csv"); //Input file
                line = sr.ReadLine(); //First line - Ignore it because it's just the heading
                line = sr.ReadLine(); //Second line of the input
                while (line != null) //While there is something to read from the file
                {
                    string[] split = line.Split(',');
                    //Dots are used in the data set as decimal points, which is problem for Parsing to number in .NET
                    //So first I have to replace dots with comma in order to use Parse() method                    
                    dataX.Add(double.Parse(dotPattern.Replace(split[1], ",")));
                    dataClass.Add(int.Parse(dotPattern.Replace(split[2], ",")));
                    line = sr.ReadLine(); //Read next line
                }
                sr.Close(); //Close the file
            }
            catch (Exception e)
            {
                Console.WriteLine("Loading dataset error: " + e.Message);
            }

            //Number of all input data
            int n = dataX.Count();

            //Variables for number of appearances of each class
            int r1 = 0; //0
            int r2 = 0; //1
            int r3 = 0; //2
            int r4 = 0; //3

            //Count number of each class
            for (int i = 0; i < n; i++)
            {
                if (dataClass[i] == 0)
                    r1++;
                else if (dataClass[i] == 1)
                    r2++;
                else if (dataClass[i] == 2)
                    r3++;
                else if (dataClass[i] == 3)
                    r4++;
            }

            //Probability of each class
            double p1 = r1 / n;
            double p2 = r2 / n;
            double p3 = r3 / n;
            double p4 = r4 / n;

            //Initialize sum x*ri for each class
            double sumxr1 = new double();
            double sumxr2 = new double();
            double sumxr3 = new double();
            double sumxr4 = new double();

            for (int i = 0; i < n; i++)
            {
                int ri = 0;

                //First class
                if (dataClass[i] == 0)
                    ri = 1;
                else
                    ri = 0;
                sumxr1 += dataX[i] * ri;

                //Second class
                if (dataClass[i] == 1)
                    ri = 1;
                else
                    ri = 0;
                sumxr2 += dataX[i] * ri;

                //Third class
                if (dataClass[i] == 2)
                    ri = 1;
                else
                    ri = 0;
                sumxr3 += dataX[i] * ri;

                //Fourth class
                if (dataClass[i] == 3)
                    ri = 1;
                else
                    ri = 0;
                sumxr4 += dataX[i] * ri;
            }

            //Calculate mean of each class
            double mean1 = sumxr1 / r1;
            double mean2 = sumxr2 / r2;
            double mean3 = sumxr3 / r3;
            double mean4 = sumxr4 / r4;

            double sq1 = new double();
            double sq2 = new double();
            double sq3 = new double();
            double sq4 = new double();

            //Calculate S-square of each class
            for (int i = 0; i < n; i++)
            {
                int ri = 0;
                if (dataClass[i] == 0)
                    ri = 1;
                else
                    ri = 0;
                sq1 += Math.Pow((dataX[i] - mean1), 2) * ri;

                if (dataClass[i] == 1)
                    ri = 1;
                else
                    ri = 0;
                sq2 += Math.Pow((dataX[i] - mean2), 2) * ri;

                if (dataClass[i] == 2)
                    ri = 1;
                else
                    ri = 0;
                sq3 += Math.Pow((dataX[i] - mean3), 2) * ri;

                if (dataClass[i] == 3)
                    ri = 1;
                else
                    ri = 0;
                sq4 += Math.Pow((dataX[i] - mean4), 2) * ri;
            }

            //S-square
            double s1 = sq1 / r1;
            double s2 = sq2 / r2;
            double s3 = sq3 / r3;
            double s4 = sq4 / r4;

            //Input test data set
            List<double> testX = new List<double>();            
            try
            {
                string line;
                StreamReader sr = new StreamReader("cls_test.csv"); //Input file
                line = sr.ReadLine(); //First line - Ignore it because it's just the heading
                line = sr.ReadLine(); //Second line of the input
                while (line != null) //While there is something to read from the file
                {
                    string[] split = line.Split(',');
                    //Again, the same problem with Parsing data - needs to be replaced first
                    testX.Add(double.Parse(dotPattern.Replace(split[1], ",")));
                    line = sr.ReadLine(); //Read next line
                }
                sr.Close(); //Close the file
            }
            catch (Exception e)
            {
                Console.WriteLine("Loading dataset error: " + e.Message);
            }
            //Create correspondingly sized array for results (according to the size of our testing data set)
            int[] testClassResult = new int[testX.Count()];
            Console.WriteLine(testX[0] + " " + testX[1] + " " + testX[2]);
            //Create output file and prepare the stream
            FileStream stream = new FileStream("cls_result.csv", FileMode.Create);
            StreamWriter file = new StreamWriter(stream);
            using (file)
            {
                for (int i = 0; i < testX.Count(); i++)
                {
                    //DISCRIMINANT FUNCTIONS
                    double g1, g2, g3, g4;

                    //Since we have Gaussian distribution, we can calculate discriminants as follows
                    g1 = -1 * Math.Log(s1) - Math.Pow((testX[i] - mean1), 2) / (2 * s1);
                    g2 = -1 * Math.Log(s2) - Math.Pow((testX[i] - mean2), 2) / (2 * s2);
                    g3 = -1 * Math.Log(s3) - Math.Pow((testX[i] - mean3), 2) / (2 * s3);
                    g4 = -1 * Math.Log(s4) - Math.Pow((testX[i] - mean4), 2) / (2 * s4);

                    //Get max value from all discriminants
                    double maxG = new double[] { g1, g2, g3, g4 }.Max();

                    //Select corresponding class
                    if (maxG == g1)
                        testClassResult[i] = 0;
                    else if (maxG == g2)
                        testClassResult[i] = 1;
                    else if (maxG == g3)
                        testClassResult[i] = 2;
                    else if (maxG == g4)
                        testClassResult[i] = 3;

                    //Write the output to the file
                    file.WriteLine("{0},{1},{2}", i, columnPattern.Replace(testX[i].ToString(), "."), testClassResult[i]);
                }
            }
        }
    }
}
