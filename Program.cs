using System.Diagnostics;
var numbers = Enumerable.Range(1, 5000000);
var stopwatch = Stopwatch.StartNew();
var linqResult = numbers
    .Select(HeavyCalculation)
    .ToList();
stopwatch.Stop();
long linqTime = stopwatch.ElapsedMilliseconds;
Console.WriteLine("LINQ:");
Console.WriteLine($"Time: {linqTime} ms");
Console.WriteLine();
stopwatch.Restart();
var plinqResult = numbers
    .AsParallel()
    .Select(HeavyCalculation)
    .ToList();
stopwatch.Stop();
long plinqTime = stopwatch.ElapsedMilliseconds;
Console.WriteLine("PLINQ:");
Console.WriteLine($"Time: {plinqTime} ms");
Console.WriteLine();
Console.WriteLine("========== PERFORMANCE ==========");
Console.WriteLine($"LINQ:  {linqTime} ms");
Console.WriteLine($"PLINQ: {plinqTime} ms");
double speedup = linqTime / plinqTime;
Console.WriteLine($"Speedup: {speedup}x");
Console.WriteLine();

TestDegreeOfParallelism(numbers, 2);
TestDegreeOfParallelism(numbers, 4);
TestDegreeOfParallelism(numbers, 8);//daje 8 s trudom viderjal v 16 umret
Console.WriteLine();
var sNumbers = Enumerable.Range(1, 100);
var sResult = sNumbers
    .AsParallel()
    .Select(x => x)
    .ToList();
Console.WriteLine("PLINQ bez AsOrdred:");
Console.WriteLine(string.Join(", ", sResult));
Console.WriteLine();
var ordResult = sNumbers
    .AsParallel()
    .AsOrdered()
    .Select(x => x)
    .ToList();
Console.WriteLine("PLINQ s AsOrdered:");
Console.WriteLine(string.Join(", ", ordResult));
Console.WriteLine();

stopwatch.Restart();
var sLinqResult = numbers
    .Select(x => x * 2)
    .ToList();
stopwatch.Stop();
long sLinqTime = stopwatch.ElapsedMilliseconds;
stopwatch.Restart();
var sPlinqResult = numbers
    .AsParallel()
    .Select(x => x * 2)
    .ToList();
stopwatch.Stop();
long sPlinqTime = stopwatch.ElapsedMilliseconds;
Console.WriteLine($"LINQ:  {sLinqTime} ms");
Console.WriteLine($"PLINQ: {sPlinqTime} ms");
if (sPlinqTime > 0)
{
    double sSpeedup =sLinqTime / sPlinqTime;
    Console.WriteLine($"Speedup: {sSpeedup}x");
}
Console.WriteLine();
static long HeavyCalculation(int number)
{
    long result = 0;
    for (int i = 1; i <= 10_000; i++)
    {
        result += (number * i) % 1234567;
    }
    return result;
}
static void TestDegreeOfParallelism(
    IEnumerable<int> numbers,
    int degree)
{
    var stopwatch = Stopwatch.StartNew();
    var result = numbers
        .AsParallel()
        .WithDegreeOfParallelism(degree)
        .Select(HeavyCalculation)
        .ToList();
    stopwatch.Stop();
    Console.WriteLine($"Koqda degree paralel {degree}: {stopwatch.ElapsedMilliseconds} ms");
}

// PLINQ выполняет элементы параллельно
// поэтому разные части коллекции могут завершаться в разное время

// Для чего нужен AsOrdered()?
// Чтобы сохранить исходный порядок элементов

// Есть ли цена у сохранения порядка?
// Да Нужно дополнительно отслеживать порядок
// поэтому выполнение может стать немного медленнее

// Почему PLINQ в первом тесте может быть значительно быстрее, 
// а во втором — преимущество может 
// быть маленьким или PLINQ вообще может оказаться медленнее?
// HeavyCalculation тяжёлая операция 
// PLINQ распределяет вычисления между ядрами процессора

// Когда стоит использовать PLINQ?
// Когда есть большая коллекция и 
// тяжёлые вычисления

// когда PLINQ действительно может ускорить программу;
// Когда коллекция большая
// а операции над элементами тяжёлые

// когда PLINQ использовать бессмысленно;
// Когда операции очень простые

// почему большая коллекция сама по себе не гарантирует ускорение;
// Потому что важна еще сложность обработки элементов

// почему CPU-bound операции являются хорошим кандидатом;
// Они нагружают CPU и 
// могут выполняться одновременно на нескольких ядрах

// как AsOrdered() влияет на выполнение
// Сохраняет исходный порядок элементов
// но добавляет накладные расходы и может немного замедлить выполнение

// зачем нужен WithDegreeOfParallelism().
// Чтобы ограничить количество параллельных вычислений