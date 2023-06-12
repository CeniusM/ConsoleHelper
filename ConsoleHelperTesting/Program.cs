using ConsoleHelper;

ConsoleHelper.ConsoleHelper console = new ConsoleHelper.ConsoleHelper();

Prompt get = new Prompt("Type in the distance", ConsoleColor.Green, ConsoleColor.White);
Prompt fail = new Prompt("Invalid input", ConsoleColor.Red, ConsoleColor.White);
int a = console.GetIntegerInput(get, fail);

console.WaitForEnter("Just chilling with" + a);