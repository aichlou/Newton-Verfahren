using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
bool Error = false;

//Todo: Konstanten adden (e & pi z.B.)

do {
    Error = false;
    Console.WriteLine("Newton Verfahren aber von Louis");
    Console.Write("Funktion: f(x)="); 
    char[] function = (Console.ReadLine() ?? "").ToCharArray();
    List<Element> Elemente = new List<Element> {new Element("number", "0")};
    string? ErrorMessage = null;
    char variable = 'x';
    foreach (char item in function)
    {
        var lastElement = Elemente[Elemente.Count() - 1];

        if (int.TryParse(item.ToString(), out int number))
        {
            switch(lastElement.GetDatatype())
            {
                case "number":
                    lastElement.SetValue((lastElement.GetValue() + item.ToString()).ToString());
                    break;
                case "float":
                    /*Console.Write($"{float.Parse(lastElement.GetValue())} + (0.1 * {float.Parse(item.ToString())})");
                    lastElement.SetValue((float.Parse(lastElement.GetValue()) + (0.1 * float.Parse(item.ToString()))).ToString());
                    Console.WriteLine($" = {lastElement.GetValue()}"); */
                    /*if (!lastElement.GetValue().Contains('.'))
                    {
                        lastElement.SetValue(lastElement.GetValue() + ".0");
                    } */

                    int decimaldigits = lastElement.GetValue()
                        .ToCharArray()
                        .SkipWhile(x => x != '.')
                        .Skip(1)
                        .Count();
                    //Console.WriteLine($"Es hat {decimaldigits} Nachkommastelle(n)");
                    decimal newdigit = decimal.Parse(item.ToString());
                    //Console.WriteLine($"Das neue Zeichen ist {newdigit}");
                    decimal divisor = decimal.One;
                    for (int i = 0; i < decimaldigits + 1; i++)
                        divisor *= 10m;
                    newdigit /= divisor;
                    //Console.WriteLine($"Das neue Zeichen ist ausgerechnet {newdigit}");
                    bool isNegative = (float.Parse(lastElement.GetValue()) < 0);
                    if (isNegative) lastElement.SetValue((- float.Parse(lastElement.GetValue())).ToString());
                    lastElement.SetValue((float.Parse(lastElement.GetValue()) + (float)newdigit).ToString());
                    if (isNegative) lastElement.SetValue((- float.Parse(lastElement.GetValue())).ToString());
                    decimal result = decimal.Parse(lastElement.GetValue());
                    result = Math.Round(result, decimaldigits + 2);
                    lastElement.SetValue(result.ToString());
                    //Console.WriteLine($"Das Letzte Element ist jetzt {lastElement.GetValue()}");
                    break;
                case "bracket":
                    if (lastElement.GetValue() == ")")
                    {
                        Elemente.Add(new Element("biop", "*"));
                        Elemente.Add(new Element("number", item.ToString()));
                    }
                    else
                    {
                        Elemente.Add(new Element("number", item.ToString()));
                    }
                    break;
                case "biop":
                    if (lastElement.GetValue() == "-")
                    {
                        switch(Elemente[Elemente.Count() - 2].GetDatatype())
                        {
                            case "variable":
                                Elemente[Elemente.Count() - 1] = new Element("biop", "+");
                                Elemente.Add(new Element("number", (- number).ToString()));
                                break;
                            case "bracket":
                                if (Elemente[Elemente.Count() - 2].GetValue() == "(")
                                {
                                    Elemente[Elemente.Count() - 1] = new Element("number", (-number).ToString());
                                }
                                else if (Elemente[Elemente.Count() - 2].GetValue() == ")") {
                                    Elemente[Elemente.Count() -1] = new Element("biop", "+");
                                    Elemente.Add(new Element("number", (-number).ToString()));
                                }
                                else throw new Exception("Klammer ist weder offen noch zu");
                                break;
                            case "biop":
                                Elemente[Elemente.Count() - 1] = new Element("number", (-number).ToString());
                                break;
                            case "number":
                                Elemente.Add(new Element("number", item.ToString()));
                                break;
                            case "float":
                                Elemente.Add(new Element("number", item.ToString()));
                                break;
                            default:
                                throw new Exception($"Datentyp {Elemente[Elemente.Count() -2].GetDatatype()} noch nicht implementiert...");
                        }
                    }
                    else
                    {
                        Elemente.Add(new Element("number", item.ToString()));
                    }
                    break;
                default:
                    Elemente.Add(new Element("number", item.ToString()));
                    break;
            }
            continue;
        }
        else if (item == '.') {
            string Zwischenspeicher = lastElement.GetValue();
            if (lastElement.GetDatatype().Trim() != "number") {
                ErrorMessage = $"Element vor dem Punkt ist keine Zahl, sondern ein  {lastElement.GetDatatype()}\n"; 
                break; 
            }
            lastElement.SetValue("0");
            lastElement.SetDatatype("float");
            lastElement.SetValue(Zwischenspeicher);
            Elemente[Elemente.Count() - 1] = lastElement;
            //Console.WriteLine($"Das Element vor dem Punkt ist jetzt {Zwischenspeicher}");
        }
        else if (IsBiOperation(item))
        {
            if (lastElement.GetDatatype() != "biop") {
                Elemente.Add(new Element("biop", item.ToString()));
            }
            else
            {
                ErrorMessage += "Sie dürfen nicht zwei binäre Operatoren hintereinanderhängen \n";
                break;
            }
        }
        else if (item == ' ')
        {
            continue;
        }
        else if (item == '(')
        {
            Elemente.Add(new Element("bracket", "("));
            continue;
        }
        else if (item == ')')
        {
            Elemente.Add(new Element("bracket", ")"));
            continue;
        }
        else if (IsConst(item) != 0)
        {
            Elemente.Add(new Element("float", IsConst(item).ToString()));
        }
        else if (item == variable)
        {
            Elemente.Add(new Element("variable", variable.ToString()));
        }
        else
        {
            ErrorMessage += $"Verbotene Zeichen {item} \n";
        }
    }

    Console.WriteLine(string.Join("", Elemente.ConvertAll(e => e.GetValue())));


    if (ErrorMessage != null)
    {
        Console.WriteLine(ErrorMessage);
        Console.WriteLine("Bitte versuchen sie es Erneut");
        Error = true;
    }
    else
    {
        Console.WriteLine();
        bool StartwertError = false;
        do {
            StartwertError = false;
            Console.WriteLine("Mit welchem Startwert möchten Sie starten?");
            string Eingabe = Console.ReadLine() ?? "";
            if (float.TryParse(Eingabe, out float StartWert)) {
                bool WiederholungsError = false;
                do
                {
                    WiederholungsError = false;
                    Console.WriteLine("Wie viele Wiederholungen möchten Sie machen?");
                    string WiederholungenString = Console.ReadLine() ?? "";
                    if(int.TryParse(WiederholungenString, out int Wiederholungen) && Wiederholungen > 0)
                    {
                        foreach(Element item in Elemente)
                        {
                            Console.WriteLine($"{item.GetDatatype()}, {item.GetValue()}");
                        }
                        Console.WriteLine(string.Join("", Elemente.ConvertAll(e => e.GetValue())));
                        float Result = Run(Elemente.ToArray(), StartWert);
                        Console.WriteLine($"The Result is {Result}");

                    }
                    else
                    {
                        Console.WriteLine("Bitte geben sie eine gültige Zahl ein");
                        WiederholungsError = true;
                    }

                } while (WiederholungsError);
            }
            else
            {
                Console.WriteLine("Bitte geben sie eine Zahl ein");
                StartwertError = true;
            }
        } while (StartwertError);






        Console.WriteLine("Möchtest du es nocheinmal versuchen?");
        string Response = Console.ReadLine() ?? "";
        if (Response.ToLower() == "y" || Response.ToLower() == "j" ||Response.ToLower() == "") Error = true;
        else
        {
            Console.WriteLine("Programm wird beendet...");
            break;
        }
    }
    Console.WriteLine();
} while (Error);

bool IsBiOperation(char c)
{
    if (c == '+'  || c == '-' || c == '*' || c == '/' || c == '^')
    {
        return true;
    }
    return false;
}

float IsConst(char constant)
{
    switch(constant)
    {
        case 'p':
            return (float)3.14159;
        case 'e':
            return (float)2.71828;
        default:
            return (float)0;
    }
}

float Run(Element[] function, float value) {
    for(int i = 0; i > function.Count(); i++)
    {
        Console.WriteLine(function[i].GetValue());
        if (function[i].GetDatatype() == "variable")
        {
            function[i] = new Element("float", value.ToString()); 
        }
        
    }
    return float.Parse((Calculate(function.ToList()).FirstOrDefault() ?? throw new Exception("Da ist irgendwas gewaltig schiefgeloffen")).GetValue());
}

Element[] Calculate(List<Element> term)
{
     
    while (term.Any(e => e.GetValue() == "(")) {
        int indexOpen = term 
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "(")
            .Select(x => x.i)
            .FirstOrDefault(-1);
        if (indexOpen == -1) throw new InvalidOperationException($"Keine öffnende Klammer gefunden (Index: {indexOpen})");
        int counter = 0;
        int indexClose = -1;
        for (int i = indexOpen; i < term.Count; i++)
        {
            if (term[i].GetValue() == "(")
                counter++;

            if (term[i].GetValue() == ")")
                counter--;

            if (counter == 0)
            {
                indexClose = i;
                break;
            }
        }
        
        if (indexClose == -1) throw new InvalidOperationException($"Keine öffnende Klammer gefunden (Index: {indexClose})");

        List<Element> BracketTerm = term
            .Skip(indexOpen)
            .Take(term.Count() - indexClose + 1)
            .ToList();
        
        term.RemoveRange(indexOpen, indexClose - indexOpen + 1);
        term.InsertRange(indexOpen, Calculate(BracketTerm));
    }

    while (true)
    {
        int PowTermInt = term
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "^")
            .Select(x => x.i)
            .FirstOrDefault(-1);
        if (PowTermInt == -1) break;
        
        Element[] PointTerm = {term[PowTermInt -1], term[PowTermInt], term[PowTermInt + 1]};
        term.RemoveRange(PowTermInt - 1, 3);
        term.InsertRange(PowTermInt - 1, Calculate(PointTerm.ToList()));
    }
    Console.WriteLine($"Hoch abgearbeitet, Term: {term.Select(x => x.GetValue()).ToArray()}");

    while (true)
    {
        int DotTermInt = term
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "/" || x.value.GetValue() == "*")
            .Select(x => x.i)
            .FirstOrDefault(-1);

        if (DotTermInt == -1) break;        
        
        Element[] PointTerm = {term[DotTermInt -1], term[DotTermInt], term[DotTermInt + 1]};
        term.RemoveRange(DotTermInt - 1, 3);
        term.InsertRange(DotTermInt - 1, Calculate(PointTerm.ToList()));
    }

    while(true)
    {
        int DashTermInt = term
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "+" || x.value.GetValue() == "-")
            .Select(x => x.i)
            .FirstOrDefault(-1);
         
         if (DashTermInt == -1) break;

        Element[] DashTerm = {term[DashTermInt -1], term[DashTermInt], term[DashTermInt + 1]};
        term.RemoveRange(DashTermInt - 1, 3);
        term.InsertRange(DashTermInt - 1,Calculate(DashTerm.ToList()));
    }
    return term.ToArray();
}

class IsOperation()
{
    public static (int OpenBrackets, int CloseBrackets) Brackets(Element[] term)
    {
        int OpenBrackets = 0;
        int CloseBrackets = 0;
        int Tcounter = 0;
        foreach (Element element in term)
        {
            Tcounter++;
            if (element.GetDatatype() == "bracket")
            {
                if (element.GetValue() == "(")
                {
                    Console.WriteLine($"Klammer auf bei Element {element.GetDatatype()}, {element.GetValue()}, counter: {Tcounter}");
                    OpenBrackets ++;  
                } 
                if (element.GetValue() == ")")
                {
                    Console.WriteLine($"Klammer zu bei Element {element.GetDatatype()}, {element.GetValue()}, counter: {Tcounter}");
                    CloseBrackets ++;
                }
            }
        }
        Console.WriteLine($"Klammern Auf: {OpenBrackets}");
        Console.WriteLine($"Klammer Zu: {CloseBrackets}");
        if (OpenBrackets - CloseBrackets > 0) throw new Exception($" Es  gibt {OpenBrackets - CloseBrackets} Klammer(n) Auf zu wenig");
        else if (CloseBrackets - OpenBrackets > 0) throw new Exception($"Es gibt {CloseBrackets - OpenBrackets} Klammer(n) Zu zu wenig");
        return (OpenBrackets, CloseBrackets);
    }

    /*public static int[] Muliplication(Element[] term) 
    {
        return term
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "*")
            .Select(x => x.i)
            .ToArray();
    }
    public static int[] Division(Element[] term)
    {
        return term
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "/")
            .Select(x => x.i)
            .ToArray();
    }*/
}


class Element
{
    private int? Intvalue;
    private float? floatvalue;
    private char? Biop;
    private string? Bracket;
    private string? Variable;
    private string datatype;
    public Element(string datatype, string value) {
        this.datatype = datatype;
        switch (datatype)
        {
            case "number":
                Intvalue = int.Parse(value);
                break;
            case "float":
                floatvalue = float.Parse(value);
                break;
            case "biop":
                Biop = value.ToCharArray()[0];
                break;
            case "bracket":
                Bracket = value; 
                break;
            case "variable":
                Variable = value;
                break;
            default:
                break;
        }
    }
    public string GetDatatype()
    {
        return this.datatype;
    }
    public void SetDatatype(string datatype)
    {
        if (datatype == "number" || datatype == "float" || datatype == "biop" || datatype == "bracket" || datatype == "variable")
        {
            this.datatype = datatype;
            return;
        }
        else
        {
            throw new Exception("Dieser Datentyp existiert nicht");
        }
    }
    public string GetValue()
    {
        switch(this.datatype)
        {
            case "number":
                return Intvalue.ToString() ?? throw new Exception("Intvalue ist leer");
            case "float":
                return floatvalue.ToString() ?? throw new Exception("floatvalue ist leer");
            case "biop":
                return Biop.ToString() ?? throw new Exception("Operator ist leer");
            case "bracket":
                return Bracket ?? throw new Exception("Es existiert keine Klammer");
            case "variable":
                return Variable ?? throw new Exception("Es gibt keine Variable");
            default:
                throw new Exception("Element hat kein annerkannten Datentyp");
        }
    }
    public void SetValue(string value)
    {
        switch(this.datatype)
        {
            case "number":
                this.Intvalue = int.Parse(value);
                break;
            case "float":
                this.floatvalue = float.Parse(value);
                break;
            case "biop":
                this.Biop = value.ToCharArray()[0];
                break;
            case "bracket":
                this.Bracket = value;
                break;
            case "variable":
                this.Variable = value;
                break;
            default:
                Console.WriteLine("Bananig");
                break;
        }
    }
}