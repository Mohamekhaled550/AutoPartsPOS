
namespace AutoPartsPOS.Models
{



public class ReturnItem
{
    public int Id { get; set; }

    public int ReturnId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Return Return { get; set; }
    public Product Product { get; set; }
}


}