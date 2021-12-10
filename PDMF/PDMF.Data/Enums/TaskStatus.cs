namespace PDMF.Data.Enums
{
    public enum TaskStatus
    {
        Pending,
        Processing,
        WaitingForNextStep,
        Complete,
        Failed,
        Finishing,
        Timeout
    }
}