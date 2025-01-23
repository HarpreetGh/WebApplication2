using NSwag;
using NSwag.CodeGeneration.CSharp;

// UPDATE THE PORT NUMBER

public class ClientGenerator
{
    public async Task GenerateClient(int PORT)
    {
        using var httpClient = new HttpClient();
        var swaggerJson = await httpClient.GetStringAsync($"http://localhost:{PORT}/swagger/v1/swagger.json");
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "CustomdApiClient",
            CSharpGeneratorSettings = { Namespace = "CustomNamespace" }
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();

        await File.WriteAllTextAsync("GeneratedApiClient.cs", code);
    }
}