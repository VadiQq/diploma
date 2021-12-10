using System.Linq;

namespace PDMF.Algorithms.GMDH.Services
{
    public static class CalculationExtensions
    {
        public static int CumulativeSum(int n)
        {
            var sum = Enumerable.Range(1, n - 1).Sum(i => i);
            
            return sum;
        }
        
        public static int GetNumberOfPreviousModels(int modelIndex, int modelsNumber)
        {
            var modelsShift = (1 + modelIndex) * modelIndex / 2 ;

            var index = modelIndex * modelsNumber - modelsShift;

            return index;
        }
    }
}