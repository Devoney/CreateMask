using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IGitHubIssueCreator
    {
        void CreateIssue(ErrorReport errorReport);
    }
}
