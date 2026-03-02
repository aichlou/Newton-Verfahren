using System.ComponentModel;
using System.Drawing;
using System.Net;
using Microsoft.VisualBasic;
bool Error = false;
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
                    lastElement.SetValue((int.Parse(lastElement.GetValue()) * 10 + int.Parse(item.ToString())).ToString());
                    break;
                case "double":
                    lastElement.SetValue((Double.Parse(lastElement.GetValue()) + 0.1 * int.Parse(item.ToString())).ToString() ?? throw new Exception("Bidde geben sie was ein"));
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
                            case "double":
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
            int Zwischenspeicher = int.Parse(lastElement.GetValue());
            lastElement.SetValue("0");
            lastElement.SetDatatype("double");
            lastElement.SetValue(Zwischenspeicher.ToString());
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
        else if (item == variable)
        {
            Elemente.Add(new Element("variable", variable.ToString()));
        }
        else
        {
            ErrorMessage += $"Verbotene Zeichen {item} \n";
        }
    }
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
            if (double.TryParse(Eingabe, out double StartWert)) {
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
                        double Result = Run(Elemente.ToArray(), StartWert);
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

double Run(Element[] function, double value) {
    for(int i = 0; i > function.Count(); i++)
    {
        Console.WriteLine(function[i].GetValue());
        if (function[i].GetDatatype() == "variable")
        {
            function[i] = new Element("double", value.ToString()); 
        }
        
    }
    return double.Parse((Calculate(function.ToList()).FirstOrDefault() ?? throw new Exception("Da ist irgendwas gewaltig schiefgeloffen")).GetValue());
}

Element[] Calculate(List<Element> term)
{
    int OpenBrackets = 0;
    int CloseBrackets = 0;
    foreach (Element element in term)
    {
        if (element.GetDatatype() == "bracket")
        {
            if (element.GetValue() == "(") OpenBrackets ++;
            if (element.GetValue() == ")") CloseBrackets ++;
        }
    }
    if (OpenBrackets - CloseBrackets > 0) throw new Exception($" Es  gibt {OpenBrackets - CloseBrackets} Klammern Auf zu wenig");
    else if (CloseBrackets - OpenBrackets > 0) throw new Exception($"Es gibt {CloseBrackets - OpenBrackets} Klammern Zu zu wenig");
     
    while (IsOperation.Brackets(term.ToArray()).OpenBrackets > 0) {
        int indexOpen = term 
            .Select((value, i) => new {value, i})
            .Where(x => x.value.GetValue() == "(")
            .Select(x => x.i)
            .FirstOrDefault(-1);
        if (indexOpen == -1) throw new Exception("Ja ich hab gekackt und irgendein Mumpitz gekotet (Close)");
        int counter = 0;
            /*int indexClose = term
            .Select((value, i) => new {value, i})
            .Skip(indexOpen) //Ist eigendlich irrelevant
            .Where(x => !(x.value.GetValue() == "(" && counter++ > 0))
            .Where(x => x.value.GetValue() == ")" && counter-- > 0)
            .Select(x => x.i)
            .FirstOrDefault(-1); */
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
        
        if (indexClose == -1) throw new Exception("Ja ich hab gekackt und irgendein Mumpitz gekotet (Close)");

        List<Element> BracketTerm = term
            .Skip(indexOpen)
            .Reverse()
            .Skip(indexClose)
            .Reverse()
            .ToList();
        
        term.RemoveRange(indexOpen, indexClose - indexOpen);
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
        foreach (Element element in term)
        {
            if (element.GetDatatype() == "bracket")
            {
                if (element.GetValue() == "(") OpenBrackets ++;
                if (element.GetValue() == ")") CloseBrackets ++;
            }
        }
        if (OpenBrackets - CloseBrackets > 0) throw new Exception($" Es  gibt {OpenBrackets - CloseBrackets} Klammern Auf zu wenig");
        else if (CloseBrackets - OpenBrackets > 0) throw new Exception($"Es gibt {CloseBrackets - OpenBrackets} Klammern Zu zu wenig");
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
    private double? Doublevalue;
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
            case "double":
                Doublevalue = double.Parse(value);
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
        if (datatype == "number" || datatype == "double" || datatype == "biop" || datatype == "bracket" || datatype == "variable")
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
            case "double":
                return Doublevalue.ToString() ?? throw new Exception("Doublevalue ist leer");
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
            case "double":
                this.Doublevalue = Double.Parse(value);
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