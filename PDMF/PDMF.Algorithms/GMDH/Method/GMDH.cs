using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Models;
using PDMF.Algorithms.GMDH.Services;
using PDMF.Algorithms.Shared;
using PDMF.Data.Algorithms.Matrices;

namespace PDMF.Algorithms.GMDH.Method
{
    public class GMDH
    {
        public WienerBinaryModel CreateModel(int desiredColumn, Matrix dataset, int parallelProcessesNumber)
        {
            Matrix previousXValues = null;
            Matrix previousXTestValues = null;
            
            var xMatrix =  dataset.CreateMatrixWithoutColumn(desiredColumn - 1);
            var yMatrix = dataset.CreateMatrixFromColumn(desiredColumn - 1);

            var valuesX = xMatrix.CutMatrixHorizontal(xMatrix.Rows / 2);
            var valuesY = yMatrix.CutMatrixHorizontal(yMatrix.Rows / 2);

            var testXMatrix = xMatrix.CutMatrixHorizontal(0, xMatrix.Rows / 2);
            var testYMatrix = yMatrix.CutMatrixHorizontal(0, yMatrix.Rows / 2);

            var firstRoundModels = GenerateFirstGenerationModels(parallelProcessesNumber, valuesX, valuesY, testXMatrix, testYMatrix);

            var filteredFirstRoundModels = FilterModels(firstRoundModels);

            WienerBinaryModel finalModel;
            
            if (filteredFirstRoundModels.Count < 3)
            {
                finalModel = firstRoundModels.FirstOrDefault();
                return finalModel;
            }
            
            double minExternalCriterion = 1;
            
            var minModelsCriterion = firstRoundModels[0].ExternalCriterion;
            minExternalCriterion = minModelsCriterion > minExternalCriterion ? minExternalCriterion : minModelsCriterion;
            var previousRoundModels = new List<WienerBinaryModel>();

            var newXValues = new double[valuesX.Rows, firstRoundModels.Count];
            for (int i = 0; i < firstRoundModels.Count; i++)
            {
                var model = firstRoundModels[i];
                for (int j = 0; j < valuesX.Rows; j++)
                {
                    newXValues[j, i] = model.CalculateModelValue(valuesX[j, model.FirstVariableIndex],
                        valuesX[j, model.SecondVariableIndex]);
                }
            }

            var newXTestValues = new double[testXMatrix.Rows, firstRoundModels.Count];
            for (int i = 0; i < firstRoundModels.Count; i++)
            {
                var model = firstRoundModels[i];
                for (int j = 0; j < testXMatrix.Rows; j++)
                {
                    newXTestValues[j, i] = model.CalculateModelValue(testXMatrix[j, model.FirstVariableIndex],
                        testXMatrix[j, model.SecondVariableIndex]);
                }
            }

            int round = 0;
            
            Console.WriteLine($"Round {round} - calculating");
            Console.WriteLine($"Round minimum external criterion - {minExternalCriterion}");
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var newXMatrix = new Matrix(valuesX.Rows, firstRoundModels.Count, newXValues);
            var newXTestMatrix = new Matrix(testXMatrix.Rows, firstRoundModels.Count, newXTestValues);

            while (true)
            {
                Console.WriteLine($"Round {round + 1} - calculating");
                
                var variablePairs = new List<Pair<int, int>>();
                var roundModels = new List<WienerBinaryModel>();
                
                if (round == 0)
                {
                    previousXValues = newXMatrix;
                    previousXTestValues = newXTestMatrix;
                    previousRoundModels = firstRoundModels;
                }

                if (round != 0)
                {
                    var possibleValuableModels = FilterModels(previousRoundModels);
                    
                    var amountToTake = possibleValuableModels.Count * 2 / 3;
                    if (amountToTake < 3)
                    {
                        finalModel = possibleValuableModels[0];
                        break;
                    }

                    previousRoundModels = possibleValuableModels.Take(amountToTake).ToList();
                }

                variablePairs = new List<Pair<int, int>>();
                for (int i = 0; i < previousRoundModels.Count; i++)
                {
                    for (int j = i + 1; j < previousRoundModels.Count; j++)
                    {
                        variablePairs.Add(new Pair<int, int>(i, j));
                    }
                }
                
                parallelProcessesNumber = parallelProcessesNumber > variablePairs.Count
                    ? variablePairs.Count
                    : parallelProcessesNumber;
                
                var processWorkAmount = variablePairs.Count / parallelProcessesNumber;
                var offset = variablePairs.Count % parallelProcessesNumber;
                var tasks = new ConcurrentBag<Task>();
                
                var taskRoundModels = new List<WienerBinaryModel>[parallelProcessesNumber];

                for (int i = 0; i < parallelProcessesNumber; i++)
                {
                    var index = i;
                    tasks.Add(new Task(() =>
                        ProcessRoundModelGenerationAsync(
                            index,
                            processWorkAmount,
                            offset,
                            parallelProcessesNumber,
                            round,
                            taskRoundModels,
                            variablePairs,
                            previousRoundModels,
                            previousXTestValues,
                            testYMatrix,
                            previousXValues,
                            yMatrix)));
                }

                foreach (var task in tasks)
                {
                    task.Start();
                }

                Task.WaitAll(tasks.ToArray());

                foreach (var modelsList in taskRoundModels)
                {
                    roundModels.AddRange(modelsList);
                }

                roundModels = roundModels.OrderBy(m => m.ExternalCriterion).ToList();

                if (roundModels[0].ExternalCriterion >= minModelsCriterion)
                {
                    Console.WriteLine("Models are degrade - abort");

                    finalModel = previousRoundModels[0];
                    
                    break;
                }
                
                Console.WriteLine($"Round {round + 1} min external criterion - {roundModels[0].ExternalCriterion}");
                
                minModelsCriterion = roundModels[0].ExternalCriterion;
                previousRoundModels = roundModels.ToList();

                var modelXValues = new double[previousXValues.Rows, roundModels.Count];
                for (int i = 0; i < roundModels.Count; i++)
                {
                    var model = roundModels[i];
                    for (int j = 0; j < previousXValues.Rows; j++)
                    {
                        modelXValues[j, i] = model.CalculateModelValue(
                            previousXValues[j, model.SubModels[0].ModelIndex],
                            previousXValues[j, model.SubModels[1].ModelIndex]);
                    }
                }

                var modelXTestValues = new double[previousXTestValues.Rows, roundModels.Count];
                for (int i = 0; i < roundModels.Count; i++)
                {
                    var model = roundModels[i];
                    for (int j = 0; j < previousXTestValues.Rows; j++)
                    {
                        modelXTestValues[j, i] = model.CalculateModelValue(
                            previousXTestValues[j, model.SubModels[0].ModelIndex],
                            previousXTestValues[j, model.SubModels[1].ModelIndex]);
                    }
                }

                previousXValues = new Matrix(previousXValues.Rows, roundModels.Count, modelXValues);
                previousXTestValues = new Matrix(previousXTestValues.Rows, roundModels.Count, modelXTestValues);
                round++;
            }

            //printing result to console
            var bestModel = finalModel;
            Console.WriteLine("Time elapsed: " + (float) stopWatch.ElapsedMilliseconds / 1000);
            Console.Write($"Best model: m{bestModel.RoundIndex}{bestModel.ModelIndex}; External criterion: {bestModel.ExternalCriterion}; ");
            
            Console.WriteLine(bestModel.ExternalCriterion < 0.05
                ? "Model is valuable; "
                : "Model external criterion is larger then expected. Model might be not accurate;");

            return bestModel;
        }

        private List<WienerBinaryModel> GenerateFirstGenerationModels(
            int parallelProcessesNumber,
            Matrix xValues,
            Matrix yValues,
            Matrix xTestValues,
            Matrix yTestValues)
        {
            var variablePairs = new List<Pair<int, int>>();
            
            var processVariablesCount = xValues.Columns;
            
            for (int i = 0; i < processVariablesCount; i++)
            {
                for (int j = i + 1; j < processVariablesCount; j++)
                {
                    variablePairs.Add(new Pair<int, int>(i, j));
                }
            }
            
            parallelProcessesNumber = parallelProcessesNumber > variablePairs.Count
                ? variablePairs.Count
                : parallelProcessesNumber;

            var processWorkAmount = variablePairs.Count / parallelProcessesNumber;
            var offset = variablePairs.Count % parallelProcessesNumber;
            
            var firstRoundModels = new List<WienerBinaryModel>[parallelProcessesNumber];
            var tasks = new ConcurrentBag<Task>();
            
            for (int i = 0; i < parallelProcessesNumber; i++)
            {
                var index = i;
                tasks.Add(new Task(() =>
                    ProcessFirstRoundModelGenerationAsync(
                        index,
                        processWorkAmount,
                        offset,
                        parallelProcessesNumber,
                        firstRoundModels,
                        variablePairs,
                        xValues,
                        yValues,
                        xTestValues,
                        yTestValues)));
            }

            foreach (var task in tasks)
            {
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());

            var modelsList = firstRoundModels
                .SelectMany(models => models)
                .OrderBy(model => model.ExternalCriterion)
                .ToList();
            
            var counter = 0;
            foreach (var model in modelsList)
            {
                model.ModelIndex = counter++;
            }

            return modelsList.OrderBy(model => model.ExternalCriterion).ToList();
        }
        
        private void ProcessFirstRoundModelGenerationAsync(
            int index, 
            int processWorkAmount, 
            int offset, 
            int processesNumber,
            List<WienerBinaryModel>[] firstRoundModels,
            List<Pair<int, int>> variablePairs,
            Matrix xValues,
            Matrix yValues,
            Matrix xTestValues,
            Matrix yTestValues)
        {
            var startPosition = index * processWorkAmount;
            var amount = index == processesNumber - 1
                ? processWorkAmount + offset
                : processWorkAmount;

            var variablePairsBatch = variablePairs.Skip(startPosition).Take(amount);

            var models = new List<WienerBinaryModel>();
            
            foreach (var pair in variablePairsBatch)
            {
                models.Add(new WienerBinaryModel(0, pair.First, pair.Second, xValues, yValues));
            }

            //calculating zero round models' external criterion
            
            foreach (var model in models)
            {
                model.GetExternalCriterion(xTestValues, yTestValues);
            }

            firstRoundModels[index] = models;
        }
        
        private void ProcessRoundModelGenerationAsync(
            int index, 
            int processWorkAmount, 
            int offset, 
            int processesNumber,
            int round,
            List<WienerBinaryModel>[] taskModels,
            List<Pair<int, int>> variablePairs,
            List<WienerBinaryModel> previousRoundModels,
            Matrix previousXTestValues,
            Matrix testYMatrix,
            Matrix previousXValues,
            Matrix yMatrix)
        {
            var startPosition = index * processWorkAmount;
            var amount = index == processesNumber - 1
                ? processWorkAmount + offset
                : processWorkAmount;

            var variablePairsBatch = variablePairs.Skip(startPosition).Take(amount);
            var length = previousRoundModels.Count;

            var currentTaskModels = new List<WienerBinaryModel>();

            foreach (var pair in variablePairsBatch)
            {
                int modelIndex = CalculationExtensions.GetNumberOfPreviousModels(pair.First, length) + (pair.Second - pair.First - 1);
                currentTaskModels.Add(new WienerBinaryModel(modelIndex, round + 1, previousXValues, yMatrix,
                    previousRoundModels[pair.First],
                    previousRoundModels[pair.Second]));
            }

            foreach (var model in currentTaskModels)
            {
              model.GetExternalCriterion(previousXTestValues, testYMatrix);
            }

            taskModels[index] = currentTaskModels;
        }

        private List<WienerBinaryModel> FilterModels(List<WienerBinaryModel> models, double maxExternalCriterion = 0.1d)
        {
            return models.Where(m => m.ExternalCriterion < 0.1).ToList();
        }
    }
}