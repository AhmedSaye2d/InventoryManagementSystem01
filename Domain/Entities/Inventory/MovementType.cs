namespace Domain.Entities.Inventory
{
    public enum MovementType
    {
        StockIn = 1,
        StockOut = 2,
        Adjustment = 3,
        Return = 4,
        RelocationIn = 5,
        RelocationOut = 6
    }
}
