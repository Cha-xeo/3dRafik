using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mono.Data.Sqlite;
using TMPro;

public class Register : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField usernameField = null;
    [SerializeField] private TMP_InputField passwordField = null;
    [SerializeField] private TMP_InputField emailField = null;

    [Header("Error Texts")]
    [SerializeField] private Text usernameErrorText = null;
    [SerializeField] private Text passwordErrorText = null;
    [SerializeField] private Text emailErrorText = null;

    private string connectionString;

    private void Awake()
    {
        Debug.Log(Application.dataPath);
        connectionString = "URI=file:" + Application.dataPath + "/Users.db";
    }

    private void Start()
    {
        CreateDB();

    }

    private void CreateDB()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Players (PlayerID INT IDENTITY PRIMARY KEY, Username NVARCHAR(50), PasswordHash NVARCHAR(255), Email NVARCHAR(255), RegistrationDate DATETIME, LastLoginDate DATETIME, TotalScore INT)";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void Submit()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string email = emailField.text;
        DateTime now = DateTime.Now;

        string hashedPassword = HashPassword(password);

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            Debug.Log(connection);
            connection.Open();

            string query = "INSERT INTO Players (Username, PasswordHash, Email, RegistrationDate, LastLoginDate, TotalScore) " +
                "VALUES (@username, @password, @email, @registrationDate, @lastLoginDate, @totalScore)";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@registrationDate", now);
                command.Parameters.AddWithValue("@lastLoginDate", now);
                command.Parameters.AddWithValue("@totalScore", 0);

                command.ExecuteNonQuery();
            }

            Debug.Log(query);

            connection.Close();
        }
    }

    private string HashPassword(string password)
    {

        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

}
