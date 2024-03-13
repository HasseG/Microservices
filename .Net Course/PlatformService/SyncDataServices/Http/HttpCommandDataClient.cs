using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _conf;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration conf)
    {
        _httpClient = httpClient;
        _conf = conf;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_conf["CommandService"]}", plat);

        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync POST to CommandService was OK!");
        }
        else
        {
            Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
        }
    }
}
