using System;
using System.Threading.Tasks;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IReleaseManager
    {
        Task CheckForNewReleaseAsync(CheckForReleaseArgs checkForReleaseArgs);
    }
}
