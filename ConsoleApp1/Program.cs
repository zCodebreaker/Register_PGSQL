using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Npgsql;

class Program
{
    static void Main()
    {
        string conDB = "Host=localhost;Username=postgres;Password=adryanmelo12;Database=postgres";
        Console.Title = "Register System";
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("S I M P L E - R E G I S T E R - S Y S T E M");
        Console.WriteLine();
        Console.WriteLine("by Codebreaker");
        Console.WriteLine("");
        Console.WriteLine("███████╗ ██████╗ ██████╗ ██████╗ ███████╗");
        Console.WriteLine("╚══███╔╝██╔════╝██╔═══██╗██╔══██╗██╔════╝");
        Console.WriteLine("  ███╔╝ ██║     ██║   ██║██║  ██║█████╗");
        Console.WriteLine(" ███╔╝  ██║     ██║   ██║██║  ██║██╔══╝");
        Console.WriteLine("███████╗╚██████╗╚██████╔╝██████╔╝███████╗");
        Console.WriteLine("╚══════╝ ╚═════╝ ╚═════╝ ╚═════╝ ╚══════╝");
        Console.WriteLine();
        Console.Write("Login: ");
        string login = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Senha: ");
        string password = ReadPassword();

        string hashedPassword = EncryptPassword(password);

        using (var conn = new NpgsqlConnection(conDB))
        {
            try
            {
                conn.Open();
                string checkQuery = "SELECT COUNT(*) FROM accounts WHERE login = @login OR email = @email";
                using (var cmd = new NpgsqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        Console.WriteLine("Login ou email já estão em uso.");
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO accounts (login, password, email) VALUES (@login, @password, @email)";
                        using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@login", login);
                            insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                            insertCmd.Parameters.AddWithValue("@email", email);

                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Registro realizado com sucesso!");
                            }
                            else
                            {
                                Console.WriteLine("Falha ao registrar usuário.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
        Console.WriteLine("Pressione enter para fechar.");
        Console.ReadKey();
    }
    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
    static string EncryptPassword(string password)
    {
        string salt = "/x!a@r-$r%an¨.&e&+f*f(f(a)";
        using (var hmac = new HMACMD5(System.Text.Encoding.UTF8.GetBytes(salt)))
        {
            byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
