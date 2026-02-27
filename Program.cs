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
                    if (lastElement.GetValue() == "close")
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
                                if (Elemente[Elemente.Count() - 2].GetValue() == "open")
                                {
                                    Elemente[Elemente.Count() - 1] = new Element("number", (-number).ToString());
                                }
                                else if (Elemente[Elemente.Count() - 2].GetValue() == "close") {
                                    Elemente[Elemente.Count() -1] = new Element("biop", "+");
                                    Elemente.Add(new Element("number", (-number).ToString()));
                                }
                                else throw new Exception("Klammer ist weder offen noch zu");
                                break;
                            case "biop":
                                Elemente[Elemente.Count() - 1] = new Element("number", (-number).ToString());
                                break;
                            case "number":
                                break;
                            case "double":
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
            Elemente.Add(new Element("bracket", "open"));
            continue;
        }
        else if (item == ')')
        {
            Elemente.Add(new Element("bracket", "close"));
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
                        double Result = Run(Elemente.ToArray(), StartWert);

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
        if (function[i].GetDatatype() == "variable")
        {
            function[i] = new Element("double", value.ToString());
        }
    }
    return 0;
}




class Element
{
    private int? Intvalue;
    private double? Doublevalue;
    private char? Biop;
    private string? Bracket;
    private string datatype;
    public Element(string datatype, string value) {
        this.datatype = datatype;
        switch (datatype)
        {
            case "number":
                Intvalue = int.Parse(value);
                break;
            case "double":
                Intvalue = int.Parse(value);
                break;
            case "biop":
                Biop = value.ToCharArray()[0];
                break;
            case "bracket":
                Bracket = value; 
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
        if (datatype == "number" || datatype == "double" || datatype == "biop" || datatype == "bracket")
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
            default:
                Console.WriteLine("Bananig");
                break;
        }
    }
}