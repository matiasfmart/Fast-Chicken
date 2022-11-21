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

		public void AddOrder(Order newOrder)
		{
			MySqlTransaction mySqlTransaction = null;
			using(MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
					string SQL = "INSERT INTO `FastChicken`.`Orders` (`idOrders`, `Total`, `Date`) VALUES(@Id, @Total, @Date);";
					MySqlCommand mySqlCommand = new MySqlCommand(SQL, connection);

					mySqlCommand.Parameters.AddWithValue("@Id", newOrder.Id);
					mySqlCommand.Parameters.AddWithValue("@Total", newOrder.Total);
					mySqlCommand.Parameters.AddWithValue("@Date", newOrder.Date);

					mySqlTransaction.Commit();

					mySqlCommand.Dispose();
                }
                catch (MySqlException ex)
				{

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
        private void Update()
        {

        }
        private void Delete()
        {

        }
    }
}