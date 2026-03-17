using System.Text;
using System.Text.RegularExpressions;

namespace ShadowWire.Shared;

/// <summary>
/// Manages client username.
/// </summary>
/// <remarks>
/// Provides validation, prompting, and persistence.
/// </remarks>
public class UsernameManager
{
    private const int MINIMUM_LENGTH = 4;
    private const int MAXIMUM_LENGTH = 32;
    private const string CHAR_SET = "[a-zA-Z0-9-_+]"; // RegEx pattern

    public string Username { get; private set; } = "";


    /// <summary>
    /// Loads a valid username from a file or prompts the user to create one.
    /// </summary>
    /// <param name="usernameFile">Path to the username file.</param>
    public UsernameManager(string usernameFile)
    {
        // Load existing username
        if (File.Exists(usernameFile))
        {
            Username = File.ReadAllText(usernameFile);

            if (IsUsernameValid(Username))
                return; // Valid username loaded
        }

        // Create a new username
        while (true)
        {
            Username = PromptForUsername();

            // Validate username
            if (IsUsernameValid(Username))
                break;
            else
                Console.WriteLine("Invalid, try again!");
        }
        File.WriteAllText(usernameFile, Username); // Save new username
    }

    /// <summary>
    /// Checks if a username matches the allowed characters and length limits.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the username is valid; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsUsernameValid(string username)
    {
        if (!Regex.IsMatch(username, "^" + CHAR_SET + "+$"))
            return false;

        if (username.Length < MINIMUM_LENGTH)
            return false;
        if (username.Length > MAXIMUM_LENGTH)
            return false;

        return true;
    }

    // TODO: Maybe extract into separate classes
    /// <summary>
    /// Prompts the user for a username.
    /// </summary>
    /// <remarks>
    /// Supports editing and navigation, and enforces allowed characters 
    /// and length constraints.
    /// </remarks>
    /// <returns>The username provided by the user.</returns>
    private static string PromptForUsername()
    {
        // Display prompt
        StringBuilder sb = new();
        sb.Append("  {");
        sb.Append(" length: [")
          .Append(MINIMUM_LENGTH)
          .Append(", ")
          .Append(MAXIMUM_LENGTH)
          .Append("] ");
        sb.Append(';');
        sb.Append(" chars: ")
          .Append(CHAR_SET)
          .Append(' ');
        sb.Append('}')
          .AppendLine();
        sb.Append("Enter a username: ");
        Console.Write(sb.ToString());

        string input = string.Empty;
        int index = 0;
        int initialCursorX = Console.CursorLeft;
        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true); // Prevent printing

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break; // Submit username
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.Backspace:
                    if (index > 0)
                    {
                        input = input.Remove(index - 1, 1);
                        index--;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (index > 0)
                        index--;
                    break;
                case ConsoleKey.RightArrow:
                    if (index < input.Length)
                        index++;
                    break;
                case ConsoleKey.DownArrow:
                    index = input.Length - 1;
                    break;
                case ConsoleKey.UpArrow:
                    index = 0;
                    break;
                default:
                    if (input.Length >= MAXIMUM_LENGTH && keyInfo.Key != ConsoleKey.Backspace)
                        continue; // Maximum length reached

                    // Character
                    var ch = keyInfo.KeyChar.ToString();
                    if (!Regex.IsMatch(ch, CHAR_SET))
                        continue; // Invalid character

                    // Update input
                    input = input.Insert(index, ch); // Store valid character
                    index++;

                    // Redraw text
                    Console.Write(input.Substring(index - 1));
                    break;
            }

            // Move cursor
            int x = initialCursorX + index;
            if (x > 0 && x < Console.BufferWidth)
                Console.SetCursorPosition(x, Console.CursorTop);

            // Redraw after deletion
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                // Redraw text
                Console.Write(input.Substring(index) + ' '); // and delete phantom character

                // Move cursor back
                if (x > 0 && x < Console.BufferWidth)
                    Console.SetCursorPosition(x, Console.CursorTop);
            }
        }

        return input;
    }
}
