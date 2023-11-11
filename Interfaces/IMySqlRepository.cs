using System;
using System.Collections.Specialized;
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
        bool checkPassword(string username, string password);
        IList<Drink> getDrinks();
        IList<Drink> getBigDrinks();
        IList<Side> getSides();
        Drink getDrink(int id);
        Drink getBigDrink(int id);
        Side getSide(int id);
        Product getProduct(int id);
        IList<Drink> getDrinksByCombo(string comboId);
        IList<Side> getSidesByCombo(string comboId);
        IList<Product> getProductsByCombo(string comboId);
        IList<Product> getProducts();
        IList<Combo> getCombos();
        void UpdateDrinksQty(int idDrink, double quantity);
        void UpdateBigDrinksQty(int idDrink, double quantity);
        void UpdateProductsQty(int idProduct, double quantity);
        void UpdateSidesQty(int idSide, double quantity);
        Combo GetCombo(string id);
        Dictionary<int,int> getQuantityById(string idCombo, string type);
        void GrabarCombo(Combo combo);
        void DecrementProduct(int idProduct, int value);
        void DecrementDrink(int idDrink, int value);
        void DecrementSide(int idSide, int value);
        int GetComboQuantity(string comboId, int idDetail, string Type);
        TotalJournal GetTotalJournal(int idJournal);

        void GrabarDrink(Drink drink);
        void GrabarSide(Side side);
        void GrabarProduct(Product product);
        void GrabarBigDrink(Drink drink);
    }
}