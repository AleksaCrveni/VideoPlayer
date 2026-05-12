using System.Buffers.Binary;
using System.Text;
string[] lines = File.ReadAllLines("input.txt");
int count = 0;
List<string> outList = new();
foreach (string l in lines)
{
  byte[] arr = Encoding.ASCII.GetBytes(l);
  Console.WriteLine(l);
  if (arr.Length != 4)
  {
    Console.WriteLine("BAD");
    continue;
  }
    

  uint val = BinaryPrimitives.ReadUInt32BigEndian(arr);
  if (char.IsDigit((char)arr[0]))
    outList.Add($"_{l} = {val},");
  else 
    outList.Add($"{l} = {val},");
  count++;
}

Console.WriteLine($"Lines processed: {count}");
File.WriteAllLines("output.txt", outList);
