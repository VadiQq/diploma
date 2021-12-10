using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PDMF.Data.Algorithms.Matrices;
using PDMF.Data.Enums;
using PDMF.Parsing.Models;
using PDMF.Parsing.Models.Exceptions;
using PDMF.Parsing.Services.Abstract;

namespace PDMF.Parsing.Services
{
    public class DatasetParser : IDatasetParser
    {
        private const double Epsilon = 0.00001d;

        public async Task<ParsedDataset> Parse(Stream dataset, ParseOptions parseOptions = null)
        {
            parseOptions ??= ParseOptions.CreateDefault();

            var dataSeparator = ParseHelper.ValidateDataSeparator(parseOptions.DataSeparator);
            var parseResult = new ParsedDataset();
            var parseValues = new List<double[]>();

            using var reader = new StreamReader(dataset);

            string line = string.Empty;
            
            try
            {
                line = await reader.ReadLineAsync();
                var length = line.Split(dataSeparator).Count(s => s != "");

                parseResult.Headers = parseOptions.Mode == ParseMode.WithHeaders ? 
                    line.Split(dataSeparator) : 
                    ParseHelper.CreateDefaultHeaders(length);

                while (line != string.Empty || (line = await reader.ReadLineAsync()) != null)
                {
                    double[] parsedValues = new double[length];

                    var values = line.Split(dataSeparator).Where(s => s != "").ToArray();
                    
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (double.TryParse(values[j], NumberStyles.Number, CultureInfo.InvariantCulture, out var x))
                        {
                            parsedValues[j] = x < Epsilon ? Epsilon : x;
                        }
                    }

                    line = string.Empty;
                    parseValues.Add(parsedValues);
                }

                parseResult.Values = new Matrix(parseValues.Count, length, ToMultiArray(parseValues));
                return parseResult;
            }
            catch (Exception exception)
            {
                throw new ParseException
                {
                    InitialException = exception,
                    InitialExceptionType = exception.GetType()
                };
            }
        }

        private double[,] ToMultiArray(List<double[]> collection)
        {
            var lines = collection.Count;
            var columns = collection[0].Length;
            var multiArray = new double[lines, columns];

            for (int i = 0; i < lines; i++)
            {
                var values = collection[i];
                for (int j = 0; j < columns; j++)
                {
                    multiArray[i, j] = values[j];
                }
            }

            return multiArray;
        }
    }
}