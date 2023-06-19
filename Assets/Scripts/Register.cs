using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mono.Data.Sqlite;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Data.Common;
using UnityEditor.Search;

public class Register : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] TMP_InputField usernameField = null;
    [SerializeField] TMP_InputField passwordField = null;
    [SerializeField] TMP_InputField emailField = null;

    [Header("Error Texts")]
    [SerializeField] TMP_Text usernameErrorText = null;
    [SerializeField] TMP_Text passwordErrorText = null;
    [SerializeField] TMP_Text emailErrorText = null;

    SqliteConnectionStringBuilder connectionStringBuilder = new();
    public SqliteConnection connection;


    private void Start()
    {
        emailField.contentType = TMP_InputField.ContentType.EmailAddress;
        passwordField.contentType = TMP_InputField.ContentType.Password;
        connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        connection = new SqliteConnection(connectionStringBuilder.ToString());
        CreateDB();
    }

    private void CreateDB()
    {
        //using (var connection = new SqliteConnection(connectionStringBuilder.ToString()))
        //{
            connection.Open();
            string query = "CREATE TABLE IF NOT EXISTS Users (PlayerID INT IDENTITY PRIMARY KEY, Username NVARCHAR(50), PasswordHash NVARCHAR(255), Email NVARCHAR(255), RegistrationDate DATETIME, LastLoginDate DATETIME, TotalScore INT)";
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }

          //  connection.Close();
        //}
    }

    public void Submit()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string email = emailField.text;
        DateTime now = DateTime.Now;

        string hashedPassword = HashPassword(password);

        //using (SqliteConnection connection = new SqliteConnection(connectionStringBuilder.ToString()))
        //{
          //  connection.Open();

            string query = "INSERT INTO Users (Username, PasswordHash, Email, RegistrationDate, LastLoginDate, TotalScore) " +
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

          //  connection.Close();
        //}
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
