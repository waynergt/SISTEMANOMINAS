using System.Net.Http.Json;

public class ExpedienteService
{
    private readonly HttpClient _http;

    public ExpedienteService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ExpedienteValidacionResult?> ValidarExpediente(int empleadoId)
    {
        return await _http.GetFromJsonAsync<ExpedienteValidacionResult>($"api/Expediente/validar/{empleadoId}");
    }
}

public class ExpedienteValidacionResult
{
    public int empleadoId { get; set; }
    public bool expedienteCompleto { get; set; }
    public List<string> documentosFaltantes { get; set; } = new();
}