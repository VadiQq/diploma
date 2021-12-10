namespace PDMF.Data.Azure.Models
{
    public class BatchProgramSettings
    {
        public string ProgramName { get; set; }
        public string ProgramPath { get; set; }
        public int DesiredColumn { get; set; }
        public string DatasetId { get; set; }
        public string ModelingTaskId { get; set; }
    }
}