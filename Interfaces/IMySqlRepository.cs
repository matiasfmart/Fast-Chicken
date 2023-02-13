using System;
using FastChicken.Models;

namespace FastChicken.Interfaces
{
	public interface IMySqlRepository
    {
        List<Combo> GetCombos();
        int AddOrder(Order newOrder);
        void AddOrderItem(OrderItem newOrderItem);
        int getNewNumOrder();

        int startCashJournal();
        int endCashJournal();
    }
}