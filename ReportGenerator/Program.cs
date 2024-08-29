using System;

Console.WriteLine("Введите путь к директории:");
string path = Console.ReadLine()!;
if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
{
    Console.WriteLine("Введенный путь некорректный или директория не найдена");
    return;
}

string[] FileName = Directory.GetFiles(path, "*.log").Select(n => Path.GetFileName(n)).ToArray();
Dictionary<string, List<string>> Files = new Dictionary<string, List<string>>();
foreach (string file in FileName)
{
    string[] f = file.Split('.');
    string key = f[0];
    if (f.Length == 2)
    {
        if (!Files.ContainsKey(key))
            Files.Add(key, new List<string>());
    }
    else if(f.Length == 3)
    {
        
        if (Files.ContainsKey(key))
            Files[key].Add(file);
        else
            Files.Add(key, new List<string>() {file});
    }
}

Array.Clear(FileName);

foreach (var item in Files)
{
    Report report = new Report(item.Key, item.Value.Count + 1);
    string[] rFile =  File.ReadAllLines(path + @$"\{item.Key}.log");
    foreach (string row in rFile)
        FillReport(report, row);
    foreach (var it in item.Value)
    {
        rFile = File.ReadAllLines(path + @$"\{it}");
        foreach (string row in rFile)
            FillReport(report, row);
    }
    Console.WriteLine($"Имя сервиса: {report.reportName}");
    Console.WriteLine($"Дата и время самой ранней записи в логах: {report.GetStartTime()}");
    Console.WriteLine($"Дата и время самой последней записи в логах: {report.GetEndTime()}");
    int SumCountSeverity = report.SumCountSeverity();
    int SumCountCategory = report.SumCountCategory();
    Console.WriteLine("Количество записей в разрезе severity: " );
    foreach (var row in report.Severity)
        Console.WriteLine($"{row.Key}: {row.Value}, {Math.Round(row.Value * 100.0 / SumCountSeverity, 2)}%");
    Console.WriteLine("Количество записей в разрезе category: ");
    foreach (var row in report.Category)
        Console.WriteLine($"{row.Key}: {row.Value}, {Math.Round(row.Value * 100.0 / SumCountCategory, 2)}%");
    Console.WriteLine($"Количество ротаций: {report.countRotation}");
    Console.WriteLine();

}
void FillReport(Report report, string row)
{
    string[] s = row.Split(']').Select(n => n.Replace("[", "")).ToArray();
    report.data.Add(s[0]);
    if (!report.Severity.ContainsKey(s[1]))
        report.Severity.Add(s[1], 1);
    else
        report.Severity[s[1]]++;
    if (!report.Category.ContainsKey(s[2]))
        report.Category.Add(s[2], 1);
    else
        report.Category[s[2]]++;
}
class Report
{
    internal string reportName;
    internal List<string> data;
    internal Dictionary<string, int> Severity;
    internal Dictionary<string, int> Category;
    internal int SumCountSeverity() => Severity.Sum(n => n.Value);
    internal int SumCountCategory() => Category.Sum(n => n.Value);
    internal int countRotation;
    internal string GetStartTime()
        => data.Count > 0 ? data.OrderBy(x => x).First() : "";
    internal string GetEndTime()
        => data.Count > 0 ? data.OrderBy(x => x).Last() : "";
    internal Report(string reportName)
    {
        this.reportName = reportName;
        data = new List<string>();
        Severity = new Dictionary<string, int>();
        Category = new Dictionary<string, int>();
        countRotation = 0;
    }
    internal Report(string reportName, int Rotation)
    {
        this.reportName = reportName;
        data = new List<string>();
        Severity = new Dictionary<string, int>();
        Category = new Dictionary<string, int>();
        countRotation = Rotation;
    }
}