using Core.Kernel.Models;

namespace Core.Services;

public interface IStatusService {
    Task<List<QueueStatus>> QueueStatus();
}