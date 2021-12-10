using System.Globalization;
using System.IO;
using System.Linq;
using PDMF.Data.Algorithms.Matrices;

namespace PDMF.Algorithms.GMDH.Services
{
    public static class DataManager
    {
        public const double Epsilon = 0.00001d;
        public static string[] ReadFromFile(string path)
        {
            var content = File.ReadLines(path).ToArray();
            return content;
        }

        public static Matrix GetValues(string[] readData, string dataSeparator)
        {
            var length = readData[0].Split(dataSeparator).Length - 1;
            double[,] valuesX = new double[readData.Length, length];

            for (int i = 0; i < readData.Length; i++)
            {
                string valueString = readData[i];
                var values = valueString.Split(dataSeparator).Where(s => s != "").ToArray();
                for (int j = 1; j <  values.Length; j++)
                {
                        if (double.TryParse(values[j],NumberStyles.Number, CultureInfo.InvariantCulture, out double x))
                        {
                            valuesX[i, j - 1] = x < Epsilon ? Epsilon : x;
                        }
                }
            }
 
            var xMatrix = new Matrix(readData.Length, length, valuesX);
            return xMatrix;
        }

        public static int GetDataLength(string[] readData, string dataSeparator)
        {
            return readData[0].Split(dataSeparator).Length - 1;
        }
    }
}
