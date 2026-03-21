namespace Credenciamento.Application.Handlers.User;

public class GetOtpCodeQueryHandler : IRequestHandler<GetOtpCodeQuery, GetOtpCodeQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IUserService _service;
    public GetOtpCodeQueryHandler(
        ILogger<GetOtpCodeQueryHandler> logger,
        IUserService service)
    {
        _logger = logger;
        _service = service;
    }   

    public async Task<GetOtpCodeQueryResponse> Handle(GetOtpCodeQuery request, CancellationToken cancellationToken)
    {
        GetOtpCodeQueryResponse response = new();

        try
        {
            response.Success = await _service.GenerateOnetTimePasswordAsync(request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return response;
    }
}


