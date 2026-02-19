using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using NutBuddiesApp.Models;

namespace NutBuddiesApp.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public ApiClient()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(NutBuddiesApp.AppConfig.ApiBaseUrl)
        };
    }

    public void SetBearer(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var res = await _http.GetAsync("/api/products");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ProductDto>>(json, _json) ?? new();
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var payload = JsonSerializer.Serialize(new { email, password }, _json);

        var res = await _http.PostAsync("/api/auth/login",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString();
    }
    
    public record PlaceOrderItemRequest(int ProductId, int Quantity);
    public record PlaceOrderRequest(string CustomerName, string Phone, List<PlaceOrderItemRequest> Items);

    public async Task<int?> PlaceOrderAsync(string customerName, string phone, List<PlaceOrderItemRequest> items)
    {
        var payload = JsonSerializer.Serialize(new PlaceOrderRequest(customerName, phone, items), _json);

        var res = await _http.PostAsync("/api/orders",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadAsStringAsync();
        System.Diagnostics.Debug.WriteLine("PLACE ORDER RESPONSE: " + json);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("orderId").GetInt32();
    }
    
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var res = await _http.GetAsync($"/api/products/{id}");
        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Product>(json, _json);
    }
    
    public async Task<bool> AddReviewAsync(int productId, int rating, string comment)
    {
        var payload = new
        {
            productId = productId,
            rating = rating,
            comment = comment
        };

        
        var res = await _http.PostAsJsonAsync("/api/reviews", payload);
        var body = await res.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"POST /api/reviews => {(int)res.StatusCode} {res.StatusCode}");
        System.Diagnostics.Debug.WriteLine(body);
        return res.IsSuccessStatusCode;
    }



}



public record ProductDto(int Id, string Name, decimal Price, int StockQty, string? Category);

