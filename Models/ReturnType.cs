namespace AutoPartsPOS.Models
{
    public enum ReturnType
    {
        Sales = 1,
        Maintenance = 2, // صيانة (شاملة مصنعية وقطع)
        ServiceOnly = 3  // إرجاع مبلغ مصنعية فقط
    }
}
