

namespace ConsoleHelper;

/// <summary>
/// Contains the values for ConsoleH to print text to the console
/// </summary>
public class Prompt
{
    public static readonly Prompt Empty = new Prompt();

    internal string _text;
    public string Text
    {
        get { return _text; }
        set { _ThrowIfInvalidString(value); _text = value; }
    }

    public ConsoleColor TextColor = ConsoleColor.White;
    public ConsoleColor BackColor = ConsoleColor.Black;
    public bool Bold = false;
    internal bool _empty = false;

    public Prompt(string str)
    {
        _text = str;
    }

    public Prompt(string str, ConsoleColor textColor, ConsoleColor backColor)
    {
        _text = str;
        TextColor = textColor;
        BackColor = backColor;
    }

    internal Prompt()
    {
        _text = "";
        _empty = true;
    }

    internal void _ThrowIfInvalidString(string str)
    {
        if (str.Contains('\n'))
            throw new Exception("The given string for text can not be null");
        if (str.Length > 512)
            throw new Exception("A prompt can not be more than 512 charecters long");
    }

    public static implicit operator Prompt(string str) => new Prompt(str);
}

public class Options<T>
{
    internal List<(T Value, Prompt Prompt)> _options = new List<(T Value, Prompt Propmt)>();
    internal ConsoleColor _defaultTextColor;
    internal ConsoleColor _defaultBackColor;
    internal ConsoleColor _selectedTextColor;
    internal ConsoleColor _selectedBackColor;

    public Options(ConsoleColor defaultTextColor, ConsoleColor defaultBackColor, ConsoleColor selectedTextColor, ConsoleColor selectedBackColor)
    {
        _defaultTextColor = defaultTextColor;
        _defaultBackColor = defaultBackColor;
        _selectedTextColor = selectedTextColor;
        _selectedBackColor = selectedBackColor;
    }

    public void AddOption(T value, Prompt text)
    {
        text.TextColor = _defaultTextColor;
        text.BackColor = _defaultBackColor;
        _options.Add((value, text.Text));
    }

    public int Count()
    {
        return _options.Count;
    }

    public (T Value, Prompt Prompt) this[int i]
    {
        get { return _options[i]; }
    }
}

    /// <summary>
    /// This is a tool for making it esier to get values and show text to a user by using the commandline. This is not compateble with multithreading
    /// </summary>
    public class ConsoleHelper
    {
        private readonly string EMPTYLINE = new string(' ', 512);

        /// <summary>
        /// Used to keep track of how much to clear on each line
        /// </summary>
        internal List<int> _lineLenghts = new List<int>();
        public ConsoleColor ClearColor;

        public ConsoleHelper(ConsoleColor clearColor = ConsoleColor.Black)
        {
            ClearColor = clearColor;
        }

        public ConsoleHelper(ConsoleColor DefaultTextColor, ConsoleColor DefaultBackColor)
        {
            throw new NotImplementedException();
        }

        public bool TryGetIntegerInput(out int result, Prompt initPrompt, Prompt errorMessage, int attempts = int.MaxValue)
        {
            Clear();

            _HandlePrompt(initPrompt);

            while (true)
            {

                string input = Console.ReadLine()!;
                Clear();

                if (int.TryParse(input, out result))
                    return true;

                attempts--;

                if (attempts == 0)
                    break;

                _HandlePrompt(initPrompt);
                _HandlePrompt(errorMessage);
            }

            return false;
        }

        public int GetIntegerInput(Prompt initPrompt, Prompt errorMessage)
        {
            Clear();

            _HandlePrompt(initPrompt);

            while (true)
            {
                string input = Console.ReadLine()!;
                Clear();

                if (int.TryParse(input, out int result))
                    return result;

                _HandlePrompt(initPrompt);
                _HandlePrompt(errorMessage);
            }
        }

        public string GetStringInputPrompt(Prompt initPrompt)
        {
            Clear();

            _HandlePrompt(initPrompt);

            string input = Console.ReadLine()!;

            Clear();

            return input;
        }

        /// <summary>
        /// Shows a list of options to the user for them to choose, then the given options value will be returned
        /// </summary>
        public T GetOption<T>(Options<T> options, Prompt titlePromt,
            ConsoleKey consoleKeySelect = ConsoleKey.Enter, ConsoleKey consoleKeyUp = ConsoleKey.UpArrow, ConsoleKey consoleKeyDown = ConsoleKey.DownArrow)
        {
            Clear();

            int selectedOption = 0;

            while (true)
            {
                _HandlePrompt(titlePromt);

                for (int i = 0; i < options.Count(); i++)
                {
                    if (i != selectedOption)
                    {
                        _HandlePrompt(options[i].Prompt);
                        continue;
                    }

                    _HandlePrompt(options[i].Prompt, options._selectedTextColor, options._selectedBackColor);
                }

                var key = Console.ReadKey();

                Clear();

                if (key.Key == consoleKeySelect)
                {
                    return options[selectedOption].Value;
                }
                else if (key.Key == consoleKeyUp)
                {
                    selectedOption--;
                    if (selectedOption < 0)
                        selectedOption = 0;
                }
                else if (key.Key == consoleKeyDown)
                {
                    selectedOption++;
                    if (selectedOption >= options.Count())
                        selectedOption = options.Count() - 1;
                }
            }
        }

        public void WaitForEnter(Prompt prompt)
        {
            Clear();
            _HandlePrompt(prompt);
            Console.ReadLine();
            Clear();
        }

        public void WaitForKeyPress(Prompt prompt)
        {
            Clear();
            _HandlePrompt(prompt);
            Console.ReadKey();
            Clear();
        }

        /// <summary>
        /// Used for smoother clearing of the console
        /// </summary>
        public void Clear()
        {
            if (_lineLenghts.Count == 0)
                return;
            Console.SetCursorPosition(0, 0);
            Console.ResetColor();

            for (int i = 0; i < _lineLenghts.Count; i++)
                Console.WriteLine(EMPTYLINE, _lineLenghts[i]);

            Console.SetCursorPosition(0, 0);
            _lineLenghts.Clear();
        }

        /// <summary>
        /// Used for completly clearing the console
        /// </summary>
        public void HardClear()
        {
            Console.ResetColor();
            Console.Clear();
            _lineLenghts.Clear();
        }

        public void Print(Prompt prompt)
        {
            _HandlePrompt(prompt);
        }

        internal void _HandlePrompt(Prompt prompt, ConsoleColor OverrideTextColor = ConsoleColor.Black, ConsoleColor OverrideBackColor = ConsoleColor.Black)
        {
            if (prompt._empty)
                return;

            int length = prompt._text.Length;
            _lineLenghts.Add(length);

            // Scuffed at best
            if (OverrideTextColor == ConsoleColor.Black && OverrideBackColor == ConsoleColor.Black)
            {
                Console.ForegroundColor = prompt.TextColor;
                Console.BackgroundColor = prompt.BackColor;
            }
            else
            {
                Console.ForegroundColor = OverrideTextColor;
                Console.BackgroundColor = OverrideBackColor;
            }

            Console.WriteLine(prompt._text);
        }
    }













//public class Option<T>
//{
//    /// <summary>
//    /// The returned value if selected
//    /// </summary>
//    public T Value;

//    /// <summary>
//    /// The shown prompt for given option
//    /// </summary>
//    public Prompt Prompt;

//    /// <summary>
//    /// Color of text when hovered over
//    /// </summary>
//    public ConsoleColor HoverOverTextColor;

//    /// <summary>
//    /// Color of background when hovered over
//    /// </summary>
//    public ConsoleColor HoverOverBackColor;

//    public Option(T value, Prompt text, ConsoleColor hoverOverTextColor, ConsoleColor hoverOverBackColor)
//    {
//        Value = value;
//        Prompt = text;
//        HoverOverTextColor = hoverOverTextColor;
//        HoverOverBackColor = hoverOverBackColor;
//    }
//}