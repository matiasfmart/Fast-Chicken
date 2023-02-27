using System;

using System.Collections;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using FastChicken.Models;
using FastChicken.Interfaces;
using static NuGet.Packaging.PackagingConstants;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace FastChicken.DBConnection
{
	public class MySqlRepository : IMySqlRepository
    {
		private string _connectionString { get; set; }
		public MySqlRepository()
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional:true, reloadOnChange:true)
				.AddEnvironmentVariables()
				.Build();
			_connectionString = config.GetSection("ConnectionStrings:DefaultConnection").Value;
		}

		public int AddOrder(Order newOrder)
		{
			using(MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
					int idOrder = -1;

                    connection.Open();
					string SQL = "INSERT INTO `FastChicken`.`Orders` (`Total`, `Date`, `OrderNum`) VALUES(@Total, @Date, @OrderNum);";
					MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

					mySqlCommand.Parameters.AddWithValue("@Total", newOrder.Total);
					mySqlCommand.Parameters.AddWithValue("@Date", newOrder.Date);
					mySqlCommand.Parameters.AddWithValue("@OrderNum", newOrder.OrderNum);
					mySqlCommand.ExecuteNonQuery();

					mySqlCommand.CommandText = "SELECT LAST_INSERT_ID()";

					idOrder = Convert.ToInt32(mySqlCommand.ExecuteScalar());

					mySqlCommand.Dispose();

					return idOrder;
                }
                catch (MySqlException ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);

					return -1;
				}
			}
		}
		public void AddOrderItem(OrderItem newOrderItem)
		{
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand mySqlCommand = connection.CreateCommand();
					mySqlCommand.CommandText = "SELECT idDrink FROM Drinks WHERE Name=@Name";

                    mySqlCommand.Parameters.AddWithValue("@Name", newOrderItem.Drink);

					int drinkId = Convert.ToInt32(mySqlCommand.ExecuteScalar());

					mySqlCommand.Dispose();

                    mySqlCommand = connection.CreateCommand();

                    mySqlCommand.CommandText = "SELECT idSide FROM Sides WHERE Name=@Name";

                    mySqlCommand.Parameters.AddWithValue("@Name", newOrderItem.Side);

                    int sideId = Convert.ToInt32(mySqlCommand.ExecuteScalar());

                    mySqlCommand.Dispose();

                    mySqlCommand = connection.CreateCommand();

                    mySqlCommand.CommandText = "INSERT INTO `OrderItems` (`OrderId`, `ComboId`, `Price`, `SideId`, `DrinkId`, `Ice`) " +
                        "				VALUES(@OrderId, @ComboId, @Price, @SideId, @DrinkId, @Ice);";

                    mySqlCommand.Parameters.AddWithValue("@OrderId", newOrderItem.OrderId);
                    mySqlCommand.Parameters.AddWithValue("@ComboId", newOrderItem.ComboId);
                    mySqlCommand.Parameters.AddWithValue("@Price", newOrderItem.Price);
                    mySqlCommand.Parameters.AddWithValue("@SideId", sideId);
                    mySqlCommand.Parameters.AddWithValue("@DrinkId", drinkId);
                    mySqlCommand.Parameters.AddWithValue("@Ice", newOrderItem.Ice);
                    
					mySqlCommand.ExecuteNonQuery();

                    mySqlCommand.Dispose();
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        public List<Combo> GetCombos()
        {
			List<Combo> Combos = new List<Combo>();
			using (MySqlConnection connection = new MySqlConnection(_connectionString))
			{
				connection.Open();
				string SQL = "select idCombo, Name, Description, Price, Type from FastChicken.Combos";
				MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
				using (var reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Combos.Add(new Combo()
						{
							ComboId = reader["idCombo"].ToString(),
							Name = reader["Name"].ToString(),
							Description = reader["Description"].ToString(),
                            Price = reader["Price"].ToString(),
							Type = reader["Type"].ToString(),
                        });
					}
				}

				mySqlCommand.Dispose();
			}
			return Combos;
        }

		public int getNewNumOrder()
		{
			using (MySqlConnection connection = new MySqlConnection(_connectionString))
			{
				connection.Open();

                MySqlCommand command = new MySqlCommand("UPDATE NumOrders SET id=id+1;SELECT id FROM NumOrders", connection);

                int newNumOrder = Convert.ToInt32( command.ExecuteScalar());

				return newNumOrder;
            }            
        }

        public bool checkPassword(string username, string password)
        {
            bool retValue = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT password FROM Users WHERE username=@username AND password=SHA1(@password)", connection);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        retValue = true;
                    }
                }

                command.Dispose();
            }

            return retValue;
        }
        public int startCashJournal()
		{
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT idCashJournal FROM CashJournals WHERE state=1", connection);

                int idCashJournal = Convert.ToInt32(command.ExecuteScalar());

				if(idCashJournal <= 0 )
				{
					command = connection.CreateCommand();

					command.CommandText = "INSERT INTO CashJournals(StartDate, EndDate, State) VALUES(NOW(), NULL, 1);SELECT LAST_INSERT_ID()";

                    idCashJournal = Convert.ToInt32(command.ExecuteScalar());
                }

                return idCashJournal;
            }
        }
        public int endCashJournal()
		{
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT idCashJournal FROM CashJournals WHERE state=1", connection);

                int idCashJournal = Convert.ToInt32(command.ExecuteScalar());

                if (idCashJournal > 0)
                {
                    command = connection.CreateCommand();

                    command.CommandText = "UPDATE Orders SET idCashJournal=@idCashJournal WHERE idCashJournal IS NULL";
                    
                    command.Parameters.AddWithValue("@idCashJournal", idCashJournal);

                    command.ExecuteNonQuery();

                    command.Dispose();

                    command = connection.CreateCommand();

                    command.CommandText = "UPDATE CashJournals SET EndDate=NOW(), State=0 WHERE idCashJournal=@idCashJournal";

                    command.Parameters.AddWithValue("@idCashJournal", idCashJournal);

                    command.ExecuteNonQuery();

                    command = connection.CreateCommand();

                    command.CommandText = "UPDATE NumOrders SET id=0";

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                command.Dispose();

                return idCashJournal;
            }
        }

        private void Update()
        {

        }
        private void Delete()
        {

        }
        public Drink getDrink(int id)
        {
            Drink drink = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idDrink, Name, Price, Quantity from Drinks WHERE idDrink=@idDrink";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                
                mySqlCommand.Parameters.AddWithValue("@idDrink", id);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        drink = new Drink();
                        drink.idDrink = reader.GetInt32(0);
                        drink.Name = reader.GetString(1);
                        drink.Price = reader.GetDouble(2);
                        drink.Quantity = reader.GetInt32(3);
                    }
                }

                mySqlCommand.Dispose();
            }
            return drink;
        }
        public Side getSide(int id)
        {
            Side side = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idSide, Name, Price, Quantity from Sides WHERE idSide=@idSide";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                mySqlCommand.Parameters.AddWithValue("idSide", id);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        side = new Side();
                        side.idSide = reader.GetInt32(0);
                        side.Name = reader.GetString(1);
                        side.Price = reader.GetDouble(2);
                        side.Quantity = reader.GetInt32(3);
                    }
                }

                mySqlCommand.Dispose();
            }
            return side;
        }
        public Product getProduct(int id)
        {
            Product product = null;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idProduct, Name, Price, Quantity from Products WHERE idProduct=@idProduct";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                mySqlCommand.Parameters.AddWithValue("idProduct", id);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product();
                        product.idProduct = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.Price = reader.GetDouble(2);
                        product.Quantity = reader.GetInt32(3);
                    }
                }

                mySqlCommand.Dispose();
            }
            return product;
        }
        public IList<Drink> getDrinks()
        {
            List<Drink> drinks = new List<Drink>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idDrink, Name, Price, Quantity from Drinks";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drinks.Add(new Drink()
                        {
                            idDrink = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        }) ;
                    }
                }

                mySqlCommand.Dispose();
            }
            return drinks;
        }
        public IList<Side> getSides()
        {
            List<Side> sides = new List<Side>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idSide, Name, Price, Quantity from Sides";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sides.Add(new Side()
                        {
                            idSide = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return sides;

        }
        public IList<Product> getProducts()
        {
            List<Product> products = new List<Product>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idProduct, Name, Price, Quantity from Products";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product()
                        {
                            idProduct = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return products;
        }
        public IList<Drink> getDrinksByCombo(string comboId)
        {
            List<Drink> drinks = new List<Drink>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "SELECT d.idDrink, d.Name, d.Price, d.Quantity FROM Drinks d " +
                                                "INNER JOIN ComboDetails cd ON (d.idDrink = cd.idDetail " +
                                                "OR cd.idDetail = -1) AND cd.Type = 'D' " +
                                                "WHERE cd.idCombo = @idCombo";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", comboId);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        drinks.Add(new Drink()
                        {
                            idDrink = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return drinks;
        }
        public IList<Side> getSidesByCombo(string comboId)
        {
            List<Side> sides = new List<Side>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "SELECT s.idSide, s.Name, s.Price, s.Quantity FROM Sides s " +
                            "INNER JOIN ComboDetails cd ON (s.idSide = cd.idDetail OR " +
                            "cd.idDetail = -1) AND cd.Type = 'S' WHERE cd.idCombo = @idCombo";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", comboId);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sides.Add(new Side()
                        {
                            idSide = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return sides;
        }

        public IList<Product> getProductsByCombo(string comboId)
        {
            List<Product> products = new List<Product>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string SQL = "SELECT p.idProduct, p.Name, p.Price, p.Quantity FROM Products p " +
                            "INNER JOIN ComboDetails cd ON (p.idProduct = cd.idDetail OR " +
                            "cd.idDetail = -1) AND cd.Type = 'P' WHERE cd.idCombo = @idCombo";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", comboId);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product()
                        {
                            idProduct = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDouble(2),
                            Quantity = reader.GetInt32(3),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return products;
        }

        public IList<Combo> getCombos()
        {
            List<Combo> combos = new List<Combo>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idCombo, Name, Description, Price, Type from Combos";
                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        combos.Add(new Combo()
                        {
                            ComboId = reader.GetString(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetString(3),
                            Type = reader.GetString(4),
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            return combos;
        }
        public void UpdateDrinksQty(int idDrink, double quantity)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Drinks SET Quantity=@Quantity WHERE idDrink=@idDrink";

                mySqlCommand.Parameters.AddWithValue("@idDrink", idDrink);
                mySqlCommand.Parameters.AddWithValue("@Quantity", quantity);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }
        public void UpdateSidesQty(int idSide, double quantity)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Sides SET Quantity=@Quantity WHERE idSide=@idSide";

                mySqlCommand.Parameters.AddWithValue("@idSide", idSide);
                mySqlCommand.Parameters.AddWithValue("@Quantity", quantity);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }

        public void UpdateProductsQty(int idProduct, double quantity)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Products SET Quantity=@Quantity WHERE idProduct=@idProduct";

                mySqlCommand.Parameters.AddWithValue("@idProduct", idProduct);
                mySqlCommand.Parameters.AddWithValue("@Quantity", quantity);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }

        public Combo GetCombo(string id)
        {
            Combo combo = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idCombo, Name, Description, Price, Type from Combos WHERE idCombo=@idCombo";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", id);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        combo = new Combo();

                        combo.ComboId = reader.GetString(0);
                        combo.Name = reader.GetString(1);
                        combo.Description = reader.GetString(2);
                        combo.Price = reader.GetString(3);
                        combo.Type = reader.GetString(4);
                    }
                }

                mySqlCommand.Dispose();
            }
            return combo;
        }
        public Dictionary<int, int> getQuantityById(string idCombo, string type)
        {
            Dictionary<int,int> dictionary = new Dictionary<int,int>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string SQL = "select idComboDetail, idCombo, Type, Quantity, idDetail FROM " +
                                    "ComboDetails WHERE idCombo=@idCombo AND Type=@Type";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", idCombo);

                mySqlCommand.Parameters.AddWithValue("@Type", type);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dictionary[reader.GetInt32(4)] = reader.GetInt32(3);
                    }
                }

                mySqlCommand.Dispose();
            }
            return dictionary;
        }

        public void GrabarCombo(Combo combo)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                
                string SQL = "SELECT COUNT(1) FROM Combos WHERE idCombo=@idCombo";

                MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

                mySqlCommand.Parameters.AddWithValue("@idCombo", combo.ComboId);

                int cantidad = Convert.ToInt32(mySqlCommand.ExecuteScalar());

                mySqlCommand.Dispose();

                mySqlCommand = connection.CreateCommand();

                if ( cantidad > 0 )
                {
                    mySqlCommand.CommandText = "UPDATE Combos SET Name=@Name, Description=@Description, " +
                                                            "Price=@Price, Type=@Type WHERE idCombo=@idCombo";
                } else
                {
                    mySqlCommand.CommandText = "INSERT INTO Combos(idCombo, Name, Description, " +
                                        "Price, Type) VALUES(@idCombo, @Name, @Description, @Price, @Type)";
                }

                mySqlCommand.Parameters.AddWithValue("@idCombo", combo.ComboId);
                mySqlCommand.Parameters.AddWithValue("@Name", combo.Name);
                mySqlCommand.Parameters.AddWithValue("@Description", combo.Description);
                mySqlCommand.Parameters.AddWithValue("@Price", combo.Price);
                mySqlCommand.Parameters.AddWithValue("@Type", combo.Type);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();

                List<int> idDetails = new List<int>();

                if( combo.Drinks != null && combo.Drinks.Count > 0)
                {
                    foreach (var drink in combo.Drinks)
                    {
                        this.GrabarComboDetail(drink, connection);
                        idDetails.Add(drink.idDetail);
                    }
                }

                DeleteComboDetails(combo.ComboId, "D", idDetails, connection);

                idDetails.Clear();

                if ( combo.Sides != null && combo.Sides.Count > 0)
                {
                    foreach (var side in combo.Sides)
                    {
                        this.GrabarComboDetail(side, connection);
                        idDetails.Add(side.idDetail);
                    }
                }

                DeleteComboDetails(combo.ComboId, "S", idDetails, connection);

                idDetails.Clear();

                if ( combo.Products != null && combo.Products.Count > 0)
                {
                    foreach (var product in combo.Products)
                    {
                        this.GrabarComboDetail(product, connection);
                        idDetails.Add(product.idDetail);
                    }
                }

                DeleteComboDetails(combo.ComboId, "P", idDetails, connection);
            }
        }

        private void GrabarComboDetail(ComboDetail comboDetail, MySqlConnection connection)
        {
            string SQL = "SELECT COUNT(1) FROM ComboDetails WHERE idCombo=@idCombo AND " +
                                                            "Type=@Type AND idDetail=@idDetail";

            MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

            mySqlCommand.Parameters.AddWithValue("@idCombo", comboDetail.idCombo);
            mySqlCommand.Parameters.AddWithValue("@Type", comboDetail.Type);
            mySqlCommand.Parameters.AddWithValue("@idDetail", comboDetail.idDetail);

            int cantidad = Convert.ToInt32(mySqlCommand.ExecuteScalar());

            mySqlCommand.Dispose();

            mySqlCommand = connection.CreateCommand();

            if (cantidad > 0)
            {
                mySqlCommand.CommandText = "UPDATE ComboDetails SET Quantity=@Quantity " +
                                        "WHERE idCombo=@idCombo AND Type=@Type AND idDetail = @idDetail";
            }
            else
            {
                mySqlCommand.CommandText = "INSERT INTO ComboDetails(idCombo, Type, Quantity, " +
                                    "idDetail) VALUES(@idCombo, @Type, @Quantity, @idDetail)";
            }

            mySqlCommand.Parameters.AddWithValue("@idCombo", comboDetail.idCombo);
            mySqlCommand.Parameters.AddWithValue("@Type", comboDetail.Type);
            mySqlCommand.Parameters.AddWithValue("@Quantity", comboDetail.Quantity);
            mySqlCommand.Parameters.AddWithValue("@idDetail", comboDetail.idDetail);

            mySqlCommand.ExecuteNonQuery();

            mySqlCommand.Dispose();
        }

        private void DeleteComboDetails(string idCombo, string Type, IList<int> idDetails, MySqlConnection connection)
        {
            MySqlCommand mySqlCommand = connection.CreateCommand();

            mySqlCommand.CommandType = System.Data.CommandType.Text;
            mySqlCommand.CommandText = "DELETE FROM ComboDetails WHERE Type=@Type AND idCombo=@idCombo";

            if( idDetails.Count > 0 )
            {
                mySqlCommand.CommandText += " AND idDetail NOT IN(";

                List<string> args = new List<string>();

                for(int i = 0; i < idDetails.Count; i++)
                {
                    if( mySqlCommand.Parameters.Count > 0 )
                    {
                        mySqlCommand.CommandText += ", ";
                    }

                    mySqlCommand.CommandText += " @arg" + i.ToString();
                    mySqlCommand.Parameters.AddWithValue("@arg" + i.ToString(), idDetails[i]);
                }

                mySqlCommand.CommandText += " )";
            }

            mySqlCommand.Parameters.AddWithValue("@Type", Type);
            mySqlCommand.Parameters.AddWithValue("@idCombo", idCombo);

            mySqlCommand.ExecuteNonQuery();

            mySqlCommand.Dispose();
        }

        public void DecrementProduct(int idProduct, int value)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Products SET Quantity = IF( Quantity > 0, Quantity-@value, 0) WHERE idProduct = @idProduct";

                mySqlCommand.Parameters.AddWithValue("@idProduct", idProduct);
                mySqlCommand.Parameters.AddWithValue("@value", value);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }
        public void DecrementDrink(int idDrink, int value)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Drinks SET Quantity = IF( Quantity > 0, Quantity-@value, 0) WHERE idDrink = @idDrink";

                mySqlCommand.Parameters.AddWithValue("@idDrink", idDrink);
                mySqlCommand.Parameters.AddWithValue("@value", value);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }
        public void DecrementSide(int idSide, int value)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();

                mySqlCommand.CommandText = "UPDATE Sides SET Quantity = IF( Quantity > 0, Quantity-@value, 0) WHERE idSide = @idSide";

                mySqlCommand.Parameters.AddWithValue("@idSide", idSide);
                mySqlCommand.Parameters.AddWithValue("@value", value);

                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();
            }
        }

        public int GetComboQuantity(string comboId, int idDetail, string Type)
        {
            int quantity = 0;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand mySqlCommand = connection.CreateCommand();
                mySqlCommand.CommandText = "SELECT Quantity FROM ComboDetails WHERE " +
                                "idCombo=@comboId AND Type=@Type AND idDetail=@idDetail";

                mySqlCommand.Parameters.AddWithValue("@comboId", comboId);
                mySqlCommand.Parameters.AddWithValue("@Type", Type);
                mySqlCommand.Parameters.AddWithValue("@idDetail", idDetail);

                MySqlDataReader reader = mySqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    quantity = reader.GetInt32(0);
                }
                else
                {
                    reader.Close();

                    mySqlCommand.CommandText = "SELECT Quantity FROM ComboDetails WHERE " +
                                    "idCombo=@comboId AND Type=@Type AND idDetail=-1";

                    reader = mySqlCommand.ExecuteReader();

                    if (reader.Read())
                    {
                        quantity = reader.GetInt32(0);
                    }
                }
                mySqlCommand.Dispose();
            }

            return quantity;
        }
    }
}