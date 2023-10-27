using Alercroy.Database.Timer;
using AutoMapper;
using Serilog;

namespace Alercroy.Services.Timer;

public class TimerService : ITimerService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly TimerDbContext _timerDbContext;

    public TimerService(
        ILogger logger,
        IMapper mapper,
        TimerDbContext timerDbContext)
    {
        _logger = logger;
        _mapper = mapper;
        _timerDbContext = timerDbContext;
    }

    public Task<Models.Timer> TimerById(Guid timerId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Models.Timer>> TimersByChat(ulong chatId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTimer(Guid timerId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task AddTimer(Models.Timer timer, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}