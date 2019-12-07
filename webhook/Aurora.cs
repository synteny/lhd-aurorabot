using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace test
{
    //This class works with data from https://services.swpc.noaa.gov/text/aurora-nowcast-map.txt
    public static class Aurora
    {
        public static StringSplitOptions RemoveEmptyEntries { get; private set; }

        public static int GetProbability(double longitude, double latitude)
        {
            const double k = 0.3515625;
            int row = 18 + (int)(k * longitude); //There are 18 lines of header strings in the files
            int col = (int)(k * (latitude + 90)); //Latitude varies from -90 to 90

            const string path = "https://services.swpc.noaa.gov/text/aurora-nowcast-map.txt";

            int probability = Convert.ToInt32(ReadAtPos(path, row, col));

            return probability;
        }

        private static int ReadAtPos(string path, int row, int col)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://services.swpc.noaa.gov/text/aurora-nowcast-map.txt");
            StreamReader reader = new StreamReader(stream);

            // Read the file line by line
            string line;
            for (int counter = 0; (line = reader.ReadLine()) != null; counter++)
            {
                if (counter == row)
                {
                    string[] split = line.Replace("   ", " ").Replace("  ", " ").Split(' ');

                    var foos = new List<string>(split);
                    foos.RemoveAt(0);
                    split = foos.ToArray();

                    return Convert.ToInt32(split[col]);
                }
            }

            return -1;
        }
    }
}