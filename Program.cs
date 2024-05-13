// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NAudio.MediaFoundation;
using NAudio.Wave.SampleProviders;

public class Program{

    public static int dagMax;
    public static int nachtMax;
    public static int dbMin = 30;
    public static bool nacht = false;
    public static string file = "Details.txt";
    public static TimeOnly timeParse;
    public static int dbParse;

    static void Main()
    {
        
        FileCheck();
        while(true){
        Startup();
        Berekening();
    }
    }

    public static void FileCheck(){
        if(File.Exists(file)){
            ReadFile();
        }
        else{
            writeFile();
        }

        if(dagMax == 0  || nachtMax == 0 ){
            writeFile();
        }
    }

        public static void Startup(){
        Console.WriteLine("Dagmax: "  + dagMax);
        Console.WriteLine("Nachtmax: " + nachtMax);
        Console.WriteLine("** Vul '/' in om aan te passen **");
        Console.WriteLine("");
        while(true){
        Console.WriteLine("Vul tijd in");
        var time = Console.ReadLine();
        if(time == "/"){
            File.Delete(file);
            writeFile();
            Console.WriteLine("Dagmax: "  + dagMax);
            Console.WriteLine("Nachtmax: " + nachtMax);
            Console.WriteLine("** Vul '/' in om aan te passen **");
            Console.WriteLine("");
        }
        else{
        try{
        timeParse = TimeOnly.Parse(time ?? "0");
        Console.WriteLine("Ingegeven tijd: " + timeParse);
        if(timeParse.Hour > 24){
            Console.WriteLine("Geef een geldige tijd in");
        }
        else{
            nacht = IsNight(timeParse);
            break;
        }
        }
        catch{
            Console.WriteLine("Geef een geldige tijd in");
        }
        }
        }
        Console.WriteLine("");
        while(true){
        Console.WriteLine("Vul decibel in");
        var db = Console.ReadLine();
        try{
            dbParse = int.Parse(db ?? "0");
                if(dbParse < 1){
                    Console.WriteLine("Vul een geldige hoeveelheid decibel in");
                }
                else{
                    break;
                }
        }
        catch{
            Console.WriteLine("Vul een geldige hoeveelheid decibel in");
        }
        }
        Console.WriteLine("Ingegeven aantal decibel: " + dbParse);
        Console.WriteLine("");
        Console.WriteLine("Berekenen");
    }

    public static void Berekening(){
        var selectedTime = nacht ? nachtMax : dagMax;
        var soundDag = Math.Pow(10, metDecimaal(dagMax, 10));
        var soundIn = Math.Pow(10, metDecimaal(dbParse, 10));
        var marge = metDecimaal(soundIn, soundDag);
        var margeDb = metDecimaal(dbParse, dagMax);
        var margeCombi = ((3 * marge) + defineX(timeParse)*  (0.1 * margeDb)) * 1.3;
        var soundCombi = soundIn * margeCombi;  
        var dbCombi = Math.Log(soundCombi, 10) * 10;


        Outcome(soundDag, soundIn, marge, margeDb, soundCombi, dbCombi, selectedTime);
    }

    public static void Outcome(double soundDag, double soundIn, double marge, double margeDb, double soundCombi, double dbCombi, int selectedTime){
        Console.WriteLine("Luidheid Omgeving Max: " + soundDag);
        Console.WriteLine("Luidheid ingegeven: " + soundIn);
        Console.WriteLine();
        Console.WriteLine("Marge: " + marge);   
        Console.WriteLine("Marge Decibel: " + margeDb);
        Console.WriteLine();
        Console.WriteLine("Geluid rateltikker: " + soundCombi);
        Console.WriteLine("Decibel rateltikker: " + Math.Round(dbCombi, 1));
        if(dbCombi < dbMin){
            Console.WriteLine();
            Console.WriteLine("Decibel lager dan " + dbMin + "db");
            Console.WriteLine("Decibel word opgeschaald naar" + dbMin + "db");
            dbCombi = dbMin;
            Console.WriteLine("Decibel rateltikker: " + Math.Round(dbCombi, 1));
        }
        else if(dbCombi > selectedTime){
            Console.WriteLine();
            Console.WriteLine("Decibel hoger dan " + selectedTime + "db");
            Console.WriteLine("Decibel word omlaag gebracht naar" + selectedTime + "db");
            dbCombi = selectedTime;
            Console.WriteLine("Decibel rateltikker: " + Math.Round(dbCombi, 1));
        }
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");
    }

    public static double metDecimaal(double Divide1, double Divide2){
        double bf = Divide1 / Divide2;
        return bf;
    }

    public static double defineX(TimeOnly time){
        var hours = time.Hour;
        var mins = time.Minute;
        var timeInt = hours + metDecimaal(mins, 60);
        var aftersin = 10.5 + 3 * Math.Sin(0.25 * (timeInt - 7));
        // if(aftersin > 10){
        //     aftersin = 10;
        // }
        if(aftersin < 1){
            aftersin = 1;
        }
        return aftersin;
    }

    public static bool IsNight(TimeOnly time){
        if(time >= TimeOnly.Parse("7:00") && time < TimeOnly.Parse("22:30")){
            return false;
        }
        else{
            return true;
        }
    }

    public static void writeFile(){
    try
    {
    //Open the File
    StreamWriter sw = new StreamWriter(file, true, Encoding.ASCII);
    while(true){
        Console.WriteLine("Vul DagMax in:");
        try{
            var input = Console.ReadLine();
            if(input != null){
            int dgmax = int.Parse(input);
            dagMax = dgmax;
            sw.WriteLine("Dagmax:" + dgmax);
            break;
            }
        }
        catch{
            Console.WriteLine("Foute input. Probeer opnieuw");
        }
    }
    while(true){
        Console.WriteLine("Vul NachtMax in:");
                try{
                    var input = Console.ReadLine();
                    if(input != null){
                    int ngmax = int.Parse(input);
                    nachtMax = ngmax;
                    sw.WriteLine("nachtMax:" + ngmax);
                    break;
                    }
                }
                catch{
                    Console.WriteLine("Foute input. Probeer opnieuw");
                }
            }
        sw.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Opslaan gegevend.");
        }
    }

    public static List<string> ReadFile(){
        List<string> strings = new();
            try
    {
    //Open the File
    StreamReader sw = new(file);
    string? line = sw.ReadLine();
    while(line != null){
        strings.Add(line);
        line = sw.ReadLine();
    }
    sw.Close();
    var dgmax = strings.Where(x => x.ToLower().Contains("dagmax")).LastOrDefault();
    try{
    if(dgmax != null){
        dagMax = int.Parse(dgmax.Split(":").Last());
    }
    string? ngmax = strings.Where(x => x.ToLower().Contains("nachtmax")).LastOrDefault();
    if(ngmax != null){
        nachtMax = int.Parse(ngmax.Split(":").Last());
    }
    }
    catch{
        Console.WriteLine("Er is een onbekende fout opgetreden.");
    }
    }
    catch(Exception e){
        Console.WriteLine(e);
    }
    return strings;
    }
}