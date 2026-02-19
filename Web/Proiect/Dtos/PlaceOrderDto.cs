namespace Proiect.Dtos;

public class PlaceOrderDto
{
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public List<PlaceOrderItemDto> Items { get; set; } = new();
}

public class PlaceOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}