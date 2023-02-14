using System;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using FastChicken.Models;
using FastChicken.Interfaces;
using static NuGet.Packaging.PackagingConstants;

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
    }
}