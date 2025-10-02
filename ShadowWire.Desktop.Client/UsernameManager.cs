using System.Text;
using System.Text.RegularExpressions;

namespace ShadowWire.Desktop.Client;

internal class UsernameManager
{
    private const int MINIMUM_LENGTH = 4;
    private const int MAXIMUM_LENGTH = 32;
    private const string INV_CHAR_SET = "[^!-~]"; // Inverted RegEx pattern

    public string Username { get; private set; }


    // TODO: Fix handling
    public UsernameManager(string usernameFile)
    {
        //// Load existing username
        //if (File.Exists(usernameFile))
        //{
        //    Username = SanitiseUsername(File.ReadAllText(usernameFile));

        //    if (string.IsNullOrWhiteSpace(Username))
        //        return; // Valid username loaded
        //}

        // Create a new username
        Username = PromptForUsername();

        // TODO: Remove, for debugging
        if (Username.Count() < 4)
            Console.WriteLine("Invalid!");

        Username = SanitiseUsername(Username); // Unnessasary but a precaution
        File.WriteAllText(usernameFile, Username); // Save new username
    }

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
          .Append('[')
          .Append(INV_CHAR_SET[2..]) // (Remove invert flag)
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
                        input = input.Remove((index - 1), 1);
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
                    if (Regex.IsMatch(ch, INV_CHAR_SET))
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

    private static string SanitiseUsername(string username)
    {
        // Remove invalid characters
        string cleaned = Regex.Replace(username, INV_CHAR_SET, string.Empty);

        // Ensure minimum length
        if (cleaned.Length < MINIMUM_LENGTH)
            return string.Empty;

        // Truncate to maximum length
        return cleaned[..Math.Min(MAXIMUM_LENGTH, cleaned.Length)];
    }
}
